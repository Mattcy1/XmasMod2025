using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppAssets.Scripts.Unity.CollectionEvent.CollectionEventMapBonusSaveData;

namespace XmasMod2025.Bloons
{
    internal class SnowBloon : ModBloon
    {
        public override string BaseBloon => BloonType.sWhite;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth += 3;
            bloonModel.leakDamage += 3;
            bloonModel.layerNumber = 1;
            bloonModel.RemoveAllChildren();
            //bloonModel.icon = "";

            StunTowersInRadiusActionModel stun = Game.instance.model.GetBloon("Vortex1").GetBehavior<StunTowersInRadiusActionModel>().Duplicate();
            stun.radius = 35f;
            stun.stunDuration = 1.2f;
            stun.actionId = "SnowBloonStun";
            bloonModel.AddBehavior(stun);
        }
    }
}
