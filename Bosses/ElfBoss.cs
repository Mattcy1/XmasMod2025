using XmasMod2025.ModBossContenet;
using BTD_Mod_Helper.Api;
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
using XmasMod2025.Bloons;

namespace XmasMod2025.Bosses
{
    internal class ElfBoss : ModBoss
    {
        public override string BossName => "Evil Elf";
        public override int SkullCount => 3;
        public override string HealthBar => "";
        public override string IconGuid => "";
        public override int Stars => 6;
        public override string CustomSkullIcon => "";
        public override string HealthBarBackground => "";
        public override int SpawnsRound => 40;
        public override string BaseBloon => BloonType.sBad;

        public BloonModel Bloon = null;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 1000;
            bloonModel.RemoveAllChildren();

            TimeTriggerModel timeTrigger = new TimeTriggerModel("ElfTax", 30, false, new string[] { "ElfTax" });

            SpawnBloonsActionModel spawn1 = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
            spawn1.actionId = "ModdedSkull" + bloonModel.baseId;
            spawn1.bloonType = BloonType.sMoab;
            spawn1.spawnCount = 1;
            spawn1.bossName = "";
            spawn1.spawnTrackMax += 0.5f;
            spawn1.spawnTrackMin -= 0.5f;

            bloonModel.AddBehavior(spawn1);
            bloonModel.AddBehavior(timeTrigger);

            Bloon = bloonModel;
        }

        public override string BossID => Bloon.baseId;

        public override BloonModel GetBloon() => Game.instance.model.GetBloon(Id);
    }
}
