using BTD_Mod_Helper.Api.Towers;

namespace XmasMod2025.Towers;

public abstract class ChristmasTower : ModTower<XmasTowerSet>
{
    public virtual CurrencyType Currency => CurrencyType.Gift;
}

public abstract class ChristmasUpgrade : ModUpgrade
{
    public abstract ChristmasTower ChristmasTower { get; }
    
    public override ModTower Tower => ChristmasTower;
    public virtual CurrencyType Currency => CurrencyType.Gift;
}
public abstract class ChristmasUpgrade<T> : ChristmasUpgrade where T : ChristmasTower
{
    public override ChristmasTower ChristmasTower => GetInstance<T>();
}