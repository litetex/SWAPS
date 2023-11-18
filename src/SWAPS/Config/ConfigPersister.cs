using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPS.Config;
using SWAPS.Persistence;

namespace SWAPS.Config
{
   public class ConfigPersister : PersistenceManager<Configuration>
   {
      public static ConfigPersister Instance { get; } = new ConfigPersister();
   }
}
