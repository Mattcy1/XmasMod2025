using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace XmasMod2025.Towers;

public class ElfMonkey : ChristmasTower
{
    public override string BaseTower => TowerType.DartMonkey;
    public override bool DontAddToShop => false;
    public override string Icon => VanillaSprites.MonkeyVillageElfPetIcon;
    public override string Portrait => Icon;
    public override string Description => "One of Santa's Minions, help you defend.";
    public override int Cost => 45;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        var proj = towerModel.GetWeapon().projectile;

        proj.GetDamageModel().damage = 1;
        proj.pierce = 2;
        proj.ApplyDisplay<Gift1>();
        proj.id = "ElfProj";
        proj.AddBehavior(new RandomDisplayModel("RandomDisplayModel_",
            new Il2CppReferenceArray<PrefabReference>([
                new PrefabReference(GetDisplayGUID<Gift1>()), new PrefabReference(GetDisplayGUID<Gift2>()),
                new PrefabReference(GetDisplayGUID<Gift3>()), new PrefabReference(GetDisplayGUID<Gift4>()),
                new PrefabReference(GetDisplayGUID<Gift5>()), new PrefabReference(GetDisplayGUID<Gift6>())
            ]), true));
    }
}

public class ElfMonkeyDisplay : ModTowerDisplay<ElfMonkey>
{
    public override string BaseDisplay => MonkeyVillageElfPet;

    public override bool UseForTower(params int[] tiers)
    {
        return tiers[1] < 3;
    }

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, "ElfGeo");
        node.GetMeshRenderer().ApplyOutlineShader();
    }
}