using System;
using BTD_Mod_Helper.Api.Bloons;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Attack;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;

namespace XmasMod2025.Bloons;

public class RoundStaller : ModBloon
{
    public static Bloon? AliveStaller;


    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        bloonModel.speed = 0;
        bloonModel.maxHealth = Int32.MaxValue;
    }

    public override string BaseBloon => BloonType.sRed;

    [HarmonyPatch(typeof(Attack), nameof(Attack.GetTargets))]
    private static class Attack_GetTargets
    {
        public static void Postfix(ref IEnumerable<Target> __result)
        {
            List<Target> targets = new List<Target>(__result);
            for (int i = __result.Count() - 1; i >= 0; i--)
            {
                Target t = __result.ElementAt(i);
                if (t.bloon.bloonModel.baseId == BloonID<RoundStaller>())
                {
                    targets.Remove(t);
                }
            }
            
            __result = targets.Cast<IEnumerable<Target>>();
        }
    }

    [HarmonyPatch(typeof(Bloon), nameof(Bloon.Initialise))]
    private static class Bloon_Initialize
    {
        public static void Postfix(Bloon __instance, Model modelToUse)
        {
            if(__instance.bloonModel.baseId == BloonID<RoundStaller>())
            {
                AliveStaller = __instance;
            }
        }
    }
    [HarmonyPatch(typeof(Bloon), nameof(Bloon.OnDestroy))]
    private static class Bloon_OnDestroy
    {
        public static void Postfix(Bloon __instance)
        {
            if(__instance.bloonModel.baseId == BloonID<RoundStaller>())
            {
                AliveStaller = __instance;
            }
        }
    }
}