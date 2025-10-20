using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using Octokit;
using XmasMod2025;
using XmasMod2025.Towers;
using XmasMod2025.UI;
using static MelonLoader.MelonLogger;
using static XmasMod2025.UI.Gift;

[assembly: MelonInfo(typeof(XmasMod2025.XmasMod2025), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace XmasMod2025;

public class XmasMod2025 : BloonsTD6Mod
{
    public delegate void GiftsUpdated_Delegate(int gifts);

    public static event GiftsUpdated_Delegate? OnGiftsUpdated;

    private static int gifts;

    public static int Gifts
    {
        get => gifts;
        set
        {
            int increase = value - gifts;
            gifts = value;

            OnGiftsUpdated?.Invoke(value);

            if (increase > 0)
            {
                totalGifts += increase;
            }
        }
    }

    private static int totalGifts;

    public static int TotalGifts
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
    }

    public override void OnRestart()
    {
        gifts = 25;
    }

    public override void OnTowerSelected(Tower tower)
    {
        if (GiftOpenerUI.instance != null)
        {
            GiftOpenerUI.instance.Close();
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