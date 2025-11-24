using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmasMod2025;
using XmasMod2025.Towers;
using static UnityEngine.ExpressionEvaluator;
using static XmasMod2025.Towers.CandyCaneMonkey;

namespace XmasMod2025.Towers.Upgrades.CandyCane
{
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
                foreach (var weapons in towerModel.GetWeapons())
                {
                    weapons.projectile.pierce += 1;
                }

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
                CreateProjectileOnExhaustPierceModel createProjectileOnContactModel = new CreateProjectileOnExhaustPierceModel("candycanebreak", smallerProj, new ArcEmissionModel("ArcEmissionModel_Split", 3, 0, 30, null, true, false), 0, 3, 0, true, CreatePrefabReference<ShardProj>(), 5, true, false);

                proj.AddBehavior(createProjectileOnContactModel);
            }
        }

        public class Tier3CaneTop : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => TOP;
            public override int Tier => 3;
            public override int Cost => 150;
            public override string DisplayName => "";
            public override string Description => "";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
            }
        }
    }

    public class Tier4CaneTop : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => TOP;
        public override int Tier => 4;
        public override int Cost => 800;
        public override string DisplayName => "";
        public override string Description => "";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }

    public class Tier5CaneTop : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => TOP;
        public override int Tier => 5;
        public override int Cost => 2000;
        public override string DisplayName => "";
        public override string Description => "";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }
}

