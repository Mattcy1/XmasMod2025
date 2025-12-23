using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using UnityEngine;
using XmasMod2025.Assets;

namespace XmasMod2025.Towers.Upgrades;

internal class ElfMiddlePath
{
    public class FastThrow : ChristmasUpgrade<ElfMonkey>
    {
        public override int Path => MIDDLE;
        public override int Tier => 1;
        public override int Cost => 10;
        public override string Icon => "Elf010Icon";
        public override string Description => "Faster projectile speed, with more pierce";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var weapons in towerModel.GetWeapons())
            {
                weapons.projectile.pierce += 1;
                weapons.projectile.GetBehavior<TravelStraitModel>().speed *= 1.21f;
            }
        }
    }

    public class ElfStrongerThrow : ChristmasUpgrade<ElfMonkey>
    {
        public override int Path => MIDDLE;
        public override int Tier => 2;
        public override int Cost => 20;
        public override string Icon => "Elf020Icon";
        public override string DisplayName => "Stronger Throw";
        public override string Description => "Deals more damage to bloons.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var weapons in towerModel.GetWeapons()) weapons.projectile.GetDamageModel().damage += 1;
        }
    }

    public class TierMiddle3 : ChristmasUpgrade<ElfMonkey>
    {
        public override int Path => MIDDLE;
        public override int Tier => 3;
        public override int Cost => 55;
        public override string DisplayName => "Hammah Time";
        public override string Icon => "Elf030Icon";
        public override string Description => "Stuns bloon on impacts up to MOABS.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().RemoveWeapon(towerModel.GetWeapon(0));
            var newAtk = Game.instance.model.GetTowerFromId("Sauda").GetAttackModel().Duplicate();
            newAtk.weapons[0].projectile.pierce = 3;
            newAtk.weapons[0].projectile.GetDamageModel().damage = 2;
            newAtk.weapons[0].rate -= 0.1f;
            var SoundModel = new SoundModel("bonk", GetAudioClipReference<XmasMod2025>("bonk"));
            newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound1 = SoundModel;
            newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound2 = SoundModel;
            newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound3 = SoundModel;
            newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound4 = SoundModel;
            newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound5 = SoundModel;

            towerModel.AddBehavior(newAtk);

            foreach (var weapon in towerModel.GetWeapons())
            {
                var stun = Game.instance.model.GetTowerFromId("BombShooter-400").GetAttackModel().weapons[0].projectile
                    .GetBehavior<CreateProjectileOnContactModel>().Duplicate();
                stun.projectile.GetDamageModel().damage = 0;
                stun.projectile.pierce = 1;
                stun.projectile.GetBehavior<SlowModel>().multiplier = 0;
                stun.projectile.GetBehavior<SlowModel>().lifespan = 0.5f;
                stun.projectile.GetBehavior<SlowModel>().Lifespan = 0.5f;
                stun.projectile.GetBehavior<SlowModel>().mutationId = "Elf030";

                stun.projectile.RemoveBehavior<PushBackModel>();
                stun.projectile.RemoveBehavior<FilterBloonIfDamageTypeModel>();
                stun.projectile.RemoveBehavior<SlowModifierForTagModel>();

                weapon.projectile.AddBehavior(stun);
                weapon.projectile.UpdateCollisionPassList();

                weapon.projectile.GetDamageModel().damage += 1;
                weapon.projectile.pierce += 1;
            }
        }

        public class HammahElf : ModTowerCustomDisplay<ElfMonkey>
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => Name;

            public override bool UseForTower(params int[] tiers)
            {
                return tiers[1] == 3;
            }

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.ApplyOutlineShader();
                    renderer.SetOutlineColor(new Color32(128, 77, 54, 255));
                    if(renderer.name.StartsWith("Elf"))
                    {
                        renderer.SetMainTexture(GetTexture("ElfGeo"));
                    }
                    else
                    {
                        renderer.material.mainTexture = AssetHelper.Get<Texture2D>("Texture_01");
                        if (AssetHelper.Get<Texture2D>("Texture_01") == null)
                        {
                            XmasMod2025.Log("Texture_01 doesn't exist");
                        }
                    }
                }
            }
        }
    }
}

public class TierMiddle4 : ChristmasUpgrade<ElfMonkey>
{
    public override int Path => MIDDLE;
    public override int Tier => 4;
    public override int Cost => 415;
    public override string DisplayName => "Huge Hammah";

    public override string Description =>
        "Better overall stats, can now stun BFBs, Slam: Stuns almost all bloons on screen.";

    public override string Icon => "Elf040Icon";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        foreach (var weapon in towerModel.GetWeapons())
        {
            weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                .mutationId = "Elf040";
            weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                .lifespan = 0.7f;
            weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                .Lifespan = 0.7f;
            weapon.projectile.pierce += 1;
            weapon.projectile.GetDamageModel().damage += 3;
            weapon.rate -= 0.1f;
        }

