using System.Collections.Generic;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Unity.Achievements.List;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using UnityEngine;
using XmasMod2025.GiftShop.BuffsItems;
using XmasMod2025.Towers.SubTowers;
using XmasMod2025.Towers.Upgrades;

namespace XmasMod2025.Towers;

public abstract class ChristmasTower : ModTower<XmasTowerSet>
{
    public static readonly Dictionary<int, List<GameObject>> ButtonsToUnlock = [];
    
    public virtual CurrencyType Currency => CurrencyType.Gift;

    public virtual int UnlockRound => 0;

    private GameObject ShopButton { get; set; }

    #region Unlock Tower Patches
    [HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.CreateTowerButton))]
    private static class ShopButton_Start
    {
        
        public static void Postfix(TowerModel model, ITowerPurchaseButton __result)
        {
            if (model.baseId == TowerID<ToyCart.ToyCartTower>() || model.baseId == TowerID<FestiveSpiritTower>() || model.baseId == TowerID<ElfSpawner>() || model.baseId == TowerID<ElfHelper>())
            {
                __result.GameObject.transform.parent.gameObject.Destroy();
            }
            
            if (model.GetModTower() is ChristmasTower cTower && cTower.UnlockRound != 0)
            {
                cTower.ShopButton = __result.GameObject.transform.parent.gameObject;
                cTower.ShopButton.SetActive(false);

                if (ButtonsToUnlock.TryGetValue(cTower.UnlockRound, out var buttons))
                {
                    buttons.Add(cTower.ShopButton);
                }
                else
                {
                    if (ButtonsToUnlock.ContainsKey(cTower.UnlockRound))
                    {
                        ButtonsToUnlock[cTower.UnlockRound].Add(cTower.ShopButton);
                    }
                    else
                    {
                        ButtonsToUnlock.Add(cTower.UnlockRound, [cTower.ShopButton]);
                        ModHelper.Msg<XmasMod2025>(cTower.Id);
                    }
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
                    if (!button)
                    {
                        ModHelper.Warning<XmasMod2025>("An unlock button is null?");
                        return;
                    }
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