using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Hooks;
using BTD_Mod_Helper.Api.Hooks.BloonHooks;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.MapEditor;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.ContentBrowser;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BTD_Mod_Helper.Api.Helpers;
using Il2CppAssets.Scripts.Data.MapSets;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Unity.Bridge;
using UnityEngine;
using XmasMod2025;
using XmasMod2025.Bloons;
using XmasMod2025.GiftShop.BuffsItems;
using XmasMod2025.Towers;
using XmasMod2025.UI;
using static XmasMod2025.UI.Gift;

[assembly: MelonInfo(typeof(XmasMod2025.XmasMod2025), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace XmasMod2025;

public class XmasMod2025 : BloonsTD6Mod
{
    public static MapModel customMap;
    
    private MapModel? GetEmbeddedMap(string resourceName)
    {
        var stream = MelonAssembly.Assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            return null;
        }
        
        var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();

        return ModelSerializer.DeserializeModel<MapModel>(json);
    }

    public override void OnTitleScreen()
    {
        var map = GetEmbeddedMap("map");
        if (map == null)
        {
            return;
        }
        
        LoggerInstance.Msg($"Old count: {GameData.Instance.mapSet.Maps.items.Count}");

        GameData.Instance.mapSet.Maps.items = GameData.Instance.mapSet.Maps.items.AddTo(new MapDetails
        {
            id = map.mapName,
            coopMapDivisionType = CoopDivision.FREE_FOR_ALL,
            difficulty = MapDifficulty.Intermediate,
            mapSprite = ModContent.GetSpriteReference(this, "XmasMap"),
            hasWater = true,
            theme = MapTheme.Snow,
            unlockDifficulty = MapDifficulty.Beginner,

        });
        
        
        LoggerInstance.Msg($"New count: {GameData.Instance.mapSet.Maps.items.Count}");
    }

    public static float PresentBloonChance = 0f;
    public static IntMinMax TreeDropRates = new(3, 5);
    public static int FestiveSpritActiveRounds = 0;
    public static bool FestiveSpiritActive = false;
    public static Tower FestiveSpiritTower = null;
    public static List<TowerModel> GiftOfGivingTowersIds = new List<TowerModel>();

    public delegate void CurrencyChangedDelegate(double amount);

    public static event CurrencyChangedDelegate? OnGiftsUpdated;
    public static event CurrencyChangedDelegate? OnSnowflakesUpdated;

    private static double gifts;

    public static double GetCurrency(CurrencyType currency)
    {
        switch (currency)
        {
            case CurrencyType.Gift:
                return Gifts;
            case CurrencyType.Cash:
                return InGame.instance.GetCash();
            case CurrencyType.Snowflake:
                return Snowflakes;
            default:
                return Gifts;
        }
    }
    public static void SetCurrency(CurrencyType currency, double amount)
    {
        switch (currency)
        {
            case CurrencyType.Gift:
                Gifts = amount;
                return;
            case CurrencyType.Cash:
                InGame.instance.SetCash(amount);
                return;
            case CurrencyType.Snowflake:
                Snowflakes = amount;
                return;
            default:
                Gifts = amount;
                return;
        }
    }
    public static void AddCurrency(CurrencyType currency, double amount)
    {
        switch (currency)
        {
            case CurrencyType.Gift:
                Gifts += amount;
                return;
            case CurrencyType.Cash:
                InGame.instance.AddCash(amount);
                return;
            case CurrencyType.Snowflake:
                 Snowflakes += amount;
                 return;
            default:
                Gifts += amount;
                return;
        }
    }

    private static double snowflakes;

    public static double Snowflakes
    {
        get => snowflakes;
        set
        {
            double increase = value - snowflakes;
            snowflakes = value;
            
            OnSnowflakesUpdated?.Invoke(increase);

            if (increase > 0)
            {
                totalSnowflakes += increase;   
            }
        }
    }

    public static double Gifts
    {
        get => gifts;
        set
        {
            double increase = value - gifts;
            gifts = value;

            OnGiftsUpdated?.Invoke(value);

            if (increase > 0)
            {
                totalGifts += increase;
            }
        }
    }

    private static double totalGifts;

    public static double TotalGifts => totalGifts;
    
    private static double totalSnowflakes;

    public static double TotalSnowflakes => totalSnowflakes;

    public override void OnMatchStart()
    {
        if (GiftOpenerUI.instance == null)
        {
            GiftOpenerUI.CreatePanel();
        }

        if (GiftCounterUI.instance == null)
        {
            GiftCounterUI.CreatePanel();
        }

        gifts = 25;
        totalGifts = 25;
    }

    public override void OnRestart()
    {
        gifts = 25;
        totalGifts = 25;
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        if(tower.towerModel.baseId == ModContent.TowerID<FestiveSpiritTower>())
        {
            FestiveSpiritTower = tower;
        }
    }
    public override void OnTowerDestroyed (Tower tower)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<ElfHelper>())
        {
            InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<GiftEffect>(), tower.Position, 0, 2, 120);
        }

        if (tower.towerModel.baseId == ModContent.TowerID<FestiveSpiritTower>())
        {
            FestiveSpiritTower = null;
        }
    }

    public override void OnRoundEnd()
    {
        BuffHandler.GiftOfGivingHandler(false);

        if(FestiveSpritActiveRounds > 0)
        {
            FestiveSpritActiveRounds--;
        }

        if(FestiveSpiritActive)
        {
            if(FestiveSpritActiveRounds <= 0)
            {
                if(FestiveSpiritTower != null)
                {
                    FestiveSpiritTower.worth = 0;
                    FestiveSpiritTower.SellTower();
                }
            }
        }

        foreach(var tower in InGame.instance.GetTowers())
        {
            if(tower.towerModel.baseId == ModContent.TowerID<ElfSpawner>())
            {
                tower.SellTower();
            }
        }
    }
    /*public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        var modTower = tower.towerModel.GetModTower();

        if (modTower != null && modTower is ChristmasTower cTower && cTower.Currency != CurrencyType.Cash)
        {
        }
    }*/

    [HookTarget(typeof(BloonDamageHook), HookTargetAttribute.EHookType.Postfix)]
    [HookPriority(HookPriorityAttribute.Low)]
    public static bool BloonDamagePostfix(Bloon @this, ref float totalAmount, Projectile projectile, ref bool distributeToChildren, ref bool overrideDistributeBlocker, ref bool createEffect, Tower tower, BloonProperties immuneBloonProperties, BloonProperties originalImmuneBloonProperties, ref bool canDestroyProjectile, ref bool ignoreNonTargetable, ref bool blockSpawnChildren, HookNullable<int> powerActivatedByPlayerId)
    {
        if(@this.bloonModel.baseId == ModContent.BloonID<SnowBloon>())
        {
            System.Random rand = new System.Random();
            var val = rand.Next(4);

            if(val >= 0)
            {
                TimeTriggerModel time = Game.instance.model.GetBloon("Vortex1").GetBehavior<TimeTriggerModel>().Duplicate();
                time.interval = 1f;
                time.actionIds = new string[] { "SnowBloonStun" };
                time.name = "SnowBloonStun";

                if (@this != null)
                {
                    @this.bloonModel.AddBehavior(time);
                }
            }
        }

        return true;
    }
}

