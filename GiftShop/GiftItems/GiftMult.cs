using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.GiftItems;

public class GiftMultiplier : GiftShopItem
{
    public override ShopType Shop => ShopType.Gift;
    public override double BaseCost => 0;
    public override string Description => "Increase the total Multiplier of gifts.";
    public override string Icon => "GiftMult";

    public override int MaxUpgrades => 5;

    public override void Buy(InGame game)
    {
        XmasMod2025.GiftMult += 0.2;
    }

    public override void Reset()
    {
        base.Reset();
        XmasMod2025.GiftMult = 1;
    }

    public override double GetCostForUpgradeNumber(int upgrade)
    {
        return upgrade switch
        {
            1 => 1000,
            2 => 1750,
            3 => 3400,
            4 => 7000,
            5 => 14000,
            _ => base.GetCostForUpgradeNumber(upgrade)
        };
    }
}