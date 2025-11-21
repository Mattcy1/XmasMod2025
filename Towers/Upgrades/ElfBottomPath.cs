using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
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

namespace XmasMod2025.Towers.Upgrades
{
    internal class ElfBottomPath
    {
        public class Tier1 : ChristmasUpgrade<ElfMonkey>
        {
            public override int Path => BOTTOM;
            public override int Tier => 1;
            public override int Cost => 50;
            public override string DisplayName => "Less heavy presents";
            public override string Description => "The presents, become lighter allowing for a bit more attack speed.";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var weapons in towerModel.GetWeapons())
                {
                    weapons.rate -= 0.1f;
                }
            }
        }

        public class Tier2 : ChristmasUpgrade<ElfMonkey>
        {
            public override int Path => BOTTOM;
            public override int Tier => 2;
            public override int Cost => 70;
            public override string DisplayName => "Better Vision";
            public override string Description => "The elf monkey wear special glasses to see camo, and increased range.";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach(var attackModel in towerModel.GetAttackModels())
                {
                    attackModel.range += 10;
                }

                towerModel.range += 10;
                towerModel.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            }
        }

        public class Tier3 : ChristmasUpgrade<ElfMonkey>
        {
            public override int Path => BOTTOM;
            public override int Tier => 3;
            public override int Cost => 150;
            public override string DisplayName => "Gift Making";
            public override string Description => "The elf monkey learns the art of making gifts, and will now produce gifts at the cost of not attacking.";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.RemoveBehavior<AttackModel>();

                AttackModel MarketPlace = Game.instance.model.GetTowerFromId("BananaFarm").GetAttackModel().Duplicate();
                var weapon = MarketPlace.weapons[0];
                var cashModel = weapon.projectile.GetBehavior<CashModel>();
                weapon.projectile.GetBehavior<CreateTextEffectModel>().assetId = new("");

                cashModel.minimum = 2;
                weapon.GetBehavior<EmissionsPerRoundFilterModel>().count = 6;
                cashModel.maximum = 4;
                cashModel.name = "Elf003";
                weapon.projectile.id = "Elf003";

                weapon.projectile.ApplyDisplay<Gift1>();
                weapon.projectile.AddBehavior(new RandomDisplayModel("RandomDisplayModel_", new Il2CppReferenceArray<PrefabReference>([new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()), new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()), new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())]), true));

                CollectCashZoneModel collectCashZoneModel = new CollectCashZoneModel("ElfRange", 30, 30, 0, "", false, true, false, false, false, 0.05f);

                towerModel.AddBehavior(collectCashZoneModel);
                towerModel.AddBehavior(MarketPlace);
            }
        }
    }

    public class Tier4 : ChristmasUpgrade<ElfMonkey>
    {
        public override int Path => BOTTOM;
        public override int Tier => 4;
        public override int Cost => 800;
        public override string DisplayName => "Better Wrapping Paper";
        public override string Description => "The elf monkey buy better wrapping paper making gift more valuable.";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var weapon = towerModel.GetWeapon();
            var cashModel = weapon.projectile.GetBehavior<CashModel>();

            cashModel.minimum = 4;
            cashModel.maximum = 5;
            weapon.GetBehavior<EmissionsPerRoundFilterModel>().count = 8;
        }
    }

    public class Tier5 : ChristmasUpgrade<ElfMonkey>
    {
        public override int Path => BOTTOM;
        public override int Tier => 5;
        public override int Cost => 2000;
        public override string DisplayName => "End of day bonus";
        public override string Description => "The elf monkey get paid end of round bonus (150 Gifts).";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var weapon = towerModel.GetWeapon();
            var cashModel = weapon.projectile.GetBehavior<CashModel>();

            cashModel.minimum = 6;
            cashModel.maximum = 8;
            weapon.GetBehavior<EmissionsPerRoundFilterModel>().count = 10;
        }
    }

    [HarmonyPatch(typeof(Projectile), nameof(Projectile.Pickup))]
    public class HandlePickup
    {
        [HarmonyPostfix]

        public static void Prefix(Projectile __instance)
        {
            if (__instance.projectileModel.id == "Elf003")
            {
                var cashModel = __instance.projectileModel.GetBehavior<CashModel>();
                var random = new System.Random().Next((int)cashModel.minimum, (int)cashModel.maximum);

                if (InGame.instance != null || InGame.instance.bridge != null)
                {
                    InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position, ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gifts", true);
                }

                XmasMod2025.Gifts += random;
            }
        }
    }
}

