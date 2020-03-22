using System;
using System.Collections.Generic;
using System.Text;

namespace Donation.Data.DBMapping
{
   public class DBSetting:IDBSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    
}
