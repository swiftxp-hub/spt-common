using SPT.Reflection.Utils;

namespace SwiftXP.SPT.Common.Sessions;

public static class SptSession
{
    public static ISession Session => ClientAppUtils.GetMainApp().GetClientBackEndSession();
}