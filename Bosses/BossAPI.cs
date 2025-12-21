using System;
using BossAPI.Bosses;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Hooks;
using BTD_Mod_Helper.Api.Hooks.BloonHooks;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Legends;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity.Scenes;
using Il2CppAssets.Scripts.Unity.UI_New.Achievements;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.BloonMenu;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using UnityEngine;
using XmasMod2025.Assets;
using XmasMod2025.Bosses;
using XmasMod2025.UI;
using static XmasMod2025.BossAPI.BossAPI;
using Info = BTD_Mod_Helper.Api.Components.Info;
using Il2CppAssets.Scripts.Unity;
using XmasMod2025.Bloons.Moabs;

namespace XmasMod2025.BossAPI;
public class BossAPI
{
    //Store the skulls
    public static List<ModHelperImage> skullUIs = new List<ModHelperImage>();

    //Creates the infos
    public class BossInfo
    {
        public Bloon Boss { get; set; }
        public bool HasSkull { get; set; }
        public int SkullsCount { get; set; }
        public string SkullIcon { get; set; }
        public int StarsCount { get; set; }
        public string HealthBarIcon { get; set; }
        public string HealthBarBackground { get; set; }
        public string DisplayName { get; set; }
        public string BossIcon { get; set; }
        
        public string BossPreviewIcon { get; set; }
        public string BossID { get; set; }
        public int SpawnRound { get; set; }
        public string BossToSpawn { get; set; }

        public string Description { get; set; }
    }

    public static List<BossInfo> BossInfos = new List<BossInfo>();

    public static Dictionary<int, BossInfo> roundsSpawn = new Dictionary<int, BossInfo>();

    public static int RoundsUntilNextBoss = 0;

    public static int NearestBoss = 0;

    public static int elaspedRound = 0;

    public static BossInfo diedTo = null;
}
public class Hooks
{
    //Updates Health Bar
    [HookTarget(typeof(BloonDamageHook), HookTargetAttribute.EHookType.Postfix)]
    [HookPriority(HookPriorityAttribute.Higher)]
    public static bool BloonDamagePostfix(Bloon @this, ref float totalAmount, Projectile projectile, ref bool distributeToChildren, ref bool overrideDistributeBlocker, ref bool createEffect, Tower tower, BloonProperties immuneBloonProperties, BloonProperties originalImmuneBloonProperties, ref bool canDestroyProjectile, ref bool ignoreNonTargetable, ref bool blockSpawnChildren, HookNullable<int> powerActivatedByPlayerId)
    {
        foreach (var bossInfo in BossAPI.BossInfos)
        {
            if (XmasMod2025.KrampusAlive)
            {
                if (@this.bloonModel.baseId == ModContent.BloonID<KrampusBoss>())
                {
                    if(BossUI.instance == null)
                    {
                        BossUI.CreateBossBar(@this.GetInfo());
                    }

                    BossUI.HandleUI(@this);
                }
            }
            else
            {
                if (@this.bloonModel.IsModdedBoss())
                {
                    if (BossUI.instance == null)
                    {
                        BossUI.CreateBossBar(@this.GetInfo());
                    }

                    BossUI.HandleUI(@this);
                }
            }
        }

        return true;
    }
    public static T StartMonobehavior<T>() where T : MonoBehaviour
    {
        var obj = InGame.instance.GetInGameUI().AddComponent<T>();

        return obj as T;
    }

    //Handle spawn
    [HarmonyPatch(typeof(InGame), nameof(InGame.RoundStart))]
    public class MakeBossSpawn
    {
        [HarmonyPostfix]

        public static void Postfix(InGame __instance)
        {
            foreach (var bossInfo in BossAPI.BossInfos)
            {
                var r = __instance.bridge.GetCurrentRound();

                var upcoming = BossAPI.BossInfos
                    .Where(b => b.SpawnRound > r)
                    .Select(b => b.SpawnRound);

                if (!upcoming.Any())
                    return;

                int nearestBoss = upcoming.Min();

                BossAPI.RoundsUntilNextBoss = nearestBoss - r - 1;

                RoundBossUI.UpdateRoundsUI();
            }
        }
    }

