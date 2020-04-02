using System;

namespace MTGG
{
    [Flags]
    internal enum ClassMessage : uint
    {
        Queued = 0x0001,
        Msg = 0x0004,
        Chat = 0x0008,
        CTCP = 0x0010,
        ACK = 0x0020
    }
}