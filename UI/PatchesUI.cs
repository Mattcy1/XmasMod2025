using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Behaviors.Towers;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Input;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using XmasMod2025.Towers;
using static XmasMod2025.UI.Gift;

namespace XmasMod2025.UI;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Open))]
internal static class MainMenu_Open
{
    public static void Postfix(MainMenu __instance)
    {
        if (GiftOpenerUI.instance != null)
        {
            GiftOpenerUI.instance.Close();
        }
    }
}
