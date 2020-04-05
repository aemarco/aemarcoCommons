using System.Collections.Generic;

namespace Contracts.Api.RequestObjects
{
    public class CheatCode
    {
        public string Key { get; set; }
        public int MaxAdult { get; set; }
        public bool IsSupervisor { get; set; }



        public static List<CheatCode> Cheats { get; } = new List<CheatCode>
        {
            new CheatCode{ Key = "idkfa", MaxAdult = 101, IsSupervisor = true },
            new CheatCode{ Key = "idfa" ,MaxAdult = 79, IsSupervisor = false },
            new CheatCode{ Key = "idka" ,MaxAdult = 59, IsSupervisor = false }
        };
    }
}
