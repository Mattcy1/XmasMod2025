using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using Octokit;
using System.Collections.Generic;
using XmasMod2025;
using XmasMod2025.GiftShop.BuffsItems;
using XmasMod2025.Towers;
using XmasMod2025.UI;
using static MelonLoader.MelonLogger;
using static XmasMod2025.UI.Gift;

[assembly: MelonInfo(typeof(XmasMod2025.XmasMod2025), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace XmasMod2025;

public class XmasMod2025 : BloonsTD6Mod
{
    public static float PresentBloonChance = 0f;
    public static int[] TreeDropRates = { 3, 5 };
    public static int FestiveSpritActiveRounds = 0;
    public static bool FestiveSpiritActive = false;
    public static Tower FestiveSpiritTower = null;
    public static List<ObjectId> GiftOfGivingTowersIds = new List<ObjectId>();

    public delegate void GiftsUpdated_Delegate(double gifts);

    public static event GiftsUpdated_Delegate? OnGiftsUpdated;

    private static double gifts;

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
            }
        }
    }

    private static double totalGifts;

    public static double TotalGifts
    {
        get => totalGifts;
    }

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

    public override void OnTowerSelected(Tower tower)
    {
        if (GiftOpenerUI.instance != null)
        {
            GiftOpenerUI.instance.Close();
        }
    }

    public override void OnTowerDestroyed(Tower tower)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<ElfHelper>())
        {
            InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<GiftEffect>(), tower.Position, 0, 2, 120);
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
            if(FestiveSpritActiveRounds == 0)
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

    public override void OnTowerDeselected(Tower tower)
    {
        if (GiftOpenerUI.instance == null)
        {
            GiftOpenerUI.CreatePanel();
        }
    }
    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        var modTower = tower.towerModel.GetModTower();

        if (modTower != null && modTower is ChristmasTower cTower && cTower.CostsGifts)
        {

        }
    }
}