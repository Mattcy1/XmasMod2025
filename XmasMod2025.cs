using System;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.Hooks;
using BTD_Mod_Helper.Api.Hooks.BloonHooks;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.MapEditor;
using Il2CppAssets.Scripts.Data.MapSets;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.ContentBrowser;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Track;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.Map;
using Il2CppAssets.Scripts.Unity.Scenes;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Utils;
using MelonLoader;
using Octokit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BTD_Mod_Helper.Api.Internal;
using CommandLine;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.MapEditorBehaviors;
using MelonLoader.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;
using XmasMod2025;
using XmasMod2025.Bloons;
using XmasMod2025.GiftShop.BuffsItems;
using XmasMod2025.Towers;
using XmasMod2025.Towers.SubTowers;
using XmasMod2025.UI;
using static MelonLoader.MelonLogger;
using static XmasMod2025.UI.Gift;
using XmasMod2025.Bosses;

[assembly: MelonInfo(typeof(XmasMod2025.XmasMod2025), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace XmasMod2025;

public class XmasMod2025 : BloonsTD6Mod
{
    public static Assembly ModAssembly => Assembly.GetExecutingAssembly();
    public static string lastMap = null;
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
            string propName = "Prop_" + props.IndexOf(prop);
            
            InGame.instance.bridge.CreateMapEditorPropAt(propName, prop.positionalData, prop.Def().propModel, ObjectId.FromString(prop.Def().propModel.name + "_" + props.IndexOf(prop)), null);
        }
    }
    
    

    public static float PresentBloonChance = 0f;
    public static IntMinMax TreeDropRates = new(3, 5);
    public static int FestiveSpritActiveRounds = 0;
    public static bool FestiveSpiritActive = false;
    public static Tower FestiveSpiritTower = null;
    public static List<TowerModel> GiftOfGivingTowersIds = new List<TowerModel>();
    public static Bloon boss = null;
    public static List<MapEditorProp> MapEditorProps => GameData.Instance.mapEditorData.mapEditorProps.ToList();


    public delegate void CurrencyChangedDelegate(double amount);

    public static event CurrencyChangedDelegate? OnGiftsUpdated;
    public static event CurrencyChangedDelegate? OnSnowflakesUpdated;

    private static double gifts;

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

    private static double snowflakes;

    public static double Snowflakes
    {
        get => snowflakes;
        set
        {
            double increase = value - snowflakes;
            snowflakes = value;
            
            OnSnowflakesUpdated?.Invoke(increase);

            if (increase > 0)
            {
                totalSnowflakes += increase;   
            }
        }
    }

    public static double Gifts
    {
        get => gifts;
        set
        {
            double increase = value - gifts;
            gifts = value;

            OnGiftsUpdated?.Invoke(value);

            if (increase > 0)
            {
                totalGifts += increase;

                if(boss != null)
                {
                    if(boss.bloonModel.baseId == ModContent.BloonID<Bosses.GiftBoss>())
                    {
                        boss.health += (int)(increase *= 3);
                    }
                }
            }
        }
    }

    public static double totalGifts;

    public static double TotalGifts => totalGifts;
    
    private static double totalSnowflakes;

    public static double TotalSnowflakes => totalSnowflakes;
    public override void OnRestart()
    {
        if (InGame.Bridge == null)
            return;

        if (InGame.Bridge.IsSandboxMode())
        {
            XmasMod2025.SetCurrency(CurrencyType.Gift, 99999);
            XmasMod2025.totalGifts = 99999;
        }
        else
        {
            XmasMod2025.SetCurrency(CurrencyType.Gift, 25);
            XmasMod2025.totalGifts = 25;
        }
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        if(tower.towerModel.baseId == ModContent.TowerID<FestiveSpiritTower>())
        {
            FestiveSpiritTower = tower;
        }
    }
    public override void OnTowerDestroyed (Tower tower)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<ElfHelper>())
        {
            InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<GiftEffect>(), tower.Position, 0, 2, 120);
        }

        if (tower.towerModel.baseId == ModContent.TowerID<FestiveSpiritTower>())
        {
            FestiveSpiritTower = null;
        }
    }
    public override void OnRoundEnd()
    {
        BuffHandler.GiftOfGivingHandler(false);

        if(FestiveSpritActiveRounds > 0)
        {
            FestiveSpritActiveRounds--;
        }

        if(FestiveSpiritActive)
        {
            if(FestiveSpritActiveRounds <= 0)
            {
                if(FestiveSpiritTower != null)
                {
                    FestiveSpiritTower.worth = 0;
                    FestiveSpiritTower.SellTower();
                }
            }
        }

        foreach(var tower in InGame.instance.GetTowers())
        {
            if(tower.towerModel.baseId == ModContent.TowerID<ElfSpawner>())
            {
                tower.SellTower();
            }

            if(tower.towerModel.baseId == ModContent.TowerID<ElfMonkey>())
            {
                if(tower.towerModel.tiers[2] == 5)
                {
                    AddCurrency(CurrencyType.Gift, 150);

                    if (InGame.instance != null || InGame.instance.bridge != null)
                    {
                        InGame.instance.bridge.simulation.CreateTextEffect(tower.Position, ModContent.CreatePrefabReference<CollectText>(), 2f, $"+150 Gifts", true);
                    }
                }
            }
        }
    }

    public static string LoadEmbeddedJson(string resourceName)
    {
        var stream = ModAssembly.GetEmbeddedResource(resourceName);

        if (stream == null)
        {
            Msg("Embedded resource not found: " + resourceName + ", valid names are the following:");
            foreach (string name in ModAssembly.GetManifestResourceNames())
            {
                Msg(name);
            }
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

    [HookTarget(typeof(BloonDamageHook), HookTargetAttribute.EHookType.Postfix)]
    [HookPriority(HookPriorityAttribute.Low)]
    public static bool BloonDamagePostfix(Bloon @this, ref float totalAmount, Projectile projectile, ref bool distributeToChildren, ref bool overrideDistributeBlocker, ref bool createEffect, Tower tower, BloonProperties immuneBloonProperties, BloonProperties originalImmuneBloonProperties, ref bool canDestroyProjectile, ref bool ignoreNonTargetable, ref bool blockSpawnChildren, HookNullable<int> powerActivatedByPlayerId)
    {
        if(@this.bloonModel.baseId == ModContent.BloonID<SnowBloon>())
        {
            System.Random rand = new System.Random();
            var val = rand.Next(4);

            if(val >= 0)
            {
                TimeTriggerModel time = Game.instance.model.GetBloon("Vortex1").GetBehavior<TimeTriggerModel>().Duplicate();
                time.interval = 1f;
                time.actionIds = new string[] { "SnowBloonStun" };
                time.name = "SnowBloonStun";

                if (@this != null)
                {
                    @this.bloonModel.AddBehavior(time);
                }
            }
        }

        if(@this.bloonModel.baseId == ModContent.BloonID<ElfBoss>())
        {
            if(tower.towerModel.baseId != ModContent.TowerID<ElfMonkey>() && tower != null)
            {
                @this.health += (int)totalAmount;
            }
        }

        return true;
    }
}

[HarmonyLib.HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.Start))]
public class CreateMap
{
    [HarmonyLib.HarmonyPostfix]
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

