using System.Globalization;
using Microsoft.Data.Sqlite;

string connectionString = @"Data Source=habit-tracker.db";
using SqliteConnection connection = new(connectionString);

connection.Open();
using SqliteCommand tableCmd = connection.CreateCommand();

tableCmd.CommandText =
@"CREATE TABLE IF NOT EXISTS drinking_water (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Date TEXT,
        Quantity INTEGER
        )";

tableCmd.ExecuteNonQuery();

connection.Close();

GetUserInput();

void GetUserInput()
{
    bool closeApp = false;
    while (!closeApp)
    {
        Console.Clear();
        Console.WriteLine("\nMAIN MENU");
        Console.WriteLine("\nWhat would you like to do?");
        Console.WriteLine("\nType 0 to Close Application.");
        Console.WriteLine("Type 1 to View All Records.");
        Console.WriteLine("Type 2 to Insert Record.");
        Console.WriteLine("Type 3 to Delete Record.");
        Console.WriteLine("Type 4 to Update Record.");
        Console.WriteLine("----------------------------------------\n");

        string? commandInput = Console.ReadLine();

        switch (commandInput)
        {
            case "0":
                closeApp = true;
                Console.WriteLine("Goodbye!");
                break;
            case "1":
                Console.WriteLine("Showing All Records\n");
                View();
                break;
            case "2":
                Console.WriteLine("Inserting New Record\n");
                Insert();
                break;
            case "3":
                Console.WriteLine("Deleting Record\n");
                Delete();
                break;
            case "4":
                Console.WriteLine("Updating Record\n");
                Update();
                break;
            default:
                Console.WriteLine("Invalid Command\n");
                break;
        }
        Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
        string? option = Console.ReadLine();
        if (option == "n")
        {
            closeApp = true;
        }
    }
}

void Insert()
{
    string date = GetDateInput();

    int quantity = GetNumberInput("Please insert the quantity of the activity. (must be whole number)");

    string connectionString = @"Data Source=habit-tracker.db";

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = "INSERT INTO drinking_water(date, quantity) VALUES (@date, @quantity)";
        _ = tableCmd.Parameters.AddWithValue("@date", date);
        _ = tableCmd.Parameters.AddWithValue("@quantity", quantity);
        _ = tableCmd.ExecuteNonQuery();

        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
}

void View()
{
    string connectionString = @"Data Source=habit-tracker.db";
    List<DrinkingWater> tableData = [];

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"SELECT * FROM drinking_water";


        using SqliteDataReader reader = tableCmd.ExecuteReader();

        if (!reader.HasRows)
        {
            Console.WriteLine("Empty Records.");
        }

        while (reader.Read())
        {
            DrinkingWater drinkingWater = new()
            {
                Id = reader.GetInt32(0),
                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", CultureInfo.CurrentCulture),
                Quantity = reader.GetInt32(2)
            };

            tableData.Add(drinkingWater);
        }
        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }

    Console.WriteLine("----------------------------------------");
    Console.WriteLine("ID\t| Date\t\t| Quantity");
    Console.WriteLine("----------------------------------------");
    foreach (DrinkingWater dw in tableData)
    {
        Console.WriteLine($"{dw.Id}\t| {dw.Date.ToString("dd-MM-yyyy", CultureInfo.CurrentCulture)}\t| {dw.Quantity}");
    }
    Console.WriteLine("----------------------------------------\n");
}

void Delete()
{
    int id = GetNumberInput("Please insert activity ID.");

    try
    {
        string connectionString = @"Data Source=habit-tracker.db";
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id = @id";
        _ = tableCmd.Parameters.AddWithValue("@id", id);
        int affectedRows = tableCmd.ExecuteNonQuery();

        if (affectedRows == 0)
        {
            Console.WriteLine($"Activity with id: {id} not found.");
        }

        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
}

void Update()
{
    int id = GetNumberInput("Please insert activity ID.");

    try
    {
        string connectionString = @"Data Source=habit-tracker.db";
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"SELECT * FROM drinking_water WHERE Id = @id";
        _ = tableCmd.Parameters.AddWithValue("@id", id);
        using SqliteDataReader reader = tableCmd.ExecuteReader();

        if (!reader.Read())
        {
            Console.WriteLine($"Activity with id: {id} not found");
            return;
        }

        DrinkingWater dw = new()
        {
            Id = reader.GetInt32(0),
            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", CultureInfo.CurrentCulture),
            Quantity = reader.GetInt32(2)
        };

        Console.WriteLine("\nShowing previous value\n");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("ID\t| Date\t\t| Quantity");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"{dw.Id}\t| {dw.Date.ToString("dd-MM-yyyy", CultureInfo.CurrentCulture)}\t| {dw.Quantity}");

        string date = GetDateInput();

        int quantity = GetNumberInput("Please insert the quantity of the activity. (must be whole number)");

        using SqliteCommand updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"UPDATE drinking_water set Date='{date}', Quantity={quantity} WHERE Id = {id}";
        _ = updateCmd.Parameters.AddWithValue("@date", date);
        _ = updateCmd.Parameters.AddWithValue("@quantity", quantity);
        _ = updateCmd.Parameters.AddWithValue("@id", id);
        _ = updateCmd.ExecuteNonQuery();

        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
}

string GetDateInput()
{
    string? dateInput = "";
    bool validDate = false;

    do
    {
        Console.WriteLine("Please insert the date (dd-mm-yy).\nEnter \"today\" to use today.");
        dateInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(dateInput))
        {
            Console.WriteLine("Date cannot be empty.");
            continue;
        }

        if (dateInput.Equals("today", StringComparison.OrdinalIgnoreCase))
        {
            dateInput = DateTime.Today.ToString("dd-MM-yy", CultureInfo.CurrentCulture);
        }

        if (!DateTime.TryParseExact(dateInput, "dd-MM-yy", CultureInfo.CurrentCulture, DateTimeStyles.None, out _))
        {
            Console.WriteLine("Invalid date format.");
            continue;
        }

        validDate = true;

    } while (!validDate);

    return dateInput;
}

int GetNumberInput(string message)
{
    string? numberInput = "";
    int number = 0;
    bool validNumber = false;

    do
    {
        Console.WriteLine(message);
        numberInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(numberInput))
        {
            Console.WriteLine("Input cannot be empty.");
            continue;
        }

        if (!int.TryParse(numberInput, out number))
        {
            Console.WriteLine("Input is not a valid number.");
            continue;
        }

        if (number <= 0)
        {
            Console.WriteLine("Input must be greater than 0");
            continue;
        }

        validNumber = true;

    } while (!validNumber);

    return number;
}

public class DrinkingWater
{
    public int Id
    {
        get;
        set;
    }

    public DateTime Date
    {
        get;
        set;
    }

    public int Quantity
    {
        get;
        set;
    }
}
