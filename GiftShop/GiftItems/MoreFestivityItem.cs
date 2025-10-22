using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.GiftItems;

public class MoreFestivity : GiftShopItem
{
    public override ShopType Shop => ShopType.Gift;
    public override double BaseCost => 0;
    public override void Buy(InGame game)
    {
        XmasMod2025.TreeDropRates[0] += 5;
        XmasMod2025.TreeDropRates[1] += 7;
    }
    public override string Description => "Increases, the xmas tree’s gifts drop rate.";

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