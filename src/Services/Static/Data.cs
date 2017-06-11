using DEA.Common.Items;
using DEA.Common.Utilities;
using Discord;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace DEA.Services.Static
{
    internal static class Data
    {
        public static Credentials Credentials { get; }
        public static Armour[] Armour { get; }
        public static Crate[] Crates { get; }
        public static CrateItem[] CrateItems { get; }
        public static Fish[] Fish { get; }
        public static Food[] Food { get; }
        public static Gun[] Guns { get; }
        public static Item[] Miscellanea { get; }
        public static Item[] Items { get; }
        public static Knife[] Knives { get; }
        public static Meat[] Meat { get; }
        public static Weapon[] Weapons { get; }
        public static int CrateItemOdds { get; }
        public static int FishAcquireOdds { get; }
        public static int MeatAcquireOdds { get; }

        static Data()
        {
            try
            {
                Credentials = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(Config.MainDirectory + @"src\Credentials.json"));
                Armour = JsonConvert.DeserializeObject<Armour[]>(File.ReadAllText(Config.MainDirectory + @"src\Data\Items\Armour.json"));
                Crates = JsonConvert.DeserializeObject<Crate[]>(File.ReadAllText(Config.MainDirectory + @"src\Data\Items\Crates.json"));
                Fish = JsonConvert.DeserializeObject<Fish[]>(File.ReadAllText(Config.MainDirectory + @"src\Data\Items\Fish.json"));
                Guns = JsonConvert.DeserializeObject<Gun[]>(File.ReadAllText(Config.MainDirectory + @"src\Data\Items\Guns.json"));
                Knives = JsonConvert.DeserializeObject<Knife[]>(File.ReadAllText(Config.MainDirectory + @"src\Data\Items\Knives.json"));
                Meat = JsonConvert.DeserializeObject<Meat[]>(File.ReadAllText(Config.MainDirectory + @"src\Data\Items\Meat.json"));
                Miscellanea = JsonConvert.DeserializeObject<Item[]>(File.ReadAllText(Config.MainDirectory + @"src\Data\Items\Miscellanea.json"));
            }
            catch (IOException e)
            {
                Logger.Log(LogSeverity.Critical, "Error while loading up data, please fix this issue and restart the bot", e.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }

            Food = (Fish as Food[]).Concat(Meat).ToArray();
            Weapons = (Guns as Weapon[]).Concat(Knives).ToArray();
            CrateItems = (Weapons as CrateItem[]).Concat(Armour).OrderByDescending(x => x.CrateOdds).ToArray();
            Items = Miscellanea.Concat(Crates).Concat(CrateItems).Concat(Food).ToArray();

            CrateItemOdds = CrateItems.Sum(x => x.CrateOdds);
            FishAcquireOdds = Fish.Sum(x => x.AcquireOdds);
            MeatAcquireOdds = Meat.Sum(x => x.AcquireOdds);
        }   
    }
}