        var map = new MapDetails
        {
            id = "Christmas Disaster",
            coopMapDivisionType = CoopDivision.FREE_FOR_ALL,
            difficulty = MapDifficulty.Intermediate,
            mapSprite = ModContent.GetSpriteReference<XmasMod2025>("XmasMapIcon"),
            hasWater = true,
            theme = MapTheme.Snow,
            unlockDifficulty = MapDifficulty.Beginner
        };

        var list = GameData.Instance.mapSet.Maps.items.ToList();
        list.Insert(0, customMap);
        list.Insert(1, map);
        GameData.Instance.mapSet.Maps.items = list.ToArray();
    }
}

//Credit goes to Timotheeee: https://github.com/Timotheeee/btd6_mods/blob/master/custom_maps_v2/Main.cs#L406

[HarmonyLib.HarmonyPatch(typeof(MapLoader), nameof(MapLoader.LoadScene))]
public class LoadMap
{
    [HarmonyLib.HarmonyPrefix]
    internal static bool Fix(ref MapLoader __instance)
    {
        XmasMod2025.lastMap = __instance.currentMapName;
        if (XmasMod2025.lastMap == "Xmas Cubism")
        {
            __instance.currentMapName = "Cubism";
        }
        else if (XmasMod2025.lastMap == "Christmas Disaster")
        {
            __instance.currentMapName = "Tutorial";
        }

        return true;
    }
}

