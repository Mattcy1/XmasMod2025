using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace XmasMod2025.UI;

[RegisterTypeInIl2Cpp(false)]
public class GiftCounterUI : MonoBehaviour
{
    public static GiftCounterUI instance;
    public static ModHelperText? giftText;

    private void Start()
    {
        AddListeners();
    }

    private void OnEnable()
    {
        AddListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    public void Close()
    {
        if (gameObject) gameObject.Destroy();
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
            var rect = InGame.instance.uiRect;
            var panel = rect.gameObject.AddModHelperPanel(new Info("Panel_", 0, 0, 0, 0), VanillaSprites.BrownPanel);
            panel.transform.SetParent(FindFirstObjectByType<CashDisplay>().transform.parent.parent);
            instance = panel.AddComponent<GiftCounterUI>();

            var Claim = panel.AddImage(new Info("Button_", 225, -90, 175),
                ModContent.GetTextureGUID<XmasMod2025>("PresentIcon"));
            giftText = Claim.AddText(new Info("Text_", 460, 0, 700, 240), XmasMod2025.Gifts.FormatNumber(), 100);
            giftText.Text.alignment = TextAlignmentOptions.Left;

            var textButton = giftText.gameObject.AddComponent<Button>();
            textButton.onClick.AddListener(SetGift);

            var iconButton = Claim.gameObject.AddComponent<Button>();
            iconButton.onClick.AddListener(SetGift);

            UpdateText(25);
        }
    }

    public static void UpdateText(double gifts)
    {
        giftText.SetText(XmasMod2025.Gifts.FormatNumber());
    }

    public static void SetGift()
    {
        if (InGame.instance != null)
            if (InGame.instance.bridge.IsSandboxMode())
            {
                var wantedGifts = (Action<int>)delegate(int newGifts)
                {
                    if (newGifts > 0) XmasMod2025.Gifts = newGifts;
                };

                PopupScreen.instance.ShowSetValuePopup("Gifts", "Set Gifts?", wantedGifts, 100000);
            }
    }
}