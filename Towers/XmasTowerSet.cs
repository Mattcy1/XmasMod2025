using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Input;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;

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
            if (modTower != null && modTower is ChristmasTower cTower && cTower.CostsGifts)
            {
                __instance.Sim.AddCash(-__instance.SaleValue, Simulation.CashType.Normal, 1, (Simulation.CashSource)12);
                XmasMod2025.Gifts += (int)__instance.SaleValue;

                if (InGame.instance != null || InGame.instance.bridge != null)
                {
                    InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position, ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{cTower.Cost} Gifts", true);
                }
            }
        }
    }
}