// Credits: Timotheeee, https://github.com/Timotheeee/btd6_mods/blob/master/custom_maps_v2/Main.cs#L427
[HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.InitMap))]
public class UnityToSimulation_InitMap
{
    public static readonly string ExportPath =
        System.IO.Path.Combine(MelonEnvironment.UserDataDirectory, "Exported Maps");
    public static void Prefix(UnityToSimulation __instance, ref MapModel map)
    {
        if (XmasMod2025.lastMap == "Christmas Disaster")
        {
            /*foreach (var ob in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                if (ob.name.Contains("Festive") || ob.name.Contains("Rocket") || ob.name.Contains("Firework") || ob.name.Contains("Box") || ob.name.Contains("Candy") || ob.name.Contains("Gift") || ob.name.Contains("Snow") || ob.name.Contains("Ripples") || ob.name.Contains("Grass") || ob.name.Contains("Christmas") || ob.name.Contains("WhiteFlower") || ob.name.Contains("Tree") || ob.name.Contains("Rock") || ob.name.Contains("Shadow") || ob.name.Contains("WaterSplashes"))// || ob.name.Contains("Body")   || ob.name.Contains("Ouch") || ob.name.Contains("Statue")|| ob.name.Contains("Chute")  || ob.name.Contains("Jump") || ob.name.Contains("Timer") || ob.name.Contains("Wheel") || ob.name.Contains("Body") || ob.name.Contains("Axle") || ob.name.Contains("Leg") || ob.name.Contains("Clock") ||
                    if (ob.name != "TutorialTerrain")
                        ob.transform.position = new Vector3(1000, 1000, 1000);
            }
            
            UnityEngine.Object.FindObjectsOfType<MeshRenderer>().First(mr => mr.name.EndsWith("Terrain")).material.mainTexture = ModContent.GetTexture<XmasMod2025>("ChristmasDisasterMap");*/
            var props = XmasMod2025.GetEmbeddedObjectFromJson<JArray>("XmasMapProps.json").ToObject<List<MapEditorPropData>>();
            
            XmasMod2025.PlacePropsFromEditorData(props);
            
            var map2 = XmasMod2025.GetEmbeddedModelFromJson<MapModel>("XmasMap.json");

            foreach (var point in map2.paths[0].points)
            {
                point.id = map2.paths[0].points.IndexOf(point).ToString();
            }
            
            map.areas = map2.areas;
            map.paths = map2.paths;
            map.name = map2.name;
            map2.mapName = map2.mapName;
            map.spawner = map2.spawner;
        }
    }
}

