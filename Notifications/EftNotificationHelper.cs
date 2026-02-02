using EFT.Communications;

namespace SwiftXP.SPT.Common.Notifications;

public static class EftNotificationHelper
{
    public static void SendAlert(string message)
    {
        Send(message, ENotificationDurationType.Default, ENotificationIconType.Alert);
    }

    public static void SendLongAlert(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Alert);
    }

    public static void SendNotice(string message)
    {
        Send(message, ENotificationDurationType.Default, ENotificationIconType.Default);
    }

    public static void SendLongNotice(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Default);
    }

#pragma warning disable CA1822 // Mark members as static

    private static void Send(string message, ENotificationDurationType duration, ENotificationIconType icon)
#pragma warning restore CA1822 // Mark members as static

    {
        GClass2551 updatedPricesMessage = new(
            message,
            duration,
            icon
        );

        NotificationManagerClass.DisplayNotification(updatedPricesMessage);
    }
}