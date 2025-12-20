using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using Il2CppSystem.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Moabs;

namespace XmasMod2025.Bloons.Bfbs
{
    public class IceBfb : ModBloon
    {
        public override string BaseBloon => BloonType.sBfb;
        public override string Icon => "IceBfb";
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth *= (int)1.5f;
            bloonModel.speed /= 2;
            bloonModel.bloonProperties = Il2Cpp.BloonProperties.White | Il2Cpp.BloonProperties.Frozen;

            bloonModel.RemoveAllChildren();
            bloonModel.AddToChildren<IceMoab>(4);

            StunTowersInRadiusActionModel stunTowersInRadiusActionModel = new("StunTowersInRadiusActionModel", "freeze", 50, 1, 1, CreatePrefabReference<IceCubeOverlay>(), true);

            HealthPercentTriggerModel healthPercentTriggerModel = new("HealthPercentTriggerModel", false, new([0.1f]), new(["freeze"]), false);

            bloonModel.AddBehavior(healthPercentTriggerModel);
            bloonModel.AddBehavior(stunTowersInRadiusActionModel);
            
            bloonModel.AddBehavior(new CreateSoundOnBloonDestroyedModel("CreateSoundOnBloonDestroyedModel_Ice", new SoundModel("IceShatter_1", GetAudioClipReference("IceShatter_1")), new SoundModel("IceShatter_2", GetAudioClipReference("IceShatter_2")), "IceMoab"));
        }
    }
    public class IceBfbDisplay : ModBloonDisplay<IceBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(80, 108, 133, 255));
            }
        }
    }

    public class IceBfbDamage1Display : ModBloonDisplay<IceBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

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

    public class IceBfbDamage2Display : ModBloonDisplay<IceBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

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

    public class IceBfbDamage3Display : ModBloonDisplay<IceBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

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

    public class IceBfbDamage4Display : ModBloonDisplay<IceBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

        public override int Damage => 4;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture("IceBfbDamage3Display"));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(80, 108, 133, 255));
            }
        }
    }
}
