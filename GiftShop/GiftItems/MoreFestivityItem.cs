using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.GiftItems;

public class MoreFestivity : GiftShopItem
{
    public override ShopType Shop => ShopType.Gift;
    public override double BaseCost => 0;
    public override string Icon => "Festive";
    public override void Buy(InGame game)
    {
        XmasMod2025.TreeDropRates += (5, 7);
    }
    public override string Description => "Increases the xmas tree’s gifts drop rate by a min of 5 and max of 7 each upgrade.";

    public override int MaxUpgrades => 2;

    public override double GetCostForUpgradeNumber(int upgrade)
    {
        return upgrade switch
        {
            1 => 750,
            2 => 1250,
            _ => base.GetCostForUpgradeNumber(upgrade)
        };
    }
}