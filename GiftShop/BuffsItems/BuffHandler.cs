using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XmasMod2025.Towers;
using static MelonLoader.MelonLogger;

namespace XmasMod2025.GiftShop.BuffsItems
{
    internal class BuffHandler
    {
        public static void GiftOfGivingHandler(bool makeActive)
        {
            var game = InGame.instance;
            if (game == null || game.bridge == null) return;

            float rateAdd = makeActive ? -0.15f : 0.15f;
            float rangeAdd = makeActive ? 15f : -15f;

            foreach (var towerId in XmasMod2025.GiftOfGivingTowersIds)
            {
                var tower = game.bridge.GetTowerFromId(towerId);
                if (tower == null) continue;

                var model = tower.towerModel;
                XmasMod2025.GiftOfGivingTowersIds.Remove(towerId);

                foreach (var weapon in model.GetWeapons())
                {
                    weapon.rate += rateAdd;
                }

                foreach (var attack in model.GetAttackModels())
                {
                    attack.range += rangeAdd;
                }
            }
        }

        public static void FestiveSpiritHandler()
        {
            Action<bool> callback = _ => { };
            InGame.instance.bridge.CreateTowerAt(new Vector2(0, 0), ModContent.GetTowerModel<FestiveSpiritTower>(), ObjectId.Create((uint)0), false, callback, true, true, false, 0, false);
        }
    }
}
