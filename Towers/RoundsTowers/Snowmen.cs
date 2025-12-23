using System;
using System.Collections.Generic;
using System.Linq;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace XmasMod2025.Towers.RoundsTowers;

internal class Snowmen
{
    public class Snowman1 : ChristmasTower
    {
        public override string BaseTower => TowerType.BananaFarm;
        protected override int Order => 1;
        public override int ShopTowerCount => 3;
        public override int Cost => 35;
        public override string DisplayName => "Beginner Snowman";
        public override string Description => "Slowy, generates gifts.";
        public override string Icon => VanillaSprites.SnowMonkey;
        public override string Portrait => Icon;
        public override int UnlockRound => 24;

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return towerSet
                .First(model => model.towerId == Game.instance.model.GetTowerFromId(TowerID<XmasTree>()).baseId)
                .towerIndex + 1;
        }

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.dontDisplayUpgrades = true;

            var proj = towerModel.GetWeapon().projectile;
            proj.GetBehavior<CashModel>().minimum = 0;
            proj.GetBehavior<CashModel>().maximum = 0;

            proj.RemoveBehavior<AgeModel>();
            proj.RemoveBehavior<CreateEffectOnExpireModel>();

            proj.ApplyDisplay<Gift1>();
            proj.AddBehavior(new RandomDisplayModel("RandomDisplayModel_",
                new Il2CppReferenceArray<PrefabReference>([
                    new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()),
                    new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()),
                    new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())
                ]), true));

            proj.id = "SnowmanT1";

            towerModel.GetWeapon().GetBehavior<EmissionsPerRoundFilterModel>().count = 3;
            towerModel.footprint = Game.instance.model.GetTower(TowerType.DartMonkey).footprint;
            towerModel.radius = Game.instance.model.GetTower(TowerType.DartMonkey).radius;
            towerModel.RadiusSquared = Game.instance.model.GetTower(TowerType.DartMonkey).RadiusSquared;
            proj.GetBehavior<CreateTextEffectModel>().assetId = new PrefabReference("");
        }
    }

    public class Snowman2 : ChristmasTower
    {
        public override string BaseTower => TowerType.BananaFarm;
        public override int Cost => 70;
        public override int ShopTowerCount => 3;
        protected override int Order => 1;
        public override string DisplayName => "Novice Snowman";
        public override string Description => "Generates gifts a bit faster then the old tier.";
        public override string Icon => VanillaSprites.SnowMonkey;
        public override string Portrait => Icon;
        public override int UnlockRound => 44;

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return towerSet
                .First(model => model.towerId == Game.instance.model.GetTowerFromId(TowerID<XmasTree>()).baseId)
                .towerIndex + 1;
        }

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.dontDisplayUpgrades = true;

            var proj = towerModel.GetWeapon().projectile;
            proj.GetBehavior<CashModel>().minimum = 0;
            proj.GetBehavior<CashModel>().maximum = 0;

            proj.RemoveBehavior<AgeModel>();
            proj.RemoveBehavior<CreateEffectOnExpireModel>();

            proj.ApplyDisplay<Gift1>();
            proj.AddBehavior(new RandomDisplayModel("RandomDisplayModel_",
                new Il2CppReferenceArray<PrefabReference>([
                    new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()),
                    new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()),
                    new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())
                ]), true));

            proj.id = "SnowmanT2";

            towerModel.GetWeapon().GetBehavior<EmissionsPerRoundFilterModel>().count = 3;
            towerModel.footprint = Game.instance.model.GetTower(TowerType.DartMonkey).footprint;
            towerModel.radius = Game.instance.model.GetTower(TowerType.DartMonkey).radius;
            towerModel.RadiusSquared = Game.instance.model.GetTower(TowerType.DartMonkey).RadiusSquared;
            proj.GetBehavior<CreateTextEffectModel>().assetId = new PrefabReference("");
        }
    }

    public class Snowman3 : ChristmasTower
    {
        public override string BaseTower => TowerType.BananaFarm;
        public override int Cost => 120;
        protected override int Order => 1;
        public override string DisplayName => "Pro Snowman";
        public override int ShopTowerCount => 3;
        public override string Description => "Generates gifts at a medium rate.";
        public override string Icon => VanillaSprites.SnowMonkey;
        public override string Portrait => Icon;
        public override int UnlockRound => 64;

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return towerSet
                .First(model => model.towerId == Game.instance.model.GetTowerFromId(TowerID<XmasTree>()).baseId)
                .towerIndex + 1;
        }

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.dontDisplayUpgrades = true;

            var proj = towerModel.GetWeapon().projectile;
            proj.GetBehavior<CashModel>().minimum = 0;
            proj.GetBehavior<CashModel>().maximum = 0;

            proj.RemoveBehavior<AgeModel>();
            proj.RemoveBehavior<CreateEffectOnExpireModel>();

            proj.ApplyDisplay<Gift1>();
            proj.AddBehavior(new RandomDisplayModel("RandomDisplayModel_",
                new Il2CppReferenceArray<PrefabReference>([
                    new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()),
                    new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()),
                    new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())
                ]), true));

            proj.id = "SnowmanT3";

            towerModel.GetWeapon().GetBehavior<EmissionsPerRoundFilterModel>().count = 3;
            towerModel.footprint = Game.instance.model.GetTower(TowerType.DartMonkey).footprint;
            towerModel.radius = Game.instance.model.GetTower(TowerType.DartMonkey).radius;
            towerModel.RadiusSquared = Game.instance.model.GetTower(TowerType.DartMonkey).RadiusSquared;
            proj.GetBehavior<CreateTextEffectModel>().assetId = new PrefabReference("");
        }
    }

    public class Snowman4 : ChristmasTower
    {
        public override string BaseTower => TowerType.BananaFarm;
        public override int Cost => 250;
        protected override int Order => 1;
        public override string DisplayName => "Elite Snowman";
        public override int ShopTowerCount => 3;
        public override string Description => "Generates gifts at a max speeds for snowmans.";
        public override string Icon => VanillaSprites.SnowMonkey;
        public override string Portrait => Icon;
        public override int UnlockRound => 84;

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return towerSet
                .First(model => model.towerId == Game.instance.model.GetTowerFromId(TowerID<XmasTree>()).baseId)
                .towerIndex + 1;
        }

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.dontDisplayUpgrades = true;

            var proj = towerModel.GetWeapon().projectile;
            proj.GetBehavior<CashModel>().minimum = 0;
            proj.GetBehavior<CashModel>().maximum = 0;

            proj.RemoveBehavior<AgeModel>();
            proj.RemoveBehavior<CreateEffectOnExpireModel>();

            proj.ApplyDisplay<Gift1>();
            proj.AddBehavior(new RandomDisplayModel("RandomDisplayModel_",
                new Il2CppReferenceArray<PrefabReference>([
                    new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()),
                    new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()),
                    new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())
                ]), true));

            proj.id = "SnowmanT4";

            towerModel.GetWeapon().GetBehavior<EmissionsPerRoundFilterModel>().count = 3;
            towerModel.footprint = Game.instance.model.GetTower(TowerType.DartMonkey).footprint;
            towerModel.radius = Game.instance.model.GetTower(TowerType.DartMonkey).radius;
            towerModel.RadiusSquared = Game.instance.model.GetTower(TowerType.DartMonkey).RadiusSquared;
            proj.GetBehavior<CreateTextEffectModel>().assetId = new PrefabReference("");
        }
    }

    public class Snowman1Display : ModTowerDisplay<Snowman1>
    {
        public override string BaseDisplay => "a02429b5250b271449a603a5a8c1e2f7";
        public override float Scale => 0.5f;

        public override bool UseForTower(params int[] tiers)
        {
            return tiers.Sum() == 0;
        }
    }

    public class Snowman2Display : ModTowerDisplay<Snowman2>
    {
        public override string BaseDisplay => "a02429b5250b271449a603a5a8c1e2f7";
        public override float Scale => 0.5f;

        public override bool UseForTower(params int[] tiers)
        {
            return tiers.Sum() == 0;
        }
    }

    public class Snowman3Display : ModTowerDisplay<Snowman3>
    {
        public override string BaseDisplay => "a02429b5250b271449a603a5a8c1e2f7";
        public override float Scale => 0.5f;

        public override bool UseForTower(params int[] tiers)
        {
            return tiers.Sum() == 0;
        }
    }

    public class Snowman4Display : ModTowerDisplay<Snowman4>
    {
        public override string BaseDisplay => "a02429b5250b271449a603a5a8c1e2f7";
        public override float Scale => 0.5f;

        public override bool UseForTower(params int[] tiers)
        {
            return tiers.Sum() == 0;
        }
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Pickup))]
public class HandlePickup
{
    [HarmonyPostfix]
    public static void Prefix(Projectile __instance)
    {
        if (__instance.projectileModel.id == "SnowmanT1")
        {
            var random = new Random().Next(1, 2);

            if (InGame.instance != null || InGame.instance.bridge != null)
                InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position,
                    ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gift {(random == 1 ? "" : "s")}",
                    true);

            XmasMod2025.Gifts += random;
        }

        if (__instance.projectileModel.id == "SnowmanT2")
        {
            var random = new Random().Next(2, 4);

            if (InGame.instance != null || InGame.instance.bridge != null)
                InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position,
                    ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gifts", true);

            XmasMod2025.Gifts += random;
        }

        if (__instance.projectileModel.id == "SnowmanT3")
        {
            var random = new Random().Next(4, 5);

            if (InGame.instance != null || InGame.instance.bridge != null)
                InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position,
                    ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gifts", true);

            XmasMod2025.Gifts += random;
        }

        if (__instance.projectileModel.id == "SnowmanT4")
        {
            var random = new Random().Next(6, 8);

            if (InGame.instance != null || InGame.instance.bridge != null)
                InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position,
                    ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gifts", true);

            XmasMod2025.Gifts += random;
        }
    }
}