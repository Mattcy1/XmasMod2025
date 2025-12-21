using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Gameplay.Mods;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Linq;
using XmasMod2025.Towers;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.BuffsItems;

public class SantaHelperItem : GiftShopItem
{
    public override ShopType Shop => ShopType.Buffs;
    public override double BaseCost => 750;
    public override string Icon => "Elf";
    public override string DisplayName => "Santa's Helpers";
    public override string Description => "Elves spawns for the entire rounds helping you.";
    public override void Buy(InGame game)
    {
        BuffHandler.SantaHelperHandler();
    }

    public override double PriceMultiplier => 1.1f;
}
