using System;

namespace MTGG
{
    [Flags]
    public enum Features : uint
    {
        ChangeStatus = 0x00000001,
        ReceiveMessage = 0x00000002,
        ChangeStatus2 = 0x00000004,
        SupportStatus80 = 0x00000010,
        GraphicStatus = 0x00000020,
        UnknownLogin = 0x00000040,
        Unknown = 0x00000100,
        ExtInfoContact = 0x00000200,
        ConfirmReceive = 0x00000400,
        NotifyTalk = 0x00002000,
        MultiLogon = 0x00004000
    }
}