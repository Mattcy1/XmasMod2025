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
using MelonLoader;
using UnityEngine;

namespace XmasMod2025.Towers.Upgrades;

public class BuildersSpirit : ChristmasUpgrade<ElfMonkey>
{
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        List<TowerFilterModel> filters =
        [
            new FilterInBaseTowerIdModel("SubTowerFilterModel_BuildersSpirit", new(["Sentry", mod.IDPrefix + "-ElfToy"]))
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

    public override int Path => Middle;
    public override int Tier => 1;
    public override int Cost => 25;
}
public class BuildersAtmosphere : ChristmasUpgrade<ElfMonkey>
{
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        List<TowerFilterModel> filters =
        [
            new FilterInBaseTowerIdModel("SubTowerFilterModel_BuildersSpirit", new(["Sentry", GetTowerModel<ToyMorter.ToyMorterTower>().baseId]))
        ];

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

    public override int Path => Middle;
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

    public override int Path => Middle;
    public override int Tier => 3;
    public override int Cost => 55;
}

public class ToyCart : ModUpgrade<ElfMonkey>
{
    public class ToyCartTower : ModSubTower
    {
        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
        }

        public override TowerSet TowerSet => GetTowerSet<XmasTowerSet>();
        public override string BaseTower => TowerType.DartMonkey;
    }

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var newWep = towerModel.GetWeapon().Duplicate();
        newWep.SetEmission(new EmissionAtClosestPathSegmentModel("EmissionAtClosestPathSegmentModel_", 1, 0, null));
        newWep.AddBehavior(new TravelAlongPathModel("TravelAlongPathModel_", 30, 9999, true, false, 0));
        newWep.projectile.name = "ToyCart_Low";
    }

    [HarmonyPatch(typeof(Projectile), nameof(Projectile.Initialise))]
    private static class Projectile_Initialize
    {
        public static Dictionary<ObjectId, Tower> TowerForProjectile = new Dictionary<ObjectId, Tower>();
        public static int ToyCartCounter = 0;
        public static void Postfix(Projectile __instance)
        {
            if (__instance.projectileModel.name == "ToyCart_Low")
            {
                ObjectId towerId = ObjectId.FromString("ToyCartTower" + ToyCartCounter++);
                InGame.instance.bridge.CreateTowerAt(__instance.Position.ToVector2().ToUnity(), GetTowerModel<ToyCartTower>(), towerId, false, new Action<bool>(suc =>
                {
                    if (suc)
                    {
                        TowerForProjectile.Add(__instance.Id, InGame.instance.bridge.GetTowerFromId(towerId));
                    }
                }));
            }
        }
    }

    /*[HarmonyPatch(typeof(Projectile), nameof(Il2CppAssets.Scripts.Unity.Towers.Projectiles.Projectile.U))]
    private static class Projectile_Update
    {
        
    }*/

    public override int Path => Middle;
    public override int Tier => 4;
    public override int Cost => 225;
}