    [HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.Start))]
    public class EditRs
    {
        [HarmonyPostfix]

        public static void Postfix(InGame __instance)
        {
            foreach (RoundSetModel roundSet in GameData.Instance.roundSets)
            {
                try
                {
                    foreach (var bossInfo in BossAPI.BossInfos)
                    {
                        var r = bossInfo.SpawnRound;
                        roundSet.rounds[r - 1].ClearBloonGroups();
                        roundSet.rounds[r - 1].AddBloonGroup(bossInfo.BossToSpawn, 1);
                    }
                }
                catch
                {

                }
            }
        }
    }

    //Create Round UI
    [HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
    public class RoundUIHandler
    {
        [HarmonyPostfix]

        public static void Postfix(InGame __instance)
        {
            foreach (var bossInfo in BossAPI.BossInfos)
            {
                if (!BossAPI.roundsSpawn.ContainsKey(bossInfo.SpawnRound))
                {
                    BossAPI.roundsSpawn.Add(bossInfo.SpawnRound, bossInfo);
                }
            }

            if (BossAPI.roundsSpawn.Count == 0)
            {
                MelonLogger.Warning("RoundSpawn dictionary is empty!");
                return;
            }

            int lowestRound = BossAPI.roundsSpawn.Keys.Min();
            BossInfo FirstBossToSpawn = BossAPI.roundsSpawn[lowestRound];

            if (RoundBossUI.instance != null)
                RoundBossUI.instance.Close();

            elaspedRound = __instance.bridge.GetCurrentRound();

            RoundBossUI.CreateRoundsUI(FirstBossToSpawn);

            NearestBoss = lowestRound - elaspedRound;
        }
    }

    //Create Round UI
    [HarmonyPatch(typeof(InGame), nameof(InGame.Restart))]
    public class Restart
    {
        [HarmonyPostfix]

        public static void Postfix(InGame __instance)
        {
            foreach (var bossInfo in BossAPI.BossInfos)
            {
                if (!BossAPI.roundsSpawn.ContainsKey(bossInfo.SpawnRound))
                {
                    BossAPI.roundsSpawn.Add(bossInfo.SpawnRound, bossInfo);
                }
            }

            if (BossAPI.roundsSpawn.Count == 0)
            {
                MelonLogger.Warning("RoundSpawn dictionary is empty!");
                return;
            }

            int lowestRound = BossAPI.roundsSpawn.Keys.Min();
            BossInfo FirstBossToSpawn = BossAPI.roundsSpawn[lowestRound];

            if (RoundBossUI.instance != null)
                RoundBossUI.instance.Close();

            elaspedRound = __instance.bridge.GetCurrentRound();

            RoundBossUI.CreateRoundsUI(FirstBossToSpawn);

            NearestBoss = lowestRound - elaspedRound;
        }
    }



    //Runs on boss spawn function and create the ui
    [HarmonyPatch(typeof(Bloon), nameof(Bloon.OnSpawn))]
    public class HandleBossSpawn_Pacth
    {
        [HarmonyPostfix]

        public static void Postfix(Bloon __instance)
        {
            foreach (var bossInfo in BossAPI.BossInfos)
            {
                if (__instance.bloonModel.IsModdedBoss() && bossInfo.BossToSpawn == __instance.bloonModel.baseId)
                {
                    bossInfo.Boss = __instance;

                    if(XmasMod2025.KrampusAlive && __instance.bloonModel.baseId == ModContent.BloonID<KrampusBoss>())
                    {
                        if (BossUI.instance != null)
                        {
                            BossUI.instance.Close();
                        }

                        BossUI.CreateBossBar(bossInfo);
                    }
                    else if(!XmasMod2025.KrampusAlive)
                    {
                        if (BossUI.instance != null)
                        {
                            BossUI.instance.Close();
                        }

                        BossUI.CreateBossBar(bossInfo);
                    }



                    if (RoundBossUI.instance != null)
                        RoundBossUI.instance.Close();

                    break;
                }
            }

            foreach (var boss in ModContent.GetContent<ModBoss>())
            {
                if (boss.Id == __instance.bloonModel.baseId)
                {
                    boss.OnSpawn(__instance);
                }
            }
        }
    }

    //Credit doombubbles add bosses to sandbox
    [HarmonyPatch(typeof(BloonMenu), nameof(BloonMenu.CreateBloonButtons))]
    public class BloonMenu_CreateBloonButtons
    {
        [HarmonyPrefix]
        public static void Prefix(Il2CppSystem.Collections.Generic.List<BloonModel> sortedBloons)
        {
            foreach (var bloon in InGame.Bridge.Model.bloons)
            {
                if (bloon.HasTag("Sandbox") && !sortedBloons.Contains(bloon) && bloon.legendsType == LegendsType.None)
                {
                    sortedBloons.Add(bloon);
                }
            }
        }
    }

    //Destroy ui on popped
    [HarmonyPatch(typeof(Bloon), nameof(Bloon.OnDestroy))]
    public class HandlePopped
    {
        [HarmonyPostfix]

        public static void Postfix(Bloon __instance)
        {
            foreach (var bossInfo in BossAPI.BossInfos)
            {
                if (__instance.bloonModel.IsModdedBoss())
                {
                    bossInfo.Boss = __instance;
                    XmasMod2025.AddCurrency(CurrencyType.Gift, 500);


                    if(XmasMod2025.KrampusAlive && __instance.bloonModel.baseId == ModContent.BloonID<KrampusBoss>())
                    {
                        if (BossUI.instance != null)
                        {
                            BossUI.instance.Close();
                        }


                        Game.instance.audioFactory.StopMusic();
                        Game.instance.audioFactory.musicFactory.PlayNewTrack(false);
                    }
                    else if(!XmasMod2025.KrampusAlive)
                    {
                        if (BossUI.instance != null)
                        {
                            BossUI.instance.Close();
                        }
                    }

                    if (BossAPI.roundsSpawn.Count > 0)
                    {
                        int lowestRound = BossAPI.roundsSpawn.Keys.Min();

                        if (RoundBossUI.instance != null)
                            RoundBossUI.instance.Close();

                        BossAPI.roundsSpawn.Remove(lowestRound);

                        if (BossAPI.roundsSpawn.Count > 0)
                        {
                            int nextLowest = BossAPI.roundsSpawn.Keys.Min();

                            BossInfo nextBoss = BossAPI.roundsSpawn[nextLowest];
                            RoundBossUI.CreateRoundsUI(nextBoss);

                            NearestBoss = nextLowest;
                        }
                    }

                    break;
                }
            }
        }
    }


    //Destroy the ui on leak
    [HarmonyPatch(typeof(Bloon), nameof(Bloon.Leaked))]
    public class HandeBossLeaks_Pacth
    {
        [HarmonyPostfix]

        public static void Postfix(Bloon __instance)
        {
            foreach (var bossInfo in BossAPI.BossInfos)
            {
                foreach (var bloon in InGame.instance.GetBloons())
                {
                    if (BossUI.instance != null)
                    {
                        BossUI.instance.Close();
                    }

                    if (XmasMod2025.KrampusAlive && __instance.bloonModel.baseId == ModContent.BloonID<KrampusBoss>())
                    {
                        diedTo = __instance.GetInfo();
                        break;
                    }
                    else if(!XmasMod2025.KrampusAlive && bloon.bloonModel.IsModdedBoss())
                    {
                        diedTo = bossInfo;
                        break;
                    }
                }
            }
        }
    }

    //Continue
    [HarmonyPatch(typeof(InGame), nameof(InGame.Continue))]
    public class HandleContinue
    {
        [HarmonyPostfix]

        public static void Postfix(Bloon __instance)
        {
            if (diedTo != null)
            {
                InGame.instance.SpawnBloons(diedTo.BossToSpawn, 1, 0);
                diedTo = null;
            }
        }
    }

    //Handles Skull UI and HealthBar
    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Trigger))]
    public class HandleSkull_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(HealthPercentTrigger __instance)
        {
            if (__instance.modl.actionIds.Contains("ModdedSkullModdedBossCoal Boss") || __instance.modl.actionIds.Contains("ModdedSkullModdedBossKrampus"))
            {
                InGame.instance.SpawnBloons(ModContent.BloonID<CoalTotem>(), 1, 0);
            }

            if (__instance.modl.actionIds.Contains("ModdedSkullModdedBossChoco Boss") || __instance.modl.actionIds.Contains("ModdedSkullModdedBossKrampus"))
            {
                InGame.instance.SpawnBloons(ModContent.BloonID<ChocoBfb>(), 2, 0);
            }

            if (BossAPI.skullUIs.Count > 0 && __instance.bloon.bloonModel.IsModdedBoss() && __instance.modl.actionIds.Contains("ModdedSkull" + __instance.bloon.bloonModel.GetBossID()))
            {
                if(XmasMod2025.KrampusAlive && __instance.bloon.bloonModel.baseId == ModContent.BloonID<KrampusBoss>())
                {
                    int lastIndex = BossAPI.skullUIs.Count - 1;
                    MelonCoroutines.Start(BossUI.HandeSkull(BossAPI.skullUIs[lastIndex], __instance.bloon.GetInfo().SkullIcon));
                }
                else if(!XmasMod2025.KrampusAlive)
                {
                    int lastIndex = BossAPI.skullUIs.Count - 1;
                    MelonCoroutines.Start(BossUI.HandeSkull(BossAPI.skullUIs[lastIndex], __instance.bloon.GetInfo().SkullIcon));
                }
            }

            if (BossUI.bossPanelInside != null && __instance.modl.actionIds.Contains("HealthBar" + __instance.bloon.bloonModel.GetBossID()))
            {
                Bloon boss = __instance.bloon;

                foreach (var bossInfo in BossAPI.BossInfos)
                {
                    if (bossInfo.BossID == __instance.bloon.bloonModel.GetBossID())
                    {
                        if (XmasMod2025.KrampusAlive && __instance.bloon.bloonModel.baseId == ModContent.BloonID<KrampusBoss>())
                        {
                            if (string.IsNullOrEmpty(bossInfo.HealthBarIcon))
                            {
                                if (boss.health <= boss.bloonModel.maxHealth * 0.9f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("90");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.8f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("80");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.7f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("70");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.6f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("60");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.5f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("50");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.4f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("40");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.3f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("30");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.2f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("20");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.1f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("10");

                            }
                            else
                            {
                                if (boss.health <= boss.bloonModel.maxHealth * 0.9f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "90");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.8f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "80");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.7f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "70");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.6f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "60");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.5f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "50");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.4f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "40");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.3f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "30");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.2f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "20");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.1f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "10");
                            }
                        }
                        else if (!XmasMod2025.KrampusAlive)
                        {
                            if (string.IsNullOrEmpty(bossInfo.HealthBarIcon))
                            {
                                if (boss.health <= boss.bloonModel.maxHealth * 0.9f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("90");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.8f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("80");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.7f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("70");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.6f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("60");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.5f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("50");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.4f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("40");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.3f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("30");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.2f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("20");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.1f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>("10");

                            }
                            else
                            {
                                if (boss.health <= boss.bloonModel.maxHealth * 0.9f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "90");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.8f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "80");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.7f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "70");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.6f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "60");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.5f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "50");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.4f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "40");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.3f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "30");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.2f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "20");
                                if (boss.health <= boss.bloonModel.maxHealth * 0.1f)
                                    BossUI.bossPanelInside.Image.sprite = ModContent.GetSprite<XmasMod2025>(bossInfo.HealthBarIcon + "10");
                            }

                            break;
                        }
                    }
                }
            }
        }
    }

    public static float[] CalculateHealthTriggerPercentages(int skullCount)
    {
        float[] percentageValues = new float[skullCount];

        for (int i = 0; i < skullCount; i++)
        {
            percentageValues[i] = 1f - ((i + 1f) / (skullCount + 1f));
        }

        return percentageValues;
    }
}

