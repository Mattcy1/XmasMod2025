using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.MapSets;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Scenes;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;
using XmasMod2025;
using XmasMod2025.Bloons.Bfbs;
using XmasMod2025.Bloons.Moabs;
using XmasMod2025.Bloons.Zomgs;
using XmasMod2025.Towers;
using Il2CppAssets.Scripts.Models.Audio;

namespace XmasMod2025.Towers.Upgrades
{
    internal class ElfMiddlePath
    {
        public class FastThrow : ChristmasUpgrade<ElfMonkey>
        {
            public override int Path => MIDDLE;
            public override int Tier => 1;
            public override int Cost => 35;
            public override string Icon => "Elf010Icon";
            public override string Description => "Faster projectile speed, with more pierce";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var weapons in towerModel.GetWeapons())
                {
                    weapons.projectile.pierce += 1;
                    weapons.projectile.GetBehavior<TravelStraitModel>().speed *= 1.1f;
                    weapons.projectile.GetBehavior<TravelStraitModel>().Speed *= 1.1f;

                }
            }
        }

        public class ElfStrongerThrow : ChristmasUpgrade<ElfMonkey>
        {
            public override int Path => MIDDLE;
            public override int Tier => 2;
            public override int Cost => 60;
            public override string Icon => "Elf020Icon";
            public override string DisplayName => "Stronger Throw";
            public override string Description => "More damages, to bloons";
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                foreach (var weapons in towerModel.GetWeapons())
                {
                    weapons.projectile.GetDamageModel().damage += 1;
                }
            }
        }

        public class TierMiddle3 : ChristmasUpgrade<ElfMonkey>
        {
            public class HammahElf : ModTowerCustomDisplay<ElfMonkey>
            {
                public override bool UseForTower(params int[] tiers) => tiers[1] == 3;

                public override string AssetBundleName => "xmas";
                public override string PrefabName => Name;

                public override void ModifyDisplayNode(UnityDisplayNode node)
                {
                    foreach (var renderer in node.GetMeshRenderers())
                    {
                        renderer.ApplyOutlineShader();
                        renderer.SetOutlineColor(new Color32(128, 77, 54, 255));
                    }
                }
            }
            
            public override int Path => MIDDLE;
            public override int Tier => 3;
            public override int Cost => 500;
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
                var SoundModel = new SoundModel("bonk", ModContent.GetAudioClipReference<XmasMod2025>("bonk"));
                newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound1 = SoundModel;
                newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound2 = SoundModel;
                newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound3 = SoundModel;
                newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound4 = SoundModel;
                newAtk.weapons[0].GetBehavior<CreateSoundOnProjectileCreatedModel>().sound5 = SoundModel;

                towerModel.AddBehavior(newAtk);

                foreach(var weapon in towerModel.GetWeapons())
                {
                    var stun = Game.instance.model.GetTowerFromId("BombShooter-400").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().Duplicate();
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
        }
    }

    public class TierMiddle4 : ChristmasUpgrade<ElfMonkey>
    {
        public class HammahElf2 : ModTowerCustomDisplay<ElfMonkey>
        {
            public override bool UseForTower(params int[] tiers) => tiers[1] == 4;

            public override string AssetBundleName => "xmas";
            public override string PrefabName => Name;

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.ApplyOutlineShader();
                    renderer.SetOutlineColor(new Color32(128, 77, 54, 255));
                }
            }
        }
        public override int Path => MIDDLE;
        public override int Tier => 4;
        public override int Cost => 1200;
        public override string DisplayName => "Huge Hammah";
        public override string Description => "Better overall stats, can now stun BFBs, Slam: Stuns almost all bloons on screen.";
        public override string Icon => "Elf040Icon";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var weapon in towerModel.GetWeapons())
            {
                weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().mutationId = "Elf040";
                weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().lifespan = 0.7f;
                weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().Lifespan = 0.7f;
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

        public class ShockwaveEffect : ModCustomDisplay
        {
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "Shockwave";
        }
    }

    public class TierMiddle5 : ChristmasUpgrade<ElfMonkey>
    {
        public class HammahElf3 : ModTowerCustomDisplay<ElfMonkey>
        {
            public override bool UseForTower(params int[] tiers) => tiers[1] == 5;

            public override string AssetBundleName => "xmas";
            public override string PrefabName => Name;

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.ApplyOutlineShader();
                    renderer.SetOutlineColor(new Color32(128, 77, 54, 255));
                }
            }
        }
        public override int Path => MIDDLE;
        public override int Tier => 5;
        public override int Cost => 7125;
        public override string Icon => "Elf050Icon";
        public override string DisplayName => "5 Tons Hammer";
        public override string Description => "Slam all bloons causing them to be stunned for 10s Including Bosses, Bad.";
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var weapon in towerModel.GetWeapons())
            {
                weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().mutationId = "Elf050";
                weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().lifespan = 1f;
                weapon.projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().Lifespan = 1f;
                towerModel.GetAbility().displayName = "Super Slam";
                towerModel.GetAbility().description = "Stuns EVERYTHING on the screen for 10s";

                weapon.projectile.pierce += 5;
                weapon.projectile.GetDamageModel().damage += 17;
                weapon.rate -= 0.1f;
            }
        }
    }

    [HarmonyPatch(typeof(Ability), nameof(Ability.Activate))]
    public static class PatchAbility
    {
        public static void Postfix(Ability __instance)
        {
            if(__instance.abilityModel.displayName == "Slam")
            {
                foreach(var bloon in InGame.instance.GetBloons())
                {
                    var slow = Game.instance.model.GetTowerFromId(ModContent.TowerID<ElfMonkey>(0, 4, 0)).GetWeapon().projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().Duplicate();
                    slow.mutationId = "Slam";
                    slow.lifespan = 240;
                    slow.Lifespan = 240;
                    bloon.AddMutator(slow.Mutator, 240, false, true, false, false, false, false, false, 2, false, true);
                }
            }

            if (__instance.abilityModel.displayName == "Super Slam")
            {
                foreach (var bloon in InGame.instance.GetBloons())
                {
                    var slow = Game.instance.model.GetTowerFromId(ModContent.TowerID<ElfMonkey>(0, 4, 0)).GetWeapon().projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().Duplicate();
                    slow.mutationId = "SuperSlam";
                    slow.lifespan = 600;
                    slow.Lifespan = 600;
                    bloon.AddMutator(slow.Mutator, 600, false, true, false, false, false, false, false, 2, false, true);
                }
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
                if(__instance.mutationId == "Elf030")
                {
                    if (bloon.isMoab && !bloon.HasTag("Moab"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (__instance.mutationId == "Elf040")
                {
                    if (bloon.isMoab && bloon.HasTag("Zomg") || bloon.HasTag("Bad"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (__instance.mutationId == "Elf050")
                {
                    if (bloon.isMoab && bloon.HasTag("Bad"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (__instance.mutationId == "Slam")
                {
                    if (bloon.isMoab && bloon.HasTag("Ddt") || bloon.HasTag("Bad"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (__instance.mutationId == "SuperSlam")
                {
                    return true;
                }
            }

            return true;
        }
    }
}

