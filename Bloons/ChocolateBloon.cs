using System.Collections.Generic;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity.Display;

namespace XmasMod2025.Bloons;

public class ChocolateBloon : ModBloon
{
    public override string Icon => Name + "Icon";
    public override string BaseBloon => BloonType.sPurple;

    public override IEnumerable<string> DamageStates => [];

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth += 2;
        bloonModel.leakDamage += 5;
        bloonModel.RemoveAllChildren();
        bloonModel.speed += 0.1f;
        bloonModel.AddToChildren(BloonType.sGreen, 2);
        //bloonModel.icon = "";

        bloonModel.disallowCosmetics = true;
    }
}

public class ChocolateBloonDisplay : ModBloonDisplay<ChocolateBloon>
{
    public override string BaseDisplay => Generic2dDisplay;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        Set2DTexture(node, "ChocolateBloon");
    }
}