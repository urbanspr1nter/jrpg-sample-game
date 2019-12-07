using System.Collections;
using System.Collections.Generic;
using Jrpg.System;
using Jrpg.InventorySystem.PgItems;
using Jrpg.PartySystem;
using UnityEngine;

public class MainGame
{
    public static Vector2 PreviousPosition = new Vector2(0, 0);
    public static bool Initialized { get; private set; }

    public static Party Party()
    {
        return Store().MainParty;
    }

    public static Dictionary<string, List<DropSource>> DropSourcesDb()
    {
        return Store().Get<Dictionary<string, List<DropSource>>>("DropSourcesDb");
    }

    public static ItemGenerator ItemGenerator()
    {
        return Store().Get<ItemGenerator>("ItemGenerator");
    }

    public static void Log(string message)
    {
        if (Store().Get<List<string>>("Logger").Count == 0)
        {
            Store().Get<List<string>>("Logger").Add(message);
            return;
        }
        Store().Get<List<string>>("Logger").Insert(0, message);
    }

    public static List<string> TopLog()
    {
        var size = Store().Get<List<string>>("Logger").Count;
        if (size > 8)
        {
            size = 8;
        }
        return Store().Get<List<string>>("Logger").GetRange(0, size);
    }

    public static GameStore Store()
    {
        if (!Initialized)
        {
            GameStore.GetInstance().LoadConfig(Resources.Load<TextAsset>("Configuration").text);

            var PgItemsSources = GameStore.GetInstance().Config.Get<PgItemsSources>("PgItemsSources");

            GameStore.GetInstance().Put("ItemsDb", Newtonsoft.Json.JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>(PgItemsSources.ItemsPath).text));
            GameStore.GetInstance().Put("PrefixesDb", Newtonsoft.Json.JsonConvert.DeserializeObject<List<Affix>>(Resources.Load<TextAsset>(PgItemsSources.PrefixesPath).text));
            GameStore.GetInstance().Put("SuffixesDb", Newtonsoft.Json.JsonConvert.DeserializeObject<List<Affix>>(Resources.Load<TextAsset>(PgItemsSources.SuffixesPath).text));
            GameStore.GetInstance().Put("DropSourcesDb", new Dictionary<string, List<DropSource>>());

            foreach (var dropSourcePath in PgItemsSources.DropSourcesPath.Keys)
            {
                GameStore.GetInstance().Get<Dictionary<string, List<DropSource>>>("DropSourcesDb")
                    .Add(dropSourcePath, Newtonsoft.Json.JsonConvert.DeserializeObject<List<DropSource>>(Resources.Load<TextAsset>(PgItemsSources.DropSourcesPath[dropSourcePath]).text));
            }

            GameStore.GetInstance().Put("ItemGenerator", new ItemGenerator(
                GameStore.GetInstance().Get<List<Item>>("ItemsDb"),
                GameStore.GetInstance().Get<List<Affix>>("PrefixesDb"),
                GameStore.GetInstance().Get<List<Affix>>("SuffixesDb")
            ));


            // Add Member Tags
            GameStore.GetInstance().Put("AlexaTag", GameStore.GetInstance().Config.Get<List<string>>("MemberTags")[0]);
            GameStore.GetInstance().Put("RogerTag", GameStore.GetInstance().Config.Get<List<string>>("MemberTags")[1]);

            // Initialize Log
            GameStore.GetInstance().Put("Logger", new List<string>());

            Initialized = true;
        }

        return GameStore.GetInstance();
    }
}
