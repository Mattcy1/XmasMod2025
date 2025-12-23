using System;
using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;

namespace XmasMod2025.Towers.Upgrades;

public class BuildersSpirit : ChristmasUpgrade<ElfMonkey>
{
    public override string Description =>
        "All sub-towers within range created by engineer monkeys and elf monkeys attack faster and get more range.";

    public override int Path => Top;
    public override int Tier => 1;
    public override int Cost => 25;

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        List<TowerFilterModel> filters =
        [
            new FilterInBaseTowerIdModel("SubTowerFilterModel_BuildersSpirit",
                new Il2CppStringArray(["Sentry", TowerID<ToyMortar.ToyMortarTower>(), TowerID<ToyCart.ToyCartTower>()]))
        ];

        var rateSupportModel = new RateSupportModel("RateSupportModel_BuildersSpirit", 0.85f, true,
            "Rate:BuildersSpirit", false, 1, new Il2CppReferenceArray<TowerFilterModel>(filters.ToArray()), "", "");
        var rangeSupportModel = new RangeSupportModel("RangeSupportModel_BuildersSpirit", true, 0.15f, 0,
            "Range:BuildersSpirit", new Il2CppReferenceArray<TowerFilterModel>(filters.ToArray()), false, "", "");
        rateSupportModel.ApplyBuffIcon<BuildersSpiritBuffIcon>();

        towerModel.AddBehavior(rateSupportModel);
        towerModel.AddBehavior(rangeSupportModel);
    }

    public class BuildersSpiritBuffIcon : ModBuffIcon;
}

public class BuildersAtmosphere : ChristmasUpgrade<ElfMonkey>
{
    public override string Description =>
        "Builder's spirit has more range, and provides an extra 5% range.";

    public override int Path => Top;
    public override int Tier => 2;
    public override int Cost => 45;
    public override string Icon => "Elf200Icon";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var rateSupportModel = towerModel.GetBehavior<RateSupportModel>("RateSupportModel_BuildersSpirit");
        var rangeSupportModel = towerModel.GetBehavior<RangeSupportModel>("RangeSupportModel_BuildersSpirit");

        rateSupportModel.customRadius = towerModel.range *= 1.1f;
        rateSupportModel.isCustomRadius = true;
        rangeSupportModel.customRadius = towerModel.range * 1.1f;
        rangeSupportModel.isCustomRadius = true;
        rangeSupportModel.multiplier += 0.05f;
    }
}

public class ToyMortar : ChristmasUpgrade<ElfMonkey>
{
    public override int Path => Top;
    public override int Tier => 3;
    public override int Cost => 55;

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var toyMortarAttack = Game.instance.model.GetTowerFromId("EngineerMonkey-100").GetAttackModel(1).Duplicate();
        var toyMortarWeapon = toyMortarAttack.weapons[0];
        toyMortarAttack.name = "ToyMortar";
        toyMortarWeapon.name = "ToyMortar";
        toyMortarWeapon.projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<ToyMortarTower>();
        toyMortarWeapon.AddBehavior(new EmissionsPerRoundFilterModel("EmissionsPerRoundFilterModel", 2));
        //toyMortarAttack.weapons[0].projectile.ApplyDisplay<ToyBox>(); // Display doesn't exist yet.

        towerModel.AddBehavior(toyMortarAttack);
    }

    public class ToyMortarTower : ModSubTower
    {
        public override string Portrait => "ToyMortar-Icon";

        public override TowerSet TowerSet => GetTowerSet<XmasTowerSet>();
        public override string BaseTower => TowerType.Sentry;

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.range = 40f;
            towerModel.GetAttackModel().range = 40f;
            towerModel.GetAttackModel().RemoveBehavior<DisplayModel>();
            towerModel.GetAttackModel().RemoveBehavior<RotateToTargetModel>();

            var weapon = towerModel.GetWeapon();
            weapon.SetProjectile(Game.instance.model.GetTowerFromId("MortarMonkey-201").GetWeapon().projectile
                .Duplicate());
            weapon.projectile.ApplyDisplay<ToyBomb>();
        }

        public class ToyMortarTowerDisplay : ModTowerCustomDisplay<ToyMortarTower>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "Mortar";

            public override bool UseForTower(params int[] tiers)
            {
                return true;
            }

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
            }
        }

        public class ToyBomb : ModDisplay2D
        {
            protected override string TextureName => Name;

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                node.customScaleAnimator = node.gameObject.AddComponent<CustomScaleAnimator>();
                node.customScaleAnimator.RootScale = node.Scale;
                node.customScaleAnimator.curve =
                    new AnimationCurve(new Keyframe(0, 1), new Keyframe(0, 1.25f), new Keyframe(1, 1));
            }
        }
    }
}

