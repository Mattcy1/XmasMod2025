
using System.Collections.Generic;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Linq;
using BTD_Mod_Helper;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Simulation.Input;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Track;
using Il2CppAssets.Scripts.Unity.Bridge;
using MelonLoader;
using UnityEngine;
using Vector2 = Il2CppAssets.Scripts.Simulation.SMath.Vector2;

namespace XmasMod2025.Towers.Upgrades;

public class BuildersSpirit : ChristmasUpgrade<ElfMonkey>
{
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        List<TowerFilterModel> filters =
        [
            new FilterInBaseTowerIdModel("SubTowerFilterModel_BuildersSpirit", new(["Sentry", TowerID<ToyMorter.ToyMorterTower>(), TowerID<ToyCart.ToyCartTower>()]))
        ];

        var rateSupportModel = new RateSupportModel("RateSupportModel_BuildersSpirit", 0.85f, true, "Rate:BuildersSpirit", false, 1, new Il2CppReferenceArray<TowerFilterModel>(filters.ToArray()), "", "");
        var rangeSupportModel = new RangeSupportModel("RangeSupportModel_BuildersSpirit", true, 0.15f, 0, "Range:BuildersSpirit", new Il2CppReferenceArray<TowerFilterModel>(filters.ToArray()), false, "", "");
        rateSupportModel.ApplyBuffIcon<BuildersSpiritBuffIcon>();

        towerModel.AddBehavior(rateSupportModel);
        towerModel.AddBehavior(rangeSupportModel);
    }

    public class BuildersSpiritBuffIcon : ModBuffIcon;

    public override string Description =>
        "All sub-towers within range created by engineer monkeys and elf monkeys attack faster and get more range.";

    public override int Path => Top;
    public override int Tier => 1;
    public override int Cost => 25;
}
public class BuildersAtmosphere : ChristmasUpgrade<ElfMonkey>
{
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var rateSupportModel = towerModel.GetBehavior<RateSupportModel>("RateSupportModel_BuildersSpirit");
        var rangeSupportModel = towerModel.GetBehavior<RangeSupportModel>("RangeSupportModel_BuildersSpirit");

        rateSupportModel.customRadius = towerModel.range *= 1.2f;
        rateSupportModel.isCustomRadius = true;
        rangeSupportModel.customRadius = towerModel.range * 1.2f;
        rangeSupportModel.isCustomRadius = true;
        rangeSupportModel.multiplier += 0.05f;
    }

    public override string Description =>
        "Builder's spirit has more range, and provides an extra 5% range.";

    public override int Path => Top;
    public override int Tier => 2;
    public override int Cost => 45;
}

public class ToyMorter : ChristmasUpgrade<ElfMonkey>
{
    public class ToyMorterTower : ModSubTower
    {
        public override string Portrait => "ToyMorter-Portrait";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.range = 40f;
            towerModel.GetAttackModel().range = 40f;
            towerModel.GetAttackModel().ApplyDisplay<ToyMorterAttackDisplay>();

            var weapon = towerModel.GetWeapon();
            weapon.SetProjectile(Game.instance.model.GetTowerFromId("BombShooter-200").GetWeapon().projectile.Duplicate());
            weapon.projectile.ApplyDisplay<ToyBomb>();
        }

        public class ToyMorterTowerDisplay : ModTowerCustomDisplay<ToyMorterTower>
        {
            public override bool UseForTower(params int[] tiers) => true;

            public override string AssetBundleName => "xmas";
            public override string PrefabName => "ToyTurretLegs";
        }
        public class ToyMorterAttackDisplay : ModCustomDisplay
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "ToyTurretBody";
        }

        public class ToyBomb : ModDisplay2D
        {
            protected override string TextureName => Name;

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                node.customScaleAnimator = node.gameObject.AddComponent<CustomScaleAnimator>();
                node.customScaleAnimator.RootScale = node.Scale;
                node.customScaleAnimator.curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0, 1.25f), new Keyframe(1, 1));
            }
        }

        public override TowerSet TowerSet => GetTowerSet<XmasTowerSet>();
        public override string BaseTower => TowerType.Sentry;
    }

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var toyMorterAttack = Game.instance.model.GetTowerFromId("EngineerMonkey-100").GetAttackModel(1).Duplicate();
        var toyMorterWeapon = toyMorterAttack.weapons[0];
        toyMorterAttack.name = "ToyMorter";
        toyMorterWeapon.name = "ToyMorter";
        toyMorterWeapon.projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<ToyMorterTower>();
        toyMorterWeapon.AddBehavior(new EmissionsPerRoundFilterModel("EmissionsPerRoundFilterModel", 2));
        //toyMorterAttack.weapons[0].projectile.ApplyDisplay<ToyBox>(); // Display doesn't exist yet.

        towerModel.AddBehavior(toyMorterAttack);
    }

    public override int Path => Top;
    public override int Tier => 3;
    public override int Cost => 55;
}

