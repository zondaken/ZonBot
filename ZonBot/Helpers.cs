
using System.Runtime.CompilerServices;

namespace ZonBot
{
    public class Helpers
    {
        public static string? GetThisFilePath([CallerFilePath] string? path = null)
        {
            return path;
        }

        public static string? GetThisPath()
        {
            var path = GetThisFilePath(); // path = @"path\to\your\source\code\file.cs"
            var directory = Path.GetDirectoryName(path); // directory = @"path\to\your\source\code"
            return directory;
        }

        private const string SOLUTION_NAME = "ZonBot";
        
        public static string SolutionPath =>
            //return @"E:\Daten\Informatik\Programmierung (Neu)\C#\RiderProjects\ZonBot\ZonBot";
            Path.Combine("E:",
                "Daten",
                "Informatik",
                "Programmierung (Neu)",
                "C#",
                "RiderProjects",
                SOLUTION_NAME);

        private const string PROJECT_NAME = "ZonBot";

        public static string ProjectPath =>
            Path.Combine(SolutionPath, PROJECT_NAME);

        private const string CONFIG_FOLDER = "Configs";
        
        public static string ConfigPath =>
             Path.Combine(ProjectPath, CONFIG_FOLDER);
    }
}