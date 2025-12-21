using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppAssets.Scripts.Unity.CollectionEvent.CollectionEventMapBonusSaveData;

namespace XmasMod2025.Bloons
{
    public class GiftMoab : ModBloon
    {
        public override string Icon => Name + "Icon";
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
}
