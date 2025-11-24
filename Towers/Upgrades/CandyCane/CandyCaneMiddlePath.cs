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

        public class Tier3CaneMiddle : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => Middle;
            public override int Tier => 3;
            public override int Cost => 150;
            public override string DisplayName => "";
            public override string Description => "";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
            }
        }
    }

    public class Tier4CaneMiddle : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => Middle;
        public override int Tier => 4;
        public override int Cost => 800;
        public override string DisplayName => "";
        public override string Description => "";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }

    public class Tier5CaneMiddle : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => Middle;
        public override int Tier => 5;
        public override int Cost => 2000;
        public override string DisplayName => "";
        public override string Description => "";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }
}

