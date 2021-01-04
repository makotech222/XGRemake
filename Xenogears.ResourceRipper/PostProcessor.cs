using System;
using System.Collections.Generic;
using System.Text;
using Stride.Graphics;
//using Stride.Core;
//using Stride.Core.Assets;
//using Stride.Core.Yaml;
//using Stride.Core.Assets.Serializers;
using System.IO;
//using Stride.Core.Assets.Yaml;

namespace Xenogears.ResourceRipper
{
    /// <summary>
    /// Does any post processing to the ripped files. TODO: Will eventually be responsible for creating any Stride assets too, like SpriteSheets.
    /// </summary>
    public class PostProcessor
    {
        public void Start()
        {
            DeleteJunkFolders();
            RenameFolders();

            //Move to project root directory
            var rootDir = new DirectoryInfo(@"Output/XG_bin_iso/");
            Directory.Move(rootDir.FullName, Path.Combine(rootDir.FullName, "../../../../../../XenoRip"));

            var graphicsDevice = GraphicsDevice.New(DeviceCreationFlags.None, GraphicsProfile.Level_11_0);
            SpriteSheet s = new SpriteSheet();
        }

        /// <summary>
        /// Remove all the junk we don't currently use.
        /// </summary>
        private void DeleteJunkFolders()
        {
            Console.WriteLine("Deleting Junk Folders");
            var exclusionFolders = new List<string>()
            {
                "3921","3938","3380","2925","2617","605","426"
            };
            var directory = new DirectoryInfo(@"Output/XG_bin_iso/");
            foreach (var folder in directory.GetDirectories())
            {
                if (!exclusionFolders.Contains(folder.Name))
                    folder.Delete(true);
            }

            File.Delete(Path.Combine(directory.FullName, "toc.bin"));
            File.Delete(Path.Combine(directory.FullName, "toc.txt"));
        }

        private void RenameFolders()
        {
            Console.WriteLine("Renaming to friendly folder names");
            var rootDir = new DirectoryInfo(@"Output/XG_bin_iso/");
            Directory.Move(Path.Combine(rootDir.FullName, "426"), Path.Combine(rootDir.FullName, "FieldPCSprites1"));
            Directory.Move(Path.Combine(rootDir.FullName, "605/FieldNPCSprites"), Path.Combine(rootDir.FullName, "FieldNPCSprites"));
            Directory.Move(Path.Combine(rootDir.FullName, "605/MapTextures"), Path.Combine(rootDir.FullName, "MapTextures"));
            Directory.Delete(Path.Combine(rootDir.FullName, "605"), true);
            Directory.Move(Path.Combine(rootDir.FullName, "2617"), Path.Combine(rootDir.FullName, "EnemyBattleSprites"));
            Directory.Move(Path.Combine(rootDir.FullName, "2925"), Path.Combine(rootDir.FullName, "PCBattleSprites1"));
            Directory.Move(Path.Combine(rootDir.FullName, "3380"), Path.Combine(rootDir.FullName, "PCBattleSprites2"));
            Directory.Move(Path.Combine(rootDir.FullName, "3921"), Path.Combine(rootDir.FullName, "FieldPCSprites2"));
            Directory.Move(Path.Combine(rootDir.FullName, "3938"), Path.Combine(rootDir.FullName, "FieldGearSprites"));
        }

        //private static string SerializeAsString(object instance, YamlAssetMetadata<Guid> objectReferences)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        var metadata = new AttachedYamlAssetMetadata();
        //        if (objectReferences != null)
        //        {
        //            metadata.AttachMetadata(AssetObjectSerializerBackend.ObjectReferencesKey, objectReferences);
        //        }

        //        new YamlAssetSerializer().Save(stream, instance, metadata);
        //        stream.Flush();
        //        stream.Position = 0;
        //        return new StreamReader(stream).ReadToEnd();
        //    }
        //}

        //private static object Deserialize(string yaml)
        //{
        //    var stream = new MemoryStream();
        //    var writer = new StreamWriter(stream);
        //    writer.Write(yaml);
        //    writer.Flush();
        //    stream.Position = 0;
        //    bool aliasOccurred;
        //    AttachedYamlAssetMetadata metadata;
        //    var instance = new YamlAssetSerializer().Load(stream, "MyAsset", null, true, out aliasOccurred, out metadata);
        //    return instance;
        //}
    }
}
