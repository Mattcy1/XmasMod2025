using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity.Display;

namespace XmasMod2025.Bloons;

public class GiftMoab : ModBloon
{
    public override string Icon => "GiftMoabIcon";
    public override string BaseBloon => BloonType.sMoab;

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.maxHealth = 15;
        bloonModel.leakDamage = 12;
        bloonModel.RemoveAllChildren();
        //bloonModel.icon = "";

        bloonModel.disallowCosmetics = true;
        bloonModel.AddTag("Moab");
    }
}

public class GiftMoabDisplay : ModBloonDisplay<GiftMoab>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab);

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, Name);
    }
}

public class GiftMoabDamage1Display : ModBloonDisplay<GiftMoab>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab);

    public override int Damage => 1;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, Name);
    }
}

public class GiftMoabDamage2Display : ModBloonDisplay<GiftMoab>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab);

    public override int Damage => 2;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, Name);
    }
}

public class GiftMoabDamage3Display : ModBloonDisplay<GiftMoab>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab);

    public override int Damage => 3;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, Name);
    }
}

public class GiftMoabDamage4Display : ModBloonDisplay<GiftMoab>
{
    public override string BaseDisplay => GetBloonDisplay(BloonType.sMoab);

    public override int Damage => 4;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, Name);
    }
}