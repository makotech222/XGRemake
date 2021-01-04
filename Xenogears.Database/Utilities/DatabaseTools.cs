using System;
using System.IO;
using System.Linq;

namespace Xenogears.Database
{
    internal static class DatabaseTools
    {
        /// <summary>
        /// Workaround for relative paths not working for database connection strings.
        /// </summary>
        private static string DBPath = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.GetDirectories().First(x => x.Name == "Xenogears").FullName, "Database.mdf");
        public static string SQLConnection = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""{DBPath}"";Integrated Security=True;Connect Timeout=30";
    }
}