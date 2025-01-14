using System.ComponentModel;

namespace BarkerSharper.Data;

public enum NotificationLevel
{
    [Description("critical")] Critical,
    [Description("active")] Active,
    [Description("timeSensitive")] TimeSensitive,
    [Description("passive")] Passive
}

public enum NotificationSound
{
    [Description("alarm")] Alarm,

    [Description("anticipate")] Anticipate,

    [Description("bell")] Bell,

    [Description("birdsong")] Birdsong,

    [Description("bloom")] Bloom,

    [Description("calypso")] Calypso,

    [Description("chime")] Chime,

    [Description("choo")] Choo,

    [Description("descent")] Descent,

    [Description("electronic")] Electronic,

    [Description("fanfare")] Fanfare,

    [Description("glass")] Glass,

    [Description("gotosleep")] GoToSleep,

    [Description("healthnotification")] HealthNotification,

    [Description("horn")] Horn,

    [Description("ladder")] Ladder,

    [Description("mailsent")] MailSent,

    [Description("minuet")] Minuet,

    [Description("multiwayinvitation")] MultiwayInvitation,

    [Description("newmail")] NewMail,

    [Description("newsflash")] NewsFlash,

    [Description("noir")] Noir,

    [Description("paymentsuccess")] PaymentSuccess,

    [Description("shake")] Shake,

    [Description("sherwoodforest")] SherwoodForest,

    [Description("silence")] Silence,

    [Description("spell")] Spell,

    [Description("suspense")] Suspense,

    [Description("telegraph")] Telegraph,

    [Description("tiptoes")] Tiptoes,

    [Description("typewriters")] Typewriters,

    [Description("update")] Update
}