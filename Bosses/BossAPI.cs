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
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity.Scenes;
using Il2CppAssets.Scripts.Unity.UI_New.Achievements;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.BloonMenu;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XmasMod2025.Bosses;
using Info = BTD_Mod_Helper.Api.Components.Info;

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
        public string BossID { get; set; }
        public int SpawnRound { get; set; }
        public string BossToSpawn { get; set; }

        public string Description { get; set; }
    }

    public static List<BossInfo> BossInfos = new List<BossInfo>();
}
public class Hooks
{
    //Updates Health Bar
    [HookTarget(typeof(BloonDamageHook), HookTargetAttribute.EHookType.Postfix)]
    [HookPriority(HookPriorityAttribute.Higher)]
    public static bool BloonDamagePostfix(Bloon @this, ref float totalAmount, Projectile projectile, ref bool distributeToChildren, ref bool overrideDistributeBlocker, ref bool createEffect, Tower tower, BloonProperties immuneBloonProperties, BloonProperties originalImmuneBloonProperties, ref bool canDestroyProjectile, ref bool ignoreNonTargetable, ref bool blockSpawnChildren, HookNullable<int> powerActivatedByPlayerId)
    {
        foreach (var tag in @this.bloonModel.tags)
        {
            foreach (var bossInfo in BossAPI.BossInfos)
            {
                if (@this.bloonModel.IsModdedBoss())
                {
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

                if (r == bossInfo.SpawnRound - 1)
                {
                    InGame.instance.SpawnBloons(bossInfo.BossToSpawn, 1, 0);
                }
            }
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

                    if (BossUI.instance != null)
                    {
                        BossUI.instance.Close();
                    }

                    BossUI.CreateBossBar(bossInfo);

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

            if (!__instance.bloonModel.HasTag("Choco") && !__instance.bloonModel.isBoss && XmasMod2025.boss != null && XmasMod2025.boss.bloonModel.baseId == ModContent.BloonID<ChocoBoss>())
            {
                var root = __instance.bloonModel.Duplicate();

                root.maxHealth *= 2;

                __instance.UpdateRootModel(root);
                __instance.health = __instance.bloonModel.maxHealth;

                MelonLogger.Msg(__instance.health);

                __instance.bloonModel.AddTag("Choco");
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
                if (bloon.HasTag("Sandbox") && !sortedBloons.Contains(bloon))
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
                    if (BossUI.instance != null)
                    {
                        BossUI.instance.Close();
                    }

                    XmasMod2025.boss = null;

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
                if (__instance.bloonModel.IsModdedBoss())
                {
                    foreach (var BossInfo in BossAPI.BossInfos)
                    {
                        if (BossInfo.BossID == __instance.bloonModel.GetBossID())
                        {
                            if (BossUI.instance != null)
                            {
                                BossUI.instance.Close();
                            }
                            break;
                        }
                    }
                }
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

            if (BossAPI.skullUIs.Count > 0 && __instance.bloon.bloonModel.IsModdedBoss() && __instance.modl.actionIds.Contains("ModdedSkull" + __instance.bloon.bloonModel.GetBossID()))
            {
                int lastIndex = BossAPI.skullUIs.Count - 1;
                MelonCoroutines.Start(BossUI.HandeSkull(BossAPI.skullUIs[lastIndex], __instance.bloon.GetInfo().SkullIcon));
            }

            if (BossUI.bossPanelInside != null && __instance.modl.actionIds.Contains("HealthBar" + __instance.bloon.bloonModel.GetBossID()))
            {
                Bloon boss = __instance.bloon;

                foreach (var bossInfo in BossAPI.BossInfos)
                {
                    if (bossInfo.BossID == __instance.bloon.bloonModel.GetBossID())
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


    public static bool AddInfo(this BloonModel bloon, bool hasSkull, int skullsCount, string customSkullIcon, int starsCount, string healthBarIcon, string healthBarBackground, string displayName, string bossIcon, string bossID, int round, string ToSpawn, string description)
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
            Description = description
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
            bossLeftBackground = bossPanel.AddImage(new Info("LeftBackground", -725, 0f, 250, 250), VanillaSprites.BossTiersIconSmall);
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

            var desc1 = bossPanel.AddPanel(new Info("DescriptionHolder", 0, -400, 1000, 500), VanillaSprites.MainBGPanelBlue);
            var desc2 = bossPanel.AddPanel(new Info("DescriptionInside", 0, -400, 950, 450), VanillaSprites.BlueInsertPanel);
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