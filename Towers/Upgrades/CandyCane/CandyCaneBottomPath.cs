using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.MapSets;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.Scenes;
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

        public class LuckyCandyCane : ChristmasUpgrade<CandyCaneMonkey>
        {
            public override int Path => BOTTOM;
            public override int Tier => 3;
            public override int Cost => 150;
            public override string Description => "Candy cane have a small chance to grant gifts.";
            public override string Icon => "BottomPathCane3";
            public override string Portrait => "Candy003Portrait";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var weapons in towerModel.GetWeapons())
                {
                    weapons.projectile.GetDamageModel().damage += 1;
                    weapons.projectile.id = "Cane003";
                    weapons.projectile.ApplyDisplay<LuckyCandyCaneProj>();
                }

                towerModel.ApplyDisplay<CandyCaneMonkey003>();
            }
        }


        public class LuckyCandyCaneProj : ModDisplay
        {
            public override string BaseDisplay => Generic2dDisplay;

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                Set2DTexture(node, "LuckyCandyCane");
            }
        }
    }

    public class FocusedShots : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => BOTTOM;
        public override int Tier => 4;
        public override int Cost => 650;
        public override string Description => "Slower, attack speed, less pierce, but INSANE single target damage perfect for taking down these pesky moab.";
        public override string Icon => "BottomPathCane4";
        public override string Portrait => "Candy004Portrait";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var proj = towerModel.GetWeapon().projectile;

            proj.GetDamageModel().damage += 30;
            proj.pierce = 1;
            proj.RemoveBehavior<CreateProjectileOnExhaustPierceModel>();


            towerModel.GetWeapon().rate += 0.3f;
            towerModel.range += 7;
            towerModel.GetAttackModel().range += 7;
            towerModel.ApplyDisplay<CandyCaneMonkey004>();
        }
    }

    public class ExtremlyFocusedShots : ChristmasUpgrade<CandyCaneMonkey>
    {
        public override int Path => BOTTOM;
        public override int Tier => 5;
        public override int Cost => 5500;
        public override string Description => "With this guy in town, no boss will dare fight you.";
        public override string Icon => "BottomPathCane5";
        public override string Portrait => "Candy005Portrait";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var proj = towerModel.GetWeapon().projectile;

            proj.GetDamageModel().damage += 5;

            towerModel.GetWeapon().rate += 0.1f;
            towerModel.GetWeapon().emission = new ArcEmissionModel("ArcEmissionModel_", 3, 0, 20, null, false, false);

            towerModel.range += 7;
            towerModel.GetAttackModel().range += 7;
            towerModel.ApplyDisplay<CandyCaneMonkey005>();
        }
    }

    public class CandyCaneMonkey003 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("Alchemist-004").display.AssetGUID;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, Name);
            SetMeshTexture(node, Name, 1);
            SetMeshTexture(node, Name, 2);
            node.RemoveBone("AlchemistRig:Propjectile_R");
        }
    }


    public class CandyCaneMonkey004 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("DartMonkey-004").display.AssetGUID;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, Name);
            SetMeshTexture(node, Name, 1);
            SetMeshTexture(node, Name, 2);
            SetMeshTexture(node, Name, 3);
            SetMeshTexture(node, Name, 4);
        }
    }


    public class CandyCaneMonkey005 : ModDisplay
    {
        public override string BaseDisplay => Game.instance.model.GetTowerFromId("SniperMonkey-013").display.AssetGUID;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, Name);
            SetMeshTexture(node, Name, 1);
            SetMeshOutlineColor(node, UnityEngine.Color.green);
            SetMeshOutlineColor(node, UnityEngine.Color.green, 1);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Weapon), nameof(Weapon.Emit))]
    public class DropGifts
    {
        [HarmonyLib.HarmonyPostfix] 
        public static void Postfix(Weapon __instance)
        {
            if(__instance.weaponModel.projectile.id == "Cane003")
            {
                var shouldDrop = new System.Random().Next(1, 3);

                if (shouldDrop >= 2)
                {
                    var random = new System.Random().Next(1, 4);
                    if (InGame.instance != null || InGame.instance.bridge != null)
                    {
                        InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position, ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gifts", true);
                    }

                    XmasMod2025.Gifts += random;
                }
            }
        }
    }
}