        var ability = Game.instance.model.GetTowerFromId("BombShooter-040").GetAbility().Duplicate();
        ability.displayName = "Slam";
        ability.animation = 3;
        ability.AnimationOffset = 0.4167f; // 10/24
        ability.GetBehavior<CreateEffectOnAbilityModel>().effectModel = new EffectModel("Shockwave",
            CreatePrefabReference<ShockwaveEffect>(), 1, 4, Fullscreen.No, false, false, true, false, false, false);
        ability.RemoveBehavior<CreateSoundOnAbilityModel>();
        ability.RemoveBehavior<ActivateAttackModel>();
        ability.description = "Slam all bloons on screen expect a few slammed bloon are stunned.";
        towerModel.AddBehavior(ability);
    }

    public class HammahElf2 : ModTowerCustomDisplay<ElfMonkey>
    {
        public override string AssetBundleName => "xmas";
        public override string PrefabName => Name;

        public override bool UseForTower(params int[] tiers)
        {
            return tiers[1] == 4;
        }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                if(renderer.name.StartsWith("Elf"))
                {
                    renderer.SetMainTexture(GetTexture("ElfGeo"));
                }
                renderer.ApplyOutlineShader();
                renderer.SetOutlineColor(new Color32(128, 77, 54, 255));
            }
        }
    }

    public class ShockwaveEffect : ModCustomDisplay
    {
        public override string AssetBundleName => "xmas";
        public override string PrefabName => "Shockwave";
    }
}

public class TierMiddle5 : ChristmasUpgrade<ElfMonkey>
{
    public override int Path => MIDDLE;
    public override int Tier => 5;
    public override int Cost => 4000;
    public override string Icon => "Elf050Icon";
    public override string DisplayName => "5 Tons Hammer";
    public override string Description => "Slam all bloons causing them to be stunned for 10s Including Bosses, Bad.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        foreach (var weapon in towerModel.GetWeapons())
        {
            weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                .mutationId = "Elf050";
            weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                .lifespan = 1f;
            weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                .Lifespan = 1f;
            towerModel.GetAbility().displayName = "Super Slam";
            towerModel.GetAbility().description = "Stuns EVERYTHING on the screen for 10s";

            weapon.projectile.pierce += 5;
            weapon.projectile.GetDamageModel().damage += 17;
            weapon.rate -= 0.1f;
        }
    }

    public class HammahElf3 : ModTowerCustomDisplay<ElfMonkey>
    {
        public override string AssetBundleName => "xmas";
        public override string PrefabName => Name;

        public override bool UseForTower(params int[] tiers)
        {
            return tiers[1] == 5;
        }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                if(renderer.name.StartsWith("Elf"))
                {
                    renderer.SetMainTexture(GetTexture("ElfGeo"));
                }
                renderer.ApplyOutlineShader();
                renderer.SetOutlineColor(new Color32(128, 77, 54, 255));
            }
        }
    }
}

[HarmonyPatch(typeof(Ability), nameof(Ability.Activate))]
public static class PatchAbility
{
    public static void Postfix(Ability __instance)
    {
        if (__instance.abilityModel.displayName == "Slam")
            foreach (var bloon in InGame.instance.GetBloons())
            {
                var slow = Game.instance.model.GetTowerFromId(ModContent.TowerID<ElfMonkey>(0, 4)).GetWeapon()
                    .projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                    .Duplicate();
                slow.mutationId = "Slam";
                slow.lifespan = 240;
                slow.Lifespan = 240;
                bloon.AddMutator(slow.Mutator, 240, false, true, false, false, false, false, false, 2, false, true);
            }

        if (__instance.abilityModel.displayName == "Super Slam")
            foreach (var bloon in InGame.instance.GetBloons())
            {
                var slow = Game.instance.model.GetTowerFromId(ModContent.TowerID<ElfMonkey>(0, 4)).GetWeapon()
                    .projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>()
                    .Duplicate();
                slow.mutationId = "SuperSlam";
                slow.lifespan = 600;
                slow.Lifespan = 600;
                bloon.AddMutator(slow.Mutator, 600, false, true, false, false, false, false, false, 2, false, true);
            }
    }
}

[HarmonyPatch(typeof(SlowModel.SlowMutator), nameof(SlowModel.SlowMutator.Mutate))]
public static class SlowMutator_Mutate
{
    public static bool Prefix(SlowModel.SlowMutator __instance, Model baseModel)
    {
        if (baseModel.Is<BloonModel>(out var bloon))
        {
            if (__instance.mutationId == "Elf030")
            {
                if (bloon.isMoab && !bloon.HasTag("Moab")) return false;

                return true;
            }

            if (__instance.mutationId == "Elf040")
            {
                if ((bloon.isMoab && bloon.HasTag("Zomg")) || bloon.HasTag("Bad")) return false;

                return true;
            }

            if (__instance.mutationId == "Elf050")
            {
                if (bloon.isMoab && bloon.HasTag("Bad")) return false;

                return true;
            }

            if (__instance.mutationId == "Slam")
            {
                if ((bloon.isMoab && bloon.HasTag("Ddt")) || bloon.HasTag("Bad")) return false;

                return true;
            }

            if (__instance.mutationId == "SuperSlam") return true;
        }

        return true;
    }
}