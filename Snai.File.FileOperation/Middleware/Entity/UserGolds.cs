using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snai.File.FileOperation.Middleware.Entity
{
    public class UserGolds
    {
        public UserGolds()
        {
            RecordDate = new DateTime(1970, 01, 01);
            UserID = 0;
            Golds = 0;
        }

        public DateTime RecordDate { get; set; }
        public int UserID { get; set; }
        public int Golds { get; set; }
    }
}
