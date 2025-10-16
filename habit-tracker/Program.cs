using System.Globalization;
using Microsoft.Data.Sqlite;

string connectionString = @"Data Source=habit-tracker.db;Foreign Keys=True;";

MigrateDatabase();
SeedDatabase();

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
        Console.WriteLine("Type 1 to View Habit Report.");
        Console.WriteLine("Type 2 to View All Habit Type.");
        Console.WriteLine("Type 3 to Insert New Habit Type.");
        Console.WriteLine("Type 4 to View All Habit Records.");
        Console.WriteLine("Type 5 to Insert Habit Record.");
        Console.WriteLine("Type 6 to Delete Habit Record.");
        Console.WriteLine("Type 7 to Update Habit Record.");
        Console.WriteLine("----------------------------------------\n");

        string? commandInput = Console.ReadLine();

        switch (commandInput)
        {
            case "0":
                Console.WriteLine("Goodbye!");
                return;
            case "1":
                Console.WriteLine("Generating Report\n");
                Report();
                break;
            case "2":
                Console.WriteLine("Showing All Habit Type\n");
                List<HabitType> habitTypes = GetAllHabitTypes();
                ShowAllHabitTypes(habitTypes);
                break;
            case "3":
                Console.WriteLine("Inserting New Habit Type\n");
                InsertNewHabitType();
                break;
            case "4":
                Console.WriteLine("Showing All Habit Records\n");
                View();
                break;
            case "5":
                Console.WriteLine("Inserting New Habit Record\n");
                Insert();
                break;
            case "6":
                Console.WriteLine("Deleting Habit Record\n");
                Delete();
                break;
            case "7":
                Console.WriteLine("Updating Habit Record\n");
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
    int habitTypeId = SelectHabit();

    string date = GetDateInput();

    int quantity = GetNumberInput("Please insert the quantity of the activity. (must be whole number)");

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = "INSERT INTO habit(HabitType, date, quantity) VALUES (@habitType, @date, @quantity)";
        _ = tableCmd.Parameters.AddWithValue("@habitType", habitTypeId);
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
    List<Habit> tableData = [];

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"SELECT habit.Id, Date, Quantity, habit_type.Name FROM habit JOIN habit_type ON habit.HabitType = habit_type.Id";

        using SqliteDataReader reader = tableCmd.ExecuteReader();

        if (!reader.HasRows)
        {
            Console.WriteLine("Empty Records.");
        }

        while (reader.Read())
        {
            Habit habit = new()
            {
                Id = reader.GetInt32(0),
                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", CultureInfo.CurrentCulture),
                Quantity = reader.GetInt32(2),
                HabitType = new()
                {
                    Name = reader.GetString(3)
                },
            };

            tableData.Add(habit);
        }
        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }

    Console.WriteLine("----------------------------------------------------------");
    Console.WriteLine("ID\t| Habit\t\t| Date\t\t| Quantity");
    Console.WriteLine("----------------------------------------------------------");
    foreach (Habit h in tableData)
    {
        Console.WriteLine($"{h.Id}\t| {h.HabitType.Name,-14}| {h.Date.ToString("dd-MM-yyyy", CultureInfo.CurrentCulture)}\t| {h.Quantity}");
    }
    Console.WriteLine("----------------------------------------------------------");
}

void Delete()
{
    int id = GetNumberInput("Please insert activity ID.");

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"DELETE FROM habit WHERE Id = @id";
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
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = $"SELECT habit.Id, Date, Quantity, habit_type.Name FROM habit JOIN habit_type ON habit.HabitType = habit_type.Id WHERE habit.Id = @id";
        _ = tableCmd.Parameters.AddWithValue("@id", id);
        using SqliteDataReader reader = tableCmd.ExecuteReader();

        if (!reader.Read())
        {
            Console.WriteLine($"Activity with id: {id} not found");
            return;
        }

        Habit h = new()
        {
            Id = reader.GetInt32(0),
            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", CultureInfo.CurrentCulture),
            Quantity = reader.GetInt32(2),
            HabitType = new()
            {
                Name = reader.GetString(3)
            },
        };

        Console.WriteLine("\nShowing previous value\n");
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine("ID\t| Habit\t\t| Date\t\t| Quantity");
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine($"{h.Id}\t| {h.HabitType.Name,-14}| {h.Date.ToString("dd-MM-yyyy", CultureInfo.CurrentCulture)}\t| {h.Quantity}");
        Console.WriteLine("----------------------------------------------------------");

        string date = GetDateInput();

        int quantity = GetNumberInput("Please insert the quantity of the activity. (must be whole number)");

        using SqliteCommand updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"UPDATE habit set Date='{date}', Quantity={quantity} WHERE Id = {id}";
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

string GetStringInput(string message)
{
    string? input;

    while (true)
    {
        Console.WriteLine(message);
        input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Input cannot be empty.");
            continue;
        }
        break;
    }

    return input;
}

