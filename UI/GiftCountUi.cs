using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace XmasMod2025.UI;
public class GiftCount
{
    [RegisterTypeInIl2Cpp(false)]
    public class GiftCounterUI : MonoBehaviour
    {
        public static GiftCounterUI instance = null;
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
                instance = panel.AddComponent<GiftCounterUI>();

                var Claim = panel.AddImage(new("Button_", -820, 1200, 175), ModContent.GetTextureGUID<XmasMod2025>("PresentIcon"));
                gift = Claim.AddText(new("Text_", 400, 0, 700, 240), "$10,000,000", 100);

                var textButton = gift.gameObject.AddComponent<Button>();
                textButton.onClick.AddListener(SetGift);

                var iconButton = Claim.gameObject.AddComponent<Button>();
                iconButton.onClick.AddListener(SetGift);

                UpdateCount();
            }
        }
        public static void UpdateCount()
        {
            if(gift != null)
            {
                gift.Text.text = "$" + XmasMod2025.Gifts.ToString("#,###");
            }
        }

        public static void SetGift()
        {
            if(InGame.instance != null)
            {
                if(InGame.instance.bridge.IsSandboxMode())
                {
                    Il2CppSystem.Action<int> wantedGifts = (Il2CppSystem.Action<int>)delegate (int newGifts)
                    { if (newGifts > 0) { XmasMod2025.Gifts = newGifts; } };

                    PopupScreen.instance.ShowSetValuePopup("Gifts", "Set Gifts?", wantedGifts, 100000);
                }
            }
        }
    }
}
