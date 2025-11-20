using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Hooks;
using BTD_Mod_Helper.Api.Hooks.BloonHooks;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.MapEditor;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.ContentBrowser;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using System.Collections.Generic;
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
    public override void OnApplicationStart()
    {
    }

    public static float PresentBloonChance = 0f;
    public static IntMinMax TreeDropRates = new(3, 5);
    public static int FestiveSpritActiveRounds = 0;
    public static bool FestiveSpiritActive = false;
    public static Tower FestiveSpiritTower = null;
    public static List<TowerModel> GiftOfGivingTowersIds = new List<TowerModel>();
    public static Bloon boss = null;

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

    private static double totalGifts;

    public static double TotalGifts => totalGifts;
    
    private static double totalSnowflakes;

    public static double TotalSnowflakes => totalSnowflakes;

    public override void OnMatchStart()
    {
        if (GiftOpenerUI.instance == null)
        {
            GiftOpenerUI.CreatePanel();
        }

        if (GiftCounterUI.instance == null)
        {
            GiftCounterUI.CreatePanel();
        }

        gifts = 25;
        totalGifts = 25;
    }

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
        GameObject game = GameObject.Find("CubismTerrain");

        if (game != null)
        {
            game.GetComponent<MeshRenderer>().material.mainTexture = ModContent.GetTexture<XmasMod2025>("Map");
        }
        else
        {
            PopupScreen.instance.ShowOkPopup("It's recommended to play this mod on cubsim, The code for loading a custom map was made by datjanedoe credits to him.");

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