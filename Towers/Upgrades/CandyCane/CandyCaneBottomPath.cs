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
using Il2CppAssets.Scripts.Unity.UI_New.Store.LootItem;
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
    internal class BottomPath
    {
        public class StrongerThrow : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => Bottom;
            public override int Tier => 1;
            public override int Cost => 70;
            public override string Description => "The cane monkey, throws the candy cane harder allowing for more damage.";
            public override string Icon => "BottomPathCane1";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var weapons in towerModel.GetWeapons())
                {
                    weapons.projectile.GetDamageModel().damage += 1;
                }
            }
        }

        public class EvenStrongerThrow : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => BOTTOM;
            public override int Tier => 2;
            public override int Cost => 95;
            public override string Description => "Throws candy cane even harder for more damage, at the cost of less range.";
            public override string Icon => "BottomPathCane2";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var weapons in towerModel.GetWeapons())
                {
                    weapons.projectile.GetDamageModel().damage += 1;
                }

                foreach(var attackModel in towerModel.GetAttackModels())
                {
                    attackModel.range -= 5;
                }

                towerModel.range -= 5;

                towerModel.GetWeapon().projectile.GetBehavior<CreateProjectileOnExhaustPierceModel>().projectile.GetDamageModel().damage += 1;
            }
        }

        public class Tier3CaneBottom : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => BOTTOM;
            public override int Tier => 3;
            public override int Cost => 150;
            public override string DisplayName => "";
            public override string Description => "";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
            }
        }
    }

    public class Tier4CaneBottom : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => BOTTOM;
        public override int Tier => 4;
        public override int Cost => 800;
        public override string DisplayName => "";
        public override string Description => "";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }

    public class Tier5CaneBottom : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => BOTTOM;
        public override int Tier => 5;
        public override int Cost => 2000;
        public override string DisplayName => "";
        public override string Description => "";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }
}

