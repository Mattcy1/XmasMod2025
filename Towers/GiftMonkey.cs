using Bosses.BossAPI;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Data.Behaviors.Projectiles;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using static MelonLoader.MelonLogger;
using Projectile = Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile;

namespace XmasMod2025.Towers
{
    public class GiftMonkey : ChristmasTower
    {

        public static List<string> towers = new List<string>();
        public override string BaseTower => TowerType.DartMonkey;
        public override int Cost => 50;
        public override string Portrait => Icon;
        public override int MiddlePathUpgrades => 5;

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.dontDisplayUpgrades = false;
            towerModel.RemoveBehavior<AttackModel>();

            var abilityModel = Game.instance.GetModel().GetTowerFromId("DartlingGunner-040").GetBehavior<AbilityModel>().Duplicate();
            abilityModel.RemoveBehavior<ActivateAttackModel>();
            abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
            abilityModel.displayName = "RollMonkey";

            towerModel.AddBehavior(abilityModel);
        }
    }

    public class Upgrade1 : ChristmasUpgrade<GiftMonkey>
    {
        public override int Path => MIDDLE;

        public override int Tier => 1;

        public override int Cost => 30;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }

    public class Upgrade2 : ChristmasUpgrade<GiftMonkey>
    {
        public override int Path => MIDDLE;

        public override int Tier => 2;

        public override int Cost => 60;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }


    public class Upgrade3 : ChristmasUpgrade<GiftMonkey>
    {
        public override int Path => MIDDLE;

        public override int Tier => 3;

        public override int Cost => 100;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }


    public class Upgrade4 : ChristmasUpgrade<GiftMonkey>
    {
        public override int Path => MIDDLE;

        public override int Tier => 4;

        public override int Cost => 150;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }


    public class Upgrade5 : ChristmasUpgrade<GiftMonkey>
    {
        public override int Path => MIDDLE;

        public override int Tier => 5;

        public override int Cost => 300;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
        }
    }


    [HarmonyPatch(typeof(Ability), nameof(Ability.Activate))]
    public class HandleAbility_Patch
    {
        [HarmonyPostfix]

        public static void Postfix(Ability __instance)
        {
            if (__instance.abilityModel.displayName == "RollMonkey")
            {
                var random = new System.Random();
                var tierToRoll = __instance.tower.GetPathUpgradeTier(Il2CppAssets.Scripts.Simulation.Towers.TowerUpgradePath.Middle);

                var pathToRoll = random.Next(0, 3);

                List<string> blacklist = new List<string>
                {
                    ModContent.TowerID<XmasTree>(),
                    ModContent.TowerID<GiftMonkey>(),
                };

                if (GiftMonkey.towers.Count == 0)
                {
                    foreach (var t in Game.instance.GetModel().towers)
                    {
                        if (t.IsBaseTower && !t.isSubTower && !t.IsHero() && !blacklist.Contains(t.baseId))
                        {
                            GiftMonkey.towers.Add(t.baseId);
                        }
                    }
                }

                string baseTower = GiftMonkey.towers[random.Next(GiftMonkey.towers.Count)];

                int[] tiers = new int[3] { 0, 0, 0 };
                tiers[pathToRoll] = tierToRoll;

                string rolledTower;

                if (tiers[0] == 0 && tiers[1] == 0 && tiers[2] == 0)
                {
                    rolledTower = $"{baseTower}";
                }
                else
                {
                    rolledTower = $"{baseTower}-{tiers[0]}{tiers[1]}{tiers[2]}";
                }

                MelonLogger.Msg($"Rolled: {rolledTower}");
                Action<bool> callback = _ => { };

                InGame.instance.bridge.CreateTowerAt(new Vector2(__instance.tower.Position.X, __instance.tower.Position.Y), Game.instance.model.GetTowerFromId(rolledTower), ObjectId.Create((uint)random.Next(0, 99999), 0), false, callback, true, true, false, 0, false);
                InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<GiftEffect>(), __instance.Position, 0, 2, 120);

                __instance.tower.worth = 0;
                __instance.tower.SellTower();


            }
        }
    }

    public class GiftEffect : ModDisplay
    {
        public override string BaseDisplay => "6d84b13b7622d2744b8e8369565bc058";

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            RendererExt.SetMainTexture(((Il2CppArrayBase<Renderer>)node.genericRenderers)[0], ModContent.GetTexture<BossAPI>("blank"));
            RendererExt.SetMainTexture(((Il2CppArrayBase<Renderer>)node.genericRenderers)[3], ModContent.GetTexture<BossAPI>("Gift1"));
            RendererExt.SetMainTexture(((Il2CppArrayBase<Renderer>)node.genericRenderers)[2], ModContent.GetTexture<BossAPI>("Gift1"));
            RendererExt.SetMainTexture(((Il2CppArrayBase<Renderer>)node.genericRenderers)[1], ModContent.GetTexture<BossAPI>("blank"));
            ((Component)((Il2CppArrayBase<Renderer>)node.genericRenderers)[2]).GetComponent<ParticleSystem>().startSpeed *= 0.2f;
        }
    }
}
