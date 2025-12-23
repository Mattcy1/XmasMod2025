using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity.Display;

namespace XmasMod2025.Bloons;

public class GiftBloon : ModBloon
{
    public override string Icon => Name + "Icon";
    public override string BaseBloon => BloonType.sRainbow;

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth += 5;
        bloonModel.leakDamage += 12;
        bloonModel.RemoveAllChildren();
        //bloonModel.icon = "";

        bloonModel.disallowCosmetics = true;
    }
}

public class GiftBloonDisplay : ModBloonDisplay<GiftBloon>
{
    public override string BaseDisplay => Generic2dDisplay;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        Set2DTexture(node, "GiftBloon");
    }
}