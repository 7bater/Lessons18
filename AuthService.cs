using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Serilog;

namespace Lessons18
{
    public class AuthService
    {
        private readonly string _usersFilePath;
        private List<User> _users;

        public AuthService(string usersFilePath)
        {
            _usersFilePath = usersFilePath;
            _users = LoadUsers();
        }

        private List<User> LoadUsers()
        {
            try
            {
                if (!File.Exists(_usersFilePath))
                {
                    Log.Warning("Файл користувачів {FilePath} не знайдено. Створено порожній список.", _usersFilePath);
                    return new List<User>();
                }

                string json = File.ReadAllText(_usersFilePath);
                var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                Log.Information("Завантажено {Count} користувачів із файлу.", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Помилка під час завантаження бази користувачів.");
                return new List<User>();
            }
        }

        public void SaveUsers()
        {
            try
            {
                string dir = Path.GetDirectoryName(_usersFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_users, options);
                File.WriteAllText(_usersFilePath, json);
                Log.Debug("Базу користувачів успішно збережено у {FilePath}.", _usersFilePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Помилка під час збереження бази користувачів.");
            }
        }

        public bool Register(User newUser)
        {
            if (_users.Any(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
            {
                Log.Warning("Спроба реєстрації з існуючим логіном: {Username}", newUser.Username);
                return false;
            }

            if (_users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
            {
                Log.Warning("Спроба реєстрації з існуючим Email: {Email}", newUser.Email);
                return false;
            }

            _users.Add(newUser);
            SaveUsers();
            Log.Information("Зареєстровано нового користувача: {Username} ({FirstName} {LastName})", newUser.Username, newUser.FirstName, newUser.LastName);
            return true;
        }

        public User Login(LoginDto loginDto)
        {
            var user = _users.FirstOrDefault(u => u.Username == loginDto.Username && u.Password == loginDto.Password);

            if (user == null)
            {
                Log.Warning("Невдала спроба входу (невірний логін або пароль) для: {Username}", loginDto.Username);
                return null;
            }

            Log.Information("Користувач {Username} успішно увійшов у систему.", user.Username);
            return user;
        }

        public void GenerateFakeUsers(int count)
        {
            string[] firstNames = { "Олександр", "Марія", "Іван", "Анна", "Максим", "Софія", "Дмитро", "Юлія" };
            string[] lastNames = { "Петренко", "Коваленко", "Шевченко", "Мельник", "Бойко", "Кравченко", "Ткаченко" };

            Random rand = new Random();
            int addedCount = 0;

            for (int i = 0; i < count; i++)
            {
                string fName = firstNames[rand.Next(firstNames.Length)];
                string lName = lastNames[rand.Next(lastNames.Length)];
                string uname = $"user_{rand.Next(1000, 9999)}";
                string email = $"{uname}@example.com";
                string pass = "Password123!";

                if (!_users.Any(u => u.Username.Equals(uname, StringComparison.OrdinalIgnoreCase)))
                {
                    _users.Add(new User(fName, lName, email, uname, pass));
                    addedCount++;
                }
            }

            SaveUsers();
            Log.Information("Згенеровано та додано {Count} фейкових користувачів.", addedCount);
        }
    }
}