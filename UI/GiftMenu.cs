using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using XmasMod2025.Assets;

namespace XmasMod2025.UI;

[RegisterTypeInIl2Cpp(false)]
public class GiftMenu : MonoBehaviour
{
    public static GiftMenu instance = null;
    public static ModHelperText? giftText;

    public void Close()
    {
        if (gameObject)
        {
            gameObject.Destroy();
        }
    }

    public static void CreatePanel()
    {
        if (InGame.instance != null)
        {
            RectTransform rect = InGame.instance.uiRect;
            var panel = rect.gameObject.AddModHelperPanel(new("Panel_", 0, 0, 0, 0), VanillaSprites.BrownPanel);
            instance = panel.AddComponent<GiftMenu>();

            var Claim = panel.AddImage(new("Button_", 0, 0, 500), AssetHelper.GetSprite("ChristmasPanel"));
        }
    }
}