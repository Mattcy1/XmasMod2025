using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using DialogLib;
using DialogLib.Ui;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using XmasMod2025.Assets;
using XmasMod2025.Bloons;
using static BTD_Mod_Helper.Api.ModContent;

namespace XmasMod2025;

public partial class XmasMod2025
{
    public static Voice KrampusVoice = new(GetAudioClipReference<XmasMod2025>("KrampusVoice1").AssetGUID,
        GetAudioClipReference<XmasMod2025>("KrampusVoice2").AssetGUID,
        GetAudioClipReference<XmasMod2025>("KrampusVoice3").AssetGUID);

    public static Voice SantaVoice = new(GetAudioClipReference<XmasMod2025>("SantaVoice1").AssetGUID,
        GetAudioClipReference<XmasMod2025>("SantaVoice2").AssetGUID,
        GetAudioClipReference<XmasMod2025>("SantaVoice3").AssetGUID);

    public static bool TookWallet;

    private static SpriteReference GetSpriteReference(string name)
    {
        return GetSpriteReference<XmasMod2025>(name);
    }


    public static Dialog Krampus(string emotion, string message, int round, DialogOption[] options = null)
    {
        return new Dialog("Krampus", message, GetSpriteReference<XmasMod2025>("Krampus" + emotion), round, KrampusVoice)
            { Options = options, Background = AssetHelper.GetSprite("KrampusPanel") };
    }

    public static Dialog Santa(string emotion, string message, int round, DialogOption[] options = null,
        float textSpeed = 1, int charactersPerSound = 3)
    {
        return new Dialog("Santa Claus", message, GetSpriteReference<XmasMod2025>("Santa" + emotion), round, SantaVoice)
        {
            Options = options, Background = AssetHelper.GetSprite("ChristmasPanel"),
            CharactersPerSound = charactersPerSound
        };
    }

    public static Dialog Player(string emotion, string message, int round, DialogOption[] options = null)
    {
        return new Dialog(PlayerName, message, GetSpriteReference<XmasMod2025>("Player" + emotion), round, Voice.Medium)
            { Options = options, Background = AssetHelper.GetSprite("ChristmasPanel") };
    }

