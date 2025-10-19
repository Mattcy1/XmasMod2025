using Il2CppAssets.Scripts.Models.Towers;

namespace XmasMod2025.Towers;

public class SnowflakeTest : ChristmasTower
{
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        
    }

    public override string BaseTower => TowerType.DartMonkey;
    public override int Cost => 4;
}