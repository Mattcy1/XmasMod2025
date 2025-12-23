using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.BuffsItems;

public class FestiveSpirit : GiftShopItem
{
    public override ShopType Shop => ShopType.Buffs;
    public override double BaseCost => 700;
    public override string Icon => "XmasTree-Icon";

    public override string Description =>
        "All towers are in the festive mood for the duration of the next 3 rounds, gaining a bit of range and attack speed.";

    public override double PriceMultiplier => 1.15f;

    public override void Buy(InGame game)
    {
        XmasMod2025.FestiveSpiritActive = true;
        XmasMod2025.FestiveSpritActiveRounds += 3;
        BuffHandler.FestiveSpiritHandler();
    }
}