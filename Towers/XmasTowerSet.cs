using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Input;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using MelonLoader;

namespace XmasMod2025.Towers;

public class XmasTowerSet : ModTowerSet
{
    public override string DisplayName => "Christmas Towers";

    [HarmonyPatch(typeof(TowerPurchaseButton), nameof(TowerPurchaseButton.Update))]
    private static class TowerPurchaseButton_Update
    {
        public static void Postfix(TowerPurchaseButton __instance)
        {
            var modTower = __instance.towerModel.GetModTower();
            if (modTower != null && modTower is ChristmasTower cTower && cTower.CostsGifts)
            {
                __instance.Cast<TowerPurchaseButton>().costText.text = $"{cTower.Cost} Gifts";
                if (XmasMod2025.Gifts < cTower.Cost)
                {
                    __instance.SetUnavailable();
                    __instance.SetNotEnoughCash();
                }
                else __instance.SetAvailable();
            }
        }
    }

    [HarmonyPatch(typeof(TowerPurchaseButton), nameof(TowerPurchaseButton.ButtonActivated))]
    private static class TowerPurchaseButton_StartDragging
    {
        public static bool Prefix(TowerPurchaseButton __instance)
        {
            var modTower = __instance.towerModel.GetModTower();
            if (modTower != null && modTower is ChristmasTower cTower && cTower.CostsGifts)
            { 
                if (XmasMod2025.Gifts < cTower.Cost)
                {
                    return false;
                }
            }
            
            return true; 
        }
    }

    [HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.TowerCreated))]
    private static class ShopMenu_TowerCreated
    {
        public static void Postfix(TowerToSimulation tower)
        {
            var modTower = tower.tower.towerModel.GetModTower();
            if (modTower != null && modTower is ChristmasTower cTower && cTower.CostsGifts)
            { 
                tower.sim.AddCash(tower.tower.worth, (Simulation.CashSource)12);
                XmasMod2025.Gifts -= cTower.Cost;
                tower.tower.worth = cTower.Cost;

                if (InGame.instance != null || InGame.instance.bridge != null)
                {
                    InGame.instance.bridge.simulation.CreateTextEffect(tower.position.ToSMathVector(), ModContent.CreatePrefabReference<CollectText>(), 2f, $"-{cTower.Cost} Gifts", true); 
                }
            }
        }
    }

    [HarmonyPatch(typeof(Tower), nameof(Tower.OnSold))]
    private static class Tower_OnSold
    {
        public static void Postfix(Tower __instance)
        {
            var modTower = __instance.towerModel.GetModTower();
            if (modTower != null && modTower is ChristmasTower cTower && cTower.CostsGifts && __instance.worth > 0)
            {
                __instance.Sim.AddCash(-__instance.SaleValue, Simulation.CashType.Normal, 1, (Simulation.CashSource)12);
                XmasMod2025.Gifts += (int)__instance.SaleValue;

                if (InGame.instance != null || InGame.instance.bridge != null)
                {
                    InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position, ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{__instance.SaleValue} Gifts", true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.GetUpgradeStatus))]
    private static class UpgradeButton_CheckCash
    {
        public static bool Prefix(UpgradeButton __instance, ref UpgradeButton.UpgradeStatus __result)
        {
            if (!__instance || __instance.upgrade == null)
            {
                return true;
            }
            ModUpgrade? modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade != null && modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.CostsGifts)
            {
                if (XmasMod2025.Gifts < cUpgrade.Cost)
                {
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __result = UpgradeButton.UpgradeStatus.CanNotAfford;
                    return false;
                }
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.UpdateCostVisuals))]
    private static class UpgradeButton_UpdateCostVisuals
    {
        public static void Postfix(UpgradeButton __instance)
        {
            if (!__instance || __instance.upgrade == null)
            {
                return;
            }
            ModUpgrade? modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade { CostsGifts: true } cUpgrade)
            {
                __instance.cost.text = $"{cUpgrade.Cost} Gifts";
            }
        }
    }
    [HarmonyPatch(typeof(TowerManager), nameof(TowerManager.UpgradeTower))]
    private static class TowerManager_UpgradeTower
    {
        [HarmonyPrefix]
        public static void Prefix(ref Tower tower, ref TowerModel def, ref string __state)
        {
            __state = null;
            foreach (var upgrade in def.appliedUpgrades)
            {
                if (def.appliedUpgrades == null) return;
                if (!tower.towerModel.appliedUpgrades.Contains(upgrade))
                {
                    __state = upgrade;
                    var unrefTower = tower;
                    var unrefDef = def;
                    tower = unrefTower;
                    def = unrefDef;
                }
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Tower tower, TowerModel def, float upgradeCost, string __state)
        {
            if (__state != null)
            {
                var modTower = tower.towerModel.GetModTower();
                if (modTower is ChristmasTower { CostsGifts: true })
                {
                    var modUpgrade =  GetContent<ModUpgrade>().Find(m => m.Id == __state);
                    if (modUpgrade is ChristmasUpgrade { CostsGifts: true } cUpgrade)
                    {
                        tower.worth -= upgradeCost;
                        tower.worth += cUpgrade.Cost;
                        tower.GetTowerToSim().sim.AddCash(upgradeCost, Simulation.CashSource.TowerUpgraded);
                        XmasMod2025.Gifts -= cUpgrade.Cost;
                        
                        if (InGame.instance != null || InGame.instance.bridge != null)
                        {
                            InGame.instance.bridge.simulation.CreateTextEffect(tower.Position, CreatePrefabReference<CollectText>(), 2f, $"-{cUpgrade.Cost} Gifts", true);
                        }
                    }
                }
            }
        }
    }
}