public class ToyCart : ChristmasUpgrade<ElfMonkey>
{
    public static Dictionary<Projectile, Tower> TowerForProjectile = new Dictionary<Projectile, Tower>();
    public static Dictionary<Tower, Projectile> ProjectileForTower = new Dictionary<Tower, Projectile>();
    public class ToyCartTower : ModTower
    {
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
            weapon.SetProjectile(Game.instance.model.GetTower(TowerType.BoomerangMonkey, 0, 0, 0).GetWeapon().projectile.Duplicate());
            weapon.rate = 0.175f; // Just under 6/s (~5.88)
        }

        public override TowerSet TowerSet => GetTowerSet<XmasTowerSet>();
        public override string BaseTower => TowerType.DartMonkey;
        public override int Cost => 0;

        public override string DisplayName => "Toy Cart";
    }

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var newAtk = towerModel.GetAttackModel().Duplicate();
        newAtk.fireWithoutTarget = true;
        var weapon = newAtk.weapons[0];
        var projectile = weapon.projectile;
        weapon.SetEmission(new EmissionAtClosestPathSegmentModel("EmissionAtClosestPathSegmentModel_", 1, 0, new Il2CppReferenceArray<EmissionBehaviorModel>(0)));
        projectile.AddBehavior(new TravelAlongPathModel("TravelAlongPathModel_", 30, 9999, true, false, 0));
        projectile.RemoveBehavior<TravelStraitModel>();
        projectile.id = "ToyCart_Low";
        projectile.pierce = Single.MaxValue;
        projectile.canCollideWithBloons = false;
        projectile.GetDamageModel().damage = 3;
        projectile.ApplyDisplay<ToyCartProjectile>();
        projectile.RemoveBehavior<RandomDisplayModel>();
        weapon.fireWithoutTarget = true;
        weapon.rate = 5;

        towerModel.AddBehavior(newAtk);
    }

    public class ToyCartProjectile : ModCustomDisplay
    {
        public override string AssetBundleName => "xmas";
        public override string PrefabName => "ToyCartT1";

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
    }

    // Credits to grahamkracker https://github.com/GrahamKracker/BloonsClicker/blob/a2bc8512d11d99bfdd2394b64c030e9f7e18cb90/Main.cs#L127
    [HarmonyPatch(typeof(TowerInventory), nameof(TowerInventory.GetTowerInventoryCount))]
    [HarmonyPrefix]
    static void TowerInventory_GetTowerInventoryCount(TowerInventory __instance, TowerModel def)
    {
        if (def.baseId == GetInstance<ToyCartTower>().Id && !__instance.towerCounts.TryGetValue(def.baseId, out _))
        {
            __instance.towerCounts[def.baseId] = -1;
        }
    }

    [HarmonyPatch(typeof(Projectile), nameof(Projectile.Initialise))]
    private static class Projectile_Initialize
    {
        public static Projectile LastProjectile;
        public static uint ToyCartCounter = 0;
        public static void Postfix(Projectile __instance)
        {
            if (__instance.projectileModel.id == "ToyCart_Low")
            {
                ObjectId towerId = ObjectId.Create(9999 + ToyCartCounter++);
                LastProjectile = __instance;
                InGame.instance.bridge.CreateTowerAt(__instance.Position.ToUnity(), GetTowerModel<ToyCartTower>(), towerId, false, new Action<bool>(suc => { }), true, true, false, 0, false);
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
        public static void Postfix(Tower __instance, Model modelToUse)
        {
            if (modelToUse.Cast<TowerModel>().baseId == TowerID<ToyCartTower>())
            {
                TowerForProjectile.Add(Projectile_Initialize.LastProjectile, __instance);
            }
        }
    }

    [HarmonyPatch(typeof(TravelAlongPath), nameof(TravelAlongPath.Process))]
    private static class TravelAlongPath_Process
    {
        public static void Postfix(TravelAlongPath __instance)
        {
            var projectile = __instance.projectile;
            var tower = TowerForProjectile[projectile];
            tower.PositionTower(projectile.Position.ToVector2());
            tower.Rotation = projectile.Rotation;
        }
    }

    public override int Path => Top;
    public override int Tier => 4;
    public override int Cost => 225;

    public override string Description =>
        "Elf monkey can now create carts which run over bloons for <b>three</b> damage and have fast shooting turrets that only do <b>one</b> damage each shot.";
}
