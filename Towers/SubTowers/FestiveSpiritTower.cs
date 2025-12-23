using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace XmasMod2025.Towers.SubTowers;

public class FestiveSpiritTower : ChristmasTower
{
    public override string BaseTower => TowerType.MonkeyVillage + "-200";
    public override string Icon => VanillaSprites.MonkeyVillageElfPetIcon;
    public override string Portrait => Icon;
    public override string Description => "One of Santa's Minions, help you defend.";
    public override int Cost => 0;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.dontDisplayUpgrades = true;
        towerModel.displayScale = 0;
        towerModel.display = new PrefabReference("");

        foreach (var am in towerModel.GetAttackModels()) am.range = 9999;

        towerModel.range = 9999;
        towerModel.ignoreTowerForSelection = true;

        var range = towerModel.GetBehavior<RangeSupportModel>();
        range.multiplier = 0.2f;
        range.mutatorId = "FestiveSpiritRangeBuff";
        range.name = "FestiveSpiritRange";
        range.buffIconName = "";
        range.buffLocsName = "";
        range.customRadius = 9999;
        range.showBuffIcon = false;

        var rate = towerModel.GetBehavior<RateSupportModel>();
        rate.multiplier = 0.6f;
        rate.mutatorId = "FestiveSpiritRateBuff";
        rate.name = "FestiveSpiritRate";
        rate.ApplyBuffIcon<FestiveSpiritBuff>();
        rate.customRadius = 9999;
        rate.showBuffIcon = true;

        towerModel.radius = 0;
    }
}

public class FestiveSpiritBuff : ModBuffIcon
{
    public override string Icon => "Gift1";
}