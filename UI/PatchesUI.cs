using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using static XmasMod2025.UI.Gift;

namespace XmasMod2025.UI;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Open))]
internal static class MainMenu_Open
{
    public static void Postfix(MainMenu __instance)
    {
        if (GiftOpenerUI.instance != null) GiftOpenerUI.instance.Close();
    }
}