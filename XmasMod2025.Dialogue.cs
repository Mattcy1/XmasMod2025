using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using ChristmasMod;
using DialogLib;
using DialogLib.Ui;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using XmasMod2025.Assets;
using static BTD_Mod_Helper.Api.ModContent;

namespace XmasMod2025;

public partial class XmasMod2025
{
    private static SpriteReference GetSpriteReference(string name) => GetSpriteReference<XmasMod2025>(name);
    
    public static Voice KrampusVoice = new Voice(GetAudioClipReference<XmasMod2025>("KrampusVoice1").AssetGUID,
        GetAudioClipReference<XmasMod2025>("KrampusVoice2").AssetGUID,
        GetAudioClipReference<XmasMod2025>("KrampusVoice3").AssetGUID);
    public static Voice SantaVoice = new Voice(GetAudioClipReference<XmasMod2025>("SantaVoice1").AssetGUID,
        GetAudioClipReference<XmasMod2025>("SantaVoice2").AssetGUID,
        GetAudioClipReference<XmasMod2025>("SantaVoice3").AssetGUID);

    
    public static Dialog Krampus(string emotion, string message, int round, DialogOption[] options = null) =>
        new Dialog("Krampus", message, GetSpriteReference<XmasMod2025>("Krampus" + emotion), round, KrampusVoice) {Options = options, Background = AssetHelper.GetSprite("KrampusPanel")};
    public static Dialog Santa (string emotion, string message, int round, DialogOption[] options = null, float textSpeed = 1, int charactersPerSound = 3) => 
        new Dialog("Santa Claus", message, GetSpriteReference<XmasMod2025>("Santa" + emotion), round, SantaVoice) {Options = options, Background = AssetHelper.GetSprite("ChristmasPanel"), CharactersPerSound = charactersPerSound};
    public static Dialog Player (string emotion, string message, int round, DialogOption[] options = null) => 
        new Dialog(PlayerName, message, GetSpriteReference<XmasMod2025>("Player" + emotion), round, Voice.Medium) {Options = options, Background = AssetHelper.GetSprite("ChristmasPanel")};
    public static Dialog ElfElder (string emotion, string message, int round, DialogOption[] options = null) => 
        new Dialog("Elf Elder", message, GetSpriteReference<XmasMod2025>("ElfElder" + emotion), round, Voice.Medium) {Options = options, Background = AssetHelper.GetSprite("ChristmasPanel")};

    public override void OnMatchStart()
    {
        /*if (InGame.instance.GetGameModel().gameMode != ModContent.GameModeId<ChristmasGameMode>())
        {
            DialogUi.instance.AddToDialogQueue(
                Santa("Happy", "To play this mod with the story, you are <b>required</b> to play on the Christmas Game Mode!", InGame.instance.bridge.GetCurrentRound() + 1)
            );
            return;
        }*/
        
        DialogUi.instance.AddToDialogQueue(
            [Santa("Happy", $"Welcome back {PlayerName}! It sure is great not needing to worry about the <b><color=#b7be19>Grinch</color></b> anymore.", 1),
            Santa("Happy", "I'm sure there'll be no other disturbances this year...", 1),
            Santa("Happy", "Right..?", 1, textSpeed: 20, charactersPerSound: 1),
            Player("Neutral", "Yeah! We kicked the <b><color=#b7be19>Grinch's</color></b> butt so hard last year that I bet he'll never return. And I don't think anyone else will try to ruin Christmas after that disaster.", 1),
            Santa("Happy", "Anyway I used a little bit of my Christmas magic to improve the look of the game!", 1),
            Santa("Worry", "Uh... what's that noise?", 4),
            Player("Confused", "What noise? I don't here anything", 4),
            Santa("Disappointed", "Maybe it was in my head then.", 4),
            Santa("Worry", "Yeah, it had to be in my head!", 4),
            Krampus("Yawn", "<i>Yawn</i> What a long rest... Well time to-", 5),
            Krampus("Neutral", "Oh, it's you Santa Claus.", 5),
            Santa("Worry", "Oh, K-Krampus, it's you...", 5),
            Krampus("Neutral", "I never understood why you enjoy spreading cheer so much. I find much more enjoyment in... punishing.", 5),
            Krampus("Neutral", "And, I think it's about time you get some punishment. I can't believe the Grinch failed last year.", 5),
            Santa("Worry", "Wait... Just what are you doing Krampus? What are you doing with that bag-", 5),
            Santa("Captured", "Hey! Get me out of here!", 5),
            Krampus("Neutral", $"I hope you won't miss your buddy here. Because you will never see him again, Santa Claus. Same goes for you {PlayerName}, don't even think about trying to save him this time.", 5),
            Player("Shocked", "Oh no! Santa's been captured this time!", 5),
            Player("Sad", "And I didn't even do anything to stop it...", 5),
            Player("Neutral", "Oh look. A wallet that just so happened to appear right where Santa was. Should I take it?", 5, [
                new GreenOption("Yes!", 
                    new Dialog(PlayerName, "Wow! 3,500 cash! What a find!", GetSpriteReference<XmasMod2025>("PlayerHappy"), 0) {Background = AssetHelper.GetSprite("ChristmasPanel") }
                ) { OnClick = () =>
                {
                    InGame.instance.AddCash(3500);
                }},
                new RedOption("No.", 
                    new Dialog(PlayerName, "I shouldn't. It's probably Santa's and he wouldn't give me any presents if I took his or anyone else's wallet.", GetSpriteReference<XmasMod2025>("PlayerNeutral"), 0) {Background = AssetHelper.GetSprite("ChristmasPanel") }
                )
            ]),
            ElfElder("Neutral", "Hey, have you seen santa anywhere? He's been gone for a bit...", 10),
            Player("Confused", "Yeah so about that...", 10),
            ElfElder("Neutral", "Did something bad happen to him? The Grinch didn't come back this year, right?", 10),
            Player("Neutral", "Well, there was this weird black furry guy with antlers who-", 10),
            ElfElder("Panicked", "Oh no! This is horribly bad! That person you saw was Krampus! He hates Santa, you must get Santa back immediately!", 10),
            ElfElder("Neutral", "You should know that Krampus is much more wise than the Grinch, and you should be very weary of him, since he definitely has a plan if you try to take Santa back.", 10),
            new Dialog("Big Ol' Present", "So many presents to make bad, it's like there's no end to them, how does Santa make so many of these?", GetSpriteReference("GiftBossIcon"), 19, Voice.MezzoSopranoFemale),
            new Dialog("Big Ol' Present", "At least I'm helping Krampus out.", GetSpriteReference("GiftBossIcon"), 19, Voice.MezzoSopranoFemale),
            Player("Confused", "Uh, WHO is that? I wonder if I should be scared.", 19),
            new Dialog("Big Ol' Present", "Don't mind me, I'm just coming through with some perfectly normal presents, ♪ laladee laladoo laladum ♪", GetSpriteReference("GiftBossIcon"), 20, Voice.MezzoSopranoFemale),
            Player("Shocked", "What?! I thought present bloons could only be big as moabs! I guess Krampus figured out how to make bigger ones.", 20),
            
        ]);
    }
}