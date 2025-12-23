using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity.Display;

namespace XmasMod2025.Bloons;

public class CandyCaneBloon : ModBloon
{
    public override string BaseBloon => BloonType.sRainbow;

    public override string Icon => Name + "Icon";

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth += 2;
        bloonModel.leakDamage += 5;
        bloonModel.RemoveAllChildren();
        bloonModel.speed -= 0.5f;
        bloonModel.AddToChildren(BloonID<MiniCandyCaneBloon>(), 2);

        bloonModel.disallowCosmetics = true;
        bloonModel.AddTag("Candy");
    }
}

public class MiniCandyCaneBloon : ModBloon
{
    public override string BaseBloon => BloonType.sWhite;

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth += 1;
        bloonModel.RemoveAllChildren();
        bloonModel.speed -= 0.3f;

        bloonModel.disallowCosmetics = true;
        bloonModel.dontShowInSandbox = true;
        bloonModel.AddTag("Candy");
    }
}

public class CandyCaneBloonDisplay : ModBloonDisplay<CandyCaneBloon>
{
    public override string BaseDisplay => Generic2dDisplay;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        Set2DTexture(node, "CandyCaneBloon");
    }
}

public class MiniCandyCaneBloonDisplay : ModBloonDisplay<MiniCandyCaneBloon>
{
    public override string BaseDisplay => Generic2dDisplay;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        Set2DTexture(node, "MiniCandyCaneBloon");
    }
}