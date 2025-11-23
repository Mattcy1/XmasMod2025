using XmasMod2025.BossAPI;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;

namespace BossAPI.Bosses;

public abstract class ModBoss : ModBloon
{
    public abstract int SkullCount { get; }
    public abstract string HealthBar { get; }
    public abstract string IconGuid { get; }
    public abstract int Stars { get; }
    public abstract string CustomSkullIcon { get; }
    public abstract string HealthBarBackground { get; }
    public abstract string BossName { get; }
    public abstract int SpawnsRound { get; }
    static ModBoss()
    {
    }

    public override void Register()
    {
        base.Register();
        IconReference = GetSpriteReferenceOrDefault(mod, Icon);

        BloonModel bossBloonModel = Game.instance.model.GetBloon(Id);

        bossBloonModel.isBoss = true;
        bossBloonModel.tags = new string[] { "Moabs", "Bad", "Boss", "Sandbox" };

        bossBloonModel.BecomeModdedBoss(BossName);
        string registeredBossId = bossBloonModel.GetBossID();

        bossBloonModel.icon = IconReference;

        if (SkullCount > 0)
        {
            float[] skullPercentageValues = Hooks.CalculateHealthTriggerPercentages(SkullCount);
            HealthPercentTriggerModel skullTriggerModel = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<HealthPercentTriggerModel>().Duplicate();
            skullTriggerModel.percentageValues = skullPercentageValues;
            skullTriggerModel.actionIds = new string[] { "ModdedSkull" + bossBloonModel.GetBossID()};
            bossBloonModel.AddBehavior(skullTriggerModel);
        }

        float[] healthBarPercentageValues = Hooks.CalculateHealthTriggerPercentages(10);
        HealthPercentTriggerModel healthBarTriggerModel = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<HealthPercentTriggerModel>().Duplicate();
        healthBarTriggerModel.percentageValues = healthBarPercentageValues;
        healthBarTriggerModel.actionIds = new string[] { "HealthBar" + bossBloonModel.GetBossID()};
        bossBloonModel.AddBehavior(healthBarTriggerModel);

        bossBloonModel.disallowCosmetics = true;
        bossBloonModel.AddInfo(SkullCount > 0, SkullCount, CustomSkullIcon, Stars, HealthBar, HealthBarBackground, BossName, IconGuid, registeredBossId, SpawnsRound, Id, Description);
    }
    public virtual void OnSpawn(Bloon bloon) { }
    public virtual string Icon => Name + "-Icon";
    protected virtual bool IconIsGUID => false;
    public override string Description => "Default description for Boss " + Id + ".";
    public SpriteReference IconReference { get; protected set; }
}