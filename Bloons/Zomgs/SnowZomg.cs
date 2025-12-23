using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using UnityEngine;

namespace XmasMod2025.Bloons.Moabs;

public class SnowZomg : ModBloon
{
    public override string BaseBloon => BloonType.sZomg;

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.RemoveAllChildren();
        bloonModel.AddToChildren<SnowBfb>(4);
        bloonModel.danger -= 1;
        bloonModel.maxHealth /= (int)1.5f;
        bloonModel.speed *= 1.20f;
        bloonModel.isImmuneToSlow = true;
        bloonModel.bloonProperties = BloonProperties.White;
        bloonModel.AddBehavior(new CreateSoundOnDamageBloonModel("CreateSoundOnDamageBloonModel_Snow",
            new Il2CppReferenceArray<AudioClipReference>([
                GetAudioClipReference("SnowBloon_1"),
                GetAudioClipReference("SnowBloon_2"),
                GetAudioClipReference("SnowBloon_3")
            ])));
        bloonModel.AddTag("Zomg");
    }

    public class SnowZomgDisplay : ModBloonDisplay<SnowZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
            }
        }
    }

    public class SnowZomgDamage1Display : ModBloonDisplay<SnowZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg, 1);

        public override int Damage => 1;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
            }
        }
    }

    public class SnowZomgDamage2Display : ModBloonDisplay<SnowZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg, 2);

        public override int Damage => 2;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
            }
        }
    }

    public class SnowZomgDamage3Display : ModBloonDisplay<SnowZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg, 3);

        public override int Damage => 3;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
            }
        }
    }

    public class SnowZomgDamage4Display : ModBloonDisplay<SnowZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg, 4);

        public override int Damage => 4;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
            }
        }
    }
}