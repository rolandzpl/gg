using System;

namespace MTGG
{
    [Flags]
    public enum Channel : uint
    {
        None = 0x00000000,
        Audio = 0x00000001,
        Video = 0x00000002,
        Mobile = 0x00100000,
        AllowUnknownContacts = 0x00800000
    }
}