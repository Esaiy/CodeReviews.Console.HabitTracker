using System.Globalization;
using Microsoft.Data.Sqlite;

string connectionString = @"Data Source=habit-tracker.db";
SqliteConnection connection = new(connectionString);

connection.Open();
SqliteCommand tableCmd = connection.CreateCommand();

tableCmd.CommandText =
@"CREATE TABLE IF NOT EXISTS drinking_water (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Date TEXT,
        Quantity INTEGER
        )";

tableCmd.ExecuteNonQuery();

connection.Close();

GetUserInput();

static void GetUserInput()
{
    Console.Clear();
    bool closeApp = false;
    while (!closeApp)
    {
        Console.WriteLine("\nMAIN MENU");
        Console.WriteLine("\nWhat would you like to do?");
        Console.WriteLine("\nType 0 to Close Application.");
        Console.WriteLine("Type 1 to View All records.");
        Console.WriteLine("Type 2 to Insert Record.");
        Console.WriteLine("Type 3 to Delete Record.");
        Console.WriteLine("Type 4 to Update Record.");
        Console.WriteLine("--------------------------\n");

        string? commandInput = Console.ReadLine();

        switch (commandInput)
        {
            case "0":
                closeApp = true;
                Console.WriteLine("Goodbye!");
                break;
            case "1":
                Console.WriteLine("view");
                View();
                break;
            case "2":
                Console.WriteLine("insert");
                Insert();
                break;
            case "3":
                Console.WriteLine("delete");
                Delete();
                break;
            case "4":
                Console.WriteLine("update record");
                Update();
                break;
            default:
                Console.WriteLine("invalid");
                break;
        }

    }
}

static void Insert()
{
    string date = GetDateInput();

    int quantity = GetNumberInput("quantity: 0 to return");

    string connectionString = @"Data Source=habit-tracker.db";

    using SqliteConnection connection = new(connectionString);
    connection.Open();

    SqliteCommand tableCmd = connection.CreateCommand();
    tableCmd.CommandText = $"INSERT INTO drinking_water(date, quantity) VALUES ('{date}', '{quantity}')";
    _ = tableCmd.ExecuteNonQuery();

    connection.Close();
}

static void View()
{
    string connectionString = @"Data Source=habit-tracker.db";
    using SqliteConnection connection = new(connectionString);
    connection.Open();

    SqliteCommand tableCmd = connection.CreateCommand();
    tableCmd.CommandText = $"SELECT * FROM drinking_water";

    List<DrinkingWater> tableData = [];

    SqliteDataReader reader = tableCmd.ExecuteReader();

    if (!reader.HasRows)
    {
        Console.WriteLine("no rows found");
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

    Console.WriteLine("----------------------\n");
    foreach (DrinkingWater dw in tableData)
    {
        Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MM-yyyy", CultureInfo.CurrentCulture)} - Quantity: {dw.Quantity}");
    }
    Console.WriteLine("----------------------\n");
}

static void Delete()
{
    // how do you delete, with what id
    int id = GetNumberInput("id to delete");

    string connectionString = @"Data Source=habit-tracker.db";
    using SqliteConnection connection = new(connectionString);
    connection.Open();

    SqliteCommand tableCmd = connection.CreateCommand();
    tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id = {id}";
    _ = tableCmd.ExecuteNonQuery();

    //TODO: handle when not exists

    connection.Close();
}

static void Update()
{
    int id = GetNumberInput("id to update");
    // how do you update, with what id
    string connectionString = @"Data Source=habit-tracker.db";
    using SqliteConnection connection = new(connectionString);
    connection.Open();

    SqliteCommand tableCmd = connection.CreateCommand();
    tableCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {id})";
    int checkQuery = Convert.ToInt32(tableCmd.ExecuteScalar(), CultureInfo.CurrentCulture);

    if (checkQuery == 0)
    {
        Console.WriteLine($"id {id} doesnt exists");
        return;
    }

    string date = GetDateInput();

    int quantity = GetNumberInput("quantity: 0 to return");

    SqliteCommand updateCmd = connection.CreateCommand();
    updateCmd.CommandText = $"UPDATE drinking_water set Date='{date}', Quantity={quantity} WHERE Id = {id}";
    _ = updateCmd.ExecuteNonQuery();

    connection.Close();
}

static string GetDateInput()
{
    Console.WriteLine("Please insert the date: type 0 to return");

    string? dateInput = Console.ReadLine();

    while (string.IsNullOrWhiteSpace(dateInput))
    {
        Console.WriteLine("cannot be empty");
        Console.WriteLine("Please insert the date: type 0 to return");
        dateInput = Console.ReadLine();
    }

    // insanity
    if (dateInput == "0")
    {
        GetUserInput();
    }

    return dateInput;
}

static int GetNumberInput(string message)
{
    Console.WriteLine(message);

    string? numberInput = Console.ReadLine();

    while (string.IsNullOrWhiteSpace(numberInput))
    {
        Console.WriteLine("cannot be empty");
        Console.WriteLine(message);
        numberInput = Console.ReadLine();
    }

    // insanity
    if (numberInput == "0")
    {
        GetUserInput();
    }

    int finalInput = Convert.ToInt32(numberInput, CultureInfo.CurrentCulture);

    return finalInput;
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
