using BossAPI.Bosses;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using System.Collections.Generic;
using UnityEngine;
using XmasMod2025.Bloons.Moabs;

namespace XmasMod2025.Bosses
{
    internal class ElfBoss : ModBoss
    {
        public class ElfBossDisplay : ModBloonCustomDisplay<ElfBoss>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "ElfBoss";

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.ApplyOutlineShader();
                    renderer.SetOutlineColor(new Color32(102, 163, 73, 255));

                    if (renderer.name == "Propeller")
                    {
                        renderer.gameObject.AddComponent<CustomRotationSimple>();
                        renderer.SetMainTexture(GetTexture("ElfBossDisplay"));
                    }
                    else if (renderer.name.StartsWith("ElfGeo"))
                    {
                        renderer.SetMainTexture(GetTexture("ElfBossDisplay2"));
                    }
                    else
                    {
                        renderer.SetMainTexture(GetTexture("ElfBossDisplay"));
                    }
                }
            }
        }
        public override string BossName => "Evil Elf";
        public override int SkullCount => 3;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string CustomSkullIcon => "";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 40;
        public override string BaseBloon => BloonType.sBad;
        public override string Description => "Santa wasn't paying this elf enough so he decided to quit and fight back. (CAN ONLY BE DAMAGED BY ELF MONKEY!!)";
        public override IEnumerable<string> DamageStates => [];
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 900;
            bloonModel.RemoveAllChildren();

            TimeTriggerModel timeTrigger = new TimeTriggerModel("ElfTax", 30, false, new string[] { "ElfTax" });

            SpawnBloonsActionModel spawn1 = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
            spawn1.actionId = "ModdedSkullModdedBossEvil Elf";
            spawn1.bloonType = ModContent.BloonID<IceMoab>();
            spawn1.spawnCount = 1;
            spawn1.bossName = "";
            spawn1.spawnTrackMax += 0.5f;
            spawn1.spawnTrackMin -= 0.5f;

            bloonModel.AddBehavior(spawn1);
            bloonModel.AddBehavior(timeTrigger);
        }

        public override void OnSpawn(Bloon bloon)
        {
            if (!XmasMod2025.KrampusAlive)
            {
                XmasMod2025.boss = bloon;
            }
        }
    }
}
