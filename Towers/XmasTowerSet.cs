using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;

namespace XmasMod2025.Towers;

public class XmasTowerSet : ModTowerSet
{
    public override string DisplayName => "Christmas Towers";

    #region Custom Currency Support for Towers

    [HarmonyPatch(typeof(TowerPurchaseButton), nameof(TowerPurchaseButton.Update))]
    private static class TowerPurchaseButton_Update
    {
        public static void Postfix(TowerPurchaseButton __instance)
        {
            try
            {
                var modTower = __instance.towerModel.GetModTower();
                if (modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash)
                {
                    __instance.Cast<TowerPurchaseButton>().costText.text = cTower.Cost == 0
                        ? "Free"
                        : $"{cTower.Cost} {cTower.Currency + (cTower.Cost == 1 ? "" : "s")}";
                    if (XmasMod2025.GetCurrency(cTower.Currency) < cTower.Cost)
                    {
                        __instance.SetUnavailable();
                        __instance.SetNotEnoughCash();
                    }
                    else if (__instance.GetLockedState() == TowerPurchaseLockState.ButtonDisabled ||
                             __instance.GetLockedState() == TowerPurchaseLockState.PurchasesLocked)
                    {
                        __instance.SetAvailable();
                    }
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(TowerPurchaseButton), nameof(TowerPurchaseButton.ButtonActivated))]
    private static class TowerPurchaseButton_StartDragging
    {
        public static bool Prefix(TowerPurchaseButton __instance)
        {
            var modTower = __instance.towerModel.GetModTower();
            if (modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash)
                if (XmasMod2025.GetCurrency(cTower.Currency) < cTower.Cost)
                    return false;

            return true;
        }
    }

    [HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.TowerCreated))]
    private static class ShopMenu_TowerCreated
    {
        public static void Postfix(TowerToSimulation tower)
        {
            var modTower = tower.tower.towerModel.GetModTower();
            if (modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash)
            {
                tower.sim.AddCash(tower.tower.worth, (Simulation.CashSource)12);
                XmasMod2025.AddCurrency(cTower.Currency, -cTower.Cost);
                tower.tower.worth = cTower.Cost;
            }
        }
    }

