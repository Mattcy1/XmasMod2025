using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Scenarios;
using BTD_Mod_Helper.Extensions; 
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Difficulty;
using Il2CppAssets.Scripts.Models.Gameplay.Mods;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Simulation;
using UnityEngine;
using UnityEngine.Video;
using XmasMod2025.Bloons;
using XmasMod2025.Bloons.Bfbs;
using XmasMod2025.Bloons.Moabs;
using XmasMod2025.Bloons.Zomgs;

namespace ChristmasMod;

internal class ChristmasRouds : ModRoundSet
{

    public override string BaseRoundSet => RoundSetType.Default;
    public override int DefinedRounds => 100;
    public override string DisplayName => "Christmas Rounds";

    public override void ModifyEasyRoundModels(RoundModel roundModel, int round)
    {
        switch (round)
        {
            case 4:
                roundModel.AddBloonGroup(ModContent.BloonID<ChocolateBloon>(), 3, 0, 10);
                break;
            case 6:
                roundModel.AddBloonGroup(ModContent.BloonID<SnowBloon>(), 3, 0, 10);
                break;
            case 10:
                roundModel.AddBloonGroup(ModContent.BloonID<ChocolateBloon>(), 3, 0, 10);
                roundModel.AddBloonGroup(ModContent.BloonID<SnowBloon>(), 3, 0, 25);
                break;
            case 13:
                roundModel.AddBloonGroup(ModContent.BloonID<SnowMoab.WeakSnowMoab>(), 1, 0, 0);
                roundModel.AddBloonGroup(ModContent.BloonID<IceBloon>(), 2, 0, 10);
                roundModel.AddBloonGroup(ModContent.BloonID<SnowBloon>(), 5, 0, 40);
                break;
            case 19:
                roundModel.AddBloonGroup(ModContent.BloonID<SnowMoab.WeakSnowMoab>(), 5, 0, 25);
                roundModel.AddBloonGroup(ModContent.BloonID<ChocolateBloon>(), 2, 0, 20);
                roundModel.AddBloonGroup(ModContent.BloonID<SnowBloon>(), 5, 0, 30);
                break;
            case 24:
                roundModel.AddBloonGroup(ModContent.BloonID<SnowMoab.WeakSnowMoab>(), 3, 0, 10);
                roundModel.AddBloonGroup(ModContent.BloonID<IceBloon>(), 2, 0, 10);
                roundModel.AddBloonGroup(ModContent.BloonID<ChocolateBloon>(), 5, 0, 40);
                roundModel.AddBloonGroup(ModContent.BloonID<SnowBloon>(), 5, 0, 30);
                break;
            case 29:
                roundModel.AddBloonGroup(ModContent.BloonID<CandyCaneBloon>(), 2, 0, 10);
                break;

            case 34:
                roundModel.AddBloonGroup(ModContent.BloonID<SnowMoab.WeakSnowMoab>(), 1, 0, 10);
                break;

            case 38:
                roundModel.AddBloonGroup(ModContent.BloonID<SnowMoab>(), 2, 0, 10);
                roundModel.AddBloonGroup<GiftBloon>(3, 0, 50);
                break;
        }
    }

    public override void ModifyMediumRoundModels(RoundModel roundModel, int round)
    {
        switch (round)
        {
            case 42:
                roundModel.AddBloonGroup<ChocoMoab>(1, 100, 1000);
                break;
            case 43:
                roundModel.AddBloonGroup<SnowMoab>(4, 500, 900);
                roundModel.AddBloonGroup<GiftBloon>(10, 0, 1000);
                break;
            case 45:
                roundModel.AddBloonGroup<IceMoab>(1, 100, 101);
                roundModel.AddBloonGroup<IceBloon>(10, 0, 100);
                roundModel.AddBloonGroup<SnowMoab>(10, 100, 200);
                break;
            case 54:
                roundModel.AddBloonGroup<SnowBfb>();
                break;
            case 60:
                roundModel.AddBloonGroup<SnowBfb>(1, 100, 100);
                roundModel.AddBloonGroup<IceMoab>(5, 0, 3000);
                break;
        }
    }

