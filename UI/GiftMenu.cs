using System;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using System.Collections.Generic;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using XmasMod2025.Assets;
using XmasMod2025.GiftShop;

namespace XmasMod2025.UI
{
    public enum ShopType
    {
        Buffs,
        Gift
    }

    [RegisterTypeInIl2Cpp(false)]
    public class GiftMenu : MonoBehaviour
    {
        public static GiftMenu instance = null;
        public static ModHelperText? giftText;
        public static ModHelperText? TotalgiftText;
        public static int page = 0;
        public static ModHelperButton leftArrow;
        public static ModHelperButton rightArrow;
        public static List<ModHelperText> slotTitle = new();
        public static List<ModHelperImage> slotIcon = new();
        public static List<ModHelperButton> slotButton = new();
        public static ModHelperText section;

        public void Close()
        {
            if (gameObject)
            {
                gameObject.Destroy();
            }

            giftText = null;
            TotalgiftText = null;
            leftArrow = null;
            rightArrow = null;
            section = null;
            slotTitle.Clear();
            slotIcon.Clear();
            page = 0;
        }

        public static ShopType CurrentPage = ShopType.Buffs;

        public static string FormatNumber(double num)
        {
            if (num < 1000)
            {
                return num.ToString("###");
            }
            int zeros = (int)Math.Log10(num);
            if (num >= 50)
            {
                double newNum = Math.Round(num / (10 ^ zeros), 2);
                return newNum.ToString("#.##") + $"e{zeros}";
            }

            int index = (zeros - 3) / 3; // 1000 = 0, 10000 = 0.33 (0), 100000 = 0.67 (0), 1000000 = 1 (1)... 
            if (index < 0)
            {
                return num.ToString("###");
            }

            string[] suffixes = ["K", "M", "B", "T", "Qd", "Qn", "Sx", "Sp", "Oc", "No", "De", "UDe", "DDe"];

            return (num / (10 ^ (zeros - 3))).ToString("#.##") + suffixes[index]; // 10,134,560,000,000 > 10.13T
        }

        private static ModHelperPanel ShopPanel(GiftShopItem item)
        {
            float w = 1900 / 3.3f;
            float h = 1300;

            var mainPanel = ModHelperPanel.Create(new("Panel_" + item.Id, 0, 0, w, h), "");
            mainPanel.Background.SetSprite(AssetHelper.GetSprite("ChristmasPanelBright"));
            mainPanel.AddImage(new("InnerPanel", 0, 0, w - 75, h - 75), AssetHelper.GetSprite("ChristmasInsertPanelBright")).UseCustomScaling();
            mainPanel.AddImage(new("Icon", 0, 397.5f, 350), item.IconReference.GetGUID());
            mainPanel.AddText(new("Name", 0, 72.5f, 1000, 100), item.DisplayName).EnableAutoSizing(80);
            mainPanel.AddText(new("Description", 0, -227.5f, 1000, 600), item.Description).EnableAutoSizing(45);
            mainPanel.AddButton(new("Purchase", 0, -550, 281, 100), VanillaSprites.GreenBtnLong, new Action(() =>
            {
                var cost = item.GetCostForUpgradeNumber(item.Upgrades + 1);
                if (cost > XmasMod2025.Gifts)
                {
                    PopupScreen.instance.ShowOkPopup(
                        $"You need {cost - XmasMod2025.Gifts} more gifts to purchase this item!");
                    return;
                }

                XmasMod2025.Gifts -= cost;
                item.Upgrades++;
                item.Buy(InGame.instance);
                if (item.Upgrades >= item.MaxUpgrades && item.MaxUpgrades > 0)
                {
                    mainPanel.transform.FindChild("Purchase").GetComponent<Button>().interactable = false;
                    mainPanel.transform.FindChild("CountText").GetComponent<NK_TextMeshProUGUI>().text = "Max Upgrades!";
                }
            }));

            return mainPanel;
        }

