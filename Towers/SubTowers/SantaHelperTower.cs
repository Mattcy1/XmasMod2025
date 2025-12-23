using System.Linq;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace XmasMod2025.Towers.SubTowers;

public class ElfSpawner : ChristmasTower
{
    public override string BaseTower => TowerType.DartMonkey;
    public override string Icon => VanillaSprites.MonkeyVillageElfPetIcon;
    public override string Portrait => Icon;
    public override string Description => "One of Santa's Minions, help you defend.";
    public override int Cost => 0;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.dontDisplayUpgrades = true;
        towerModel.range = 999;
        towerModel.ignoreTowerForSelection = true;
        towerModel.RemoveBehavior<AttackModel>();

        AttackModel[] Avatarspawner =
        {
            Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels()
                .First(a => a.name == "AttackModel_Spawner_").Duplicate()
        };
        Avatarspawner[0].weapons[0].rate = 10f;
        Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
        Avatarspawner[0].name = "ElfSpawner";
        Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower",
            GetTowerModel<ElfHelper>(), 0, false, false, false, false, false));
        Avatarspawner[0].range = 999;
        towerModel.AddBehavior(Avatarspawner[0]);

        towerModel.radius = 0;
        towerModel.display = new PrefabReference("");
        towerModel.displayScale = 0;
    }
}

public class ElfHelper : ChristmasTower
{
    public override string BaseTower => TowerType.SniperMonkey + "-202";
    public override string Icon => VanillaSprites.MonkeyVillageElfPetIcon;
    public override string Portrait => Icon;
    public override string Description => "One of Santa's Minions, help you defend.";
    public override int Cost => 0;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.dontDisplayUpgrades = true;
        towerModel.isSubTower = true;
        towerModel.ApplyDisplay<ElfMonkeyDisplay>()
        towerModel.AddBehavior(new TowerExpireModel("TowerExpireModel", 20, 1, true, false));
    }
}