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
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using static Il2CppAssets.Scripts.Unity.CollectionEvent.CollectionEventMapBonusSaveData;

namespace XmasMod2025.Bloons
{
    public class SnowBloon : ModBloon
    {
        public override string Icon => Name + "Icon";
        public override string BaseBloon => BloonType.sWhite;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth += 3;
            bloonModel.leakDamage += 3;
            bloonModel.RemoveAllChildren();
            //bloonModel.icon = "";

            StunTowersInRadiusActionModel stun = Game.instance.model.GetBloon("Vortex1").GetBehavior<StunTowersInRadiusActionModel>().Duplicate();
            stun.radius = 35f;
            stun.stunDuration = 1.2f;
            stun.actionId = "SnowBloonStun";
            bloonModel.AddBehavior(stun);

            bloonModel.disallowCosmetics = true;
            
            bloonModel.AddBehavior(new CreateSoundOnDamageBloonModel("CreateSoundOnDamageBloonModel_Snow", new Il2CppReferenceArray<AudioClipReference>([
                GetAudioClipReference("SnowBloon_1"),
                GetAudioClipReference("SnowBloon_2"),
                GetAudioClipReference("SnowBloon_3")
            ])));
        }
    }

    public class SnowBloonDisplay : ModBloonDisplay<SnowBloon>
    {
        public override string BaseDisplay => Generic2dDisplay;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "SnowBloon");
        }
    }
}
