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
using BTD_Mod_Helper.Api.Display;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Moabs;
using XmasMod2025.BossAPI;
using Color = UnityEngine.Color;

namespace XmasMod2025.Bosses
{
    internal class CoalBoss : ModBoss
    {
        public class CoalDisplay : ModBloonCustomDisplay<CoalBoss>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "CoalBoss";

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.ApplyOutlineShader();
                    if (renderer.name == "propeller")
                    {
                        renderer.gameObject.AddComponent<CustomRotationSimple>();
                        renderer.SetOutlineColor(new Color(0.1f, 0.1f, 0.1f));
                    }
                    else if (renderer.name != "coal")
                    {
                        renderer.SetOutlineColor(new Color32(125, 30, 30, 255));
                    }
                }
            }
        }

        public override string BossName => "Coal Boss";
        public override int SkullCount => 5;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string Icon => "CoalBossIcon";
        public override string CustomSkullIcon => "";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 80;
        public override string BaseBloon => BloonType.sBad;
        public override string Description => "The worse present of them all.";
        public override IEnumerable<string> DamageStates => [];
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 550000;
            bloonModel.RemoveAllChildren();
            bloonModel.speed /= 2;

            //"ModdedSkullModdedBossCoal Boss"

            ReflectProjectilesInRadiusActionModel shield = Game.instance.model.GetBloon("Vortex1").GetBehavior<ReflectProjectilesInRadiusActionModel>().Duplicate();
            CreateEffectActionModel effect = Game.instance.model.GetBloon("Vortex1").GetBehavior<CreateEffectActionModel>().Duplicate();
            shield.actionId = "ModdedSkullModdedBossCoal Boss";
            effect.actionId = shield.actionId;

            bloonModel.AddBehavior(effect);
            bloonModel.AddBehavior(shield);
        }
        public override void OnSpawn(Bloon bloon)
        {
            if (!XmasMod2025.KrampusAlive)
            {
                XmasMod2025.boss = bloon;
                Hooks.StartMonobehavior<HandleTotem>();
            }
        }
    }

    internal class CoalTotem : ModBloon
    {
        public override string BaseBloon => BloonType.sRed;
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 3000;
            bloonModel.dontShowInSandbox = true;
            bloonModel.danger = 99999;

            TimeTriggerModel timeTrigger = new TimeTriggerModel("CoalTotemTimer", 15, false, new string[] { "SpawnsBloonCoal" });
            bloonModel.AddBehavior(timeTrigger);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class HandleTotem : MonoBehaviour
    {
        public static Dictionary<Bloon, float> totems = new Dictionary<Bloon, float>();

        public void Start()
        {
            totems.Clear();
        }
        public void Update()
        {
            if (XmasMod2025.boss == null)
            {
                this.Destroy();
                totems.Clear();
                return;
            }

            List<Bloon> toRemove = new List<Bloon>();

            foreach (var kvp in totems)
            {
                Bloon totem = kvp.Key;
                float target = kvp.Value;

                float perc = totem.PercThroughMap();

                if (perc >= target)
                {
                    totem.trackSpeedMultiplier = 0;
                    toRemove.Add(totem);
                }
                else
                {
                    totem.trackSpeedMultiplier = 50;
                }
            }

            foreach (var t in toRemove)
                totems.Remove(t);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Bloon), nameof(Bloon.OnSpawn))]
    public class BloonSpawn
    {
        private static System.Random rnd = new System.Random();

        [HarmonyLib.HarmonyPostfix]
        public static void Postfix(Bloon __instance)
        {
            if (__instance.bloonModel.baseId == ModContent.BloonID<CoalTotem>())
            {
                float value = (float)(0.1 + rnd.NextDouble() * (0.9 - 0.1));
                HandleTotem.totems.Add(__instance, value);
            }
        }
    }
}
