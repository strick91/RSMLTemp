using System;
using System.Collections.Generic;
using System.Text;

namespace RSMLTemp.Classes
{
    class ConfirmedDevicesInStore
    {
        public int Id { get; set; }

        public string DeviceId { get; set; }

        public string LastSeenDepartment { get; set; }

        public DateTime LastSeenTime { get; set; }
    }
}
