using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Behaviors.Projectiles;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppTMPro;
using MelonLoader;
using System.Diagnostics;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using UnityEngine;
using static MelonLoader.MelonLogger;
using Projectile = Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile;

namespace XmasMod2025.Towers
{
    public class XmasTree : ChristmasTower
    {
        public override bool CostsGifts => false;

        public override string BaseTower => TowerType.BananaFarm;
        public override int Cost => 0;
        public override int ShopTowerCount => 1;
        public override string Portrait => Icon;

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.dontDisplayUpgrades = true;

            var proj = towerModel.GetWeapon().projectile;

            proj.GetBehavior<CashModel>().maximum = 0;
            proj.GetBehavior<CashModel>().minimum = proj.GetBehavior<CashModel>().maximum;

            proj.RemoveBehavior<AgeModel>();
            proj.RemoveBehavior<CreateEffectOnExpireModel>();

            proj.ApplyDisplay<Gift1>();
            proj.AddBehavior(new RandomDisplayModel("RandomDisplayModel_", new Il2CppReferenceArray<PrefabReference>([new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()), new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()), new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())]), true));

            proj.id = "TreeGift";

            towerModel.GetWeapon().GetBehavior<EmissionsPerRoundFilterModel>().count = 5;
            proj.GetBehavior<CreateTextEffectModel>().assetId = new("");
        }

        public class Display : ModTowerCustomDisplay<XmasTree>
        {
            public override bool UseForTower(params int[] tiers) => true;
            public override string AssetBundleName => "xmas";
            public override string PrefabName => "ChristmasTreePrefab";

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                node.GetMeshRenderers().ForEach(r =>
                {
                    r.ApplyOutlineShader();
                    r.SetOutlineColor(new Color32(1, 53, 1, 255));
                });
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), nameof(Projectile.Pickup))]
    public class HandlePickup
    {
        [HarmonyPostfix]

        public static void Prefix(Projectile __instance)
        {
            if (__instance.projectileModel.id == "TreeGift")
            {
                var random = new System.Random().Next(1, 5);

                if (InGame.instance != null || InGame.instance.bridge != null)
                {
                    InGame.instance.bridge.simulation.CreateTextEffect(__instance.Position, ModContent.CreatePrefabReference<CollectText>(), 2f, $"+{random} Gifts", true);
                }

                XmasMod2025.Gifts += random;
            }
        }
    }

    public class Gift1 : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Gift1");
        }
    }

    public class Gift2 : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Gift2");
        }
    }
    public class Gift3 : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Gift3");
        }
    }
    public class Gift4 : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Gift4");
        }
    }
    public class Gift5 : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Gift5");
        }
    }
    public class Gift6 : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Gift6");
        }
    }

    public class CollectText : ModDisplay
     {
        public override string BaseDisplay => "80178409df24b3b479342ed73cffb63d";
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (Renderer renderer in node.genericRenderers)
            {
                //node.GetComponentInChildren<TextMeshPro>().fontSize *= 1f;
                node.GetComponentInChildren<TextMeshPro>().outlineColor = new Color32(207, 237, 255, 255);
                node.GetComponentInChildren<TextMeshPro>().color = new Color(1f, 1f, 1f);     
            }
        }
    }
}
