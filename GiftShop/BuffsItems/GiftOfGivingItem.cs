using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.Gameplay.Mods;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Linq;
using XmasMod2025.Towers;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.BuffsItems;

public class GiftOfGiving : GiftShopItem
{
    public override ShopType Shop => ShopType.Buffs;
    public override double BaseCost => 300;

    public override string Description => "Give a random tower a present, giving him extra range and attack speed for the duration of this round.";
    public override void Buy(InGame game)
    {
        if (InGame.instance == null) return;

        var validTowers = InGame.instance.GetTowers().Where(t =>
            !t.towerModel.isSubTower &&                                
            t.towerModel.baseId != ModContent.TowerID<XmasTree>() &&   
            t.towerModel.baseId != ModContent.TowerID<GiftMonkey>()    
        ).ToList();

        var random = new System.Random();
        var chosen = validTowers[random.Next(validTowers.Count)];

        XmasMod2025.GiftOfGivingTowersIds.Add(chosen.towerModel);

        BuffHandler.GiftOfGivingHandler(true);
    }


    public override double PriceMultiplier => 1.1f;
}
