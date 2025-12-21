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
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Display;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Moabs;
using XmasMod2025.BossAPI;
using XmasMod2025.Towers;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers;
using static XmasMod2025.XmasMod2025;

namespace XmasMod2025.Bosses
{
    internal class KrampusBoss : ModBoss
    {
        public class KrampusDisplay : ModBloonCustomDisplay<KrampusBoss>
        {
            [RegisterTypeInIl2Cpp]
            public class Shake : MonoBehaviour
            {
                public UnityEngine.Vector3 ShakeAmount = new UnityEngine.Vector3(1f, 0.4f, 0);
                public UnityEngine.Vector3 originalPos;
                public float ShakeTime = 0.2f;
                private float nextMoveTime = 0f;

                private float stateOneTime => ShakeTime * 0.25f;
                private float stateTwoTime => ShakeTime * 0.75f;

                public bool left = false;

                private void Start()
                {
                   originalPos = transform.position;
                }
                
                private void Update()
                {
                    if (Time.time >= nextMoveTime)
                    {
                        transform.position = left ? originalPos - ShakeAmount :  originalPos + ShakeAmount;
                        left = !left;
                    }
                }
            }
            
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "KrampusFull";

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.ApplyOutlineShader(); // For the body, black is fine
                    if (renderer.name.StartsWith("Prop"))
                    {
                        renderer.gameObject.AddComponent<CustomRotationSimple>();
                    }
                    else if (renderer.name.StartsWith("Horn"))
                    {
                        renderer.SetOutlineColor(new Color32(107, 106, 86, 255));
                    }
                    else if (renderer.name.StartsWith("Sack"))
                    {
                        renderer.SetOutlineColor(new Color32(94, 62, 45, 255));
                        renderer.gameObject.AddComponent<Shake>();
                    }
                    else if (renderer.name.StartsWith("Band"))
                    {
                        renderer.SetOutlineColor(new Color32(162, 114, 71, 255));
                    }
                    else if (renderer.name.StartsWith("Basket"))
                    {
                        renderer.SetOutlineColor(new Color32(71, 31, 31, 255));
                    }
                }
            }
        }
        
        public static Dictionary<Tower, Vector3Boxed> kidnapTowers = new Dictionary<Tower, Vector3Boxed>();
        public override string BossName => "Krampus";
        public override int SkullCount => 10;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string Icon => "KrampusIcon";
        public override string CustomSkullIcon => "Krampus";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 100;
        public override string BaseBloon => BloonType.sBad;
        public override string PreviewIcon => Icon + "Preview";
        public override string Description => "Most know of good ol' Saint. Nicolas. But not as many know of the much more sinister Krampus, who punishes anyone who's been bad throughout the year, in fact, his sacks are already carrying some bloons right now. Let's just say, when you hear his screech, you know that this will be a horrible night...";
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            //bloonModel.maxHealth = 20000000;
            bloonModel.maxHealth = 10000;
            bloonModel.RemoveAllChildren();
            bloonModel.speed /= 3f;

            StunTowersInRadiusActionModel shield = Game.instance.model.GetBloon("Vortex1").GetBehavior<StunTowersInRadiusActionModel>().Duplicate();
            CreateEffectActionModel effect = Game.instance.model.GetBloon("Vortex1").GetBehavior<CreateEffectActionModel>().Duplicate();
            shield.actionId = "ModdedSkullModdedBossKrampus";
            shield.radius *= 2;
            shield.Mutator.id = "KrampusStun";

            effect.actionId = shield.actionId;

            bloonModel.AddBehavior(effect);
            bloonModel.AddBehavior(shield);
            bloonModel.AddBehavior(new CreateSoundOnSpawnBloonModel("CreateSoundOnSpawnBloonModel_Roar", GetAudioClipReference("KrampusRoar")));
        }
        public override void OnSpawn(Bloon bloon)
        {
            XmasMod2025.boss = bloon;
            Hooks.StartMonobehavior<HandleTotem>();
            Hooks.StartMonobehavior<KrampusHandler>();
            XmasMod2025.KrampusAlive = true;
            BossUI.UpdateNameColor(new Color32(21, 23, 22, 255), null);
            PostProcessing.EnableNight();

            Game.instance.audioFactory.StopMusic();
            AudioClip FinalBossSound = ModContent.GetAudioClip<XmasMod2025>("FrostMoon");
            Game.instance.audioFactory.PlayMusic(FinalBossSound);
        }

        public static void KidnapTower()
        {
            if(!InGame.instance) return;
            
            foreach(var tower in InGame.instance.GetTowers())
            {
                if(tower.IsMutatedBy("TowerStun") && !kidnapTowers.ContainsKey(tower) && XmasMod2025.KrampusAlive)
                {
                    kidnapTowers.Add(tower, tower.Scale);
                    tower.Scale = Vector3Boxed.zero;
                }
                else
                {
                    foreach(var kidnap in kidnapTowers)
                    {
                        if(kidnap.Key == tower)
                        {
                            tower.Scale = kidnap.Value;
                            kidnapTowers.Remove(tower);
                        }
                    }
                }
            }
        }

        [RegisterTypeInIl2Cpp]
        public class KrampusHandler : MonoBehaviour
        {
            public bool half = false;
            public bool nextHalf = false;
            public void Start()
            {
                half = false;
                nextHalf = false;
            }
            public void Update()
            {
                if (XmasMod2025.boss == null)
                {
                    XmasMod2025.KrampusAlive = false;
                    this.Destroy();
                    return;
                }
                
                float healthPercent = XmasMod2025.boss.health / XmasMod2025.boss.bloonModel.maxHealth;
                float trackPercent = XmasMod2025.boss.DistanceTraveled;
                
                float totalSpeedMultiplier = 1 / (healthPercent * 10  / 1 + 1 / ( 1 - trackPercent < 0.1f ? 0.1f : 1 - trackPercent) / 2); // 1 - 0.1
                PostProcessing.SetPulseSpeed(totalSpeedMultiplier);

                KidnapTower();
                
                if (healthPercent <= 0.5f && !half)
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

                if (healthPercent <= 0.25f && !nextHalf)
                {
                    ModHelper.Log<XmasMod2025>(healthPercent);
                    foreach (var boss in ModContent.GetContent<ModBoss>())
                    {
                        if (boss.Id != ModContent.BloonID<KrampusBoss>())
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