void MigrateDatabase()
{
    using SqliteConnection connection = new(connectionString);
    connection.Open();

    using SqliteCommand tableCmd = connection.CreateCommand();

    tableCmd.CommandText =
    @"CREATE TABLE IF NOT EXISTS habit_type (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Name TEXT,
        UnitOfMeasure TEXT
        )";

    _ = tableCmd.ExecuteNonQuery();

    tableCmd.CommandText =
    @"CREATE TABLE IF NOT EXISTS habit (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        HabitType INTEGER,
        Date TEXT,
        Quantity INTEGER,
        FOREIGN KEY (HabitType) REFERENCES habit_type(Id)
        )";

    _ = tableCmd.ExecuteNonQuery();

    connection.Close();
}

void InsertNewHabitType()
{
    string name = GetStringInput("Please insert name of the habit.");
    string unitOfMeasure = GetStringInput("Please insert the unit of measurement");

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = "INSERT INTO habit_type(Name, UnitOfMeasure) VALUES (@Name, @UnitOfMeasure)";
        _ = tableCmd.Parameters.AddWithValue("@Name", name);
        _ = tableCmd.Parameters.AddWithValue("@UnitOfMeasure", unitOfMeasure);
        _ = tableCmd.ExecuteNonQuery();

        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
}

List<HabitType> GetAllHabitTypes()
{
    List<HabitType> habitTypes = [];

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText = "SELECT * FROM habit_type";

        using SqliteDataReader reader = tableCmd.ExecuteReader();

        if (!reader.HasRows)
        {
            Console.WriteLine("Empty Habit Type");
        }

        while (reader.Read())
        {
            HabitType habitType = new()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                UnitOfMeasure = reader.GetString(2),
            };

            habitTypes.Add(habitType);
        }
        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }

    return habitTypes;
}

void ShowAllHabitTypes(List<HabitType> habitTypes)
{
    Console.WriteLine("----------------------------------------");
    Console.WriteLine("ID\t| Name\t\t| UnitOfMeasure");
    Console.WriteLine("----------------------------------------");
    foreach (HabitType h in habitTypes)
    {
        Console.WriteLine($"{h.Id}\t| {h.Name,-14}| {h.UnitOfMeasure}");
    }
    Console.WriteLine("----------------------------------------\n");
}

int SelectHabit()
{
    List<int> validIds = [];
    int id;

    List<HabitType> habitTypes = GetAllHabitTypes();

    Console.WriteLine("----------------------------------------");
    Console.WriteLine("ID\t| Name\t\t| UnitOfMeasure");
    Console.WriteLine("----------------------------------------");
    foreach (HabitType h in habitTypes)
    {
        Console.WriteLine($"{h.Id}\t| {h.Name,-14}\t| {h.UnitOfMeasure}");
        validIds.Add(h.Id);
    }
    Console.WriteLine("----------------------------------------\n");

    while (true)
    {
        id = GetNumberInput("Please Insert Habit Type ID.");
        if (!validIds.Contains(id))
        {
            Console.WriteLine("Habit Type with id: {id} not found");
            continue;
        }
        break;
    }
    return id;
}

