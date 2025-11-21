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
    public class CandyCaneBloon : ModBloon
    {
        public override string BaseBloon => BloonType.sRainbow;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth += 2;
            bloonModel.leakDamage += 5;
            bloonModel.RemoveAllChildren();
            bloonModel.speed -= 0.3f;
            //bloonModel.icon = "";

            bloonModel.disallowCosmetics = true;
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
}
