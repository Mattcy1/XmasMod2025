using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XmasMod2025.Towers.SubTowers;
using static MelonLoader.MelonLogger;

namespace XmasMod2025.GiftShop.BuffsItems
{
    internal class BuffHandler
    {
        public static List<TowerModel> TowerBuffed = new List<TowerModel>();
        public static void GiftOfGivingHandler(bool makeActive)
        {
            var game = InGame.instance;
            if (game == null || game.bridge == null) return;

            float rateAdd = makeActive ? -0.15f : 0.15f;
            float rangeAdd = makeActive ? 15f : -15f;

            for (int i = XmasMod2025.GiftOfGivingTowersIds.Count - 1; i >= 0; i--)
            {
                var towerModel = XmasMod2025.GiftOfGivingTowersIds[i];
                //XmasMod2025.GiftOfGivingTowersIds.RemoveAt(i);
                TowerBuffed.Add(towerModel);

                if(!TowerBuffed.Contains(towerModel))
                {
                    foreach (var weapon in towerModel.GetWeapons())
                        weapon.rate += rateAdd;

                    foreach (var attack in towerModel.GetAttackModels())
                        attack.range += rangeAdd;

                    towerModel.range += rangeAdd;
                }
                else
                {
                    TowerBuffed.Remove(towerModel);

                    foreach (var weapon in towerModel.GetWeapons())
                        weapon.rate -= 0.15f;

                    foreach (var attack in towerModel.GetAttackModels())
                        attack.range -= 15;

                    towerModel.range -= 15;
                }
            }
        }


        public static void FestiveSpiritHandler()
        {
            Action<bool> callback = _ => { };
            if(XmasMod2025.FestiveSpiritTower == null)
            {
                InGame.instance.bridge.CreateTowerAt(new Vector2(0, 0), ModContent.GetTowerModel<FestiveSpiritTower>(), ObjectId.Create((uint)0), false, callback, true, true, false, 0, false);
            }
        }

        public static void SantaHelperHandler()
        {
            Action<bool> callback = _ => { };
            InGame.instance.bridge.CreateTowerAt(new Vector2(0, 0), ModContent.GetTowerModel<ElfSpawner>(), ObjectId.Create((uint)0), false, callback, true, true, false, 0, false);
        }
    }
}
