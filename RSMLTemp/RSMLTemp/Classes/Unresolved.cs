using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace RSMLTemp.Classes
{
    class Unresolved
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string DeviceId { get; set; }

        public string SuspiciousActivities { get; set; }

        public string _Date { get; set; }

        public double TimeOccured { get; set; }

        public int StoreNumber { get; set; }

        public string StoreName { get; set; }
    }
}
