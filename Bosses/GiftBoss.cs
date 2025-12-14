using BossAPI.Bosses;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Moabs;

namespace XmasMod2025.Bosses
{
    internal class GiftBoss : ModBoss
    {
        public override string BossName => "Gift Boss";
        public override int SkullCount => 3;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string CustomSkullIcon => "";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 20;
        public override string BaseBloon => BloonType.sBad;
        public override string Description => "Every 30 seconds, spawn 2 candy cane bloons, On Skull spawns a snow moab.";

        public BloonModel Bloon = null;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 1500;
            bloonModel.RemoveAllChildren();
            bloonModel.speed /= 2;

            SpawnBloonsActionModel spawn = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
            spawn.actionId = "SpawnsGift";
            spawn.bloonType = ModContent.BloonID<CandyCaneBloon>();
            spawn.spawnCount = 2;
            spawn.bossName = "";
            spawn.spawnTrackMax += 0.5f;
            spawn.spawnTrackMin -= 0.5f;

            SpawnBloonsActionModel spawn1 = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
            spawn1.actionId = "ModdedSkullModdedBossGift Boss";
            spawn1.bloonType = ModContent.BloonID<SnowMoab.WeakSnowMoab>();
            spawn1.spawnCount = 2;
            spawn1.bossName = "";
            spawn1.spawnTrackMax += 0.5f;
            spawn1.spawnTrackMin -= 0.5f;

            TimeTriggerModel timeTrigger = new TimeTriggerModel("SpawnsGiftTimer", 30, false, new string[] { "SpawnsGift" });

            bloonModel.AddBehavior(spawn);
            bloonModel.AddBehavior(spawn1);
            bloonModel.AddBehavior(timeTrigger);

            Bloon = bloonModel;
        }

        public override void OnSpawn(Bloon bloon)
        {
            XmasMod2025.boss = bloon;
        }
    }
}
