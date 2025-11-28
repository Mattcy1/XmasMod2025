using System.Collections.Generic;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
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
            new FilterInBaseTowerIdModel("SubTowerFilterModel_BuildersSpirit", new(["Sentry", GetTowerModel<ToyMortar.ToyMortarTower>().baseId]))
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

public class ToyMortar : ChristmasUpgrade<ElfMonkey>
{
    public class ToyMortarTower : ModSubTower
    {
        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 99999999;
            towerModel.GetBehavior<TowerExpireModel>().rounds = 1;
            
            towerModel.range = 40f;
            towerModel.GetAttackModel().range = 40f;

            var weapon = towerModel.GetWeapon();
            weapon.SetProjectile(Game.instance.model.GetTowerFromId("BombShooter-200").GetWeapon().projectile.Duplicate());
            weapon.projectile.ApplyDisplay<ToyBomb>();
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
        var toyMortarAttack = Game.instance.model.GetTowerFromId("EngineerMonkey-100").GetAttackModel(1).Duplicate();
        var toyMortarWeapon = toyMortarAttack.weapons[0];
        toyMortarAttack.name = "ToyMortar";
        toyMortarWeapon.name = "ToyMortar";
        toyMortarWeapon.projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<ToyMortarTower>();
        toyMortarWeapon.AddBehavior(new EmissionsPerRoundFilterModel("EmissionsPerRoundFilterModel", 2));
        //toyMortarAttack.weapons[0].projectile.ApplyDisplay<ToyBox>(); // Display doesn't exist yet.
        
        towerModel.AddBehavior(toyMortarAttack);
    }

    public override int Path => Middle;
    public override int Tier => 3;
    public override int Cost => 55;
}