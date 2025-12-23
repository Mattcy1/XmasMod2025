using System;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using MelonLoader;
using UnityEngine;

namespace XmasMod2025.UI;

public class Gift
{
    [RegisterTypeInIl2Cpp(false)]
    public class GiftOpenerUI : MonoBehaviour
    {
        public static GiftOpenerUI instance;
        public static ModHelperText? gift;

        public void Close()
        {
            if (gameObject) gameObject.Destroy();
        }

        public static void CreatePanel()
        {
            if (InGame.instance != null)
            {
                var rect = InGame.instance.uiRect;
                var panel = rect.gameObject.AddModHelperPanel(new Info("Panel_", 0, 0, 0, 0),
                    VanillaSprites.BrownPanel);
                panel.transform.SetParent(FindFirstObjectByType<RoundDisplay>().transform.parent.parent);
                instance = panel.AddComponent<GiftOpenerUI>();

                var Claim = panel.AddButton(new Info("Button_", 1515f, 980f, 240f),
                    ModContent.GetTextureGUID<XmasMod2025>("PresentsButton"), new Action(() =>
                    {
                        if (GiftMenu.instance == null)
                            GiftMenu.CreatePanel();
                        else
                            GiftMenu.instance.Close();
                    }));

                gift = Claim.AddText(new Info("Text_", 0, -65 /*-120*/, 200, 150), "Gift Menu");
            }
        }
    }
}