[HarmonyLib.HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
public class ChangeMap
{
    [HarmonyLib.HarmonyPostfix]
    public static void Postfix(InGame __instance)
    {
        if (GiftOpenerUI.instance == null)
        {
            GiftOpenerUI.CreatePanel();
        }

        if (GiftCounterUI.instance == null)
        {
            GiftCounterUI.CreatePanel();
        }

        if(__instance.bridge.IsSandboxMode())
        {
            XmasMod2025.SetCurrency(CurrencyType.Gift, 99999);
            XmasMod2025.totalGifts = 99999;
        }
        else
        {
            XmasMod2025.SetCurrency(CurrencyType.Gift, 25);
            XmasMod2025.totalGifts = 25;
        }


        GameObject terrain = GameObject.Find("CubismTerrain");

        if (terrain != null && XmasMod2025.lastMap == "Xmas Cubism")
        {
            terrain.GetComponent<MeshRenderer>().material.mainTexture = ModContent.GetTexture<XmasMod2025>("XmasMap");
            var data = XmasMod2025.LoadEmbeddedJson("XmasCubismProps.json");
            List<string> lines = JsonSerializer.Deserialize<List<string>>(data);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = System.Text.RegularExpressions.Regex.Match(line, @"^(.*?),\s*(\d+),\s*(-?[\d\.]+),\s*\(([\d\.-]+),\s*([\d\.-]+),\s*([\d\.-]+)\),\s*(-?[\d\.]+),\s*(-?[\d\.]+)$");
                if (!match.Success)
                    continue;

                string propName = match.Groups[1].Value;
                int propId = int.Parse(match.Groups[2].Value);
                float rot = float.Parse(match.Groups[3].Value);
                float x = float.Parse(match.Groups[7].Value);
                float y = float.Parse(match.Groups[8].Value);

                PositionalData positionalData = new PositionalData
                {
                    //scale = float.Parse(match.Groups[6].Value),
                    rotation = new Vector3(0, 0, rot),
                    position = new Vector3(x, 0, y)
                };

                foreach (var p in XmasMod2025.MapEditorProps)
                {
                    if (p.Def().id == propId)
                    {
                        InGame.instance.bridge.CreateMapEditorPropAt(propName, positionalData, p.Def(), default, null);
                    }
                }
            }
        }
    }
}

[HarmonyLib.HarmonyPatch(typeof(TimeTrigger), nameof(TimeTrigger.Trigger))]
public class TimeTriggerPacth
{
    [HarmonyLib.HarmonyPostfix]
    public static void Postfix(TimeTrigger __instance)
    {
        if(__instance.timeTriggerModel.name.Contains("SnowBloonStun"))
        {
            __instance.bloon.bloonModel.RemoveBehavior<TimeTriggerModel>();
        }

        if(__instance.timeTriggerModel.name.Contains("ElfTax"))
        {
            InGame.instance.AddCash(-(InGame.instance.GetCash() * 0.05f));
        }

        if(XmasMod2025.boss != null)
        {
            if (__instance.timeTriggerModel.name.Contains("ModdedSkull" + XmasMod2025.boss.bloonModel.baseId))
            {
                XmasMod2025.boss.trackSpeedMultiplier += 1.2f;
            }
        }
    }
}


[HarmonyLib.HarmonyPatch(typeof(Bloon), nameof(Bloon.OnDestroy))]
public class Bloon_Destroy
{
    [HarmonyLib.HarmonyPostfix]
    public static void Postfix(Bloon __instance)
    {
        if(__instance.bloonModel.baseId == ModContent.BloonID<GiftBloon>())
        {
            var random = new System.Random().Next(1, 25);

            if (InGame.instance != null || InGame.instance.bridge != null)
            {
                InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position, ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gifts", true);
            }

            XmasMod2025.Gifts += random;

            var bloons = Game.instance.model.bloons.ToList().FindAll(bloon => !bloon.isMoab && !bloon.isBoss);
            System.Random rand = new();

            var bloon = bloons[rand.Next(bloons.Count)];
            var countRand = rand.Next(1, 5);


            if (!bloon.baseId.Contains("Rock") && !bloon.baseId.Contains("TestBloon") && !bloon.baseId.Contains("Gold"))
            {
                InGame.instance.SpawnBloons(bloon.id, countRand, 10);
            }
        }

        if (__instance.bloonModel.isBoss)
        {
            if(BossAPI.BossUI.instance != null)
            {
                BossAPI.BossUI.instance.Close();
            }
        }
    }
}