using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using XmasMod2025.BossAPI;

namespace BossAPI.Bosses;

public abstract class ModBoss : ModBloon
{
    static ModBoss()
    {
    }

    public abstract int SkullCount { get; }
    public abstract string HealthBar { get; }
    public virtual string IconGuid => GetTextureGUID(Icon);
    public virtual string PreviewIconGuid => GetTextureGUID(PreviewIcon);
    public abstract int Stars { get; }
    public abstract string CustomSkullIcon { get; }
    public abstract string HealthBarBackground { get; }
    public abstract string BossName { get; }
    public abstract int SpawnsRound { get; }
    public override string Icon => Name + "-Icon";
    public virtual string PreviewIcon => Icon;
    public override string Description => "Default description for Boss " + Id + ".";

    public override void Register()
    {
        base.Register();

        var bossBloonModel = Game.instance.model.GetBloon(Id);

        bossBloonModel.isBoss = true;
        bossBloonModel.tags = new[] { "Moabs", "Bad", "Boss", "Sandbox" };

        bossBloonModel.BecomeModdedBoss(BossName);
        var registeredBossId = bossBloonModel.GetBossID();

        if (SkullCount > 0)
        {
            var skullPercentageValues = Hooks.CalculateHealthTriggerPercentages(SkullCount);
            var skullTriggerModel = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<HealthPercentTriggerModel>()
                .Duplicate();
            skullTriggerModel.percentageValues = skullPercentageValues;
            skullTriggerModel.actionIds = new[] { "ModdedSkull" + bossBloonModel.GetBossID() };
            bossBloonModel.AddBehavior(skullTriggerModel);
        }

        var healthBarPercentageValues = Hooks.CalculateHealthTriggerPercentages(10);
        var healthBarTriggerModel = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<HealthPercentTriggerModel>()
            .Duplicate();
        healthBarTriggerModel.percentageValues = healthBarPercentageValues;
        healthBarTriggerModel.actionIds = new[] { "HealthBar" + bossBloonModel.GetBossID() };
        bossBloonModel.AddBehavior(healthBarTriggerModel);

        bossBloonModel.disallowCosmetics = true;
        bossBloonModel.AddInfo(SkullCount > 0, SkullCount, CustomSkullIcon, Stars, HealthBar, HealthBarBackground,
            BossName, Icon, registeredBossId, SpawnsRound, Id, Description, PreviewIcon);
    }

    public virtual void OnSpawn(Bloon bloon)
    {
    }
}