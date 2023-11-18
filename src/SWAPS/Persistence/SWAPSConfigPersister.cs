using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPS.Config;

namespace SWAPS.Persistence
{
   public class SWAPSConfigPersister : PersistenceManager<Configuration>
   {
      public static SWAPSConfigPersister Instance { get; } = new SWAPSConfigPersister();
   }
}
