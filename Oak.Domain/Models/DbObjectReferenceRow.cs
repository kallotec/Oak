using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models
{
    public class DbObjectReferenceRow
    {
        public string fromSchema { get; set; }
        public string fromId { get; set; }
        public string from { get; set; }
        public string fromType { get; set; }
        public string toSchema { get; set; }
        public string toId { get; set; }
        public string to { get; set; }
        public string toType { get; set; }

        public string fromFullName
        {
            get { return string.Concat(fromSchema, ".", from); }
        }

        public string toFullName
        {
            get { return string.Concat(toSchema, ".", to); }
        }

    }
}
