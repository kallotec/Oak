using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models
{
    public class DbObjectInfo
    {
        public string name { get; set; }
        public string objectId { get; set; }
        public string objectType { get; set; }
        public string schemaName { get; set; }
        public string fullName { get; set; }
    }
}
