using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Xenogears.ResourceRipper
{
    public class ISOExtractor
    {
        private readonly string _pathToTools = @"Tools/";
        private readonly string _pathToOutput = @"Output/";

        public void Start()
        {
            if (Directory.Exists($@"{_pathToOutput}/XG_bin_iso"))
            {
                Console.WriteLine("ISO extracted data already exists. Skipping ISO Extractor.");
                return;
            }
            Console.WriteLine("Drag and drop the Xenogears NA .bin file to this window to begin.");
            var binPath = Console.ReadLine().Trim('"');
            if (!File.Exists(binPath))
            {
                Console.WriteLine("File not found.");
                throw new Exception("No File Found");
            }
            var copiedPath = $@"{_pathToOutput}XG.bin";
            Directory.CreateDirectory(_pathToOutput);

            Console.WriteLine("Copying bin to local directory");
            File.Copy(binPath, copiedPath,true);

            Console.WriteLine("Converting bin file to iso file.");
            using (var process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"powershell.exe";
                startInfo.Arguments = $@"& php.exe '{_pathToTools}psxbin2iso.php' '{copiedPath}'";
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
            }
            File.Delete(copiedPath);
            if (!File.Exists(Path.Combine(_pathToOutput, "XG.bin.iso")))
                throw new Exception("Binary file was not extracted. Make sure PHP is installed.");
            Console.WriteLine("Extracting ISO contents.");
            using (var process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"powershell.exe";
                startInfo.Arguments = $@"& php.exe '{_pathToTools}psxiso_hidden.php' '{_pathToOutput}XG.bin.iso'";
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
            }
            File.Delete($"{_pathToOutput}XG.bin.iso");
        }
    }
}