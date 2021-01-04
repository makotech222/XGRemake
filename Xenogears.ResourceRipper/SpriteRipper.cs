using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Xenogears.ResourceRipper
{
    /// <summary>
    /// Rips all the sprites that we can use.
    /// </summary>
    public class SpriteRipper
    {
        private readonly string _pathToOutput = @"Output/";
        private readonly string _pathToTools = @"Tools/";
        private readonly int _maxDegreeOfParallel = Environment.ProcessorCount - 1;

        public void Start()
        {
            BattleSprites();
            FieldNPCSprites();
            MapTextureSprites();
            FieldPlayerSprites();
        }

        private void BattleSprites()
        {
            var battlesprites = new List<string>() { "3921", "3938", "3380", "2925", "2617" };
            foreach (var folder in battlesprites)
            {
                Console.WriteLine($"Extracting sprites for folder {folder}");
                var currentDir = Environment.CurrentDirectory;
                var binFiles = new DirectoryInfo($@"{_pathToOutput}XG_bin_iso/{folder}").GetFiles("*.bin");
                Parallel.ForEach(binFiles, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallel }, bin =>
                {
                    Console.WriteLine($"Extracting bin {bin.Name}");
                    using (var process = new Process())
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = @"powershell.exe";
                        startInfo.Arguments = $@"& php.exe '{_pathToTools}xeno_battle_2d.php' '{_pathToOutput}XG_bin_iso/{folder}/{bin.Name}'";
                        startInfo.RedirectStandardOutput = true;
                        startInfo.RedirectStandardError = true;
                        startInfo.UseShellExecute = false;
                        startInfo.CreateNoWindow = true;
                        process.StartInfo = startInfo;
                        process.Start();

                        string output = process.StandardOutput.ReadToEnd();
                        string errors = process.StandardError.ReadToEnd();
                    }
                    var newDirectory = new DirectoryInfo(Path.Combine(bin.DirectoryName, Path.GetFileNameWithoutExtension(bin.FullName) + "_bin"));
                    if (!Directory.Exists(newDirectory.FullName))
                    {
                        return;
                    }
                    var rgbaFiles = newDirectory.GetFiles("*.rgba",SearchOption.AllDirectories);
                    foreach (var rbga in rgbaFiles)
                    {
                        Tools.clut2bmp.Execute(rbga.FullName);
                        File.Delete(rbga.FullName);
                    }
                    var metaFiles = newDirectory.GetFiles("*.meta", SearchOption.AllDirectories);
                    foreach (var file in metaFiles)
                    {
                        File.Delete(file.FullName);
                    }
                });
                //foreach (var bin in binFiles)
                //    File.Delete(bin.FullName);
            }
        }

        private void FieldNPCSprites()
        {
            Console.WriteLine($"Extracting Field NPC sprites for folder 605");
            var mapDirectory = new DirectoryInfo($@"{_pathToOutput}XG_bin_iso/605");
            Directory.CreateDirectory(Path.Combine(mapDirectory.FullName, "FieldNPCSprites"));
            var binFiles = mapDirectory.GetFiles("*.bin");
            Console.WriteLine($"Extracting binaries");
            Parallel.ForEach(binFiles.Batch(500), new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallel }, binCollection =>
            {
                using (var process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"powershell.exe";
                    startInfo.Arguments = $@"& php.exe '{_pathToTools}xeno_map2battle.php' {String.Join(' ', binCollection.Select(x => $"'{_pathToOutput}XG_bin_iso/605/{x.Name}'"))}";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                }
            });

            Console.WriteLine($"Extracting sub binaries");
            var currentDir = Environment.CurrentDirectory;
            var subBinFiles = mapDirectory.EnumerateDirectories().SelectMany(y => y.GetFiles("*.bin", SearchOption.AllDirectories));
            Parallel.ForEach(subBinFiles.Batch(500), new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallel }, binCollection =>
            {
                using (var process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"powershell.exe";
                    startInfo.Arguments = $@"& php.exe '{_pathToTools}xeno_battle_2d.php' {String.Join(' ', binCollection.Select(x => $"'{x.FullName.Substring(currentDir.Length + 1)}'"))}";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                }
                //foreach (var bin in binCollection)
                //    File.Delete(bin.FullName);
            });
            Console.WriteLine($"Converting to png.");
            var rgbaFiles = mapDirectory.GetFiles("*.rgba", SearchOption.AllDirectories);
            Parallel.ForEach(rgbaFiles, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallel }, rbga =>
            {
                Tools.clut2bmp.Execute(rbga.FullName);
            });
            foreach (var dir in mapDirectory.GetDirectories("*bin"))
            {
                var subdirs = dir.GetDirectories();
                foreach (var subdir in subdirs)
                {
                    var outputPath = Path.Combine(subdir.Parent.Parent.FullName, "FieldNPCSprites", dir.Name + subdir.Name);
                    if (Directory.Exists(outputPath))
                        Directory.Delete(outputPath,true);
                    Directory.Move(subdir.FullName, outputPath);
                }
                //Directory.Delete(dir.FullName,true);
            }

            foreach (var meta in mapDirectory.GetFiles("*.meta", SearchOption.AllDirectories))
                File.Delete(meta.FullName);
            foreach (var dec in mapDirectory.GetFiles("*.dec", SearchOption.AllDirectories))
                File.Delete(dec.FullName);
            foreach (var dec in mapDirectory.GetFiles("*.rgba", SearchOption.AllDirectories))
                File.Delete(dec.FullName);

        }

        private void MapTextureSprites()
        {
            Console.WriteLine($"Extracting Map Texture sprites for folder 605");
            var mapDirectory = new DirectoryInfo($@"{_pathToOutput}XG_bin_iso/605");
            Directory.CreateDirectory(Path.Combine(mapDirectory.FullName, "MapTextures"));

            var binFiles = mapDirectory.GetFiles("*.bin");
            Console.WriteLine($"Extracting binaries");
            foreach (var binCollection in binFiles.Batch(200))
            {
                using (var process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"powershell.exe";
                    startInfo.Arguments = $@"& php.exe '{_pathToTools}xeno_1201_vram.php' {String.Join(' ', binCollection.Select(x => $"'{_pathToOutput}XG_bin_iso/605/{x.Name}'"))}";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                }
            }

            Console.WriteLine($"Converting to png.");
            var currentDir = Environment.CurrentDirectory;
            var rgbaFiles = mapDirectory.GetFiles("*.clut", SearchOption.AllDirectories);
            foreach (var rbga in rgbaFiles)
            {
                Tools.clut2bmp.Execute(rbga.FullName);
            }

            foreach (var subDir in mapDirectory.GetDirectories("*bin"))
            {
                var outputPath = Path.Combine(mapDirectory.FullName, "MapTextures", subDir.Name);
                if (Directory.Exists(outputPath))
                    Directory.Delete(outputPath, true);
                subDir.MoveTo(outputPath);
            }
            foreach (var bin in mapDirectory.GetFiles("*.bin"))
            {
                //File.Delete(bin.FullName);
            }
            foreach (var bin in mapDirectory.GetFiles("*.rgba"))
            {
                File.Delete(bin.FullName);
            }
            foreach (var bin in mapDirectory.GetFiles("*.clut", SearchOption.AllDirectories))
            {
                File.Delete(bin.FullName);
            }
        }

        private void FieldPlayerSprites()
        {
            Console.WriteLine($"Extracting Field Player sprites");
            var mapDirectory = new DirectoryInfo($@"{_pathToOutput}XG_bin_iso/426");

            var binFiles = mapDirectory.GetFiles("*.bin");
            Console.WriteLine($"Decompressing binaries");
            foreach (var binCollection in binFiles.Batch(200))
            {
                using (var process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"powershell.exe";
                    startInfo.Arguments = $@"& php.exe '{_pathToTools}xeno_decode.php' {String.Join(' ', binCollection.Select(x => $"'{_pathToOutput}XG_bin_iso/426/{x.Name}'"))}";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                }
            }
            var bakFiles = mapDirectory.GetFiles("*.bak", SearchOption.AllDirectories);
            foreach (var dec in bakFiles)
                File.Delete(dec.FullName);

            Console.WriteLine($"Extracting binaries");
            binFiles = mapDirectory.GetFiles("*.bin");
            Parallel.ForEach(binFiles, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallel }, bin =>
            {
                Console.WriteLine($"Extracting bin {bin.Name}");
                using (var process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"powershell.exe";
                    startInfo.Arguments = $@"& php.exe '{_pathToTools}xeno_battle_2d.php' '{_pathToOutput}XG_bin_iso/426/{bin.Name}'";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                }
                var newDirectory = new DirectoryInfo(Path.Combine(bin.DirectoryName, Path.GetFileNameWithoutExtension(bin.FullName) + "_bin"));
                if (!Directory.Exists(newDirectory.FullName))
                {
                    return;
                }
                var rgbaFiles = newDirectory.GetFiles("*.rgba");
                foreach (var rbga in rgbaFiles)
                {
                    Tools.clut2bmp.Execute(rbga.FullName);
                    rbga.Delete();
                }
                foreach (var file in newDirectory.GetFiles("*.meta"))
                {
                    File.Delete(file.FullName);
                }
            });
        }
    }
}