    public static Dialog ElfElder(string emotion, string message, int round, DialogOption[] options = null)
    {
        return new Dialog("Elf Elder", message, GetSpriteReference<XmasMod2025>("ElfElder" + emotion), round, Voice.Low)
            { Options = options, Background = AssetHelper.GetSprite("ChristmasPanel") };
    }

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
            Santa("Happy",
                $"Welcome back {PlayerName}! It sure is great not needing to worry about the <b><color=#b7be19>Grinch</color></b> anymore.",
                1), Santa("Happy", "I'm sure there'll be no other disturbances this year...", 1),
            Santa("Happy", "Right..?", 1, textSpeed: 20, charactersPerSound: 1),
            Player("Neutral",
                "Yeah! We kicked the <b><color=#b7be19>Grinch's</color></b> butt so hard last year that I bet he'll never return. And I don't think anyone else will try to ruin Christmas after that disaster.",
                1),
            Santa("Happy", "Anyway I used a little bit of my Christmas magic to improve the look of the game!", 1),
            Santa("Worry", "Uh... what's that noise?", 4), Player("Confused", "What noise? I don't here anything", 4),
            Santa("Disappointed", "Maybe it was in my head then.", 4),
            Santa("Worry", "Yeah, it had to be in my head!", 4),
            Krampus("Yawn", "<i>Yawn</i> What a long rest... Well time to-", 5),
            Krampus("Neutral", "Oh, it's you Santa Claus.", 5), Santa("Worry", "Oh, K-Krampus, it's you...", 5),
            Krampus("Neutral",
                "I never understood why you enjoy spreading cheer so much. I find much more enjoyment in... punishing.",
                5),
            Krampus("Neutral",
                "And, I think it's about time you get some punishment. I can't believe the Grinch failed last year.",
                5), Santa("Worry", "Wait... Just what are you doing Krampus? What are you doing with that bag-", 5),
            Santa("Captured", "Hey! Get me out of here!", 5),
            Krampus("Neutral",
                $"I hope you won't miss your buddy here. Because you will never see him again, Santa Claus. Same goes for you {PlayerName}, don't even think about trying to save him this time.",
                5), Player("Shocked", "Oh no! Santa's been captured this time!", 5),
            Player("Disappointed", "And I didn't even do anything to stop it...", 5), Player("Neutral",
                "Oh look. A wallet that just so happened to appear right where Santa was. Should I take it?", 5, [
                    new GreenOption("Yes!",
                        Player("Happy", "Wow! 3,500 cash! What a find!", 0)
                    )
                    {
                        OnClick = () =>
                        {
                            InGame.instance.AddCash(3500);
                            TookWallet = true;
                        }
                    },
                    new RedOption("No.",
                        new Dialog(PlayerName,
                                "I shouldn't. It's probably Santa's and he wouldn't give me any presents if I took his or anyone else's wallet.",
                                GetSpriteReference<XmasMod2025>("PlayerNeutral"), 0)
                            { Background = AssetHelper.GetSprite("ChristmasPanel") }
                    )
                ]), ElfElder("Neutral", "Hey, have you seen santa anywhere? He's been gone for a bit...", 10),
            Player("Confused", "Yeah so about that...", 10),
            ElfElder("Neutral", "Did something bad happen to him? The Grinch didn't come back this year, right?", 10),
            Player("Neutral", "Well, there was this weird black furry guy with antlers who-", 10),
            ElfElder("Panicked",
                "Oh no! This is horribly bad! That person you saw was Krampus! He hates Santa, you must get Santa back immediately!",
                10),
            ElfElder("Neutral",
                "You should know that Krampus is much more wise than the Grinch, and you should be very weary of him, since he definitely has a plan if you try to take Santa back.",
                10),
            new Dialog("Big Ol' Present",
                    "So many presents to make bad, it's like there's no end to them, how does Santa make so many of these?",
                    GetSpriteReference("GiftBossIcon"), 19, Voice.MezzoSopranoFemale)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            new Dialog("Big Ol' Present", "At least I'm helping Krampus out.", GetSpriteReference("GiftBossIcon"), 19,
                Voice.MezzoSopranoFemale) { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Confused", "Uh, WHO is that? I wonder if I should be scared.", 19),
            new Dialog("Big Ol' Present",
                    "Don't mind me, I'm just coming through with some perfectly normal presents, ♪ laladee laladoo laladum ♪",
                    GetSpriteReference("GiftBossIcon"), 20, Voice.MezzoSopranoFemale)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Shocked",
                "What?! I thought present bloons could only be big as moabs! I guess Krampus figured out how to make bigger ones.",
                20), Player("Neutral", "Well he was pretty easy to defeat. he was just a big present after all...", 21),
            ElfElder("Neutral",
                "Good job you took that big present down! Make sure you keep beating bosses to ensure you can save Santa!",
                25),
            ElfElder("Neutral",
                "You should know however that the next boss coming can only be damaged by elves. In fact, they were an elf before they got corrupted by the bloons and become one themself!",
                25),
            ElfElder("Neutral",
                "For whatever reason Santa's magic makes it so elves can only be hurt by bloons and other elves, and not by other monkeys.",
                25),
            ElfElder("Neutral",
                "Some snowmen decided to help us too! They'll generate gifts for you, all you need to do is place them down.",
                25),
            new Dialog("Evil Elf",
                    "That darn Santa finally got what he deserved I see. He had it coming for him after not paying us elves enough...",
                    GetSpriteReference("ElfBoss-Icon"), 39, Voice.BassMale)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            new Dialog("Evil Elf", "...Or anything at all really.", GetSpriteReference("ElfBoss-Icon"), 39,
                Voice.BassMale) { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Disappointed", "Wasn't this brought up last year..?", 39),
            Player("Disappointed", "I guess it finally caught up to the other elves.", 39),
            new Dialog("Evil Elf",
                    "Aren't you the person who saved Santa last year? Don't even try it this year there's no way you'll do it.",
                    GetSpriteReference("ElfBoss-Icon"), 40, Voice.BassMale)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            new Dialog("Evil Elf", "I mean, you probably can't even beat me let alone Krampus!",
                    GetSpriteReference("ElfBoss-Icon"), 39, Voice.BassMale)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Neutral", "We'll see after I smoke you!", 40),
            ElfElder("Neutral", "Good job, you beat that elf!", 41),
            ElfElder("Neutral", "I never really like that one before he went rogue anyway...", 41),
            ElfElder("Neutral", "I should warn you about the next boss I suppose.", 45),
            ElfElder("Neutral",
                "This one is made out of chocolate, and is pretty annoying to deal with since each skull will stun nearby towers!",
                45),
            Player("Confused",
                "Is there any way I can maybe... make my towers not get stunned. Like drink a milk bucket or something?",
                45), ElfElder("Neutral", "I'm afraid not! You'll just have to do this all on your own.", 45),
            new Dialog("Chocolaty Choco", "Chocolate, chocolate I love chocolate!",
                    GetSpriteReference("ChocolateBossIcon"), 59, Voice.High)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            new Dialog("Chocolaty Choco", "But as a chocolate, I don't love being eaten by others who love chocolate",
                    GetSpriteReference("ChocolateBossIcon"), 59, Voice.High)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Neutral", "Oh boy, I do love chocolate too!", 59),
            new Dialog("Chocolaty Choco", "I heard what you said earlier! I'll have to stop you if you try eating me!",
                    GetSpriteReference("ChocolateBossIcon"), 60, Voice.High)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Confused", "Oh wow, it really is made of chocolate. I've never seen something like that before.",
                60), ElfElder("Neutral", "So have you finished your chocolate yet?", 70),
            Player("Happy", "Yes! It was very delicious.", 70),
            ElfElder("Neutral", "Well, I'm happy you enjoyed it. Though it's time I tell you about the next boss.", 70),
            ElfElder("Neutral",
                "This one is a massive stocking filled with coal that came to life from the sadness that game with getting coal from Krampus!",
                70),
            ElfElder("Neutral",
                "This negative energy can get imposed on the bloons, making them sad and go away from the boss.", 70),
            ElfElder("Neutral", "Just like last time, you kind of just need to deal with it.", 70),
            Player("Neutral", "Oh, well thanks for the heads up anyway.", 70),
            new Dialog("Crying Coal", "Why? Oh why? Why was I given coal? I never deserved this...",
                    GetSpriteReference("CoalBossIcon"), 79, Voice.Low)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            new Dialog("Crying Coal",
                    "I don't know who gave me this coal, but I'm assuming it's Santa, right? He's the one who goes to everyone's houses.",
                    GetSpriteReference("CoalBossIcon"), 79, Voice.Low)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Confused", "Does this guy not know about what Krampus is all about?", 79),
            new Dialog("Crying Coal", "Aren't you the one who wants to save Santa, just like you did last year?",
                    GetSpriteReference("CoalBossIcon"), 80, Voice.Low)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            new Dialog("Crying Coal",
                    "Look at what Santa gave to me! I'll hate him forever for this! And I hate you by extension that means!",
                    GetSpriteReference("CoalBossIcon"), 80, Voice.Low)
                { Background = AssetHelper.GetSprite("KrampusPanel") },
            Player("Neutral", "I don't think Santa gave you the coal, but whatever.", 80),
            new Dialog("Elf Elder", "Oh no! It became night, that means Krampus is up to something.",
                    GetSpriteReference("ElfElderPanicked"), 90, Voice.Low)
                { Background = AssetHelper.GetSprite("ChristmasPanel"), OnThis = PostProcessing.EnableNight },
            ElfElder("Panicked", "I'm not sure how long this night will last, but you should beware!", 90),
            new Dialog("Elf Elder",
                    "Phew, it seems night ended. Though, chances are Krampus's plan will ensue on the next night.",
                    GetSpriteReference("ElfElderNeutral"), 93, Voice.Low)
                { Background = AssetHelper.GetSprite("ChristmasPanel"), OnThis = PostProcessing.DisableNight },
            Player("Worry", "I'm scared now!", 93),
            Krampus("Neutral", "Didn't I tell you not to try and save Santa?", 98),
            Santa("Captured", "Please help me " + PlayerName + "!", 98),
            Santa("Captured", "I don't think Krampus has very nice things planed for me!", 98),
            Krampus("Neutral", "You be quiet Santa, you'll never be saved.", 98),
            Player("Neutral", "Don't think I won't take you down Krampus.", 98),
            Krampus("Yawn", "Well, time to finally finish this I suppose.", 100),
            ElfElder("Panicked", $"{PlayerName} you need to stop Krampus and save Santa now!", 100),
            Player("Confused", "Uh, that's no Krampus, that's a bloon...", 100),
            Krampus("Neutral",
                "Of course it's not me. You think I'm dumb enough to fully charge into battle like that idiot Grinch did?",
                100), Krampus("Neutral", "That was his biggest mistake.", 100),
            Player("Neutral", "Sounds like you're just a scaredy cat to me.", 100));
    }

    public static void ShowEndDialogue()
    {
        DialogUi.instance.ShowQueue(new Queue<Dialog>([
            Krampus("Neutral", "Impossible! How did you defeat my greatest creation of all time!", 100),
            Player("Happy", "I'm just too good I guess.", 100),
            Santa("Happy", $"Thanks for Saving me again, {PlayerName}. I really do owe you one.", 100),
            Player("Neutral", "Yeah, you do owe me one. Like maybe some extra presents under the Christmas tree?", 100),
            Santa("Disappointed",
                "Fine then... But only because I'm such a nice and jolly man and I pay my elves so well", 100),
            ElfElder("Neutral", "Don't pretend that you do.", 100),
            Santa("Happy", "Oh, c'mon you know that I'm a great employer.", 100),
            new Dialog("Santa Claus", "But remember, Merry Christmas (or Happy Holidays) everyone! Ho ho ho...",
                    GetSpriteReference("SantaHappy"), 100, SantaVoice)
                { OnNext = () => RoundStaller.AliveStaller?.Destroy() }
        ]));
    }
}