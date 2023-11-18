using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SWAPS.Persistence
{
   public class PersistenceManager<C>
   {
      public const string DEFAULT_SAVEPATH = "config.json";

      public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings()
      {
         ObjectCreationHandling = ObjectCreationHandling.Replace
      };

      public string SerializeToFileContent(C config)
      {
         return JsonConvert.SerializeObject(config, Formatting.Indented, Settings);
      }

      public void PopulateFrom(string filecontent, C config)
      {
         JsonConvert.PopulateObject(filecontent, config, Settings);
      }

      public void Save(C config, String savePath = DEFAULT_SAVEPATH)
      {
         var dir = Path.GetDirectoryName(savePath);
         if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

         File.WriteAllText(savePath, SerializeToFileContent(config));
      }

      public C Load(C into, String savePath = DEFAULT_SAVEPATH)
      {
         if (!File.Exists(savePath))
            throw new FileNotFoundException($"Could not find file '{savePath}'");

         PopulateFrom(File.ReadAllText(savePath), into);
         return into;
      }
   }
}