public static class Ext
{
    public static void BecomeModdedBoss(this BloonModel bloonModel, string ID)
    {
        bloonModel.AddTag("ModdedBoss" + ID);
    }

    public static bool IsModdedBoss(this BloonModel bloonModel)
    {
        string bossID = bloonModel.GetBossID();
        if (string.IsNullOrEmpty(bossID)) return false;

        return bloonModel.tags.Any(tag => tag.ContainsIgnoreCase("ModdedBoss") && tag.ContainsIgnoreCase(bossID));
    }


    public static string GetBossID(this BloonModel bloonModel)
    {
        foreach (var tag in bloonModel.tags)
        {
            if (tag.Contains("ModdedBoss"))
            {
                return tag;
            }
        }

        return "Null";
    }


    public static bool AddInfo(this BloonModel bloon, bool hasSkull, int skullsCount, string customSkullIcon, int starsCount, string healthBarIcon, string healthBarBackground, string displayName, string bossIcon, string bossID, int round, string ToSpawn, string description, string previewIcon = null)
    {
        BossAPI.BossInfos.Add(new BossAPI.BossInfo
        {
            Boss = null,
            HasSkull = hasSkull,
            SkullsCount = skullsCount,
            SkullIcon = customSkullIcon,
            StarsCount = starsCount,
            HealthBarIcon = healthBarIcon,
            HealthBarBackground = healthBarBackground,
            DisplayName = displayName,
            BossIcon = bossIcon,
            BossID = bossID,
            SpawnRound = round,
            BossToSpawn = ToSpawn,
            Description = description,
            BossPreviewIcon = string.IsNullOrEmpty(previewIcon) ? bossIcon : previewIcon,
        });


        return true;
    }

