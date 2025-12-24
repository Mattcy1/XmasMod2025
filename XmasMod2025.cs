using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.Hooks;
using BTD_Mod_Helper.Api.Hooks.BloonHooks;
using BTD_Mod_Helper.Api.Internal;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.MapEditor;
using Il2CppAssets.Scripts.Data.MapSets;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.ContentBrowser;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.Map;
using Il2CppAssets.Scripts.Unity.Scenes;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using XmasMod2025;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Bfbs;
using XmasMod2025.Bloons.Moabs;
using XmasMod2025.Bloons.Zomgs;
using XmasMod2025.Bosses;
using XmasMod2025.GiftShop;
using XmasMod2025.GiftShop.BuffsItems;
using XmasMod2025.Towers;
using XmasMod2025.Towers.SubTowers;
using XmasMod2025.Towers.Upgrades;
using XmasMod2025.UI;
using static MelonLoader.MelonLogger;
using static XmasMod2025.UI.Gift;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Map = Il2CppAssets.Scripts.Simulation.Track.Map;
using Random = System.Random;

[assembly: MelonInfo(typeof(XmasMod2025.XmasMod2025), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace XmasMod2025;

public partial class XmasMod2025 : BloonsTD6Mod
{
    public delegate void CurrencyChangedDelegate(double amount);


    private static string PlayerName;
    public static string lastMap;

    public static float PresentBloonChance = 1f;
    public static IntMinMax TreeDropRates = new(3, 5);
    public static int FestiveSpritActiveRounds;
    public static bool FestiveSpiritActive = false;
    public static double GiftMult = 1;
    public static Tower FestiveSpiritTower;
    public static List<TowerModel> GiftOfGivingTowersIds = new();
    public static Bloon boss = null;
    public static int UpgradeCount = 0;
    public static bool KrampusAlive = false;

    private static double gifts;

    private static double snowflakes;

    public static double totalGifts;

    public static Assembly ModAssembly => Assembly.GetExecutingAssembly();
    public static List<MapEditorProp> MapEditorProps => GameData.Instance.mapEditorData.mapEditorProps.ToList();

    public static double Snowflakes
    {
        get => snowflakes;
        set
        {
            var increase = value - snowflakes;
            snowflakes = value;

            OnSnowflakesUpdated?.Invoke(increase);

            if (increase > 0) TotalSnowflakes += increase;
        }
    }

    public static double Gifts
    {
        get => gifts;
        set
        {
            var increase = (value - gifts) * GiftMult;
            gifts += increase;

            OnGiftsUpdated?.Invoke(value);

            if (increase > 0)
            {
                totalGifts += increase;

                if (boss != null)
                    if (boss.bloonModel.baseId == ModContent.BloonID<GiftBoss>())
                        boss.health += (int)(increase *= 3);
            }
        }
    }

    public static double TotalGifts => totalGifts;

    public static double TotalSnowflakes { get; private set; }

    public static void Log(object msg)
    {
        ModHelper.Log<XmasMod2025>(msg);
    }

    public static T GetEmbeddedModelFromJson<T>(string resourceName) where T : Model
    {
        return ModelSerializer.DeserializeModel<T>(LoadEmbeddedJson(resourceName));
    }

    public static T GetEmbeddedObjectFromJson<T>(string resourceName) where T : class
    {
        return Il2CppJsonConvert.DeserializeObject(LoadEmbeddedJson(resourceName)).Cast<T>();
    }

    public static void PlacePropsFromEditorData(List<MapEditorPropData> props)
    {
        foreach (var prop in props)
        {
            var propName = "Prop_" + props.IndexOf(prop);

            InGame.instance.bridge.CreateMapEditorPropAt(propName, prop.positionalData, prop.Def().propModel,
                ObjectId.FromString(prop.Def().propModel.name + "_" + props.IndexOf(prop)), null);
        }
    }

    public static void PlaySound(string name)
    {
        ModContent.GetAudioClip<XmasMod2025>(name).Play();
    }

    public static event CurrencyChangedDelegate? OnGiftsUpdated;
    public static event CurrencyChangedDelegate? OnSnowflakesUpdated;

    public override void OnTitleScreen()
    {
        PostProcessing.Load();
    }

    public static double GetCurrency(CurrencyType currency)
    {
        switch (currency)
        {
            case CurrencyType.Gift:
                return Gifts;
            case CurrencyType.Cash:
                return InGame.instance.GetCash();
            case CurrencyType.Snowflake:
                return Snowflakes;
            default:
                return Gifts;
        }
    }

    public static void SetCurrency(CurrencyType currency, double amount)
    {
        switch (currency)
        {
            case CurrencyType.Gift:
                Gifts = amount;
                return;
            case CurrencyType.Cash:
                InGame.instance.SetCash(amount);
                return;
            case CurrencyType.Snowflake:
                Snowflakes = amount;
                return;
            default:
                Gifts = amount;
                return;
        }
    }

    public static void AddCurrency(CurrencyType currency, double amount)
    {
        switch (currency)
        {
            case CurrencyType.Gift:
                Gifts += amount;
                return;
            case CurrencyType.Cash:
                InGame.instance.AddCash(amount);
                return;
            case CurrencyType.Snowflake:
                Snowflakes += amount;
                return;
            default:
                Gifts += amount;
                return;
        }
    }

    public override void OnRestart()
    {
        if (InGame.Bridge == null)
            return;

        if (InGame.Bridge.IsSandboxMode())
        {
            SetCurrency(CurrencyType.Gift, 99999);
            totalGifts = 99999;
        }
        else
        {
            SetCurrency(CurrencyType.Gift, 25);
            totalGifts = 25;
        }

        foreach (var item in ModContent.GetContent<GiftShopItem>()) item.Reset();

        TookWallet = false;
        
        AddDialogue();
    }

    public override void OnNewGameModel(GameModel result)
    {
        foreach (var item in ModContent.GetContent<GiftShopItem>()) item.Reset();
    }

    public override void OnMainMenu()
    {
        foreach (var item in ModContent.GetContent<GiftShopItem>()) item.Reset();

        PlayerName = Game.instance.GetBtd6Player().LiNKAccount.DisplayName;
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<FestiveSpiritTower>()) FestiveSpiritTower = tower;
    }

    public override void OnTowerDestroyed(Tower tower)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<ElfHelper>())
            InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<GiftEffect>(),
                tower.Position, 0, 2, 120);

        if (tower.towerModel.baseId == ModContent.TowerID<FestiveSpiritTower>()) FestiveSpiritTower = null;
    }

    public override void OnRoundEnd()
    {
        BuffHandler.GiftOfGivingHandler(false);

        if (FestiveSpritActiveRounds > 0) FestiveSpritActiveRounds--;

        if (FestiveSpiritActive)
            if (FestiveSpritActiveRounds <= 0)
                if (FestiveSpiritTower != null)
                {
                    FestiveSpiritTower.worth = 0;
                    FestiveSpiritTower.SellTower();
                }

        foreach (var tower in InGame.instance.GetTowers())
        {
            if (tower.towerModel.baseId == ModContent.TowerID<ElfSpawner>()) tower.SellTower();

            if (tower.towerModel.baseId == ModContent.TowerID<ElfMonkey>())
                if (tower.towerModel.tiers[2] == 5)
                {
                    AddCurrency(CurrencyType.Gift, 150);

                    if (InGame.instance != null || InGame.instance.bridge != null)
                        InGame.instance.bridge.simulation.CreateTextEffect(tower.Position,
                            ModContent.CreatePrefabReference<CollectText>(), 2f, "+150 Gifts", true);
                }
        }
    }

    public static string LoadEmbeddedJson(string resourceName)
    {
        var stream = ModAssembly.GetEmbeddedResource(resourceName);

        if (stream == null)
        {
            Msg("Embedded resource not found: " + resourceName + ", valid names are the following:");
            foreach (var name in ModAssembly.GetManifestResourceNames()) Msg(name);
            return null;
        }

        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /*public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        var modTower = tower.towerModel.GetModTower();

        if (modTower != null && modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash)
        {
        }
    }*/

    public static IEnumerator HandleGPS()
    {
        while (UpgradeCount >= 1)
        {
            var gps = totalGifts * (UpgradeCount / 1500f);

            AddCurrency(CurrencyType.Gift, gps);

            yield return new WaitForSeconds(1f);
        }
    }
    public class GiftEffect : ModDisplay
    {
        public override string BaseDisplay => "6d84b13b7622d2744b8e8369565bc058";

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            node.genericRenderers[0].SetMainTexture(GetTexture<XmasMod2025>("blank"));
            node.genericRenderers[3].SetMainTexture(GetTexture<XmasMod2025>("Gift1"));
            node.genericRenderers[2].SetMainTexture(GetTexture<XmasMod2025>("Gift1"));
            node.genericRenderers[1].SetMainTexture(GetTexture<XmasMod2025>("blank"));
            node.genericRenderers[2].GetComponent<ParticleSystem>().startSpeed *= 0.2f;
        }
    }

    public class GiftEffectButBig : ModDisplay
    {
        public override string BaseDisplay => "6d84b13b7622d2744b8e8369565bc058";
        public override float Scale => 100;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            node.genericRenderers[0].SetMainTexture(GetTexture<XmasMod2025>("blank"));
            node.genericRenderers[3].SetMainTexture(GetTexture<XmasMod2025>("Gift1"));
            node.genericRenderers[2].SetMainTexture(GetTexture<XmasMod2025>("Gift1"));
            node.genericRenderers[1].SetMainTexture(GetTexture<XmasMod2025>("blank"));
            node.genericRenderers[2].GetComponent<ParticleSystem>().startSpeed *= 0.2f;
        }
    }

    #region Saving

    [HarmonyPatch(typeof(Map), nameof(Map.GetSaveData))]
    private static class Map_GetSaveData
    {
        [HarmonyPostfix]
        public static void Postfix(MapSaveDataModel mapData)
        {
            var json = JsonConvert.SerializeObject(TookWallet);
            if (!mapData.metaData.TryAdd("XmasMod2025-TookWallet", json))
                mapData.metaData["XmasMod2025-TookWallet"] = json;

            json = JsonConvert.SerializeObject(Gifts);
            if (!mapData.metaData.TryAdd("XmasMod2025-Gifts", json))
                mapData.metaData["XmasMod2025-Gifts"] = json;

            json = JsonConvert.SerializeObject(TotalGifts);
            if (!mapData.metaData.TryAdd("XmasMod2025-TotalGifts", json))
                mapData.metaData["XmasMod2025-TotalGifts"] = json;

            var BoughtItems = new Dictionary<string, int>();
            foreach (var giftShopItem in ModContent.GetContent<GiftShopItem>().Where(i => i.Shop == ShopType.Gift))
                BoughtItems.Add(giftShopItem.Id, giftShopItem.Upgrades);

            json = JsonConvert.SerializeObject(BoughtItems);
            if (!mapData.metaData.TryAdd("XmasMod2025-BoughtItems", json))
                mapData.metaData["XmasMod2025-BoughtItems"] = json;
        }
    }

    [HarmonyPatch(typeof(Map), nameof(Map.SetSaveData))]
    private static class Map_SetSaveData
    {
        [HarmonyPostfix]
        public static void Postfix(MapSaveDataModel mapData)
        {
            if (mapData.metaData.TryGetValue("XmasMod2025-TookWallet", out var data))
                TookWallet = JsonConvert.DeserializeObject<bool>(data)!;

            if (mapData.metaData.TryGetValue("XmasMod2025-Gifts", out data))
                Gifts = JsonConvert.DeserializeObject<double>(data)!;

            if (mapData.metaData.TryGetValue("XmasMod2025-TotalGifts", out data))
                totalGifts = JsonConvert.DeserializeObject<double>(data)!;

            if (mapData.metaData.TryGetValue("XmasMod2025-BoughtItems", out data))
            {
                var BoughtItems = JsonConvert.DeserializeObject<Dictionary<string, int>>(data)!;
                foreach (var (itemName, upgrades) in BoughtItems)
                {
                    var item = ModContent.GetContent<GiftShopItem>().Find(i => i.Id == itemName)!;

                    for (var i = 0; i < upgrades; i++) item.Buy(InGame.instance);
                    item.Upgrades = upgrades;
                }
            }
        }
    }

    #endregion
}

[HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.Start))]
public class CreateMap
{
    [HarmonyPostfix]
    public static void Postfix(TitleScreen __instance)
    {
        var maps = GameData.Instance.mapSet.Maps.items;

        var customMap = new MapDetails
        {
            id = "Xmas Cubism",
            coopMapDivisionType = CoopDivision.FREE_FOR_ALL,
            difficulty = MapDifficulty.Beginner,
            mapSprite = ModContent.GetSpriteReference<XmasMod2025>("MapIcon"),
            hasWater = true,
            theme = MapTheme.Snow,
            unlockDifficulty = MapDifficulty.Beginner
        };

        var list = GameData.Instance.mapSet.Maps.items.ToList();
        list.Insert(0, customMap);

        GameData.Instance.mapSet.Maps.items = list.ToArray();
    }
}

//Credit goes to Timotheeee: https://github.com/Timotheeee/btd6_mods/blob/master/custom_maps_v2/Main.cs#L406
[HarmonyPatch(typeof(MapLoader), nameof(MapLoader.LoadScene))]
public class LoadMap
{
    [HarmonyPrefix]
    internal static bool Fix(ref MapLoader __instance)
    {
        XmasMod2025.lastMap = __instance.currentMapName;
        if (XmasMod2025.lastMap == "Xmas Cubism") __instance.currentMapName = "Cubism";

        return true;
    }
}

[HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
public class ChangeMap
{
    [HarmonyPostfix]
    public static void Postfix(InGame __instance)
    {
        if (GiftOpenerUI.instance == null) GiftOpenerUI.CreatePanel();

        if (GiftCounterUI.instance == null) GiftCounterUI.CreatePanel();

        if (__instance.bridge.IsSandboxMode())
        {
            XmasMod2025.SetCurrency(CurrencyType.Gift, 99999);
            XmasMod2025.totalGifts = 99999;
        }
        else
        {
            XmasMod2025.SetCurrency(CurrencyType.Gift, 25);
            XmasMod2025.totalGifts = 25;
        }


        var terrain = GameObject.Find("CubismTerrain");

        if (terrain != null && XmasMod2025.lastMap == "Xmas Cubism")
        {
            terrain.GetComponent<MeshRenderer>().material.mainTexture = ModContent.GetTexture<XmasMod2025>("XmasMap");
            var data = XmasMod2025.LoadEmbeddedJson("XmasCubismProps.json");
            var lines = JsonSerializer.Deserialize<List<string>>(data);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = Regex.Match(line,
                    @"^(.*?),\s*(\d+),\s*(-?[\d\.]+),\s*\(([\d\.-]+),\s*([\d\.-]+),\s*([\d\.-]+)\),\s*(-?[\d\.]+),\s*(-?[\d\.]+)$");
                if (!match.Success)
                    continue;

                var propName = match.Groups[1].Value;
                var propId = int.Parse(match.Groups[2].Value);
                var rot = float.Parse(match.Groups[3].Value);
                var x = float.Parse(match.Groups[7].Value);
                var y = float.Parse(match.Groups[8].Value);

                var positionalData = new PositionalData
                {
                    //scale = float.Parse(match.Groups[6].Value),
                    rotation = new Vector3(0, 0, rot),
                    position = new Vector3(x, 0, y)
                };

                foreach (var p in XmasMod2025.MapEditorProps)
                    if (p.Def().id == propId)
                        InGame.instance.bridge.CreateMapEditorPropAt(propName, positionalData, p.Def(), default, null);
            }
        }
    }
}

