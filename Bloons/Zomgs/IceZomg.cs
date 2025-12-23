using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using XmasMod2025.Bloons.Bfbs;
using XmasMod2025.Bloons.Moabs;

namespace XmasMod2025.Bloons.Zomgs;

public class IceZomg : ModBloon
{
    public override string BaseBloon => BloonType.sZomg;

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth *= (int)1.5;
        bloonModel.speed /= 2;
        bloonModel.bloonProperties = BloonProperties.White | BloonProperties.Frozen;

        bloonModel.RemoveAllChildren();
        bloonModel.AddTag("Zomg");
        bloonModel.AddToChildren<IceBfb>(4);

        StunTowersInRadiusActionModel stunTowersInRadiusActionModel = new("StunTowersInRadiusActionModel", "freeze", 50,
            1, 1, CreatePrefabReference<IceCubeOverlay>(), true);

        HealthPercentTriggerModel healthPercentTriggerModel = new("HealthPercentTriggerModel", false,
            new Il2CppStructArray<float>([0.1f]), new Il2CppStringArray(["freeze"]), false);

        bloonModel.AddBehavior(healthPercentTriggerModel);
        bloonModel.AddBehavior(stunTowersInRadiusActionModel);

        bloonModel.AddBehavior(new CreateSoundOnBloonDestroyedModel("CreateSoundOnBloonDestroyedModel_Ice",
            new SoundModel("IceShatter_1", GetAudioClipReference("IceShatter_1")),
            new SoundModel("IceShatter_2", GetAudioClipReference("IceShatter_2")), "IceMoab"));
    }
}

public class IceZomgDisplay : ModBloonDisplay<IceZomg>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.GetMeshRenderers())
        {
            renderer.SetMainTexture(GetTexture(Name));
            renderer.materials[2].SetColor("_OutlineColor", new Color32(80, 108, 133, 255));
        }
    }
}

public class IceZomgDamage1Display : ModBloonDisplay<IceZomg>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

    public override int Damage => 1;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.GetMeshRenderers())
        {
            renderer.SetMainTexture(GetTexture(Name));
            renderer.materials[2].SetColor("_OutlineColor", new Color32(80, 108, 133, 255));
        }
    }
}

public class IceZomgDamage2Display : ModBloonDisplay<IceZomg>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

    public override int Damage => 2;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.GetMeshRenderers())
        {
            renderer.SetMainTexture(GetTexture(Name));
            renderer.materials[2].SetColor("_OutlineColor", new Color32(80, 108, 133, 255));
        }
    }
}

public class IceZomgDamage3Display : ModBloonDisplay<IceZomg>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

    public override int Damage => 3;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.GetMeshRenderers())
        {
            renderer.SetMainTexture(GetTexture(Name));
            renderer.materials[2].SetColor("_OutlineColor", new Color32(80, 108, 133, 255));
        }
    }
}

public class IceZomgDamage4Display : ModBloonDisplay<IceZomg>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

    public override int Damage => 4;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var renderer in node.GetMeshRenderers())
        {
            renderer.SetMainTexture(GetTexture(Name));
            renderer.materials[2].SetColor("_OutlineColor", new Color32(80, 108, 133, 255));
        }
    }
}