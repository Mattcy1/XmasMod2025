using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop.GiftItems;

public class AssemblyLine : GiftShopItem
{
    public override ShopType Shop => ShopType.Gift;
    public override double BaseCost => 0;
    public override string Description => "Gain gifts per second based on total gifts";
    public override void Buy(InGame game)
    {
        XmasMod2025.UpgradeCount += 1;
        if(XmasMod2025.UpgradeCount == 1)
        {
            MelonCoroutines.Start(XmasMod2025.HandleGPS());
        }
    }

    public override void Reset()
    {
        base.Reset();
        XmasMod2025.UpgradeCount = 0;
    }

    public override int MaxUpgrades => 4;

    public override double GetCostForUpgradeNumber(int upgrade)
    {
        return upgrade switch
        {
            1 => 2000,
            2 => 4000,
            3 => 8000,
            4 => 16000,
            _ => base.GetCostForUpgradeNumber(upgrade)
        };
    }
}