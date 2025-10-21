using System;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using XmasMod2025.UI;

namespace XmasMod2025.GiftShop;

public abstract class GiftShopItem : NamedModContent
{
    public override void Register()
    {
        IconReference = IconIsGUID ? new SpriteReference(Icon) : GetSpriteReference(mod, Icon);
    }

    public abstract ShopType Shop { get; }

    public virtual int MaxUpgrades => -1;
    
    public virtual string Icon => Name + "-Icon";
    protected virtual bool IconIsGUID => false;

    public SpriteReference IconReference { get; protected set; }
    
    public virtual string? GetTooltipText(int upgrade) => null;
    
    public abstract double BaseCost { get; }
    public virtual double PriceMultiplier => 1.25f;
    public int Upgrades = 0;

    public abstract void Buy(InGame game);
    
    public virtual double GetCostForUpgradeNumber(int upgrade) => BaseCost * Math.Pow(PriceMultiplier, upgrade);
}