namespace MTGG
{
    public enum GGStatus : uint
    {
        NotAvailable = 0x0001,
        NotAvailableDescription = 0x0015,
        TalkToMe = 0x0017,
        TalkToMeDescription = 0x0018,
        Available = 0x0002,
        AvailableDescription = 0x0004,
        Busy = 0x0003,
        BusyDescription = 0x0005,
        Disturb = 0x0021,
        DisturbDescription = 0x0022,
        Invisible = 0x0014,
        InvisibleDescription = 0x0016,
        Blocked = 0x0006,
        ImageMask = 0x0100,
        AdaptStatus = 0x0400,
        DescriptionMask = 0x4000,
        FriendsMask = 0x8000
    }
}