    public static BossAPI.BossInfo GetInfo(this Bloon bloon)
    {
        string bossID = bloon.bloonModel.GetBossID();
        if (string.IsNullOrEmpty(bossID)) return null;

        return BossAPI.BossInfos.FirstOrDefault(info => info.BossID == bossID);
    }
}

[RegisterTypeInIl2Cpp(false)]
public class RoundBossUI : MonoBehaviour
{
    public static RoundBossUI? instance;
    public static ModHelperText roundText;
    public static ModHelperButton hintButton;

    public static ModHelperPanel descriptionPanel;
    public static ModHelperText descriptionText;
    
    public void Close()
    {
        if (gameObject)
        {
            gameObject.Destroy();
        }
    }

    public static void CreateRoundsUI(BossAPI.BossInfo bossInfo)
    {
        if (InGame.instance != null)
        {

            RectTransform rect = InGame.instance.uiRect;
            var panel = rect.gameObject.AddModHelperPanel(new("Panel_", 0, 1125, 0, 0), VanillaSprites.MainBGPanelBlue);

            var fakePanel = panel.AddPanel(new Info("PanelInside_", 0, 0, 1000, 200), "");
            fakePanel.Background.SetSprite(AssetHelper.GetSprite("ChristmasPanel"));

            var panelInside = panel.AddPanel(new Info("PanelInside_", 0, 0, 950, 150), "");
            panelInside.Background.SetSprite(AssetHelper.GetSprite("ChristmasInsertPanel"));

            roundText = panelInside.AddText(new Info("Text_", 100, 0, 800, 125), "Boss Appears In: " + bossInfo.SpawnRound + " Rounds");
            roundText.EnableAutoSizing();

            var leftBg = panel.AddImage(new Info("LeftBackground", -400, 0f, 300, 300), ModContent.GetTextureGUID<XmasMod2025>("IconHolder"));
            var icon = leftBg.AddImage(new Info("leftIcon", 0, 0f, 200, 200), ModContent.GetTextureGUID<XmasMod2025>(bossInfo.BossPreviewIcon));
            RoundsUntilNextBoss = bossInfo.SpawnRound;

            var desc1 = panel.AddPanel(new Info("DescriptionHolder", 0, -400, 1000, 500), "");
            desc1.Background.SetSprite(AssetHelper.GetSprite("ChristmasPanel"));
            var desc2 = panel.AddPanel(new Info("DescriptionInside", 0, -400, 950, 450), "");
            desc2.Background.SetSprite(AssetHelper.GetSprite("ChristmasInsertPanel"));
            var desc3 = panel.AddText(new Info("Description", 0, -400, 900, 400), bossInfo.Description, 60, TextAlignmentOptions.Center);
            desc3.EnableAutoSizing();

            desc1.Hide();
            desc2.Hide();
            desc3.Hide();

            panel.AddButton(new Info("DescriptionButton", 575, 0, 120), VanillaSprites.InfoBtn2, new System.Action(() =>
            {
                if (desc1.transform.localScale == Vector3.zero)
                {
                    desc1.Show();
                    desc2.Show();
                    desc3.Show();
                }
                else
                {
                    desc1.Hide();
                    desc2.Hide();
                    desc3.Hide();
                }
            }));

            instance = panel.AddComponent<RoundBossUI>();
        }
    }