void SeedDatabase()
{
    string[] habits = ["Coding", "Prompting", "Running", "Walking", "Swimming", "Gaming"];
    string[] unit = ["hours", "hours", "KM", "KM", "times", "times"];

    if (GetAllHabitTypes().Count > 0)
    {
        return;
    }

    Random rand = new();

    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();

        tableCmd.CommandText = "INSERT INTO habit_type(Name, UnitOfMeasure) VALUES (@Name, @UnitOfMeasure)";

        SqliteParameter pName = tableCmd.CreateParameter();
        pName.ParameterName = "@Name";
        _ = tableCmd.Parameters.Add(pName);

        SqliteParameter pUnitOfMeasure = tableCmd.CreateParameter();
        pUnitOfMeasure.ParameterName = "@UnitOfMeasure";
        _ = tableCmd.Parameters.Add(pUnitOfMeasure);

        for (int i = 0; i < habits.Length; i++)
        {
            pName.Value = habits[i];
            pUnitOfMeasure.Value = unit[i];
            _ = tableCmd.ExecuteNonQuery();
        }

        List<HabitType> habitTypes = GetAllHabitTypes();
        Dictionary<string, int> mapDb = [];
        foreach (HabitType habitType in habitTypes)
        {
            mapDb[habitType.Name] = habitType.Id;
        }

        List<Habit> toBeInserted = [];
        for (int i = 0; i < 100; i++)
        {
            DateTime start = new(2010, 1, 1);
            DateTime end = DateTime.Today;
            int range = (end - start).Days;

            int randomTypeId = rand.Next(1, habits.Length);
            randomTypeId = mapDb[habits[randomTypeId]];

            DateTime randomDate = start.AddDays(rand.Next(range));

            int randomQuantity = rand.Next(1, 10);

            toBeInserted.Add(new Habit()
            {
                HabitType = new HabitType() { Id = randomTypeId },
                Date = randomDate,
                Quantity = randomQuantity
            });
        }

        tableCmd.CommandText = "INSERT INTO habit(HabitType, date, quantity) VALUES (@habitType, @date, @quantity)";

        SqliteParameter pHabitType = tableCmd.CreateParameter();
        pHabitType.ParameterName = "@habitType";
        _ = tableCmd.Parameters.Add(pHabitType);

        SqliteParameter pDate = tableCmd.CreateParameter();
        pDate.ParameterName = "@date";
        _ = tableCmd.Parameters.Add(pDate);

        SqliteParameter pQuantity = tableCmd.CreateParameter();
        pQuantity.ParameterName = "@quantity";
        _ = tableCmd.Parameters.Add(pQuantity);

        foreach (Habit habit in toBeInserted)
        {
            pHabitType.Value = habit.HabitType.Id;
            pDate.Value = habit.Date.ToString("dd-MM-yy", CultureInfo.CurrentCulture);
            pQuantity.Value = habit.Quantity;
            _ = tableCmd.ExecuteNonQuery();
        }
        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }

}

void Report()
{
    try
    {
        using SqliteConnection connection = new(connectionString);
        connection.Open();

        using SqliteCommand tableCmd = connection.CreateCommand();
        tableCmd.CommandText =
        @"SELECT
            habit_type.name,
            COUNT(habit.id) AS count,
            IFNULL(SUM(habit.quantity), 0) AS total_quantity,
            habit_type.unitofmeasure
        FROM habit_type
        LEFT JOIN habit ON habit.habittype = habit_type.id
        GROUP BY habit_type.name, habit_type.unitofmeasure
        ORDER BY count DESC;"
;

        using SqliteDataReader reader = tableCmd.ExecuteReader();

        if (!reader.HasRows)
        {
            Console.WriteLine("Nothing to report.");
        }

        Console.WriteLine("This is the report of your habit so far\n");
        while (reader.Read())
        {
            if (reader.GetInt32(1) == 0)
            {
                Console.WriteLine($"You have not done any {reader.GetString(0)}");
            }
            else
            {
                Console.WriteLine($"{reader.GetString(0)}: {reader.GetInt32(1)} times with a total quantity of {reader.GetInt32(2)} {reader.GetString(3)}");
            }
        }
        Console.WriteLine();
        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
}

public class HabitType
{
    public int Id
    {
        get;
        set;
    }

    public string? Name
    {
        get;
        set;
    }

    public string? UnitOfMeasure
    {
        get;
        set;
    }
}

public class Habit
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

    public HabitType? HabitType
    {
        get;
        set;
    }
}