    [HarmonyPatch(typeof(Tower), nameof(Tower.OnPlace))]
    private static class Tower_OnPlace
    {
        public static void Postfix(Tower __instance)
        {
            var modTower = __instance.towerModel.GetModTower();
            if (modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash && __instance.worth > 0)
                if (InGame.instance != null && InGame.instance.bridge != null)
                    InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position,
                        CreatePrefabReference<CollectText>(), 2f,
                        $"-{cTower.Cost} {cTower.Currency + (cTower.Cost == 1 ? "" : "s")}", true);
        }
    }

    [HarmonyPatch(typeof(Tower), nameof(Tower.OnSold))]
    private static class Tower_OnSold
    {
        public static void Postfix(Tower __instance)
        {
            var modTower = __instance.towerModel.GetModTower();
            if (modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash && __instance.worth > 0)
            {
                __instance.Sim.AddCash(-__instance.SaleValue, Simulation.CashType.Normal, 1, (Simulation.CashSource)12);
                XmasMod2025.AddCurrency(cTower.Currency, (int)__instance.SaleValue);

                if (InGame.instance != null && InGame.instance.bridge != null)
                    InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position,
                        CreatePrefabReference<CollectText>(), 2f,
                        $"+{__instance.SaleValue} {cTower.Currency + (__instance.SaleValue == 1 ? "" : "s")}", true);
            }
        }
    }

    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.GetUpgradeStatus))]
    private static class UpgradeButton_GetUpgradeStatus
    {
        public static void Postfix(UpgradeButton __instance, ref UpgradeButton.UpgradeStatus __result)
        {
            if (__instance.upgrade == null) return;
            var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash)
            {
                if (XmasMod2025.GetCurrency(cUpgrade.Currency) < cUpgrade.Cost)
                {
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __result = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __instance.Cost.color = __instance.costInactiveColor;
                    __instance.background.SetSprite(__instance.backgroundInactive);
                }
            }
            else if (modUpgrade is ChristmasParagonUpgrade cParagon && cParagon.Currency != CurrencyType.Cash)
            {
                if (XmasMod2025.GetCurrency(cParagon.Currency) < cParagon.Cost)
                {
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __result = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __instance.Cost.color = __instance.costInactiveColor;
                    __instance.background.SetSprite(__instance.backgroundInactive);
                }
            }
        }
    }


    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.UpdateCostVisuals))]
    private static class UpgradeButton_UpdateCostVisuals
    {
        public static void Postfix(UpgradeButton __instance)
        {
            if (__instance.upgrade == null) return;
            var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash)
                __instance.cost.text = cUpgrade.Cost == 0 ? "Free" : $"{cUpgrade.Cost} {cUpgrade.Currency + (cUpgrade.Cost == 1 ? "" : "s")}";
        }
    }

    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.CheckCash))]
    private static class UpgradeButton_CheckCost
    {
        public static void Postfix(UpgradeButton __instance)
        {
            if (__instance.upgrade == null) return;
            var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash)
            {
                if (XmasMod2025.GetCurrency(cUpgrade.Currency) < cUpgrade.Cost)
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
            }
            else if (modUpgrade is ChristmasParagonUpgrade cParagon && cParagon.Currency != CurrencyType.Cash)
            {
                if (XmasMod2025.GetCurrency(cParagon.Currency) < cParagon.Cost)
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
            }
        }
    }

    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.UpdateVisuals))]
    private static class UpgradeButton_UpdateVisuals
    {
        public static void Postfix(UpgradeButton __instance)
        {
            if (__instance.upgrade == null) return;
            var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash)
                if (XmasMod2025.GetCurrency(cUpgrade.Currency) < cUpgrade.Cost)
                {
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __instance.Cost.color = __instance.costInactiveColor;
                    __instance.background.SetSprite(__instance.backgroundInactive);
                }
        }
    }

    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.IsUpgradePathPurchasable))]
    private static class UpgradeButton_IsUpgradePathPurchasable
    {
        public static bool Prefix(UpgradeButton __instance, ref bool __result)
        {
            if (__instance.upgrade == null) return true;
            var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash)
                if (XmasMod2025.GetCurrency(cUpgrade.Currency) < cUpgrade.Cost)
                {
                    __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
                    __result = false;
                    return false;
                }

            return true;
        }
    }

    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.Update))]
    private static class UpgradeButton_Update
    {
        public static void Postfix(UpgradeButton __instance)
        {
            if (__instance.upgrade == null) return;
            var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash &&
                cUpgrade.Cost > XmasMod2025.GetCurrency(cUpgrade.Currency))
                __instance.upgradeStatus = UpgradeButton.UpgradeStatus.CanNotAfford;
        }
    }

    [HarmonyPatch(typeof(UpgradeButton), nameof(UpgradeButton.UpdateCost))]
    private static class UpgradeButton_UpdateCost
    {
        public static void Postfix(UpgradeButton __instance)
        {
            if (__instance.upgrade == null) return;
            var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __instance.upgrade.name);
            if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash)
                __instance.cost.text = cUpgrade.Cost == 0 ? "Free" : $"{cUpgrade.Cost} {cUpgrade.Currency + (cUpgrade.Cost == 1 ? "" : "s")}";
        }
    }

    [HarmonyPatch(typeof(TowerManager), nameof(TowerManager.UpgradeTower))]
    private static class TowerManager_UpgradeTower
    {
        [HarmonyPrefix]
        public static void Prefix(ref Tower tower, ref TowerModel def, ref string? __state)
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
        public static void Postfix(Tower tower, TowerModel def, float upgradeCost, string? __state)
        {
            if (__state == null) return;
            var modTower = tower.towerModel.GetModTower();

            if (modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash)
            {
                var modUpgrade = GetContent<ModUpgrade>().Find(m => m.Id == __state);
                if (modUpgrade is ChristmasUpgrade cUpgrade && cUpgrade.Currency != CurrencyType.Cash)
                {
                    tower.worth -= upgradeCost;
                    tower.worth += cUpgrade.Cost;
                    tower.GetTowerToSim().sim.AddCash(upgradeCost, Simulation.CashSource.TowerUpgraded);
                    XmasMod2025.AddCurrency(cUpgrade.Currency, -cUpgrade.Cost);

                    if (InGame.instance != null && InGame.instance.bridge != null && cUpgrade.Cost != 0)
                        InGame.instance.bridge.simulation.CreateTextEffect(tower.Position,
                            CreatePrefabReference<CollectText>(), 2f,
                            $"-{cUpgrade.Cost} {cUpgrade.Currency + (cUpgrade.Cost == 1 ? "" : "s")}", true);
                }
            }
        }
    }

    #endregion
}