    public static void UpdateRoundsUI()
    {
        if (roundText != null)
        {
            roundText.SetText("Boss Appears In: " + RoundsUntilNextBoss + " Rounds");
        }
    }
}

[RegisterTypeInIl2Cpp(false)]
public class BossUI : MonoBehaviour
{
    public static BossUI? instance;
    public static ModHelperPanel? bossPanel;
    public static ModHelperImage? bossPanelInside;
    public static ModHelperImage? bossLeftBackground;
    public static ModHelperImage? bossStars;
    public static ModHelperText? bossHealth;
    public static ModHelperImage? bossIcon;
    public static ModHelperText? bossName;
    public static ModHelperImage? bossHealthIcon;
    public void Close()
    {
        if (gameObject)
        {
            gameObject.Destroy();
        }
    }

    public static void CreateBossBar(BossAPI.BossInfo bossInfo)
    {
        if (InGame.instance != null)
        {
            RectTransform rect = InGame.instance.uiRect;
            if (bossInfo.HealthBarBackground == "")
            {
                bossInfo.HealthBarBackground = "HealthbarBG";
            }
            bossPanel = rect.gameObject.AddModHelperPanel(new("Panel_", 0, 1150, 1250, 100), ModContent.GetTextureGUID<XmasMod2025>(bossInfo.HealthBarBackground));
            instance = bossPanel.AddComponent<BossUI>();
            bossLeftBackground = bossPanel.AddImage(new Info("LeftBackground", -725, 0f, 250, 250), ModContent.GetTextureGUID<XmasMod2025>("IconHolder"));
            bossStars = bossPanel.AddImage(new Info("stars", -400, 100, 400, 80), ModContent.GetTextureGUID<XmasMod2025>($"Tier{bossInfo.StarsCount}Boss"));
            bossHealth = bossPanel.AddText(new Info("HealthText_", 450, 80, 500, 250), bossInfo.Boss.health + "/" + bossInfo.Boss.bloonModel.maxHealth, 50);
            bossIcon = bossLeftBackground.AddImage(new Info("leftIcon", 0, 0f, 250, 250), ModContent.GetTextureGUID<XmasMod2025>(bossInfo.BossIcon));
            bossName = bossPanel.AddText(new Info("NameText_", 0, 80, 750, 250), bossInfo.DisplayName, 50);

            string textureName = "100";
            if (!string.IsNullOrEmpty(bossInfo.HealthBarIcon))
            {
                textureName = bossInfo.HealthBarIcon + "100";
            }

            bossPanelInside = bossPanel.AddImage(new Info("PanelInside", 0, 0, 1250, 90), ModContent.GetTextureGUID<XmasMod2025>(textureName));

            HandleUI(bossInfo.Boss);

            if (bossInfo.HasSkull)
            {
                CreateSkulls(bossInfo, bossPanel);
            }

            var desc1 = bossPanel.AddPanel(new Info("DescriptionHolder", 0, -400, 1000, 500), "");
            desc1.Background.SetSprite(AssetHelper.GetSprite("ChristmasPanel"));
            var desc2 = bossPanel.AddPanel(new Info("DescriptionInside", 0, -400, 950, 450), "");
            desc2.Background.SetSprite(AssetHelper.GetSprite("ChristmasInsertPanel"));
            var desc3 = bossPanel.AddText(new Info("Description", 0, -400, 900, 400), bossInfo.Description, 60, TextAlignmentOptions.Center);
            desc3.EnableAutoSizing();

            desc1.Hide();
            desc2.Hide();
            desc3.Hide();

            bossPanel.AddButton(new Info("DescriptionButton", 700, 0, 120), VanillaSprites.InfoBtn2, new System.Action(() =>
            {
                if (desc1.transform.localScale == Vector3.zero)
                {
                    desc1.Show();
                    desc2.Show();
                    desc3.Show();
                }
                else
                {
                    desc1.Hide();
                    desc2.Hide();
                    desc3.Hide();
                }
            }));


            //VertexGradient bossGradient = new VertexGradient(UnityEngine.Color.red, UnityEngine.Color.yellow, UnityEngine.Color.green, UnityEngine.Color.blue);
            //BossUI.UpdateHealthColor(null, bossGradient);
        }
    }

