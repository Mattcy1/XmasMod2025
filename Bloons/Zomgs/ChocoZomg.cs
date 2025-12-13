using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;

namespace XmasMod2025.Bloons.Moabs;

public class ChocoBfb : ModBloon
{
    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth = 900;
        bloonModel.speed *= 1.25f;
        
        bloonModel.RemoveAllChildren();
        bloonModel.AddToChildren<ChocoMoab>(4);
    }

    public override string BaseBloon => BloonType.sBfb;

    public class ChocoMoabDisplay : ModBloonDisplay<ChocoBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(26, 12, 7, 255));
            }
        }
    }
    public class ChocoBfbDamage1Display : ModBloonDisplay<ChocoBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

        public override int Damage => 1;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(26, 12, 7, 255));
            }
        }
    }
    public class ChocoBfbDamage2Display : ModBloonDisplay<ChocoMoab>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

        public override int Damage => 2;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(26, 12, 7, 255));
            }
        }
    }
    public class ChocoBfbDamage3Display : ModBloonDisplay<ChocoBfb>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

        public override int Damage => 3;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(26, 12, 7, 255));
            }
        }
    }
    public class ChocoBfbDamage4Display : ModBloonDisplay<ChocoMoab>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sBfb);

        public override int Damage => 4;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(26, 12, 7, 255));
            }
        }
    }
}