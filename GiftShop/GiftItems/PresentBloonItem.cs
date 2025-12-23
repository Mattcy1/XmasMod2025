using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.GiftItems;

public class PresentBloonChance : GiftShopItem
{
    public override ShopType Shop => ShopType.Gift;
    public override double BaseCost => 0;
    public override string Description => "Increase the chances of present bloons spawning.";
    public override string Icon => "BloonChance";

    public override int MaxUpgrades => 5;

    public override void Buy(InGame game)
    {
        XmasMod2025.PresentBloonChance += 5;
    }

    public override void Reset()
    {
        base.Reset();
        XmasMod2025.PresentBloonChance = 1;
    }

    public override double GetCostForUpgradeNumber(int upgrade)
    {
        return upgrade switch
        {
            1 => 500,
            2 => 750,
            3 => 1500,
            4 => 3500,
            5 => 7000,
            _ => base.GetCostForUpgradeNumber(upgrade)
        };
    }
}