using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Gameplay.Mods;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Linq;
using UnityEngine;
using XmasMod2025.Towers;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.BuffsItems;

public class SantaHelperItem2 : GiftShopItem
{
    public override ShopType Shop => ShopType.Buffs;
    public override double BaseCost => 750;
    public override void Buy(InGame game)
    {
    }

    public override double PriceMultiplier => 1.1f;
}
