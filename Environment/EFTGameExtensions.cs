using BepInEx.Bootstrap;
using Comfort.Common;
using EFT;
using SPT.Reflection.Utils;

namespace SwiftXP.SPT.Common.EFT;

public static class EFTGameExtensions
{
    public static bool IsFikaHeadlessInstalled()
    {
        return Chainloader.PluginInfos.ContainsKey("com.fika.headless");
    }

    public static bool IsInRaid
    {
        get
        {
            bool? inRaid = Singleton<AbstractGame>.Instance?.InRaid;

            return inRaid.HasValue && inRaid.Value;
        }
    }

    public static ISession Session => ClientAppUtils.GetMainApp().GetClientBackEndSession();
}