using BTD_Mod_Helper.Api.Towers;

namespace XmasMod2025.Towers;

public abstract class ChristmasTower : ModTower<XmasTowerSet>
{
    public virtual bool CostsGifts => true;
}