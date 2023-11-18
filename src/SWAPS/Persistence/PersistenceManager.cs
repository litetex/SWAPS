using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace SWAPS.Persistence
{
   public class PersistenceManager<C>
   {
      public const string DEFAULT_SAVEPATH = "config.json";

      public JsonSerializerOptions Settings { get; set; } = new JsonSerializerOptions()
      {
         WriteIndented = true
      };

      public PersistenceManager(IJsonTypeInfoResolver typeInfoResolver)
      {
         Settings.TypeInfoResolver = typeInfoResolver;
      }

      public string SerializeToFileContent(C config)
      {
         return JsonSerializer.Serialize<C>(config, Settings);
      }

      public C DeserializeFromFileContent(string filecontent)
      {
         return JsonSerializer.Deserialize<C>(filecontent, Settings);
      }

      public void Save(C config, string savePath = DEFAULT_SAVEPATH)
      {
         var dir = Path.GetDirectoryName(savePath);
         if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

         File.WriteAllText(savePath, SerializeToFileContent(config));
      }

      public C Load(string savePath = DEFAULT_SAVEPATH)
      {
         if (!File.Exists(savePath))
            throw new FileNotFoundException($"Could not find file '{savePath}'");

         return DeserializeFromFileContent(File.ReadAllText(savePath));
      }
   }
}
