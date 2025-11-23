using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.Display;

namespace XmasMod2025.Towers;

public class CandyCaneMonkey : ChristmasTower
{
    public override string BaseTower => "DartMonkey";
    public override int Cost => 85;
    public override string Icon => "CaneMonkey";
    public override string Portrait => Icon;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        var weapon = towerModel.GetWeapon();
        var proj = weapon.projectile;

        proj.GetDamageModel().damage = 1;
        proj.pierce = 2;
        proj.ApplyDisplay<CandyCaneProj>();

        var smallerProj = proj.Duplicate();
        smallerProj.pierce = 1;
        smallerProj.ApplyDisplay<ShardProj>();
        CreateProjectileOnContactModel createProjectileOnContactModel = new CreateProjectileOnContactModel("candycanebreak", smallerProj, new ArcEmissionModel("ArcEmissionModel_Split", 2, 0, 30, null, true, false), true, false, true);

        proj.AddBehavior(createProjectileOnContactModel);
    }

    public class CandyCaneProj : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "CandyCane");
        }
    }

    public class ShardProj : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Shard");
        }
    }
}