    public override void ModifyHardRoundModels(RoundModel roundModel, int round)
    {
        switch (round)
        {
            case 61:
                roundModel.AddBloonGroup<ChocoBfb>(1, 25, 1000);
                roundModel.AddBloonGroup<SnowBfb>(1, 50, 1000);
                roundModel.AddBloonGroup<IceBfb>(1, 75, 1000);
                break;
            case 62: 
                roundModel.AddBloonGroup<CandyCaneBloon>(25, 25, 1000);
                roundModel.AddBloonGroup<SnowBloon>(25, 50, 1000);
                roundModel.AddBloonGroup<IceBloon>(10, 75, 1000);
                roundModel.AddBloonGroup<ChocolateBloon>(10, 100, 1000);
                break;
            case 69: 
                roundModel.ClearBloonGroups();
                roundModel.AddBloonGroup<ChocoBfb>(5, 25, 1000);
                roundModel.AddBloonGroup<SnowMoab>(3, 100, 500);
                roundModel.AddBloonGroup<ChocoMoab>(2, 1000, 1500);
                roundModel.AddBloonGroup<IceBfb>(2, 100, 500);
                break;
            case 72:
                roundModel.AddBloonGroup<GiftMoab>(5, 25, 500);
                break;
            case 75: 
                roundModel.AddBloonGroup<CandyCaneBloon>(1, 0, 0);
                roundModel.AddBloonGroup<SnowBloon>(1, 0, 0);
                roundModel.AddBloonGroup<ChocoBfb>(1, 0, 0);
                roundModel.AddBloonGroup<IceBfb>(1, 0, 0);
                roundModel.AddBloonGroup<SnowBfb>(1, 0, 0);
                break;
        }
    }

    public override void ModifyImpoppableRoundModels(RoundModel roundModel, int round)
    {
        switch (round)
        {
            case 81: 
                roundModel.AddBloonGroup<IceZomg>(1, 500, 1000);
                break;
            case 82:
                roundModel.AddBloonGroup<ChocoZomg>(3, 0, 100);
                roundModel.AddBloonGroup<GiftBloon>(15, 0, 100);
                roundModel.AddBloonGroup<GiftMoab>(5, 100, 500);
                break;
            case 84:
                roundModel.AddBloonGroup<SnowZomg>(3, 100, 500);
                roundModel.AddBloonGroup<GiftMoab>(2, 0, 100);
                break;
            case 89:
                roundModel.AddBloonGroup<IceZomg>(3, 1000, 2000);
                roundModel.AddBloonGroup<GiftMoab>(5, 0, 100);
                break;
            case 94:
                roundModel.AddBloonGroup<GiftMoab>(3, 0, 0);
                roundModel.AddBloonGroup<GiftBloon>(5, 0, 0);
                roundModel.AddBloonGroup<CandyCaneBloon>(5, 0, 0);
                roundModel.AddBloonGroup<SnowBloon>(5, 0, 0);
                roundModel.AddBloonGroup<IceBloon>(5, 0, 0);
                roundModel.AddBloonGroup<IceMoab>(5, 0, 0);
                roundModel.AddBloonGroup<SnowMoab>(5, 0, 0);
                roundModel.AddBloonGroup<IceBfb>(5, 0, 0);
                roundModel.AddBloonGroup<SnowBfb>(5, 0, 0);
                roundModel.AddBloonGroup<SnowZomg>(5, 0, 0);
                roundModel.AddBloonGroup<IceZomg>(5, 0, 0);
                break;
            case 97:
                roundModel.ClearBloonGroups();
                roundModel.AddBloonGroup<IceMoab>(60, 0, 720);
                roundModel.AddBloonGroup<GiftBloon>(15, 0, 720);
                roundModel.AddBloonGroup<SnowZomg>(5, 0, 720);
                break;
        }
    }
}

public class ChristmasGameMode : ModGameMode
{

    public override string Difficulty => DifficultyType.Hard;
    public override string BaseGameMode => GameModeType.Hard;
    public override string Icon => "Icon";

    public override string DisplayName => "Christmas Gamemode";

    public override void ModifyBaseGameModeModel(ModModel gameModeModel)
    {
        gameModeModel.UseRoundSet<ChristmasRouds>();
        gameModeModel.SetEndingRound(99);
    }
}