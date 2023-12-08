using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dotNET_module_16_practice
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Приложение для логирования изменений в файлах");
            Console.WriteLine("--------------------------------------------");

            string directoryPath = GetDirectoryPathFromUser();
            string logFilePath = GetLogFilePathFromUser();

            if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(logFilePath))
            {
                Console.WriteLine("Неверные пути. Приложение завершает работу.");
                return;
            }

            SetupFileSystemWatcher(directoryPath, logFilePath);

            Console.WriteLine($"Отслеживание изменений в директории {directoryPath} начато.");
            Console.WriteLine("Нажмите любую клавишу для остановки...");
            Console.ReadKey();
        }

        static string GetDirectoryPathFromUser()
        {
            string directoryPath;
            do
            {
                Console.Write("Введите путь к отслеживаемой директории: ");
                directoryPath = Console.ReadLine();
            } while (!Directory.Exists(directoryPath));

            return directoryPath;
        }

        static string GetLogFilePathFromUser()
        {
            Console.Write("Введите путь к лог-файлу: ");
            return Console.ReadLine();
        }

        static void SetupFileSystemWatcher(string directoryPath, string logFilePath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = directoryPath;
            watcher.IncludeSubdirectories = true;

            watcher.NotifyFilter = NotifyFilters.LastWrite
                                  | NotifyFilters.FileName
                                  | NotifyFilters.DirectoryName;

            watcher.Changed += (sender, eventArgs) =>
            {
                LogToFile(logFilePath, $"Файл {eventArgs.Name} был изменен в {DateTime.Now}");
            };

            watcher.Created += (sender, eventArgs) =>
            {
                LogToFile(logFilePath, $"Файл {eventArgs.Name} был создан в {DateTime.Now}");
            };

            watcher.Deleted += (sender, eventArgs) =>
            {
                LogToFile(logFilePath, $"Файл {eventArgs.Name} был удален в {DateTime.Now}");
            };

            watcher.Renamed += (sender, eventArgs) =>
            {
                LogToFile(logFilePath, $"Файл {eventArgs.OldName} был переименован в {eventArgs.Name} в {DateTime.Now}");
            };

            watcher.Error += (sender, eventArgs) =>
            {
                LogToFile(logFilePath, $"Ошибка отслеживания изменений: {eventArgs.GetException().Message}");
            };

            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Отслеживание изменений в директории {directoryPath} начато.");
        }

        static void LogToFile(string logFilePath, string logMessage)
        {
            bool success = false;
            int retryCount = 3; // Количество попыток записи

            while (!success && retryCount > 0)
            {
                try
                {
                    using (StreamWriter writer = File.AppendText(logFilePath))
                    {
                        writer.WriteLine(logMessage);
                        success = true; // Запись прошла успешно
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ошибка записи в лог-файл: {ex.Message}");
                    retryCount--;

                    // Пауза перед повторной попыткой записи
                    System.Threading.Thread.Sleep(1000);
                }
            }

            if (!success)
            {
                Console.WriteLine("Не удалось записать в лог-файл после нескольких попыток.");
            }
        }
    }
}
