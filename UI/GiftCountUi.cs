using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace XmasMod2025.UI;
[RegisterTypeInIl2Cpp(false)]
public class GiftCounterUI : MonoBehaviour
{
    public static GiftCounterUI instance = null;
    public static ModHelperText? giftText;

    public void Close()
    {
        if (gameObject)
        {
            gameObject.Destroy();
        }
    }

    private void OnDisable()
    {
        RemoveListeners();
    }
    private void Start()
    {
        AddListeners();
    }
    private void OnEnable()
    {
        AddListeners();
    }

    public void AddListeners()
    {
        RemoveListeners();
        XmasMod2025.OnGiftsUpdated += UpdateText;
    }
    public void RemoveListeners()
    {
        XmasMod2025.OnGiftsUpdated -= UpdateText;
    }

    public static void CreatePanel()
    {
        if (InGame.instance != null)
        {
            RectTransform rect = InGame.instance.uiRect;
            var panel = rect.gameObject.AddModHelperPanel(new("Panel_", 0, 0, 0, 0), VanillaSprites.BrownPanel);
            panel.transform.SetParent(FindFirstObjectByType<CashDisplay>().transform.parent.parent);
            instance = panel.AddComponent<GiftCounterUI>();

            var Claim = panel.AddImage(new("Button_", 225, -90, 175), ModContent.GetTextureGUID<XmasMod2025>("PresentIcon"));
            giftText = Claim.AddText(new("Text_", 460, 0, 700, 240), XmasMod2025.Gifts.ToString("#,###"), 100);
            giftText.Text.alignment = Il2CppTMPro.TextAlignmentOptions.Left;

            var textButton = giftText.gameObject.AddComponent<Button>();
            textButton.onClick.AddListener(SetGift);

            var iconButton = Claim.gameObject.AddComponent<Button>();
            iconButton.onClick.AddListener(SetGift);

            UpdateText(25);
        }
    }
    public static void UpdateText(double gifts)
    {
        giftText.SetText(gifts.ToString("#,###.##"));
    }
    public static void SetGift()
    {
        if (InGame.instance != null)
        {
            if (InGame.instance.bridge.IsSandboxMode())
            {
                Il2CppSystem.Action<int> wantedGifts = (Il2CppSystem.Action<int>)delegate (int newGifts)
                {
                    if (newGifts > 0)
                    {
                        XmasMod2025.Gifts = newGifts;
                    }
                };

                PopupScreen.instance.ShowSetValuePopup("Gifts", "Set Gifts?", wantedGifts, 100000);

            }
        }
    }
}