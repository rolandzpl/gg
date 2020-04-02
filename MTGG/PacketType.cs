namespace MTGG
{
    internal enum PacketType : uint
    {
        Welcome = 0x0001,
        Login80 = 0x0031,
        Login80_OK = 0x0035,
        Login80_Fail = 0x0045,
        Login_Fail = 0x0009,
        Status = 0x0036,
        ChangeStatus = 0x0038,
        EmptyContactList = 0x0012,
        NotifyFirst = 0x000F,
        NotifyLast = 0x0010,
        NotifyReply80 = 0x0037,
        NotifyAdd = 0x000D,
        NotifyRemove = 0x000E,
        UserData = 0x0044,
        SendMessage = 0x002D,
        SendMessageAck = 0x0005,
        RecvMessage = 0x002E,
        RecvMessageAck = 0x0046,
        Ping = 0x0008,
        Pong = 0x0007,
        Disconnect = 0x000B,
        DisconnectACK = 0x000D,
        TypingNotify = 0x0059,
        RecvOwnMessage = 0x005A,
        SessionsInfo = 0x005B,
        SessionDisconnect = 0x0062,
        PubDirRequest = 0x0014,
        PubDirReply = 0x000E,
        ContactListRequest = 0x0040,
        ContactListReply = 0x0041
    }
}