using Comfort.Common;
using EFT;

namespace SwiftXP.SPT.Common.EFT;

public static class EFTHelper
{
    public static bool IsInRaid
    {
        get
        {
            bool? inRaid = Singleton<AbstractGame>.Instance?.InRaid;

            return inRaid.HasValue && inRaid.Value;
        }
    }
}