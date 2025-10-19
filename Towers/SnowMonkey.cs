using Il2CppAssets.Scripts.Models.Towers;

namespace XmasMod2025.Towers;

public class SnowMonkey : ChristmasTower
{
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        
    }

    public override string BaseTower => "DartMonkey";
    public override int Cost => 35;
}