// why mattcy why
public enum PropsEnum
{
    AmbientFxDust = 10000,
    AmbientFxDust2 = 10001,
    AmbientFxFlowersYellow = 10002,
    AmbientFxFogBlue = 10003,
    AmbientFxFogGreen = 10004,
    AmbientFxLeafAutumn = 10005,
    AmbientFxLeafBrown = 10006,
    AmbientFxLeafCherryBlossom = 10007,
    AmbientFxLeafGreen = 10008,
    AmbientFxLeafOrange = 10009,
    AmbientFxLeafPink = 10010,
    AmbientFxLeafYellow = 10011,
    AmbientFxRain = 10012,
    AmbientFxSandy = 10013,
    FxCrystalGlow = 20000,
    FxFireflies = 20001,
    FXFirePit = 20002,
    FxPortal = 20003,
    FxSparks = 20004,
    FxSteamVent = 20005,
    FxTumbleweed = 20006,
    FxWaterDrops = 20007,
    FxWaterSparkles = 20008,
    Arbor = 30000,
    BalanceHedge = 30001,
    Bench = 30002,
    BlueBarrel = 30003,
    Bouys = 30004,
    BunchTreeProp = 30005,
    BunchTreePropOrange = 30006,
    BunchTreePropOrangeYellow = 30007,
    BunchTreePropPink = 30008,
    BunchTreePropPinkDark = 30009,
    BunchTreePropYellow = 30010,
    BushesRow = 30011,
    CabinBridge = 30012,
    CactusFlower = 30013,
    CactusOneBit = 30014,
    CactusTwoBits = 30015,
    CamelliaFlower = 30016,
    Candles = 30017,
    CardboardBox = 30018,
    CauldronFire = 30019,
    CherryBlossomTree = 30020,
    StringOfLights = 30021,
    Cobweb = 30022,
    CobwebHalf = 30023,
    CrackedArrow = 30024,
    CrackedTreeBig = 30025,
    CrackedTreeMed = 30026,
    CrackedTreeSmall = 30027,
    CritterBat = 30028,
    CritterCrab = 30029,
    CritterFishBlue = 30030,
    CritterFishGreen = 30031,
    CritterPigeon = 30032,
    DartHouse = 30033,
    DelphiniumFlowerBlue = 30034,
    DelphiniumFlowerPurple = 30035,
    EncryptedMedium = 30036,
    EncryptedTreeLarge = 30037,
    EncryptedTreeSmol = 30038,
    Fence = 30039,
    FenceBrown = 30040,
    FencePostHaunted = 30041,
    Fern1 = 30042,
    Fern2 = 30043,
    Fern3 = 30044,
    FiringRangePineTreeProp = 30045,
    ForestTreeProp = 30046,
    Fountain = 30047,
    FrogSign = 30048,
    FWheel = 30049,
    GardenFlowerPropOrange = 30050,
    GardenFlowerPropPurple = 30051,
    GardenFlowerPropWhite = 30052,
    GardenFlowerPropYellow = 30053,
    GateWhite = 30054,
    Grass1 = 30055,
    Grass2 = 30056,
    Grass2Blade = 30057,
    Grass3 = 30058,
    Grass4 = 30059,
    Grass5 = 30060,
    Grass6 = 30061,
    Grass7 = 30062,
    Grass8 = 30063,
    Grass9 = 30064,
    Grass10 = 30065,
    Grass11 = 30066,
    Grass12 = 30067,
    Grass13 = 30068,
    Grass14 = 30069,
    Grass15 = 30070,
    Grass16 = 30071,
    Grass17 = 30072,
    GraveBrown = 30073,
    GraveGrey = 30074,
    HauntedReed = 30075,
    HayBale = 30076,
    HedgeBush = 30077,
    HedgeEdge = 30078,
    HedgeLeaf = 30079,
    HedgeRectangularProp = 30080,
    HedgeRow = 30081,
    HedgeTreeLeaf = 30082,
    HollowLogKnobProp = 30083,
    HollowLogProp = 30084,
    Hurdle = 30085,
    HydrangeaFlower = 30086,
    HydrangeaFlowerPurple = 30087,
    HydrangeaLeaf = 30088,
    Jetti = 30089,
    JungleBushProp = 30090,
    JungleBushPropDark = 30091,
    Kelp = 30092,
    LifeSaver = 30093,
    LillyPad = 30094,
    LillyPadGroup_1 = 30095,
    LillyPadGroup_2 = 30096,
    LillyPadGroup = 30097,
    LilyFlower = 30098,
    LilyPadFlower = 30099,
    LogNotchedProp = 30100,
    LogTwoKnobsProp = 30101,
    LogWithKnobProp = 30102,
    LotusBoat = 30103,
    LotusTreePad = 30104,
    MesaBoulderLarge = 30105,
    MesaBoulderMedium = 30106,
    MesaBoulderSmall = 30107,
    MesaRock = 30108,
    MesaRock1 = 30109,
    MesaRock2 = 30110,
    MesaRock3 = 30111,
    MesaRock4 = 30112,
    MesaTree = 30113,
    MesaTreeMedium = 30114,
    MesaTreeSmol = 30115,
    MushroomProp = 30116,
    MushroomPropPurple = 30117,
    OuchPuddleProp = 30118,
    PalmTrees = 30119,
    ParkTree = 30120,
    PetBalfrog = 30121,
    PetBat = 30122,
    PetBison = 30123,
    PetBunny = 30124,
    PetChameleon = 30125,
    PetChicken = 30126,
    PetCrane = 30127,
    PetDadOfQuincy = 30128,
    PetDragon = 30129,
    PetDragonfly = 30130,
    PetElf = 30131,
    PetFox = 30132,
    PetFrog = 30133,
    PetHedgehog = 30134,
    PetHummingbird = 30135,
    PetNarwhal = 30136,
    PetNinjaKiwi = 30137,
    PetPackMule = 30138,
    PetParrot = 30139,
    PetPenguin = 30140,
    PetRat = 30141,
    PetRoomba = 30142,
    PetRubberDuck = 30143,
    PetSentaiDrone = 30144,
    PetSlimeCube = 30145,
    PetSnowman = 30146,
    PetWolf = 30147,
    PinkFlower = 30148,
    PinkLeafLoose = 30149,
    PotBushFlower = 30150,
    PropBarricade = 30151,
    PropBlockingVines = 30152,
    PropEggAir = 30153,
    PropEggLand = 30154,
    PropEggWater = 30155,
    PropFallenLogs = 30156,
    PropPartyBloon1 = 30157,
    PropPartyBloon2 = 30158,
    Pumpkin = 30159,
    Pumpkin1 = 30160,
    Pumpkin2 = 30161,
    PumpkinLeaf = 30162,
    PumpkinMedium = 30163,
    PumpkinPatch1 = 30164,
    PumpkinPatch2 = 30165,
    PumpkinSmall = 30166,
    PumpkinXL = 30167,
    QuadLeaf = 30168,
    QuadLeafDark = 30169,
    QuietStreetLamppost = 30170,
    RaceTyreBlack = 30171,
    SubmergedPlank2 = 30172,
    ResortUmbrella = 30173,
    RiverRockL = 30174,
    RiverRockM = 30175,
    RiverRockMossL = 30176,
    RiverRockMossM = 30177,
    RiverRockMossS = 30178,
    RiverRockMossXL = 30179,
    RiverRockS = 30180,
    RiverRockXL = 30181,
    RockPointyFlatProp = 30182,
    RockPointySmallProp = 30183,
    RockPointyTallProp = 30184,
    RockPointyVShapeProp = 30185,
    RockPointyWingShapeProp = 30186,
    RoundGrass = 30187,
    RoundGrassDark = 30188,
    Route66Sign = 30189,
    SanctuaryIvy01 = 30190,
    SanctuaryIvy02 = 30191,
    ScrapPile = 30192,
    ScrapyardCrate = 30193,
    ScrapyardCrate2 = 30194,
    ScrapyardIronFence = 30195,
    SeaRock = 30196,
    SeaweedRock = 30197,
    SmallCrate = 30198,
    SmallCrateOpen = 30199,
    SpiderWeb = 30200,
    SpiderWebHalf = 30201,
    StreambedRock = 30202,
    StreetLamp = 30203,
    SubmergedPlank = 30204,
    TopiaryRound = 30205,
    TopiaryTall = 30206,
    TownHall = 30207,
    TreeStump1 = 30208,
    TreeTrunk1 = 30209,
    TreeTrunk2 = 30210,
    TulipFlower = 30211,
    TulipLeaves = 30212,
    UndergroundRockSmall = 30213,
    VinePathLeaf01 = 30214,
    Vines01 = 30215,
    Vines02 = 30216,
    WaterTank = 30217,
    WesternBarrel = 30218,
    WesternPoleFence = 30219,
    WesternWaterTub = 30220,
    WesternWindmill = 30221,
    Wheelbarrow = 30222,
    WhiteFlower = 30223,
    Windmill = 30224,
    WisteriaFlower = 30225,
    WoodenBarrel = 30226,
    WoodFenceProp = 30227,
    YellowFlower = 30228,
    CabinBridgeMini = 30229,
    AmbientFxSnowLight = 10014,
    AmbientFxSnowHeavy = 10015,
    BaubleBlue = 30230,
    BaubleGold = 30231,
    BaubleGreen = 30232,
    BaubleRed = 30233,
    BrickFountain = 30234,
    CandyCane = 30235,
    ChristmasLight = 30236,
    ChristmasTree = 30237,
    CritterPenguin = 30238,
    CritterPolarBear = 30239,
    GiftBlue = 30240,
    GiftGreen = 30241,
    GiftLavender = 30242,
    GiftPink = 30243,
    GiftPurple = 30244,
    GiftRed = 30245,
    GiftYellow = 30246,
    GingerbreadPerson = 30247,
    HedgeSnowy = 30248,
    Holly = 30249,
    PeppermintCandy = 30250,
    QuietStreetHouse = 30251,
    SantaSleigh = 30252,
    SkateFishSign = 30253,
    SkateHutA = 30254,
    SkateHutB = 30255,
    SkateHutC = 30256,
    SnowFolks = 30257,
    SnowMonkey = 30258,
    SnowRock4 = 30259,
    SnowRock3 = 30260,
    SnowRock2 = 30261,
    SnowRock1 = 30262,
    IceWing = 30263,
    IceTriangle = 30264,
    IceSquare = 30265,
    IceRoundedTriangle = 30266,
    QuietStreetTree = 30267,
    PineTreeVariationSnow = 30268,
    PineTreeSnowyTop = 30269,
    AdventTreeBackground = 30270,
    SnowPatch1 = 30271,
    SnowPatch2 = 30272,
    SnowPatch3 = 30273,
    SnowPatch4 = 30274,
    SnowPatch5 = 30275,
    SnowPatch6 = 30276,
    SnowPatch7 = 30277,
    GrassChristmas1 = 30278,
    GrassChristmas2 = 30279,
    GrassChristmas3 = 30280,
    SantalessSleigh = 30281,
    FxWaterRipples = 20009,
    FxWaterMist = 20010,
    FxWaterFountain = 20011,
    FxStreamParticles = 20012,
    FxWaterSplash = 20013,
    PropFenceBlue = 30282,
    PropFencePoleBlue = 30283,
    GlacialIcicle1 = 30284,
    GlacialIcicle2 = 30285,
    GlacialIcicle3 = 30286,
    GlacialIcicle4 = 30287,
    PetCapybara = 30288,
    JellyBeanBrown = 30289,
    JellyBeanBlue = 30290,
    JellyBeanChocolate = 30291,
    JellyBeanGreen = 30292,
    JellyBeanLavender = 30293,
    JellyBeanLightPink = 30294,
    JellyBeanOrange = 30295,
    JellyBeanPink = 30296,
    JellyBeanPurple = 30297,
    JellyBeanRed = 30298,
    JellyBeanYellow = 30299,
    CandyBlue = 30300,
    CandyGreen = 30301,
    CandyOrange = 30302,
    CandyPink = 30303,
    CandyPurple = 30304,
    CandyRed = 30305,
    ChocolateBlue = 30306,
    ChocolateBrown = 30307,
    ChocolateChocolate = 30308,
    ChocolateDarkBlue = 30309,
    ChocolateGreen = 30310,
    ChocolateOrange = 30311,
    ChocolatePink = 30312,
    ChocolatePink1 = 30313,
    ChocolatePurple = 30314,
    ChocolateRed = 30315,
    ChocolateYellow = 30316,
    LollypopBlue = 30317,
    LollypopOrange = 30318,
    LollypopPurple = 30319,
    LollypopRed = 30320,
    CandySwirlBlue = 30321,
    CandySwirlPink = 30322,
    CandySwirlRed = 30323,
    CandySwirlYellow = 30324,
    ChocolateCube = 30325,
    ChocolateRock = 30326,
    WaferStick = 30327,
    WaffleHouse = 30328,
    WaffleHouseStep = 30329,
    CandyFlossTreeBlue = 30330,
    CandyFlossTreeGreen = 30331,
    CandyFlossTreeOrange = 30332,
    CandyFlossTreePink = 30333,
    CandyFlossTreePurple = 30334,
    FxChocolateRipple = 20014,
    AmbientFxButterflies = 10016,
    ButterflyBlack = 30335,
    ButterflyBlue = 30336,
    ButterflyGreen = 30337,
    ButterflyPurple = 30338,
    ButterflyRed = 30339,
    ButterflyYellow = 30340,
    EasterLeaf = 30341,
    EasterNest = 30342,
    FlowerRing = 30343,
    HedgeStatue = 30344,
    Rake = 30345,
    RakeRock = 30346,
    LadyBug1 = 30347,
    LadyBug2 = 30348,
    PropEgg0 = 30349,
    PropEgg2 = 30350,
    PropEgg3 = 30351,
    PropEgg1 = 30352,
    PetMouse = 30353,
    FxRemovableExplosion1 = 20015,
    FxRemovableExplosion2 = 20016,
    RemovableMonkeyBobcat = 30354,
    RemovableMonkeyPickaxe = 30355,
    PropBloonariusSlimeEmitter = 30356,
    PropSlimeBlob1 = 30357,
    PropSlimeBlob2 = 30358,
    CautionSign = 30359,
    CautionStand = 30360,
    WarningSign = 30361,
    IceWarningSign = 30362,
    LampCoveredGarden = 30363,
    Mop = 30364,
    RoadCone = 30365,
    TruckBlack = 30366,
    TruckBlackOpen = 30367,
    TruckBlue = 30368,
    TruckBlueOpen = 30369,
    DumpTruck = 30370,
    TruckRed = 30371,
    TruckRedOpen = 30372,
    TruckYellow = 30373,
    TruckYellowOpen = 30374,
    Building1 = 30375,
    Building2 = 30376,
    Building3 = 30377,
    Building4 = 30378,
    Building5 = 30379,
    Building6 = 30380,
    Building7 = 30381,
    Building8 = 30382,
    Building9 = 30383,
    Building10 = 30384,
    Building11 = 30385,
    Window1 = 30386,
    Window2 = 30387,
    Window3 = 30388,
    Window4 = 30389,
    Window5 = 30390,
    BuildingAdd1 = 30391,
    BuildingAdd2 = 30392,
    BuildingAdd3 = 30393,
    RoofPiece1 = 30394,
    RoofPiece2 = 30395,
    BuildingMuseum = 30396,
    Bridge = 30397,
    BuildingFan = 30398,
    WoodBeam1 = 30399,
    WoodBeam2 = 30400,
    MetalBeam1 = 30401,
    RoadCone2 = 30402,
    Chimney1 = 30403,
    Barrier1 = 30404,
    WaterTower1 = 30405,
    Crane = 30406,
    FxRemovableExplosion3 = 20017,
    FxRemovableExplosion4 = 20018,
    FxRemovableExplosion5 = 20019,
    FxRemovableExplosion6 = 20020,
    FxRemovableExplosion7 = 20021,
    FxRemovableExplosion8 = 20022,
    FxRemovableExplosion9 = 20023,
    FxRemovableExplosion10 = 20024,
    FxRemovableExplosion11 = 20025,
    FxRemovableExplosion12 = 20026,
    FxRemovableExplosion13 = 20027,
    FxRemovableExplosion14 = 20028,
    FxRemovableExplosion15 = 20029,
    RemovableMonkeyBobcat2 = 30407,
    RemovableMonkeyHammer = 30408,
    RemovableMonkeyScytheFarmer = 30409,
    RemovableMonkeySurfer = 30410,
    RemovableMonkeyWelding = 30411,
    RemovableMonkeyAxe = 30412,
    Window6 = 30413,
    Window7 = 30414,
    PetOwl = 30415,
    PetPalbot = 30416,
    BeachBridge = 30417,
    BeachPillar = 30418,
    BeachTower = 30419,
    PoolLadder = 30420,
    ResortChair = 30421,
    ResortTable = 30422,
    ResortUmbrellaBlue = 30423,
    ResortUmbrellaPink = 30424,
    ResortUmbrellaYellow = 30425,
    Slide = 30426,
    TowelGreen = 30427,
    TowelPink = 30428,
    WaterParkBeachball = 30429,
    WaterParkFence = 30430,
    WaterParkFloatiePink = 30431,
    WaterParkFloatiePurple = 30432,
    WaterParkFloatieYellow = 30433,
    WaterParkLifeguardChair = 30434,
    WaterParkPlatform = 30435,
    WaterParkStool = 30436,
    WaterParkTable = 30437,
    Wave = 30438,
    PetKangaroo = 30439,
    PetMole = 30440,
    PropBlastRock = 30441,
    PetPig = 30442,
    PropCompactGadget = 30443,
    PetCollie = 30444,
    PropTutorialArrow = 30445,
    PropTutorialRing = 30446,
    PetSheep = 30447,
    PropAreaRectangle = 30448,
    PetWalrus = 30449
}

