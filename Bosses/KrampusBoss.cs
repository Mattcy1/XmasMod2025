using System.Collections.Generic;
using BossAPI.Bosses;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using UnityEngine;
using XmasMod2025.BossAPI;
using static XmasMod2025.XmasMod2025;
using Color = UnityEngine.Color;
using Vector3 = UnityEngine.Vector3;

namespace XmasMod2025.Bosses;

internal class KrampusBoss : ModBoss
{
    public static Dictionary<Tower, Vector3Boxed> kidnapTowers = new();
    public override string BossName => "Krampus";
    public override int SkullCount => 5;
    public override string HealthBar => "";
    public override string IconGuid => "";
    public override int Stars => 6;
    public override string Icon => "KrampusIcon";
    public override string CustomSkullIcon => "Krampus";
    public override string HealthBarBackground => "";
    public override int SpawnsRound => 100;
    public override string BaseBloon => BloonType.sBad;
    public override string PreviewIcon => Icon + "Preview";

    public override string Description =>
        "Most know of good ol' Saint. Nicolas. But not as many know of the much more sinister Krampus, who punishes anyone who's been bad throughout the year, in fact, his sacks are already carrying some bloons right now. Let's just say, when you hear his screech, you know that this will be a horrible night...";

    public override IEnumerable<string> DamageStates => [];

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth = 15000000;
        bloonModel.RemoveAllChildren();
        bloonModel.speed /= 3f;

        var shield = Game.instance.model.GetBloon("Vortex1").GetBehavior<StunTowersInRadiusActionModel>().Duplicate();
        var effect = Game.instance.model.GetBloon("Vortex1").GetBehavior<CreateEffectActionModel>().Duplicate();
        shield.actionId = "ModdedSkullModdedBossKrampus";
        shield.radius *= 2;
        shield.Mutator.id = "KrampusStun";

        effect.actionId = shield.actionId;

        bloonModel.AddBehavior(effect);
        bloonModel.AddBehavior(shield);
        bloonModel.AddBehavior(new CreateSoundOnSpawnBloonModel("CreateSoundOnSpawnBloonModel_Roar",
            GetAudioClipReference("KrampusRoar")));
    }

    public override void OnSpawn(Bloon bloon)
    {
        boss = bloon;
        Hooks.StartMonobehavior<HandleTotem>();
        Hooks.StartMonobehavior<KrampusHandler>();
        KrampusAlive = true;
        BossUI.UpdateNameColor(new Color32(42, 23, 44, 255));
        PostProcessing.EnableNight();

        Game.instance.audioFactory.StopMusic();
        var FinalBossSound = GetAudioClip<XmasMod2025>("FrostMoon");
        Game.instance.audioFactory.PlayMusic(FinalBossSound);
    }

    public static void KidnapTower()
    {
        if (!InGame.instance) return;

        foreach (var tower in InGame.instance.GetTowers())
            if (tower.IsMutatedBy("TowerStun") && !kidnapTowers.ContainsKey(tower) && KrampusAlive)
            {
                kidnapTowers.Add(tower, tower.Scale);
                tower.Scale = Vector3Boxed.zero;
            }
            else
            {
                foreach (var kidnap in kidnapTowers)
                    if (kidnap.Key == tower)
                    {
                        tower.Scale = kidnap.Value;
                        kidnapTowers.Remove(tower);
                    }
            }
    }

    public class KrampusDisplay : ModBloonCustomDisplay<KrampusBoss>
    {
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
                    renderer.SetMainTexture(GetTexture("KrampusBody"));
                }
                else if (renderer.name.StartsWith("Horn"))
                {
                    renderer.SetOutlineColor(new Color32(107, 106, 86, 255));
                    renderer.SetMainTexture(GetTexture("KrampusHorns"));
                }
                else if (renderer.name.StartsWith("Sack"))
                {
                    renderer.SetOutlineColor(new Color32(94, 62, 45, 255));
                    renderer.gameObject.AddComponent<Shake>();
                    renderer.SetMainTexture(GetTexture("Palette"));
                }
                else if (renderer.name.StartsWith("Band"))
                {
                    renderer.SetOutlineColor(new Color32(162, 114, 71, 255));
                    renderer.SetMainTexture(GetTexture("Palette"));
                }
                else if (renderer.name.StartsWith("Basket"))
                {
                    renderer.SetOutlineColor(new Color32(71, 31, 31, 255));
                    renderer.SetMainTexture(GetTexture("Palette"));
                }
                else
                {
                    renderer.SetMainTexture(GetTexture("KrampusBody"));
                }
            }
        }

        [RegisterTypeInIl2Cpp]
        public class Shake : MonoBehaviour
        {
            public Vector3 ShakeAmount = new(1f, 0.4f, 0);
            public Vector3 originalPos;
            public float ShakeTime = 0.2f;

            public bool left;
            private readonly float nextMoveTime = 0f;

            private float stateOneTime => ShakeTime * 0.25f;
            private float stateTwoTime => ShakeTime * 0.75f;

            private void Start()
            {
                originalPos = transform.position;
            }

            private void Update()
            {
                if (Time.time >= nextMoveTime)
                {
                    transform.position = left ? originalPos - ShakeAmount : originalPos + ShakeAmount;
                    left = !left;
                }
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    public class KrampusHandler : MonoBehaviour
    {
        public bool half;
        public bool nextHalf;

        public void Start()
        {
            Log("spawn krampus");
            half = false;
            nextHalf = false;
        }

        public void Update()
        {
            if (boss == null)
            {
                KrampusAlive = false;
                this.Destroy();
                return;
            }

            float healthPercent = boss.health / boss.bloonModel.maxHealth;
            var trackPercent = boss.DistanceTraveled;

            var totalSpeedMultiplier =
                1 / (healthPercent * 10 / 1 + 1 / (1 - trackPercent < 0.1f ? 0.1f : 1 - trackPercent) / 2); // 1 - 0.1
            PostProcessing.SetPulseSpeed(totalSpeedMultiplier);

            KidnapTower();

            if (boss.health <= boss.bloonModel.maxHealth * 0.5f && !half)
            {
                BossUI.UpdateNameColor(Color.yellow);

                var root = boss.bloonModel.Duplicate();

                var timeTrigger = new TimeTriggerModel("ElfTax", 30, false, new[] { "ElfTax" });
                root.AddBehavior(timeTrigger);

                var heal = new TimeTriggerModel("GrinchHeal", 15, false, new[] { "GrinchHeal" });
                root.AddBehavior(heal);

                var effect = Game.instance.model.GetBloon("Vortex1").GetBehavior<CreateEffectActionModel>().Duplicate();
                effect.actionId = heal.actionIds[0];
                effect.effect = CreatePrefabReference<GiftEffectButBig>();

                boss.UpdateRootModel(root);
                half = true;
            }

            if (boss.health <= boss.bloonModel.maxHealth * 0.25f && !nextHalf)
            {
                ModHelper.Log<XmasMod2025>(healthPercent);
                foreach (var boss in GetContent<ModBoss>())
                    if (boss.Id != BloonID<KrampusBoss>())
                        InGame.instance.SpawnBloons(boss.Id, 1, 0);

                nextHalf = true;
            }
        }
    }
}