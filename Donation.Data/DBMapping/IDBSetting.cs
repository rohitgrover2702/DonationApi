using System;
using System.Collections.Generic;
using System.Text;

namespace Donation.Data.DBMapping
{
   public interface IDBSetting
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
