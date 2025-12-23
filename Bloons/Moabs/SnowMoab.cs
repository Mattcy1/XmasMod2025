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

public class SnowMoab : ModBloon
{
    public override string BaseBloon => BloonType.sMoab;

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.RemoveAllChildren();
        bloonModel.AddToChildren<SnowBloon>(5);
        bloonModel.danger -= 1;
        bloonModel.maxHealth = 100;
        bloonModel.speed *= 1.25f;
        bloonModel.isImmuneToSlow = true;
        bloonModel.bloonProperties = BloonProperties.White;
        bloonModel.AddBehavior(new CreateSoundOnDamageBloonModel("CreateSoundOnDamageBloonModel_Snow",
            new Il2CppReferenceArray<AudioClipReference>([
                GetAudioClipReference("SnowBloon_1"),
                GetAudioClipReference("SnowBloon_2"),
                GetAudioClipReference("SnowBloon_3")
            ])));

        bloonModel.AddTag("Moab");
    }

    public class WeakSnowMoab : ModBloon<SnowMoab>
    {
        protected override int Order => 1;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.id = GetId<XmasMod2025>(Name);
            bloonModel.dontShowInSandbox = true;
            bloonModel.danger -= 1;
            bloonModel.RemoveAllChildren();
            bloonModel.AddToChildren<SnowBloon>(3);
            bloonModel.maxHealth = 15;
        }
    }

    public class SnowMoabDisplay : ModBloonDisplay<SnowMoab>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
            }
        }
    }

    public class SnowMoabDamage1Display : ModBloonDisplay<SnowMoab>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab, 1);

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

    public class SnowMoabDamage2Display : ModBloonDisplay<SnowMoab>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab, 2);

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

    public class SnowMoabDamage3Display : ModBloonDisplay<SnowMoab>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab, 3);

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

    public class SnowMoabDamage4Display : ModBloonDisplay<SnowMoab>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab, 4);

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