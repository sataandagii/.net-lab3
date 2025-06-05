using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Encodings.Web;

namespace Lab3
{
    public class Application
    {
        private List<Course> courses = new();
        private List<User> users = new();

        public void Run()
        {
            const string dataFile = "data.json";
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Створити тестові дані");
                Console.WriteLine("2. Зберегти дані у файл");
                Console.WriteLine("3. Завантажити дані з файлу");
                Console.WriteLine("4. Переглянути курси (JsonDocument)");
                Console.WriteLine("5. Додати тест до курсу (JsonNode)");
                Console.WriteLine("6. Вийти");
                Console.Write("Виберіть опцію: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateSampleData();
                        break;
                    case "2":
                        SerializeData(dataFile);
                        break;
                    case "3":
                        DeserializeData(dataFile);
                        break;
                    case "4":
                        PrintCoursesWithJsonDocument(dataFile);
                        break;
                    case "5":
                        AddTestToCourseUsingJsonNode(dataFile);
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        private void CreateSampleData()
        {
            var course = new Course
            {
                Id = 1,
                Name = "Basic C#",
                Description = "Basic conceptions",
                Tests = new List<Test>
                {
                    new Test
                    {
                        Title = "Basic test",
                        Questions = new List<Question>
                        {
                            new Question
                            {
                                Text = "What is a class in C#?",
                                AnswerOptions = new List<AnswerOption>
                                {
                                    new AnswerOption { Text = "Type of data", IsCorrect = true },
                                    new AnswerOption { Text = "Function", IsCorrect = false }
                                }
                            }
                        }
                    }
                }
            };

            var user = new User
            {
                Id = 1,
                Name = "Ivan"
            };

            courses = new List<Course> { course };
            users = new List<User> { user };

            Console.WriteLine("Тестові дані створено!");
        }

        private void SerializeData(string filePath)
        {
            var data = new
            {
                Courses = courses,
                Users = users
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Дозволяє кирилицю
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Дані збережено у файл: " + filePath);
        }

        private void DeserializeData(string filePath)
        {
            try
            {
                // 1. Читання JSON з файлу
                string json = File.ReadAllText(filePath);

                // 2. Десеріалізація
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                JsonDocument document = JsonDocument.Parse(json);
                JsonElement root = document.RootElement;

                // 3. Вилучення даних
                if (root.TryGetProperty("courses", out JsonElement coursesElement))
                {
                    courses = JsonSerializer.Deserialize<List<Course>>(coursesElement, options);
                    Console.WriteLine("\nУспішно завантажено курси:");
                    foreach (var course in courses)
                    {
                        Console.WriteLine($"- {course.Name} (ID: {course.Id})");
                    }
                }

                if (root.TryGetProperty("users", out JsonElement usersElement))
                {
                    users = JsonSerializer.Deserialize<List<User>>(usersElement, options);
                    Console.WriteLine("\nУспішно завантажено користувачів:");
                    foreach (var user in users)
                    {
                        Console.WriteLine($"- {user.Name} (ID: {user.Id})");
                    }
                }

                // 4. Демонстрація структури через JsonDocument
                Console.WriteLine("\nДемонстрація структури через JsonDocument:");
                if (courses.Count > 0 && courses[0].Tests.Count > 0)
                {
                    Console.WriteLine($"Перший тест у першому курсі: {courses[0].Tests[0].Title}");
                    if (courses[0].Tests[0].Questions.Count > 0)
                    {
                        Console.WriteLine($"Перше питання: {courses[0].Tests[0].Questions[0].Text}");
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Помилка десеріалізації: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неочікувана помилка: {ex.Message}");
            }
        }

        private void PrintCoursesWithJsonDocument(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не знайдено!");
                return;
            }

            string json = File.ReadAllText(filePath);

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement root = document.RootElement;

                if (root.TryGetProperty("courses", out JsonElement coursesElement))
                {
                    foreach (JsonElement courseElement in coursesElement.EnumerateArray())
                    {
                        Console.WriteLine($"Курс: {courseElement.GetProperty("name").GetString()}");
                        Console.WriteLine($"Опис: {courseElement.GetProperty("description").GetString()}");

                        if (courseElement.TryGetProperty("tests", out JsonElement testsElement))
                        {
                            foreach (JsonElement testElement in testsElement.EnumerateArray())
                            {
                                Console.WriteLine($"  Тест: {testElement.GetProperty("title").GetString()}");
                            }
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        private void AddTestToCourseUsingJsonNode(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не знайдено!");
                return;
            }

            string json = File.ReadAllText(filePath);
            JsonNode dataNode = JsonNode.Parse(json);

            JsonNode coursesNode = dataNode["courses"];
            JsonNode firstCourse = coursesNode[0];

            var newTest = new JsonObject
            {
                ["title"] = "Новий тест",
                ["questions"] = new JsonArray()
            };

            firstCourse["tests"].AsArray().Add(newTest);

            File.WriteAllText(filePath, dataNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("Тест додано та збережено!");
        }
    }
}
