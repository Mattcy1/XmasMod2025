using BossAPI.Bosses;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors.Actions;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Moabs;
using XmasMod2025.BossAPI;
using XmasMod2025.Towers;

namespace XmasMod2025.Bosses
{
    internal class GrinchBoss : ModBoss
    {
        public override string BossName => "The Grinch";
        public override int SkullCount => 10;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string CustomSkullIcon => "";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 100;
        public override string BaseBloon => BloonType.sBad;
        public override string Description => "";
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 45000000;
            //bloonModel.maxHealth = 10000;
            bloonModel.RemoveAllChildren();
            bloonModel.speed /= 3f;

            StunTowersInRadiusActionModel shield = Game.instance.model.GetBloon("Vortex1").GetBehavior<StunTowersInRadiusActionModel>().Duplicate();
            CreateEffectActionModel effect = Game.instance.model.GetBloon("Vortex1").GetBehavior<CreateEffectActionModel>().Duplicate();
            shield.actionId = "ModdedSkullModdedBossThe Grinch";
            shield.radius *= 2;

            effect.actionId = shield.actionId;

            bloonModel.AddBehavior(effect);
            bloonModel.AddBehavior(shield);
        }
        public override void OnSpawn(Bloon bloon)
        {
            XmasMod2025.boss = bloon;
            Hooks.StartMonobehavior<HandleTotem>();
            Hooks.StartMonobehavior<GrinchHandler>();
            BossUI.UpdateNameColor(UnityEngine.Color.green, null);
        }

        [RegisterTypeInIl2Cpp]
        public class GrinchHandler : MonoBehaviour
        {
            public bool half = false;
            public bool nextHalf = false;
            public void Start()
            {
                half = false;
            }
            public void Update()
            {
                if (XmasMod2025.boss == null)
                {
                    this.Destroy();
                    return;
                }

                if (XmasMod2025.boss.health <= XmasMod2025.boss.bloonModel.maxHealth / 2 && !half)
                {
                    BossUI.UpdateNameColor(UnityEngine.Color.yellow, null);

                    var root = XmasMod2025.boss.bloonModel.Duplicate();

                    TimeTriggerModel timeTrigger = new TimeTriggerModel("ElfTax", 30, false, new string[] { "ElfTax" });
                    root.AddBehavior(timeTrigger);

                    TimeTriggerModel heal = new TimeTriggerModel("GrinchHeal", 15, false, new string[] { "GrinchHeal" });
                    root.AddBehavior(heal);

                    CreateEffectActionModel effect = Game.instance.model.GetBloon("Vortex1").GetBehavior<CreateEffectActionModel>().Duplicate();
                    effect.actionId = heal.actionIds[0];
                    effect.effect = ModContent.CreatePrefabReference<GiftEffectButBig>();

                    XmasMod2025.boss.UpdateRootModel(root);
                    half = true;
                }

                if (XmasMod2025.boss.health <= XmasMod2025.boss.bloonModel.maxHealth * 0.25f && !nextHalf)
                {
                    foreach(var boss in ModContent.GetContent<ModBoss>())
                    {
                        if(boss.Id != ModContent.BloonID<GrinchBoss>())
                        {
                            InGame.instance.SpawnBloons(boss.Id, 1, 0);
                        }
                    }
                    nextHalf = true;
                }
            }
        }
    }
}
