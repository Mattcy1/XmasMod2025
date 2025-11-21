using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System.Linq;
using UnityEngine;
using XmasMod2025.Bloons;
using XmasMod2025.Towers;
using XmasMod2025.Towers.SubTowers;
using static Il2CppSystem.DateTimeParse;

namespace XmasMod2025.Towers
{
    public class ElfMonkey : ChristmasTower
    {
        public override string BaseTower => TowerType.DartMonkey;
        public override bool DontAddToShop => false;
        public override string Icon => VanillaSprites.MonkeyVillageElfPetIcon;
        public override string Portrait => Icon;
        public override string Description => "One of Santa's Minions, help you defend.";

        public override int BottomPathUpgrades => 5;
        public override int Cost => 35;

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.ApplyDisplay<ElfDisplay>();

            var proj = towerModel.GetWeapon().projectile;

            proj.GetDamageModel().damage += 1;
            proj.pierce = 2;
            proj.ApplyDisplay<Gift1>();
            proj.id = "ElfProj";
            proj.AddBehavior(new RandomDisplayModel("RandomDisplayModel_", new Il2CppReferenceArray<PrefabReference>([new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()), new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()), new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())]), true));
        }
    }

    public class ElfDisplay : ModDisplay
    {
        public override string BaseDisplay => MonkeyVillageElfPet;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {

            foreach (var rend in node.GetMeshRenderers())
            {
                rend.SetMainTexture(GetTexture("Elf"));
                rend.ApplyOutlineShader();
            }
        }
    }


    [HarmonyLib.HarmonyPatch(typeof(Weapon), nameof(Weapon.Emit))]
    public class Weapon_Emit
    {
        [HarmonyLib.HarmonyPostfix]
        public static void Postfix(Weapon __instance)
        {
            if(__instance.weaponModel.projectile.id == "ElfProj")
            {
                XmasMod2025.AddCurrency(CurrencyType.Gift, -1);
            }
        }
    }
}