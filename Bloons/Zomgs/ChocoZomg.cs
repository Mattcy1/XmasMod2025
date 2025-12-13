using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;

namespace XmasMod2025.Bloons.Moabs;

public class ChocoZomg : ModBloon
{
    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth *= (int)1.5f;
        bloonModel.speed *= 1.15f;
        
        bloonModel.RemoveAllChildren();
        bloonModel.AddToChildren<ChocoBfb>(4);
    }

    public override string BaseBloon => BloonType.sZomg;

    public class ChocoZomgDisplay : ModBloonDisplay<ChocoZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.GetMeshRenderers())
            {
                renderer.SetMainTexture(GetTexture(Name));
                renderer.materials[2].SetColor("_OutlineColor", new Color32(26, 12, 7, 255));
            }
        }
    }
    public class ChocoZomgDamage1Display : ModBloonDisplay<ChocoZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

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
    public class ChocoZomgDamage2Display : ModBloonDisplay<ChocoZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

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
    public class ChocoZomgDamage3Display : ModBloonDisplay<ChocoZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

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
    public class ChocoZomgDamage4Display : ModBloonDisplay<ChocoZomg>
    {
        public override string BaseDisplay => GetBloonDisplay(BloonType.sZomg);

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