public class ToyCart : ChristmasUpgrade<ElfMonkey>
{
    public static Dictionary<Projectile, Tower> TowerForProjectile = new();
    public static Dictionary<Tower, Projectile> ProjectileForTower = new();

    public override int Path => Top;
    public override int Tier => 4;
    public override int Cost => 1200;

    public override string Description =>
        "Elf monkey can now create carts which run over bloons for <b>three</b> damage (MOABs are immune) and have fast shooting turrets that only do <b>one</b> damage each shot.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var newAtk = towerModel.GetAttackModel().Duplicate();
        newAtk.fireWithoutTarget = true;

        var weapon = newAtk.weapons[0];
        var projectile = weapon.projectile;
        weapon.SetEmission(new EmissionAtClosestPathSegmentModel("EmissionAtClosestPathSegmentModel_", 1, 0,
            new Il2CppReferenceArray<EmissionBehaviorModel>(0)));
        projectile.AddBehavior(new TravelAlongPathModel("TravelAlongPathModel_ToyCart", 30, 9999, true, false, 0));
        projectile.RemoveBehavior<TravelStraitModel>();
        projectile.id = "ToyCart_Low";
        projectile.pierce = float.MaxValue;
        projectile.canCollideWithBloons = false;
        projectile.GetDamageModel().damage = 2;
        projectile.ApplyDisplay<ToyCartProjectile>();
        projectile.RemoveBehavior<RandomDisplayModel>();
        projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_MoabImmunity", "Moabs", 0, 0,
            false, false));
        weapon.fireWithoutTarget = true;
        weapon.rate = 5;

        towerModel.AddBehavior(newAtk);
    }

    public class ToyCartTower : ModTower
    {
        public override string Portrait => "ToyCart-Icon";

        public override int MiddlePathUpgrades => 1;

        public override TowerSet TowerSet => GetTowerSet<XmasTowerSet>();
        public override string BaseTower => TowerType.DartMonkey;
        public override int Cost => 0;

        public override string DisplayName => "Toy Cart";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.areaTypes = new Il2CppStructArray<AreaType>([
                AreaType.ice,
                AreaType.land,
                AreaType.removable,
                AreaType.track,
                AreaType.water,
                AreaType.unplaceable,
                AreaType.water
            ]);

            towerModel.IncreaseRange(15);

            var weapon = towerModel.GetWeapon();
            weapon.SetProjectile(Game.instance.model.GetTower(TowerType.BoomerangMonkey).GetWeapon().projectile
                .Duplicate());
            weapon.projectile.GetDamageModel().damage += 1;
            weapon.rate = 0.175f; // Just under 6/s (~5.88)

            towerModel.dontDisplayUpgrades = true;

            towerModel.isSubTower = true;
            towerModel.AddBehavior(new CreditPopsToParentTowerModel("poptoelf"));
        }

        public class ToyTurretBody : ModTowerCustomDisplay<ToyCartTower>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => Name;

            public override bool UseForTower(params int[] tiers)
            {
                return tiers.Sum() == 0;
            }

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                var renderer = node.GetMeshRenderer();
                renderer.ApplyOutlineShader();
                renderer.SetOutlineColor(new Color32(40, 16, 12, 255));
            }
        }
    }

    public class ToyCartProjectile : ModCustomDisplay
    {
        public override string AssetBundleName => "xmas";
        public override string PrefabName => "ToyCartT1";

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                if (renderer.name.StartsWith("Wheel_"))
                {
                    renderer.transform.rotation = Quaternion.Euler(0, 90, 0);
                    var rot = renderer.gameObject.AddComponent<SimpleRotation>();
                }

                renderer.ApplyOutlineShader();
                renderer.SetOutlineColor(new Color32(59, 35, 13, 255));
            }
        }

        [RegisterTypeInIl2Cpp]
        public class SimpleRotation : MonoBehaviour // NK why must you set every other axis to 0 with your component
        {
            public Vector3 axis = new(0, 0, 1); // Multiplier for x, y, z

            public float rotationSpeed = 5; // Degrees/Update

            private void FixedUpdate() // 20 fps
            {
                transform.Rotate(axis, 5);
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), nameof(Projectile.Initialise))]
    private static class Projectile_Initialize
    {
        public static Projectile LastProjectile;
        public static uint ToyCartCounter;

        public static void Postfix(Projectile __instance)
        {
            if (__instance.projectileModel.id == "ToyCart_Low")
            {
                var towerId = ObjectId.Create(9999 + ToyCartCounter++);
                LastProjectile = __instance;
                InGame.instance.bridge.CreateTowerAt(__instance.Position.ToUnity(), GetTowerModel<ToyCartTower>(),
                    towerId, false, new Action<bool>(suc => { }), true, true, false, 0, false);
            }
            else if (__instance.projectileModel.id == "ToyCart_High")
            {
                var towerId = ObjectId.Create(9999 + ToyCartCounter++);
                LastProjectile = __instance;
                InGame.instance.bridge.CreateTowerAt(__instance.Position.ToUnity(), GetTowerModel<ToyCartTower>(0, 1),
                    towerId, false, new Action<bool>(suc => { }), true, true, false, 0, false);
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), nameof(Projectile.OnDestroy))]
    private static class Projectile_OnDestroy
    {
        public static void Postfix(Projectile __instance)
        {
            if (TowerForProjectile.ContainsKey(__instance))
            {
                ProjectileForTower.Remove(TowerForProjectile[__instance]);
                TowerForProjectile[__instance].Destroy();
                TowerForProjectile.Remove(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Tower), nameof(Il2CppAssets.Scripts.Simulation.Towers.Tower.OnSold))]
    [HarmonyPatch(typeof(Tower), nameof(Il2CppAssets.Scripts.Simulation.Towers.Tower.OnDestroy))]
    private static class Tower_OnDestroy
    {
        public static void Postfix(Tower __instance)
        {
            if (ProjectileForTower.ContainsKey(__instance))
            {
                TowerForProjectile.Remove(ProjectileForTower[__instance]);
                ProjectileForTower[__instance].Destroy();
                ProjectileForTower.Remove(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Tower), nameof(Il2CppAssets.Scripts.Simulation.Towers.Tower.Initialise))]
    private static class Tower_Initialize
    {
        public static void Postfix(Tower __instance)
        {
            if (__instance.towerModel.baseId == TowerID<ToyCartTower>())
            {
                TowerForProjectile.Add(Projectile_Initialize.LastProjectile, __instance);
                __instance.ParentId = Projectile_Initialize.LastProjectile.EmittedByTowerId;
            }
        }
    }

    [HarmonyPatch(typeof(TravelAlongPath), nameof(TravelAlongPath.Process))]
    private static class TravelAlongPath_Process
    {
        public static void Postfix(TravelAlongPath __instance)
        {
            if (!__instance.travelAlongPathModel.name.EndsWith("ToyCart") ||
                !TowerForProjectile.ContainsKey(__instance.projectile)) return;

            var projectile = __instance.projectile;
            var tower = TowerForProjectile[projectile];

            if (tower.IsDestroyed)
            {
                projectile.Destroy();
                MelonLogger.Msg("test");
                return;
            }

            tower.PositionTower(projectile.Position.ToVector2());
            //tower.Rotation = projectile.Rotation; // Messes up some stuff
        }
    }
}

public class MasterCrafter : ChristmasUpgrade<ElfMonkey>
{
    public override int Path => Top;
    public override int Tier => 5;
    public override int Cost => 6000;

    public override string Description =>
        "Creates newly enhanced toy carts and mortars and a much faster rate!";

    public override void LateApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetWeapon(1).projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<EnhancedToyMortar>();
    }

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var toyMortarWeapon = towerModel.GetWeapon(1);
        var toyCartWeapon = towerModel.GetWeapon(2);
        var toyCartProjectile = toyCartWeapon.projectile;

        toyMortarWeapon.RemoveBehavior<EmissionsPerRoundFilterModel>();
        toyMortarWeapon.rate *= 0.25f;


        toyCartWeapon.rate *= 0.35f;

        toyCartProjectile.id = "ToyCart_High";
        toyCartProjectile.GetDamageModel().damage = 7;
        toyCartProjectile.ApplyDisplay<Cart2>();
    }

    public class ToyCart2 : ModUpgrade<ToyCart.ToyCartTower>
    {
        public override int Path => Middle;
        public override int Tier => 1;
        public override int Cost => 0;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.ApplyDisplay<Cart2Tower>();

            var weapon = towerModel.GetWeapon();
            var projectile = weapon.projectile;

            projectile.ApplyDisplay<MetalBoomerang>();
            projectile.GetDamageModel().damage = 15;
            projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Moabs", "Moabs", 3, 0,
                false, true));
            projectile.AddBehavior(new KnockbackModel("KnockbackModel_ToyCart", 0.02f, 0.05f, 0.1f, 1, "KB:ToyCart"));
            projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;

            towerModel.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
        }

        public class Cart2Tower : ModTowerCustomDisplay<ToyCart.ToyCartTower>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "Cart2TowerFixed";

            public override bool UseForTower(params int[] tiers)
            {
                return tiers.Sum() > 0;
            }

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                var renderer = node.GetMeshRenderer();
                renderer.enabled = true;
                renderer.ApplyOutlineShader();
                renderer.SetOutlineColor(new Color32(135, 135, 135, 255));
            }
        }

        public class MetalBoomerang : ModDisplay2D
        {
            protected override string TextureName => Name;
        }
    }

    public class EnhancedToyMortar : ModSubTower
    {
        public override string Portrait => "EnhancedToyMortar-Portrait";

        public override TowerSet TowerSet => GetTowerSet<XmasTowerSet>();
        public override string BaseTower => TowerType.Sentry;

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.range = 40f;
            towerModel.GetAttackModel().range = 40f;
            towerModel.GetAttackModel().RemoveBehavior<DisplayModel>();
            towerModel.GetAttackModel().RemoveBehavior<RotateToTargetModel>();

            var weapon = towerModel.GetWeapon();
            weapon.SetProjectile(Game.instance.model.GetTowerFromId("BombShooter-202").GetWeapon().projectile
                .Duplicate());
            weapon.rate /= 4;
            var damageModel = weapon.projectile.GetDescendant<DamageModel>();
            damageModel.damage *= 7;
            damageModel.immuneBloonProperties = BloonProperties.None;
            var proj = weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile;
            proj.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Moabs", "Moabs", 2f, 3, false,
                true));
            proj.AddBehavior(Game.instance.model.GetTowerFromId("MortarMonkey-300").GetDescendant<SlowModel>()
                .Duplicate());
            weapon.projectile.ApplyDisplay<ToyMortar.ToyMortarTower.ToyBomb>();

            var tower = new TowerExpireModel("name", 999, 1, true, false);
            towerModel.AddBehavior(tower);

            towerModel.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
        }

        public class ToyMortarV2Display : ModTowerCustomDisplay<EnhancedToyMortar>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "ToyMortarV2";

            public override bool UseForTower(params int[] tiers)
            {
                return true;
            }

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.ApplyOutlineShader();
                    if (renderer.name.StartsWith("Wall"))
                        renderer.SetOutlineColor(new Color32(48, 51, 54, 255));
                    else
                        renderer.SetOutlineColor(new Color32(63, 63, 63, 255));
                }
            }
        }
    }


    public class Cart2 : ModCustomDisplay
    {
        public override string AssetBundleName => "xmas";
        public override string PrefabName => Name;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.ApplyOutlineShader();
                if (!renderer.name.StartsWith("Wheel"))
                    renderer.SetOutlineColor(new Color32(135, 135, 135, 255));
                else
                    renderer.gameObject.AddComponent<ToyCart.ToyCartProjectile.SimpleRotation>();
            }
        }
    }
}