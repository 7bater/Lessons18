//using static System.Console;
//using Serilog;

//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .WriteTo.Console()
//    .WriteTo.File("logs/applog-.txt", rollingInterval: RollingInterval.Infinite)
//    .CreateLogger();

//try
//{
//    Log.Information("Застосунок запущено");
//    string name = "Oleksandr";
//    Person person = new("Vasya", new(2006, 11, 13));

//    Log.Information("Користувач {Name} з датою народження{BDate} увійшов в систему", person.Name, person.bDate);
//    Log.Information("Вхід користувача підтверджено адміном: " + name);
//}
//catch (Exception ex)
//{
//    Log.Information(ex.Message, "Застосунок аварійно завершує свою роботу");
//}
//finally
//{
//    //...
//    Log.CloseAndFlush();
//}


//exx user 1 та 2 завдання
using System;
using System.Text;
using Serilog;
using static System.Console;

namespace Lessons18
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            OutputEncoding = Encoding.UTF8;
            InputEncoding = Encoding.UTF8;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/auth-log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Застосунок авторизації та контенту запущено.");
                string dbPath = @"C:\Users\kozer\c#\Lessons18\file_data\users.json";
                AuthService authService = new AuthService(dbPath);

                string[] quotes = {
                    "«Код — це як гумор. Якщо його треба пояснювати, то він поганий.» — Мартин Фаулер",
                    "«Простота — це душа ефективності.» — Остін Фріман",
                    "«Працює — не чіпай!» — Головне правило програміста",
                    "«Навчайся так, ніби будеш жити вічно; живи так, ніби помреш завтра.» — Махатма Ганді"
                };

                while (true)
                {
                    WriteLine("\nCистема авторизації та контенту");
                    WriteLine("1. Реєстрація");
                    WriteLine("2. Вхід у систему");
                    WriteLine("3. Згенерувати фейкових користувачів (Тестування)");
                    WriteLine("0. Вихід");
                    Write("Оберіть дію: ");

                    string choice = ReadLine();

                    if (choice == "1")
                    {
                        Write("\nІм'я: ");
                        string firstName = ReadLine();

                        Write("Прізвище: ");
                        string lastName = ReadLine();

                        Write("Email: ");
                        string email = ReadLine();

                        Write("Логін: ");
                        string username = ReadLine();

                        Write("Пароль: ");
                        string password = ReadLine();

                        User newUser = new User(firstName, lastName, email, username, password);
                        if (authService.Register(newUser))
                        {
                            WriteLine("Реєстрація успішна! Тепер ви можете увійти.");
                        }
                        else
                        {
                            WriteLine("Помилка: Користувач із таким логіном або Email вже існує.");
                        }
                    }
                    else if (choice == "2")
                    {
                        Write("\nЛогін: ");
                        string username = ReadLine();

                        Write("Пароль: ");
                        string password = ReadLine();

                        LoginDto loginDto = new LoginDto(username, password);
                        User user = authService.Login(loginDto);

                        if (user != null)
                        {
                            WriteLine($"\nВітаємо, {user.FirstName} {user.LastName}! Вхід виконано успішно.");
                            Random rand = new Random();
                            string randomQuote = quotes[rand.Next(quotes.Length)];
                            WriteLine($"Випадкова цитата дня для вас:\n{randomQuote}");
                        }
                        else
                        {
                            WriteLine("Помилка: Невірний логін або пароль.");
                        }
                    }
                    else if (choice == "3")
                    {
                        Write("\nСкільки фейкових користувачів згенерувати? Введіть число: ");
                        if (int.TryParse(ReadLine(), out int count) && count > 0)
                        {
                            authService.GenerateFakeUsers(count);
                            WriteLine($"Успішно згенеровано та збережено {count} користувачів!");
                        }
                        else
                        {
                            WriteLine("Помилка: Некоректне число.");
                        }
                    }
                    else if (choice == "0")
                    {
                        Log.Information("Застосунок завершує роботу за командою користувача.");
                        break;
                    }
                    else
                    {
                        WriteLine("Невідома команда.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Критична помилка під час роботи застосунку.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}