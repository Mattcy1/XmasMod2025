using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
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
    internal class MiddlePath
    {
        public class ImprovedEyesight : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => Middle;
            public override int Tier => 1;
            public override int Cost => 65;
            public override string DisplayName => "Improved Eyesight";
            public override string Description => "Increase the range of all weapons.";
            public override string Icon => "MiddlePathCane1";
            public override string Portrait => "CandyCaneMonkeyMiddle1-Portrait";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var attackModel in towerModel.GetAttackModels())
                {
                    attackModel.range += 10;
                }
                towerModel.range += 10;

                towerModel.ApplyDisplay<CandyCaneMonkey010>();
            }
        }
        public class EvenBetterEyesight : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => Middle;
            public override int Tier => 2;
            public override int Cost => 70;
            public override string Description => "Gains even more range, and can pop camos.";
            public override string Icon => "MiddlePathCane2";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var attackModel in towerModel.GetAttackModels())
                {
                    attackModel.range += 5;
                }

                towerModel.range += 5;
                towerModel.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            }
        }

        public class FireyCandyCane : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => Middle;
            public override int Tier => 3;
            public override int Cost => 225;
            public override string Description => "The candy cane is set on fire allowing it to pop Leads, and inflict dot";
            public override string Icon => "MiddlePathCane3";
            public override string Portrait => "Candy030Portrait";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetWeapon().projectile.GetDamageModel().immuneBloonProperties = Il2Cpp.BloonProperties.None;

                towerModel.GetWeapon().projectile.collisionPasses = new int[] { -1, 0 };
                var LavaBehavior = Game.instance.model.GetTowerFromId("Alchemist").GetDescendant<AddBehaviorToBloonModel>().Duplicate();
                LavaBehavior.GetBehavior<DamageOverTimeModel>().interval = 3f;
                LavaBehavior.GetBehavior<DamageOverTimeModel>().damage = 1;
                LavaBehavior.lifespan = 20;
                LavaBehavior.lifespanFrames = 1200;
                LavaBehavior.overlayType = "Fire";
                towerModel.GetWeapon().projectile.AddBehavior(LavaBehavior);
                towerModel.GetWeapon().projectile.ApplyDisplay<FireCandyCaneProj>();
                towerModel.ApplyDisplay<CandyCaneMonkey030>();
            }
        }
    }

    public class FireCandyCaneProj : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "FireCandyCane");
        }
    }


    public class CandyCaneRing : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => Middle;
        public override int Tier => 4;
        public override int Cost => 800;
        public override string Description => "Ability: Spawns a ring of candy cane around him.";
        public override string Icon => "MiddlePathCane4";
        public override string Portrait => "Candy040Portrait";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var proj = towerModel.GetWeapon().projectile;

            proj.GetDamageModel().damage += 1;
            proj.pierce++;

            var abilityModel = Game.instance.model.GetTowerFromId("DartlingGunner-040").GetAbility().Duplicate();
            abilityModel.cooldown = 120;
            abilityModel.icon = GetSpriteReference<XmasMod2025>("MiddlePathCane4");
            abilityModel.Cooldown = 120;
            abilityModel.name = "T4Cane";
            var activate = abilityModel.GetBehavior<ActivateAttackModel>();

            var attack = Game.instance.model.GetTowerFromId(ModContent.TowerID<CandyCaneMonkey>(0, 3, 0)).GetAttackModel().Duplicate();
            attack.weapons[0].emission = new ArcEmissionModel("ArcEmissionModel_", 80, 0, 360, null, false, false);
            attack.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan = 50;
            attack.weapons[0].projectile.GetDamageModel().damage += 10;
            attack.weapons[0].projectile.pierce = 15;
            attack.weapons[0].projectile.ApplyDisplay<CandyCaneProj>();
            attack.weapons[0].projectile.RemoveBehavior<AddBehaviorToBloonModel>();
            activate.attacks[0] = attack;
            activate.lifespan = 1;
            activate.Lifespan = 1;

            towerModel.AddBehavior(abilityModel);
            towerModel.ApplyDisplay<CandyCaneMonkey040>();
        }
    }

    public class CandyCaneStorm : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => Middle;
        public override int Tier => 5;
        public override int Cost => 3500;
        public override string Description => "Shoots candy canes so hot they melt bloons in seconds.";
        public override string Icon => "MiddlePathCane5";
        public override string Portrait => "Candy050Portrait";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.RemoveBehavior<AbilityModel>();

            var abilityModel = Game.instance.model.GetTowerFromId("DartlingGunner-040").GetAbility().Duplicate();
            abilityModel.cooldown = 180;
            abilityModel.icon = GetSpriteReference<XmasMod2025>("MiddlePathCane5");
            abilityModel.Cooldown = 180;
            abilityModel.name = "T5Cane";
            var activate = abilityModel.GetBehavior<ActivateAttackModel>();

            var attack = Game.instance.model.GetTowerFromId(ModContent.TowerID<CandyCaneMonkey>(0, 3, 0)).GetAttackModel().Duplicate();
            attack.weapons[0].emission = new ArcEmissionModel("ArcEmissionModel_", 80, 0, 360, null, false, false);
            attack.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan = 50;
            attack.weapons[0].projectile.GetDamageModel().damage += 15;
            attack.weapons[0].projectile.pierce = 30;
            var abiilityAdd = attack.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>();
            var abilityDot = abiilityAdd.GetBehavior<DamageOverTimeModel>();
            abilityDot.damage = 8f;
            abilityDot.interval = 0.25f;

            activate.attacks[0] = attack;
            activate.lifespan = 5;
            activate.attacks[0].weapons[0].fireWithoutTarget = true;
            activate.Lifespan = 5;

            towerModel.AddBehavior(abilityModel);

            var proj = towerModel.GetWeapon().projectile;

            proj.GetDamageModel().damage += 2;
            var dot = proj.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>();
            dot.damage = 5;
            dot.interval = 0.35f;

            towerModel.ApplyDisplay<CandyCaneMonkey050>();
        }
    }

    public class CandyCaneMonkey030 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("WizardMonkey").display.AssetGUID;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, Name);
            SetMeshTexture(node, Name, 1);
        }
    }

    public class CandyCaneMonkey040 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("Druid-300").display.AssetGUID;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "CandyCaneMonkey040");
            SetMeshTexture(node, "CandyCaneMonkey040", 1);
            SetMeshTexture(node, "CandyCaneMonkey040", 2);
            SetMeshOutlineColor(node, UnityEngine.Color.white);
            SetMeshOutlineColor(node, UnityEngine.Color.white, 1);
            SetMeshOutlineColor(node, UnityEngine.Color.white, 2);
        }
    }

    public class CandyCaneMonkey050 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("Druid-500").display.AssetGUID;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "CandyCaneMonkey040");
            SetMeshTexture(node, "CandyCaneMonkey040", 1);
            SetMeshOutlineColor(node, UnityEngine.Color.white);
            SetMeshOutlineColor(node, UnityEngine.Color.white, 1);
        }
    }
}

