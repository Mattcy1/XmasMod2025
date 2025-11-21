using Bosses.BossAPI;
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
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using Octokit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using UnityEngine;
using XmasMod2025;
using XmasMod2025.Bloons;
using XmasMod2025.GiftShop.BuffsItems;
using XmasMod2025.Towers;
using XmasMod2025.UI;
using static XmasMod2025.UI.Gift;

[assembly: MelonInfo(typeof(XmasMod2025.XmasMod2025), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace XmasMod2025;

public class XmasMod2025 : BloonsTD6Mod
{
    public static MapModel customMap;
    
    private MapModel? GetEmbeddedMap(string resourceName)
    {
        var stream = MelonAssembly.Assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            return null;
        }
        
        var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();

        return ModelSerializer.DeserializeModel<MapModel>(json);
    }

    public override void OnTitleScreen()
    {
        var map = GetEmbeddedMap("map");
        if (map == null)
        {
            return;
        }
        
        LoggerInstance.Msg($"Old count: {GameData.Instance.mapSet.Maps.items.Count}");

        GameData.Instance.mapSet.Maps.items = GameData.Instance.mapSet.Maps.items.AddTo(new MapDetails
        {
            id = map.mapName,
            coopMapDivisionType = CoopDivision.FREE_FOR_ALL,
            difficulty = MapDifficulty.Intermediate,
            mapSprite = ModContent.GetSpriteReference(this, "XmasMap"),
            hasWater = true,
            theme = MapTheme.Snow,
            unlockDifficulty = MapDifficulty.Beginner,

        });
        
        
        LoggerInstance.Msg($"New count: {GameData.Instance.mapSet.Maps.items.Count}");
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
        gifts = 25;
        totalGifts = 25;
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
        }
    }

    public static string LoadJson(string filename)
    {
        var asm = Assembly.GetExecutingAssembly();

        string resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(filename));

        if (resourceName == null)
        {
            MelonLogger.Error("JSON resource not found: " + filename);
            return null;
        }

        MelonLogger.Msg("Loading: " + resourceName);

        using (var stream = asm.GetManifestResourceStream(resourceName))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
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

        return true;
    }
}

[HarmonyLib.HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
public class ChangeMap
{
    [HarmonyLib.HarmonyPostfix]
    public static void Postfix()
    {
        if (GiftOpenerUI.instance == null)
        {
            GiftOpenerUI.CreatePanel();
        }

        if (GiftCounterUI.instance == null)
        {
            GiftCounterUI.CreatePanel();
        }

        XmasMod2025.SetCurrency(CurrencyType.Gift, 25);
        XmasMod2025.totalGifts = 25;

        GameObject terrain = GameObject.Find("CubismTerrain");

        if (terrain != null)
        {
            terrain.GetComponent<MeshRenderer>().material.mainTexture = ModContent.GetTexture<BossAPI>("map");
            var data = XmasMod2025.LoadJson("MapMaker-8977.json");
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
        else
        {
            PopupScreen.instance.ShowOkPopup("It's recommended to play this mod on cubsim.");

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
        }
    }
}