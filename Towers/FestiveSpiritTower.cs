using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Behaviors.Projectiles;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using Il2CppTMPro;
using MelonLoader;
using System.Diagnostics;
using UnityEngine;
using static MelonLoader.MelonLogger;
using Projectile = Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile;

namespace XmasMod2025.Towers
{ 
    public class FestiveSpiritTower : ChristmasTower
    {
        public override string BaseTower => TowerType.MonkeyVillage + "-200";
        public override bool DontAddToShop => false;
        public override string Icon => VanillaSprites.MonkeyVillageElfPetIcon;
        public override string Portrait => Icon;
        public override string Description => "One of Santa's Minions, help you defend.";
        public override int Cost => 0;

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.dontDisplayUpgrades = true;
            towerModel.displayScale = 0;
            towerModel.display = new PrefabReference("");

            foreach (var am in towerModel.GetAttackModels())
            {
                am.range = 9999;
            }

            towerModel.range = 9999;
            towerModel.ignoreTowerForSelection = true;

            var range = towerModel.GetBehavior<RangeSupportModel>();
            range.multiplier = 0.2f;
            range.mutatorId = "FestiveSpiritRangeBuff";
            range.name = "FestiveSpiritRange";
            range.buffIconName = "";
            range.buffLocsName = "";
            range.customRadius = 9999;
            range.showBuffIcon = false;

            var rate = towerModel.GetBehavior<RateSupportModel>();
            rate.multiplier = 0.6f;
            rate.mutatorId = "FestiveSpiritRateBuff";
            rate.name = "FestiveSpiritRate";
            rate.ApplyBuffIcon<FestiveSpiritBuff>();
            rate.customRadius = 9999;
            rate.showBuffIcon = true;

            towerModel.radius = 0;
        }
    }

    public class FestiveSpiritBuff : ModBuffIcon
    {
        public override string Icon => "Gift1";
    }
}
