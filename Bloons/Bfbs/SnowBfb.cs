using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using XmasMod2025.Bloons;
using UnityEngine;

namespace XmasMod2025.Bloons.Moabs
{
    public class SnowBfb : ModBloon
    {
        public override string BaseBloon => BloonType.sBfb;
        public override string Icon => "SnowBfb";
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.RemoveAllChildren();
            bloonModel.AddToChildren<SnowMoab>(4);
            bloonModel.danger -= 1;
            bloonModel.maxHealth /= 2;
            bloonModel.speed *= 1.25f;
            bloonModel.isImmuneToSlow = true;
            bloonModel.bloonProperties = Il2Cpp.BloonProperties.White;
            bloonModel.AddBehavior(new CreateSoundOnDamageBloonModel("CreateSoundOnDamageBloonModel_Snow", new Il2CppReferenceArray<AudioClipReference>([
                GetAudioClipReference("SnowBloon_1"),
                GetAudioClipReference("SnowBloon_2"),
                GetAudioClipReference("SnowBloon_3")
            ])));
        }

        public class SnowBfbDisplay : ModBloonDisplay<SnowBfb>
        {
            public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach(var renderer in node.GetMeshRenderers())
                {
                    renderer.SetMainTexture(GetTexture(Name));
                    renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
                }
            }
        }

        public class SnowBfbDamage1Display : ModBloonDisplay<SnowBfb>
        {
            public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

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

        public class SnowBfbDamage2Display : ModBloonDisplay<SnowBfb>
        {
            public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

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

        public class SnowBfbDamage3Display : ModBloonDisplay<SnowBfb>
        {
            public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

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

        public class SnowBfbDamage4Display : ModBloonDisplay<SnowBfb>
        {
            public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

            public override int Damage => 4;

            public override void ModifyDisplayNode(UnityDisplayNode node)
            {
                foreach (var renderer in node.GetMeshRenderers())
                {
                    renderer.SetMainTexture(GetTexture("SnowBfbDamage3Display"));
                    renderer.materials[2].SetColor("_OutlineColor", new Color32(161, 187, 214, 255));
                }
            }
        }
    }
}