    public static void CreateSkulls(BossAPI.BossInfo info, ModHelperPanel panel)
    {
        float panelWidth = panel.RectTransform.rect.width;

        var skullsCount = info.SkullsCount;
        float spacing = panelWidth / (skullsCount + 1);

        float[] percentageValues = Hooks.CalculateHealthTriggerPercentages(skullsCount);

        for (int i = 0; i < skullsCount; i++)
        {
            float xPosition = panelWidth * (1f - percentageValues[i]);

            ModHelperImage skull = panel.AddImage(new Info($"Skull_{i}", xPosition - panelWidth / 2, 0, 112, 112), ModContent.GetTextureGUID<XmasMod2025>($"{info.SkullIcon}Skull"));

            skull.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            skull.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            skull.RectTransform.anchoredPosition = new Vector2(xPosition - panelWidth / 2, -panel.RectTransform.rect.height / 2);

            BossAPI.skullUIs.Add(skull);
        }
    }

    public static void HandleUI(Bloon b)
    {
        if (instance == null) return;

        if (b == null || b.health <= 0f)
        {
            instance.Close();
        }

        if (bossHealth != null && bossPanelInside != null)
        {
            float currentHP = b.health;
            float maxHP = b.bloonModel.maxHealth;

            float healthPercent = currentHP / maxHP;

            bossPanelInside.transform.localScale = new Vector3(healthPercent, 1f, 1f);

            bossPanelInside.transform.localPosition = new Vector3(
                (1f - healthPercent) * -bossPanelInside.RectTransform.rect.width / 2f,
                bossPanelInside.transform.localPosition.y,
                bossPanelInside.transform.localPosition.z
            );

            bossHealth.Text.text = $"{currentHP}/{maxHP}";
        }

    }
    public static void UpdateHealthColor(UnityEngine.Color? color = null, VertexGradient? colorGradient = null)
    {
        if (bossHealth == null) return;

        var text = bossHealth.Text;

        if (color.HasValue)
        {
            text.enableVertexGradient = false;
            text.color = color.Value;
        }
        else if (colorGradient.HasValue)
        {
            text.enableVertexGradient = true;
            text.colorGradient = colorGradient.Value;

            text.ForceMeshUpdate();
        }
    }

    public static void UpdateNameColor(UnityEngine.Color? color = null, VertexGradient? colorGradient = null)
    {
        if (bossName == null) return;

        var text = bossName.Text;

        if (color.HasValue)
        {
            text.enableVertexGradient = false;
            text.color = color.Value;
        }
        else if (colorGradient.HasValue)
        {
            text.enableVertexGradient = true;
            text.colorGradient = colorGradient.Value;

            text.ForceMeshUpdate();
        }
    }
    public static IEnumerator HandeSkull(ModHelperImage skullUI, string customSkull)
    {
        if (skullUI != null)
        {
            skullUI.AddImage(new BTD_Mod_Helper.Api.Components.Info($"Skull_", 0, 0, 112, 112), ModContent.GetTextureGUID<XmasMod2025>($"{customSkull}ActivateSkull"));

            yield return new WaitForSeconds(1f);

            if (skullUI != null && skullUI.gameObject != null)
            {
                GameObject.Destroy(skullUI.gameObject);
                BossAPI.skullUIs.Remove(skullUI);
            }
        }
    }
}