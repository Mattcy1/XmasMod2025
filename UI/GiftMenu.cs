using System;
using System.Collections;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Animations;
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
        private static ModHelperText giftMultiplierText;
        public static GiftMenu instance = null;
        public static ModHelperText? giftText;
        public static ModHelperText? totalGiftText;
        public static int page = 0;
        public static ModHelperButton leftArrow;
        public static ModHelperButton rightArrow;
        public static List<ModHelperText> slotTitle = new();
        public static List<ModHelperImage> slotIcon = new();
        public static List<ModHelperButton> slotButton = new();
        public static ModHelperText section;
        public static ModHelperPanel normal;
        public static ModHelperScrollPanel scroll;
        
        public void Close()
        {
            if (gameObject)
            {
                gameObject.Destroy();
            }

            giftText = null;
            totalGiftText = null;
            leftArrow = null;
            rightArrow = null;
            section = null;
            slotTitle.Clear();
            slotIcon.Clear();
            page = 0;
        }

        public static ShopType CurrentPage = ShopType.Buffs;

        private static ModHelperPanel ShopPanel(GiftShopItem item)
        {
            var mainPanel = ModHelperPanel.Create(new("Panel_" + item.Id, 0, 0, 575, 1300), "");
            mainPanel.Background.SetSprite(AssetHelper.GetSprite("ChristmasPanelBright"));
            mainPanel.AddImage(new("Icon", 0, 397.5f, 350), item.IconReference.GetGUID());
            mainPanel.AddText(new("Name", 0, 72.5f, 525, 100), item.DisplayName).EnableAutoSizing(80);
            var desc = mainPanel.AddText(new("Description", 0, -150, 525, 1200), item.Description);
            desc.EnableAutoSizing(45);
            mainPanel.AddButton(new("Purchase", 0, -550, 500, 500 / ModHelperButton.LongBtnRatio), VanillaSprites.GreenBtnLong, new Action(() =>
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

                mainPanel.transform.FindChild("Purchase").FindChild("CountText").GetComponent<NK_TextMeshProUGUI>().text = $"{item.GetCostForUpgradeNumber(item.Upgrades + 1)} Gifts";

                if (item.Upgrades >= item.MaxUpgrades && item.MaxUpgrades > 0)
                {
                    mainPanel.transform.FindChild("Purchase").GetComponent<Button>().interactable = false;
                    mainPanel.transform.FindChild("Purchase").FindChild("CountText").GetComponent<NK_TextMeshProUGUI>().text = "Max Upgrades!";
                }
            })).AddText(new("CountText", 500, 500 / ModHelperButton.LongBtnRatio), $"{item.GetCostForUpgradeNumber(item.Upgrades + 1)} Gifts");

            var initialText = mainPanel.transform.FindChild("Purchase").FindChild("CountText").GetComponent<NK_TextMeshProUGUI>();
            if (item.Upgrades >= item.MaxUpgrades && item.MaxUpgrades > 0)
            {
                mainPanel.transform.FindChild("Purchase").GetComponent<Button>().interactable = false;
                initialText.text = "Max Upgrades!";
            }
            else
            {
                initialText.text = $"{item.GetCostForUpgradeNumber(item.Upgrades + 1)} Gifts";
            }

            return mainPanel;
        }
        
        public static void CreatePanel()
        {
            if (InGame.instance == null) return;

            RectTransform rect = InGame.instance.uiRect;
            var panel = rect.gameObject.AddModHelperPanel(new("Panel_", 0, 0, 0, 0), VanillaSprites.MainBGPanelBlue); // you setting the size to 0 lwk pmo
            instance = panel.AddComponent<GiftMenu>();
            
            CreateSidePanel(panel);

            panel.AddPanel(new("Panel_", 0, 0, 2000, 1500), "").Background.SetSprite(AssetHelper.GetSprite("ChristmasPanel"));

            panel.AddText(new("Text_", -700, 750, 700, 150), "GIFTS MENU", 100);
            section = panel.AddText(new("Text_", 0, 750, 700, 150), "BUFFS", 100);

            rightArrow = panel.AddButton(new("Button_", 1000, 0, 200), VanillaSprites.NextArrow, new System.Action(() =>
            {
                HandleArrow(false);
            }));
            leftArrow = panel.AddButton(new("Button_", -1000, 0, 200), VanillaSprites.PrevArrow, new System.Action(() =>
            {
                HandleArrow(true);
            }));

            Sprite insertPanel = AssetHelper.GetSprite("ChristmasInsertPanel");
            
            normal = panel.AddPanel(new("Content_Normal", 0, 0, 1900, 1400),"");
            normal.Background.SetSprite(insertPanel);
            scroll = panel.AddScrollPanel(new("Content_Scroll", 0, 0, 1900, 1400), RectTransform.Axis.Vertical, "", 35, 35);
            scroll.Background.SetSprite(insertPanel);

            MelonCoroutines.Start(UpdateLayouts());

            var Exit = panel.AddButton(new("Button_", 1000, 700, 200), VanillaSprites.RedBtnSquareSmall, new System.Action(() => instance.Close()));
            Exit.AddText(new("Text_", 0, 0, 150), "X", 100);

            UpdatePage();
        }

        public static void CreateSidePanel(ModHelperPanel mainPanel)
        {
            var panel = mainPanel.AddPanel(new("SidePanel_", -1400, 0, 700, 750), "");
            panel.Background.SetSprite(AssetHelper.GetSprite("ChristmasPanel"));
            giftText = panel.AddText(new Info("GiftsText", -75, 200, 400, 225), "Gifts: " + XmasMod2025.Gifts.FormatNumber(), 36, TextAlignmentOptions.Left);
            giftText.EnableAutoSizing(50);
            totalGiftText = panel.AddText(new Info("TotalGiftsText",-75, 0, 400, 225), "Total Gifts: " + XmasMod2025.TotalGifts.FormatNumber(), 36, TextAlignmentOptions.Left);
            totalGiftText.EnableAutoSizing(50);
            giftMultiplierText = panel.AddText(new Info("GiftMultiplierText", 150, -200, 850, 225),  "Gift Multi: " +XmasMod2025.GiftMult.FormatNumber() + "x", 36, TextAlignmentOptions.Left);
            giftMultiplierText.EnableAutoSizing(50);
        }
        
        private static IEnumerator UpdateLayouts()
        {
            GridLayoutGroup normalGroup = normal.AddComponent<GridLayoutGroup>();
            normalGroup.cellSize = new Vector2(575, 1300);
            normalGroup.spacing = new(35, 50);
            normalGroup.padding = new RectOffset { bottom = 50, left = 35, right = 35, top = 50 };
            normalGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            yield return new WaitForEndOfFrame();
            scroll.ScrollContent.RemoveComponent<LayoutGroup>();
            yield return new WaitForEndOfFrame();
            GridLayoutGroup scrollGroup = scroll.ScrollContent.AddComponent<GridLayoutGroup>();
            scrollGroup.cellSize = new Vector2(575, 1300);
            scrollGroup.spacing = new(35, 50);
            scrollGroup.padding = new RectOffset { bottom = 50, left = 17, right = 18, top = 50 };
            scrollGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            scroll.AddComponent<Mask>();
        }
        
        public static void HandleArrow(bool back)
        {
            List<ShopType> allTypes = ((ShopType[])Enum.GetValues(typeof(ShopType))).ToList();
            int currentPageIndex = allTypes.IndexOf(CurrentPage);
            int newIndex = currentPageIndex + (back ? -1 : 1);
            if(newIndex < 0) newIndex = allTypes.Count - 1;
            else if (newIndex >= allTypes.Count) newIndex = 0;
            CurrentPage = allTypes[newIndex];
            
            UpdatePage();
        }

        public static void UpdatePage()
        {
            List<GiftShopItem> items = GiftShopItem.GiftShopItems[CurrentPage];

            scroll.ScrollContent.gameObject.DestroyAllChildren();
            normal.gameObject.DestroyAllChildren();
            
            if (items.Count > 3)
            {
                scroll.gameObject.SetActive(true);
                normal.gameObject.SetActive(false);
                foreach (var item in items)
                {
                    scroll.AddScrollContent(ShopPanel(item));
                }
            }
            else
            {
                scroll.gameObject.SetActive(false);
                normal.gameObject.SetActive(true);
                foreach (var item in items)
                {
                    normal.AddModHelperComponent(ShopPanel(item));
                }
            }
            
            section.SetText(CurrentPage.ToString() ?? "");
        }

        public static void UpdateText()
        {
            if (giftText != null) giftText.SetText("Gifts: " + XmasMod2025.Gifts.FormatNumber());
            if (totalGiftText != null) totalGiftText.SetText("Total Gifts: " + XmasMod2025.TotalGifts.FormatNumber());
            if (giftMultiplierText != null) totalGiftText.SetText("Gift Multi: " + XmasMod2025.TotalGifts.FormatNumber() + "x");
        }
    }
}
 