using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models
{
    public class DbGraph
    {
        public DbGraph()
        {
            Objects = new List<DbObject>();
        }

        public List<DbObject> Objects { get; set; }
        public DateTime CapturedUtc { get; set; }
    }
}
