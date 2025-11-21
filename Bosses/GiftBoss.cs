using Bosses.ModBossContenet;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Mods;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public BloonModel Bloon = null;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 3500;
            bloonModel.RemoveAllChildren();
            bloonModel.speed /= 2;

            SpawnBloonsActionModel spawn = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
            spawn.actionId = "SpawnsGift";
            spawn.bloonType = BloonType.sRainbow;
            spawn.spawnCount = 4;
            spawn.bossName = "";

            SpawnBloonsActionModel spawn1 = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
            spawn1.actionId = "ModdedSkull" + bloonModel.baseId;
            spawn1.bloonType = BloonType.sMoab;
            spawn1.spawnCount = 2;
            spawn1.bossName = "";

            TimeTriggerModel timeTrigger = new TimeTriggerModel("SpawnsGiftTimer", 30, false, new string[] { "SpawnsGift" });

            bloonModel.AddBehavior(spawn);
            bloonModel.AddBehavior(spawn1);
            bloonModel.AddBehavior(timeTrigger);

            Bloon = bloonModel;
        }

        public override string BossID => Bloon.baseId;

        public override BloonModel GetBloon() => Game.instance.model.GetBloon(Id);
    }
}
