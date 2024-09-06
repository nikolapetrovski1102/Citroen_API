using Org.BouncyCastle.Utilities;
using System;
using System.IO;
using System.Linq;


namespace CitroenAPI.Controllers
{
    public class LogCleanupService
    {
        private readonly string _logDirectory;

        public LogCleanupService(string logDirectory)
        {
            _logDirectory = logDirectory;
        }

        public void CleanupOldLogs()
        {
            if (!Directory.Exists(_logDirectory))
            {
                throw new DirectoryNotFoundException($"Log directory not found: {_logDirectory}");
            }

            var logFiles = Directory.GetFiles(_logDirectory, "*.txt");
            var cutoffDate = DateTime.Now.AddDays(-14);

            foreach (var file in logFiles)
            {
              //  var fileCreationDate = File.GetCreationTime(file);
                string fileName = Path.GetFileName(file);
                String[] splitName= fileName.Split('_');
                String[] splitDate = splitName[0].Split("-");
                int year = int.Parse(splitDate[0]);
                int month = int.Parse(splitDate[1]);
                int day = int.Parse(splitDate[2]);

                // Create DateTime object
                var fileCreationDate = new DateTime(year, month, day);
                if (fileCreationDate < cutoffDate)
                {
                    try
                    {
                        File.Delete(file);
                        Console.WriteLine($"Deleted old log file: {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                    }
                }
            }
        }
    }
}