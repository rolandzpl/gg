using System.Collections.Generic;
using System.Text;
using MTGG.Packets;

namespace MTGG.Packets
{
    internal class UserDataPacket : Packet
    {
        public UserDataPacket()
        {
            this.userData = new List<UserData>();
        }
 
        public UserData[] Attributes
        {
            get { return this.userData.ToArray(); }
        }
 
 
        public override void Read()
        {
            base.Read();
 
            uint type = this.reader.ReadUInt32();
            uint count = this.reader.ReadUInt32();
 
            for (uint i = 0; i < count; ++i)
            {
                uint number = this.reader.ReadUInt32();
                UserData data = new UserData();
                data.Number = number;
                this.userData.Add(data);
 
                uint count2 = this.reader.ReadUInt32();
 
                for (uint j = 0; j < count2; ++j)
                {
                    int len = this.reader.ReadInt32();
                    string name = UTF8Encoding.UTF8.GetString(this.reader.ReadBytes(len));
                    uint typeAttr = this.reader.ReadUInt32();
                    len = this.reader.ReadInt32();
                    string value = UTF8Encoding.UTF8.GetString(this.reader.ReadBytes(len));
 
                    UserDataAttributes attr = new UserDataAttributes();
                    attr.Name = name;
                    attr.Type = typeAttr;
                    attr.Value = value;
 
                    data.Attributes.Add(attr);
                }
            }
        }

        private List<UserData> userData;
    }
}
