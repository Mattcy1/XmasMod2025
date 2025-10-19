using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using UnityEngine;

namespace XmasMod2025.UI;

public class Gift
{
    [RegisterTypeInIl2Cpp(false)]
    public class GiftOpenerUI : MonoBehaviour
    {
        public static GiftOpenerUI instance = null;
        public static ModHelperText? gift;

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
                instance = panel.AddComponent<GiftOpenerUI>();

                var Claim = panel.AddButton(new("Button_", 1530f, 1000f, 180f), ModContent.GetTextureGUID<XmasMod2025>("XmasTowerSet-Button"), new System.Action(() =>
                {
                    PopupScreen.instance?.ShowOkPopup($"WIP");
                }));

                gift = Claim.AddText(new("Text_", 0, -120, 200, 150), $"{XmasMod2025.Gifts}");
            }
        }

        public static void UpdateText()
        {
            gift.Text.text = $"{XmasMod2025.Gifts}";
        }
    }
}
