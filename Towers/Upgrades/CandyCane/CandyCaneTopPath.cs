using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using static XmasMod2025.Towers.CandyCaneMonkey;

namespace XmasMod2025.Towers.Upgrades.CandyCane;

internal class TopPath
{
    public class PointyCandycane : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => TOP;
        public override int Tier => 1;
        public override int Cost => 55;
        public override string DisplayName => "Pointy Candycane";
        public override string Description => "Allows the candy cane to pop more bloons.";
        public override string Icon => "TopPathCane1";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var weapons in towerModel.GetWeapons()) weapons.projectile.pierce += 1;

            towerModel.GetWeapon().projectile.ApplyDisplay<PointyCandyCaneProj>();
        }
    }

    public class PointyCandyCaneProj : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "PointyCandyCane");
        }
    }

    public class MoreFragileCandyCane : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => TOP;
        public override int Tier => 2;
        public override int Cost => 85;
        public override string Description => "The more fragile candycane, now splits in 3 projectile instead of 2.";
        public override string Icon => "TopPathCane2";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetWeapon().projectile.RemoveBehavior<CreateProjectileOnExhaustPierceModel>();

            var weapon = towerModel.GetWeapon();
            var proj = weapon.projectile;

            var smallerProj = proj.Duplicate();
            smallerProj.pierce = 2;
            smallerProj.ApplyDisplay<ShardProj>();
            var createProjectileOnContactModel = new CreateProjectileOnExhaustPierceModel("candycanebreak", smallerProj,
                new ArcEmissionModel("ArcEmissionModel_Split", 3, 0, 30, null, true, false), 0, 3, 0, true,
                CreatePrefabReference<ShardProj>(), 5, true, false);

            proj.AddBehavior(createProjectileOnContactModel);
        }
    }

    public class DoubleShot : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => TOP;
        public override int Tier => 3;
        public override int Cost => 250;
        public override string Description => "Now shoots, 2 Candy Cane Instead of one.";
        public override string Icon => "TopPathCane3";
        public override string Portrait => "Candy300Portrait";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetWeapon().emission = new ArcEmissionModel("ArcEmissionModel_", 2, 0, 20, null, false, false);
            towerModel.ApplyDisplay<CandyCaneMonkey300>();
        }
    }
}

public class Return : ChristmasUpgrade<CandyCaneMonkey>
{
    public override int Path => TOP;
    public override int Tier => 4;
    public override int Cost => 2000;
    public override string DisplayName => "Candy Cane Boomerang";
    public override string Description => "The projectile, now acts like a boomerang.";
    public override string Icon => "TopPathCane4";
    public override string Portrait => "Candy400Portrait";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var Return = Game.instance.model.GetTowerFromId("BoomerangMonkey").GetWeapon().projectile
            .GetBehavior<FollowPathModel>().Duplicate();
        towerModel.GetWeapon().projectile.AddBehavior(Return);
        towerModel.GetWeapon().projectile.GetBehavior<TravelStraitModel>().lifespan = 5;
        towerModel.GetWeapon().projectile.GetBehavior<TravelStraitModel>().Lifespan = 5;
        towerModel.GetWeapon().projectile.ApplyDisplay<BoomerangProj>();
        towerModel.ApplyDisplay<CandyCaneMonkey400>();
    }
}

public class CandyCaneOverlord : ChristmasUpgrade<CandyCaneMonkey>
{
    public override int Path => TOP;
    public override int Tier => 5;
    public override int Cost => 7000;

    public override string Description =>
        "Throws, 5 Boomerang at once, increased pierce and damage, projectile splits into even more shards.";

    public override string Icon => "TopPathCane5";
    public override string Portrait => "Candy500Portrait";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetWeapon().emission = new ArcEmissionModel("ArcEmissionModel_", 5, 0, 30, null, false, false);
        towerModel.GetWeapon().projectile.RemoveBehavior<CreateProjectileOnExhaustPierceModel>();

        var weapon = towerModel.GetWeapon();
        var proj = weapon.projectile;

        proj.pierce += 5;
        proj.GetDamageModel().damage = 12;

        var smallerProj = proj.Duplicate();
        smallerProj.pierce = 4;
        smallerProj.GetDamageModel().damage = 7;
        smallerProj.ApplyDisplay<ShardProj>();
        var createProjectileOnContactModel = new CreateProjectileOnContactModel("candycanebreak", smallerProj,
            new ArcEmissionModel("ArcEmissionModel_Split", 5, 0, 30, null, true, false), false, false, true);

        proj.AddBehavior(createProjectileOnContactModel);
        towerModel.ApplyDisplay<CandyCaneMonkey500>();
    }
}

public class CandyCaneMonkey500 : ModDisplay
{
    public override string BaseDisplay => Game.instance.model.GetTowerFromId("BoomerangMonkey-500").display.AssetGUID;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, Name);
        SetMeshTexture(node, Name, 1);
        SetMeshTexture(node, Name, 2);
    }
}

public class CandyCaneMonkey400 : ModDisplay
{
    public override string BaseDisplay => Game.instance.model.GetTowerFromId("BoomerangMonkey").display.AssetGUID;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, "CandyCaneMonkey400");
        SetMeshTexture(node, "CandyCaneMonkey400", 1);
    }
}

public class CandyCaneMonkey300 : ModDisplay
{
    public override string BaseDisplay => Game.instance.model.GetTowerFromId("DartMonkey-030").display.AssetGUID;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, "CandyCaneMonkey300");
        SetMeshTexture(node, "CandyCaneMonkey300", 1);
        SetMeshTexture(node, "CandyCaneMonkey300", 2);
    }
}

public class BoomerangProj : ModDisplay
{
    public override string BaseDisplay => Generic2dDisplay;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        Set2DTexture(node, "BoomerangCandy");
    }
}