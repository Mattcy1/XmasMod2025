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
    public static ModHelperText? TotalgiftText;

    public void Close()
    {
        if (gameObject)
        {
            gameObject.Destroy();
        }
    }

    public static string FormatNumber(long num)
    {
        if (num >= 1_000_000_000) return (num / 1_000_000_000f).ToString("0.#") + "B";
        if (num >= 1_000_000) return (num / 1_000_000f).ToString("0.#") + "M";
        if (num >= 1_000) return (num / 1_000f).ToString("0.#") + "k";
        return num.ToString();
    }

    public static void CreatePanel()
    {
        if (InGame.instance != null)
        {
            RectTransform rect = InGame.instance.uiRect;
            var panel = rect.gameObject.AddModHelperPanel(new("Panel_", 0, 0, 0, 0), VanillaSprites.MainBGPanelBlue);
            instance = panel.AddComponent<GiftMenu>();

            var background = panel.AddImage(new("Image_", 0, 0, 2000, 1500), AssetHelper.GetSprite("ChristmasPanel"));
            background.Image.type = Image.Type.Sliced;
            background.Image.pixelsPerUnitMultiplier = 1;

            var Inside = panel.AddImage(new("Image_", 0, 0, 1900, 1400), AssetHelper.GetSprite("ChristmasInsertPanel"));
            Inside.Image.type = Image.Type.Sliced;
            Inside.Image.pixelsPerUnitMultiplier = 1;

            var Title = panel.AddText(new("Text_", -700, 750, 700, 150), "GIFTS MENU", 100);
            var Section = panel.AddText(new("Text_", 0, 750, 700, 150), "BUFFS", 100);

            var RightArrow = panel.AddButton(new("Button_", 1000, -700, 200), VanillaSprites.NextArrow, new System.Action(() =>
            {
                instance.Close();
            }));

            var LeftArrow = panel.AddButton(new("Button_", -1000, -700, 200), VanillaSprites.PrevArrow, new System.Action(() =>
            {
                instance.Close();
            }));

            var GiftHolder = panel.AddImage(new("Image_", -1500, 550, 800, 400), AssetHelper.GetSprite("ChristmasPanel"));
            GiftHolder.Image.type = Image.Type.Sliced;
            GiftHolder.Image.pixelsPerUnitMultiplier = 1;

            var GiftHolderInside = panel.AddImage(new("Image_", -1500, 550, 725, 325), AssetHelper.GetSprite("ChristmasInsertPanel"));
            GiftHolderInside.Image.type = Image.Type.Sliced;
            GiftHolderInside.Image.pixelsPerUnitMultiplier = 1;

            var GiftImg = panel.AddImage(new("Image_", -1775, 625, 150, 150), ModContent.GetTextureGUID<XmasMod2025>("Gift1"));
            giftText = panel.AddText(new("Text_", -1525, 625, 700, 150), ": Gifts " + FormatNumber(XmasMod2025.Gifts), 75);

            var TotalGiftImg = panel.AddImage(new("Image_", -1775, 475, 150, 150), ModContent.GetTextureGUID<XmasMod2025>("PresentIcon"));
            TotalgiftText = panel.AddText(new("Text_", -1475, 475, 700, 150), ": Total Gifts " + FormatNumber(XmasMod2025.TotalGifts), 60);

            var img = panel.AddImage(new("Image_", 0, 0, 1900 / 3.3f, 1300), AssetHelper.GetSprite("ChristmasPanelBright"));
            img.Image.type = Image.Type.Sliced;
            img.Image.pixelsPerUnitMultiplier = 1;

            var img1 = panel.AddImage(new("Image_", 1900 / 3.3f + 50, 0, 1900 / 3.3f, 1300), AssetHelper.GetSprite("ChristmasPanelBright"));
            img1.Image.type = Image.Type.Sliced;
            img1.Image.pixelsPerUnitMultiplier = 1;

            var img2 = panel.AddImage(new("Image_", -1900 / 3.3f - 50, 0, 1900 / 3.3f, 1300), AssetHelper.GetSprite("ChristmasPanelBright"));
            img2.Image.type = Image.Type.Sliced;
            img2.Image.pixelsPerUnitMultiplier = 1;

            var imgInside = panel.AddImage(new("Image_", 0, 0, 1900 / 3.3f - 75, 1225), AssetHelper.GetSprite("ChristmasInsertPanelBright"));
            var imgInside1 = panel.AddImage(new("Image_", 1900 / 3.3f + 50, 0, 1900 / 3.3f - 75, 1225), AssetHelper.GetSprite("ChristmasInsertPanelBright"));
            var imgInside2 = panel.AddImage(new("Image_", -1900 / 3.3f - 50, 0, 1900 / 3.3f - 75, 1225), AssetHelper.GetSprite("ChristmasInsertPanelBright"));

            imgInside.Image.type = Image.Type.Sliced;
            imgInside.Image.pixelsPerUnitMultiplier = 1;
            imgInside1.Image.type = Image.Type.Sliced;
            imgInside1.Image.pixelsPerUnitMultiplier = 1;
            imgInside2.Image.type = Image.Type.Sliced;
            imgInside2.Image.pixelsPerUnitMultiplier = 1;

            var color = new Color(0.8f, 0.8f, 0.8f);

            imgInside.Image.color = color;
            imgInside1.Image.color = color;
            imgInside2.Image.color = color;

            var Exit = panel.AddButton(new("Button_", 1000, 700, 200), VanillaSprites.RedBtnSquareSmall, new System.Action(() =>
            {
                instance.Close();
            }));

            var ExitText = Exit.AddText(new("Text_", 0, 0, 150), "X", 100);
        }
    }

    public static void UpdateText()
    {
        if (giftText != null) giftText.SetText(": Gifts " + FormatNumber(XmasMod2025.Gifts));
        if (TotalgiftText != null) TotalgiftText.SetText(": Total Gifts " + FormatNumber(XmasMod2025.TotalGifts));
    }
}