[HarmonyLib.HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
public class ChangeMap
{
    [HarmonyLib.HarmonyPostfix]
    public static void Postfix()
    {
        GameObject game = GameObject.Find("CubismTerrain");

        List<MapEditorProp> MapEditorProps = Il2CppGenericsExt.ToList<MapEditorProp>(GameData.Instance.mapEditorData.mapEditorProps);
        Dictionary<int, (string Name, Vector3 rot, int scale, Vector3 pos)> PropToSpawns = new();

        PropToSpawns.Add((int)PropsEnum.AmbientFxSnowHeavy, ("SnowFX", new Vector3(0, 0, 0), 1, new Vector3(0, 0)));
        PropToSpawns.Add((int)PropsEnum.PineTreeVariationSnow, ("PineTree", new Vector3(0, 0, 0), 1, new Vector3(0, 0)));

        System.Action<bool> callback = _ => { };

        for (int i = 0; i < MapEditorProps.Count; i++)
        {
            var item = MapEditorProps[i];

            if (PropToSpawns.TryGetValue(item.id, out var data))
            {
                PositionalData positionalData = new PositionalData();
                positionalData.rotation = data.rot;
                positionalData.scale = data.scale;
                positionalData.position = data.pos;

                InGame.instance.bridge.CreateMapEditorPropAt(data.Name, positionalData, item.Def(), default(ObjectId), callback);
                MelonLogger.Msg("Spawning Props " + data.Name + " ID: " + item.id);
            }
        }

        if (game != null)
        {
            game.GetComponent<MeshRenderer>().material.mainTexture = ModContent.GetTexture<XmasMod2025>("Map");
        }
        else
        {
            PopupScreen.instance.ShowOkPopup("It's recommended to play this mod on cubsim, The code for loading a custom map was made by datjanedoe credits to him.");

        }
    }
}

[HarmonyLib.HarmonyPatch(typeof(TimeTrigger), nameof(TimeTrigger.Trigger))]
public class TimeTriggerPacth
{
    [HarmonyLib.HarmonyPostfix]
    public static void Postfix(TimeTrigger __instance)
    {
        if(__instance.timeTriggerModel.name.Contains("SnowBloonStun"))
        {
            __instance.bloon.bloonModel.RemoveBehavior<TimeTriggerModel>();
        }
    }
}