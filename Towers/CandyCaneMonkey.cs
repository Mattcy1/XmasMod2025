using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;

namespace XmasMod2025.Towers;

public class CandyCaneMonkey : ChristmasTower
{
    public override string BaseTower => "DartMonkey";
    public override int Cost => 85;
    public override string Icon => Portrait;
    public override string Description => "Shoots, candy cane that spilt into 2 more candy cane, deals X2 damage to Candy Type Bloon";
    public override int TopPathUpgrades => 2;
    public override int MiddlePathUpgrades => 2;
    public override int BottomPathUpgrades => 2;
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
        CreateProjectileOnExhaustPierceModel createProjectileOnContactModel = new CreateProjectileOnExhaustPierceModel("candycanebreak", smallerProj, new ArcEmissionModel("ArcEmissionModel_Split", 2, 0, 30, null, true, false), 0, 2, 0, true, CreatePrefabReference<ShardProj>(), 5, true, false);

        proj.AddBehavior(createProjectileOnContactModel);
        towerModel.ApplyDisplay<CandyCaneMonkey000>();
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

    public class CandyCaneMonkey000 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("DartMonkey-020").display.AssetGUID;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "CandyCaneMonkey000");
            SetMeshTexture(node, "CandyCaneMonkey000", 1);
            SetMeshTexture(node, "CandyCaneMonkey000", 2);
        }
    }

    public class CandyCaneMonkey010 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("DartMonkey-022").display.AssetGUID;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "CandyCaneMonkey000");
            SetMeshTexture(node, "CandyCaneMonkey000", 1);
            SetMeshTexture(node, "CandyCaneMonkey000", 2);
        }
    }
}