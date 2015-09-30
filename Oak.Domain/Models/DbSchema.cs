using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models
{
    public class DbSchema
    {
        public DbSchema()
        {
            Objects = new List<DbObjectInfo>();
            References = new List<DbObjectReferenceRow>();
        }

        public List<DbObjectInfo> Objects { get; set; }
        public List<DbObjectReferenceRow> References { get; set; }
    }
}
