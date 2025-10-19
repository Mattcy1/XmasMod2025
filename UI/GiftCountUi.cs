using BTD_Mod_Helper.Extensions;
using CommandLine;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace XmasMod2025.UI;

[RegisterTypeInIl2Cpp(false)]
public class GiftCountUi : MonoBehaviour
{
    private NK_TextMeshProUGUI text;
    private Image icon;

    public void Start()
    {
        text = GetComponentInChildren<NK_TextMeshProUGUI>();
        text.gameObject.RemoveComponent<CashDisplay>();
        icon = GetComponentInChildren<Image>();
        transform.GetChild(0).Destroy();

        while (transform.childCount > 2)
        {
            transform.GetChild(2).Destroy();
        }
    }

    void Update()
    {
        text.text = XmasMod2025.Gifts.ToString("#,###");
    }

    /*[HarmonyPatch(typeof(CashDisplay), nameof(CashDisplay.AddListeners))]
    private static class DynamicUiObject_OnGameStart
    {
        public static void Postfix(CashDisplay __instance)
        {
            try
            {
                if (__instance.transform.parent.name != "GiftDisplay")
                {
                    var obj = __instance.transform.parent.gameObject.Duplicate();
                    obj.name = "GiftDisplay";
                    obj.AddComponent<GiftCountUi>();
                }
            }
            catch
            {
                
            }
        }
    }*/
}