[HarmonyPatch(typeof(TimeTrigger), nameof(TimeTrigger.Trigger))]
public class TimeTriggerPatch
{
    [HarmonyPostfix]
    public static void Postfix(TimeTrigger __instance)
    {
        if (__instance.timeTriggerModel.name.Contains("SnowBloonStun"))
            __instance.bloon.bloonModel.RemoveBehavior<TimeTriggerModel>();

        if (__instance.timeTriggerModel.name.Contains("ElfTax"))
            InGame.instance.AddCash(-(InGame.instance.GetCash() * 0.05f));

        if (__instance.timeTriggerModel.name.Contains("GrinchHeal"))
            XmasMod2025.boss.health += (int)(XmasMod2025.boss.health * 0.05f);

        if (__instance.bloon.bloonModel.baseId == ModContent.BloonID<CoalTotem>())
        {
            var rnd = new Random();

            switch (rnd.Next(3))
            {
                case 0:
                    InGame.instance.SpawnBloons(ModContent.BloonID<IceZomg>(), 1, 0);
                    break;
                case 1:
                    InGame.instance.SpawnBloons(ModContent.BloonID<ChocoZomg>(), 1, 0);
                    break;
                case 2:
                    InGame.instance.SpawnBloons(ModContent.BloonID<SnowZomg>(), 1, 0);
                    break;
            }
        }
    }
}

[HarmonyPatch(typeof(Map), nameof(Map.GetSaveData))]
internal static class Map_GetSaveData
{
    [HarmonyPostfix]
    public static void Postfix(MapSaveDataModel mapData)
    {
        var json = JsonConvert.SerializeObject(XmasMod2025.TookWallet);
        if (!mapData.metaData.TryAdd("XmasMod2025-TookWallet", json)) mapData.metaData["XmasMod2025-TookWallet"] = json;

        json = JsonConvert.SerializeObject(XmasMod2025.Gifts);
        if (!mapData.metaData.TryAdd("XmasMod2025-Gifts", json)) mapData.metaData["XmasMod2025-Gifts"] = json;
        json = JsonConvert.SerializeObject(XmasMod2025.TotalGifts);
        if (!mapData.metaData.TryAdd("XmasMod2025-TotalGifts", json)) mapData.metaData["XmasMod2025-TotalGifts"] = json;

        var BoughtItems = new Dictionary<string, int>();
        foreach (var giftShopItem in ModContent.GetContent<GiftShopItem>().Where(i => i.Shop == ShopType.Gift))
            BoughtItems.Add(giftShopItem.Id, giftShopItem.Upgrades);

        json = JsonConvert.SerializeObject(BoughtItems);
        if (!mapData.metaData.TryAdd("XmasMod2025-BoughtItems", json))
            mapData.metaData["XmasMod2025-BoughtItems"] = json;
    }
}