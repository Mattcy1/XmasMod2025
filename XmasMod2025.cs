using BTD_Mod_Helper;
using MelonLoader;
using XmasMod2025;

[assembly: MelonInfo(typeof(XmasMod2025.XmasMod2025), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace XmasMod2025;

public class XmasMod2025 : BloonsTD6Mod
{
    private static int gifts;

    public static int Gifts
    {
        get => gifts;
        set
        {
            int increase = value - gifts;
            gifts = value;
            if (increase > 0)
            {
                totalGifts += increase;
            }
        }
    }

    private static int totalGifts;

    public static int TotalGifts
    {
        get => totalGifts;
    }
}