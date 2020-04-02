using System.Collections.Generic;

namespace MTGG.Packets
{
    public class UserData
    {
        public UserData()
        {
            this.Attributes = new List<UserDataAttributes>();
        }

        public uint Number
        {
            get;
            set;
        }

        public List<UserDataAttributes> Attributes
        {
            get;
            private set;
        }
    }
}