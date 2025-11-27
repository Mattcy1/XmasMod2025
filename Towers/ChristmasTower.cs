using System.Collections.Generic;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.Achievements.List;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using UnityEngine;

namespace XmasMod2025.Towers;

public abstract class ChristmasTower : ModTower<XmasTowerSet>
{
    public static readonly Dictionary<int, List<GameObject>> ButtonsToUnlock = [];
    
    public virtual CurrencyType Currency => CurrencyType.Gift;

    public virtual int UnlockRound => 0;

    private GameObject ShopButton { get; set; }

    #region Unlock Tower Patches
    [HarmonyPatch(typeof(TowerPurchaseButton), nameof(TowerPurchaseButton.Awake))]
    private static class ShopButton_Start
    {
        public static void Postfix(TowerPurchaseButton __instance)
        {
            if (__instance.towerModel.GetModTower() is ChristmasTower cTower && cTower.UnlockRound != 0)
            {
                cTower.ShopButton = __instance.gameObject;
                __instance.gameObject.SetActive(false);
                if (ButtonsToUnlock.TryGetValue(cTower.UnlockRound, out var buttons))
                {
                    buttons.Add(__instance.gameObject);
                }
                else
                {
                    ButtonsToUnlock.Add(cTower.UnlockRound, [__instance.gameObject]);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Simulation), nameof(Simulation.RoundStart))]
    private static class Simulation_RoundStart
    {
        public static void Postfix(int spawnedRound)
        {
            if (ButtonsToUnlock.TryGetValue(spawnedRound, out var buttons))
            {
                foreach (var button in buttons)
                {
                    button.SetActive(true);
                }
            }
        }
    }
    #endregion
}

public abstract class ChristmasUpgrade : ModUpgrade
{
    public abstract ChristmasTower ChristmasTower { get; }
    
    public override ModTower Tower => ChristmasTower;
    public virtual CurrencyType Currency => CurrencyType.Gift;
}
public abstract class ChristmasUpgrade<T> : ChristmasUpgrade where T : ChristmasTower
{
    public override ChristmasTower ChristmasTower => GetInstance<T>();
}