        public static void CreatePanel()
        {
            if (InGame.instance == null) return;

            RectTransform rect = InGame.instance.uiRect;
            var panel = rect.gameObject.AddModHelperPanel(new("Panel_", 0, 0, 0, 0), VanillaSprites.MainBGPanelBlue);
            instance = panel.AddComponent<GiftMenu>();

            panel.AddImage(new("Image_", 0, 0, 2000, 1500), AssetHelper.GetSprite("ChristmasPanel")).UseCustomScaling();
            panel.AddImage(new("Image_", 0, 0, 1900, 1400), AssetHelper.GetSprite("ChristmasInsertPanel")).UseCustomScaling();

            panel.AddText(new("Text_", -700, 750, 700, 150), "GIFTS MENU", 100);
            section = panel.AddText(new("Text_", 0, 750, 700, 150), "BUFFS", 100);

            rightArrow = panel.AddButton(new("Button_", 1000, 0, 200), VanillaSprites.NextArrow, new System.Action(() =>
            {
                handleArrows(false);
            }));
            leftArrow = panel.AddButton(new("Button_", -1000, 0, 200), VanillaSprites.PrevArrow, new System.Action(() =>
            {
                handleArrows(true);
            }));

            panel.AddImage(new("Image_", -1500, 550, 800, 400), AssetHelper.GetSprite("ChristmasPanel")).UseCustomScaling();
            panel.AddImage(new("Image_", -1500, 550, 725, 325), AssetHelper.GetSprite("ChristmasInsertPanel")).UseCustomScaling();
            panel.AddImage(new("Image_", -1775, 625, 150, 150), ModContent.GetTextureGUID<XmasMod2025>("Gift1"));
            giftText = panel.AddText(new("Text_", -1525, 625, 700, 150), ": Gifts " + FormatNumber(XmasMod2025.Gifts), 75);
            panel.AddImage(new("Image_", -1775, 475, 150, 150), ModContent.GetTextureGUID<XmasMod2025>("PresentIcon"));
            TotalgiftText = panel.AddText(new("Text_", -1475, 475, 700, 150), ": Total Gifts " + FormatNumber(XmasMod2025.TotalGifts), 60);

            float w = 1900 / 3.3f;
            float h = 1300;
            var imgCenter = panel.AddImage(new("Image_", 0, 0, w, h), AssetHelper.GetSprite("ChristmasPanelBright")).UseCustomScaling();
            var imgRight = panel.AddImage(new("Image_", w + 50, 0, w, h), AssetHelper.GetSprite("ChristmasPanelBright")).UseCustomScaling();
            var imgLeft = panel.AddImage(new("Image_", -w - 50, 0, w, h), AssetHelper.GetSprite("ChristmasPanelBright")).UseCustomScaling();

            float iw = w - 75;
            float ih = 1225;
            var imgInsideCenter = panel.AddImage(new("Image_", 0, 0, iw, ih), AssetHelper.GetSprite("ChristmasInsertPanelBright")).UseCustomScaling();
            var imgInsideRight = panel.AddImage(new("Image_", w + 50, 0, iw, ih), AssetHelper.GetSprite("ChristmasInsertPanelBright")).UseCustomScaling();
            var imgInsideLeft = panel.AddImage(new("Image_", -w - 50, 0, iw, ih), AssetHelper.GetSprite("ChristmasInsertPanelBright")).UseCustomScaling();

            Color color = new(0.8f, 0.8f, 0.8f);
            imgInsideCenter.Image.color = color;
            imgInsideRight.Image.color = color;
            imgInsideLeft.Image.color = color;

            slotTitle.Add(imgInsideCenter.AddText(new("Text_", 0, 550, 500, 150), "Buff", 80));
            slotTitle.Add(imgInsideRight.AddText(new("Text_", 0, 550, 500, 150), "Buff", 80));
            slotTitle.Add(imgInsideLeft.AddText(new("Text_", 0, 550, 500, 150), "Buff", 80));

            slotIcon.Add(imgInsideCenter.AddImage(new("Image_", 0, 350, 500, 500), VanillaSprites.BloonariusIcon));
            slotIcon.Add(imgInsideRight.AddImage(new("Image_", 0, 350, 500, 500), VanillaSprites.BloonariusIcon));
            slotIcon.Add(imgInsideLeft.AddImage(new("Image_", 0, 350, 500, 500), VanillaSprites.BloonariusIcon));

            slotTitle.Add(imgInsideCenter.AddText(new("Text_", 0, 150, 500, 150), "―――――", 80));
            slotTitle.Add(imgInsideRight.AddText(new("Text_", 0, 150, 500, 150), "―――――", 80));
            slotTitle.Add(imgInsideLeft.AddText(new("Text_", 0, 150, 500, 150), "―――――", 80));

            slotTitle.Add(imgInsideCenter.AddText(new("Text_", 0, 0, 400, 300), "Description: Spawns bloonarius idk or do smth else", 50));
            slotTitle.Add(imgInsideRight.AddText(new("Text_", 0, 0, 400, 300), "Description: Spawns bloonarius idk or do smth else", 50));
            slotTitle.Add(imgInsideLeft.AddText(new("Text_", 0, 0, 400, 300), "Description: Spawns bloonarius idk or do smth else", 50));

            slotTitle.Add(imgInsideCenter.AddText(new("Text_", 0, -150, 500, 150), "―――――", 80));
            slotTitle.Add(imgInsideRight.AddText(new("Text_", 0, -150, 500, 150), "―――――", 80));
            slotTitle.Add(imgInsideLeft.AddText(new("Text_", 0, -150, 500, 150), "―――――", 80));

            slotTitle.Add(imgInsideCenter.AddText(new("Text_", 0, -225, 400, 150), "Price: 500 Gifts", 50));
            slotTitle.Add(imgInsideRight.AddText(new("Text_", 0, -225, 400, 150), "Price: 500 Gifts", 50));
            slotTitle.Add(imgInsideLeft.AddText(new("Text_", 0, -225, 400, 150), "Price: 500 Gifts", 50));

            var Exit = panel.AddButton(new("Button_", 1000, 700, 200), VanillaSprites.RedBtnSquareSmall, new System.Action(() => instance.Close()));
            Exit.AddText(new("Text_", 0, 0, 150), "X", 100);

            //handleArrows();
        }

        public static void handleArrows(bool back)
        {
            int max = 1;
            if (page < 0) page = 0;
            if (page > max) page = max;
            leftArrow.SetActive(page != 0);
            rightArrow.SetActive(page != max);
            handlePage();
        }

        public static void handlePage()
        {
            switch (page)
            {
                case 0:
                    section.SetText("Buffs");
                    slotTitle[0].SetText("Buff 1");
                    slotTitle[1].SetText("Buff 2");
                    slotTitle[2].SetText("Buff 0");
                    break;
                case 1:
                    section.SetText("Yet To Come");
                    break;
            }
        }

        public static void UpdateText()
        {
            if (giftText != null) giftText.SetText(": Gifts " + FormatNumber(XmasMod2025.Gifts));
            if (TotalgiftText != null) TotalgiftText.SetText(": Total Gifts " + FormatNumber(XmasMod2025.TotalGifts));
        }
    }

    public static class ModHelperImageExtensions
    {
        public static ModHelperImage UseCustomScaling(this ModHelperImage img)
        {
            img.Image.type = Image.Type.Sliced;
            img.Image.pixelsPerUnitMultiplier = 1;
            return img;
        }
    }
}