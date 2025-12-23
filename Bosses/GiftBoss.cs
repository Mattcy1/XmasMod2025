using System.Collections.Generic;
using BossAPI.Bosses;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Moabs;

namespace XmasMod2025.Bosses;

internal class GiftBoss : ModBoss
{
    public BloonModel Bloon;
    public override string BossName => "Big Ol' Present";
    public override int SkullCount => 3;
    public override string HealthBar => "";
    public override string IconGuid => "";
    public override int Stars => 6;
    public override string CustomSkullIcon => "";
    public override string Icon => "GiftBossIcon";
    public override string HealthBarBackground => "";
    public override int SpawnsRound => 20;
    public override string BaseBloon => BloonType.sBad;
    public override string Description => "Don't try to open him, he doesn't like it he'd probably attack you.";
    public override IEnumerable<string> DamageStates => [];

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth = 1500;
        bloonModel.RemoveAllChildren();
        bloonModel.speed /= 2;

        var spawn = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
        spawn.actionId = "SpawnsGift";
        spawn.bloonType = BloonID<CandyCaneBloon>();
        spawn.spawnCount = 2;
        spawn.bossName = "";
        spawn.spawnTrackMax += 0.5f;
        spawn.spawnTrackMin -= 0.5f;

        var spawn1 = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<SpawnBloonsActionModel>().Duplicate();
        spawn1.actionId = "ModdedSkullModdedBossGift Boss";
        spawn1.bloonType = BloonID<SnowMoab.WeakSnowMoab>();
        spawn1.spawnCount = 2;
        spawn1.bossName = "";
        spawn1.spawnTrackMax += 0.5f;
        spawn1.spawnTrackMin -= 0.5f;

        var timeTrigger = new TimeTriggerModel("SpawnsGiftTimer", 30, false, new[] { "SpawnsGift" });

        bloonModel.AddBehavior(spawn);
        bloonModel.AddBehavior(spawn1);
        bloonModel.AddBehavior(timeTrigger);

        bloonModel.ApplyDisplay<GiftDisplay>();
        Bloon = bloonModel;
    }

    public override void OnSpawn(Bloon bloon)
    {
        if (!XmasMod2025.KrampusAlive) XmasMod2025.boss = bloon;
    }

    public class GiftDisplay : ModBloonCustomDisplay<GiftBoss>
    {
        public override string AssetBundleName => "xmas";
        public override string PrefabName => "PresentBoss";

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.ApplyOutlineShader();

                if (renderer.name == "Propeller") renderer.gameObject.AddComponent<CustomRotationSimple>();

                if (renderer.name.StartsWith("Green"))
                {
                    renderer.SetOutlineColor(new Color32(2, 90, 0, 255));
                    renderer.SetMainTexture(GetTexture("GiftBoxGreen"));
                }
                else if (renderer.name.StartsWith("Red") || renderer.name == "Ribbons")
                {
                    renderer.SetOutlineColor(new Color(0.5f, 0, 0));
                    renderer.SetMainTexture(GetTexture("GiftBox"));
                }
                else
                {
                    renderer.SetOutlineColor(new Color(0.5f, 0, 0));
                    renderer.SetMainTexture(GetTexture("PresentBossDisplay"));
                }
            }
        }
    }
}