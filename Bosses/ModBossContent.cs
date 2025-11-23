using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.BloonMenu;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Collections.Generic;
using XmasMod2025.BossAPI;
using static MelonLoader.MelonLogger;
using static XmasMod2025.BossAPI.BossAPI;
using static XmasMod2025.BossAPI.Hooks;

namespace XmasMod2025.ModBossContenet;

public abstract class ModBoss : ModBloon
{
    public abstract string BossID { get; }
    public abstract string BossName { get; }
    public abstract int SkullCount { get; }
    public abstract string HealthBar { get; }
    public abstract string IconGuid { get; }
    public abstract int Stars { get; }
    public abstract string CustomSkullIcon { get; }
    public abstract string HealthBarBackground { get; }
    public abstract int SpawnsRound { get; }
    static ModBoss()
    {
    }

    public override void Register()
    {
        base.Register();
        IconReference = GetSpriteReferenceOrDefault(mod, Icon);

        BloonModel bossBloonModel = GetBloon();
        bossBloonModel.BecomeModdedBoss(BossName);
        string registeredBossId = bossBloonModel.GetBossID();

        bossBloonModel.isBoss = true;
        bossBloonModel.tags = new string[] { "Moabs", "Bad", "Boss", "Sandbox" };

        bossBloonModel.icon = IconReference;

        if (SkullCount > 0)
        {
            float[] skullPercentageValues = CalculateHealthTriggerPercentages(SkullCount);
            HealthPercentTriggerModel skullTriggerModel = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<HealthPercentTriggerModel>().Duplicate();
            skullTriggerModel.percentageValues = skullPercentageValues;
            skullTriggerModel.actionIds = new string[] { "ModdedSkull" + BossID};
            bossBloonModel.AddBehavior(skullTriggerModel);
        }

        float[] healthBarPercentageValues = CalculateHealthTriggerPercentages(10);
        HealthPercentTriggerModel healthBarTriggerModel = Game.instance.model.GetBloon("Bloonarius1").GetBehavior<HealthPercentTriggerModel>().Duplicate();
        healthBarTriggerModel.percentageValues = healthBarPercentageValues;
        healthBarTriggerModel.actionIds = new string[] { "HealthBar" + BossID};
        bossBloonModel.AddBehavior(healthBarTriggerModel);

        bossBloonModel.disallowCosmetics = true;
        bossBloonModel.AddInfo(SkullCount > 0, SkullCount, CustomSkullIcon, Stars, HealthBar, HealthBarBackground, BossName, IconGuid, registeredBossId, SpawnsRound, BossID);
    }

    public abstract BloonModel GetBloon();
    public virtual string Icon => Name + "-Icon";
    protected virtual bool IconIsGUID => false;
    public SpriteReference IconReference { get; protected set; }
}

//Credit doombubbles
[HarmonyPatch(typeof(BloonMenu), nameof(BloonMenu.CreateBloonButtons))]
public class BloonMenu_CreateBloonButtons
{
    [HarmonyPrefix]
    public static void Prefix(Il2CppSystem.Collections.Generic.List<BloonModel> sortedBloons)
    {
        foreach (var bloon in InGame.Bridge.Model.bloons)
        {
            if (bloon.HasTag("Sandbox") && !sortedBloons.Contains(bloon))
            {
                sortedBloons.Add(bloon);
            }
        }
    }
}