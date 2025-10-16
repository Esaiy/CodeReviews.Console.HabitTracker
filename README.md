# ConsoleTimeLogger

Console based CRUD application to habit.
Developed using C# and SQLite.

# Given Requirements

- [x] This is an application where you’ll log occurrences of a habit.
- [x] This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)
- [x] Users need to be able to input the date of the occurrence of the habit
- [x] The application should store and retrieve data from a real database
- [x] When the application starts, it should create a sqlite database, if one isn’t present.
- [x] It should also create a table in the database, where the habit will be logged.
- [x] The users should be able to insert, delete, update and view their logged habit.
- [x] You should handle all possible errors so that the application never crashes.
- [x] You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.
- [x] Follow the DRY Principle, and avoid code repetition.
- [x] Your project needs to contain a Read Me file where you'll explain how your app works.
- [x] And all the challenges.

# Features

- Create habits and track its records.
- View a report of all the habits.
- Modify or delete records.
- Database migration and seeding.
- Error handling.

# Challenges

- It's a challenge using the official microsoft documentation. Sometimes I need to click a link after 2 or 3 paragraphs. Why? Just make it single page so I can just find stuff.

# Lessons Learned

- Follow tutorial to build up enough knowledge to finally use the official documentation.

# Areas to Improve

- Testing. Right now everything is manual.

# Resources Used

- [Habit Tracker App. C# Beginner Project. CRUD Console, Sqlite, VSCode](https://youtu.be/d1JIJdDVFjs)
- [MS docs for setting up SQLite with C#](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=netcore-cli)
- The clankers
