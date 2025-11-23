using BossAPI.Bosses;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using UnityEngine;
using XmasMod2025.BossAPI;

namespace XmasMod2025.Bosses
{
    internal class ChocoBoss : ModBoss
    {
        public override string BossName => "Choco Boss";
        public override int SkullCount => 5;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string CustomSkullIcon => "";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 60;
        public override string BaseBloon => BloonType.sBad;

        public BloonModel Bloon = null;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 100000;
            bloonModel.RemoveAllChildren();

            StunTowersInRadiusActionModel stun = Game.instance.model.GetBloon("Vortex1").GetBehavior<StunTowersInRadiusActionModel>().Duplicate();
            stun.actionId = "ModdedSkullModdedBossChoco Boss";

            bloonModel.AddBehavior(stun);
            Bloon = bloonModel;
        }

        public override void OnSpawn(Bloon bloon)
        {
            XmasMod2025.boss = bloon;
        }
    }
}
