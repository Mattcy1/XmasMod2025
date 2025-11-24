using BossAPI.Bosses;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using UnityEngine;
using XmasMod2025.Bloons;
using XmasMod2025.BossAPI;

namespace XmasMod2025.Bosses
{
    internal class ChocoBoss : ModBoss
    {
        public override string BossName => "Choco Boss";
        public override int SkullCount => 4;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string CustomSkullIcon => "";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 60;
        public override string BaseBloon => BloonType.sBad;
        public override string Description => "All spawned bloons while the boss is alive gets x2 health, on top of that the boss stuns your towers on skull.";


        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 100000;
            bloonModel.RemoveAllChildren();

            StunTowersInRadiusActionModel stun = Game.instance.model.GetBloon("Vortex1").GetBehavior<StunTowersInRadiusActionModel>().Duplicate();
            stun.actionId = "ModdedSkullModdedBossChoco Boss";

            bloonModel.AddBehavior(stun);
        }

        public override void OnSpawn(Bloon bloon)
        {
            XmasMod2025.boss = bloon;
        }

        public class ChocoBossDisplay : ModBloonCustomDisplay<ChocoBoss>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "ChocoBoss";

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    if (renderer.name.StartsWith("Prop"))
                    {
                        renderer.gameObject.AddComponent<CustomRotationSimple>();
                    }
                }
            }
        }
    }
}
