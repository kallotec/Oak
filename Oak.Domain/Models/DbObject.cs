using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models
{
    public class DbObject
    {
        public DbObject()
        {
            DependsOn = new List<DbObject>();
            DependedOnBy = new List<DbObject>();

        }

        public string Name { get; set; }
        public ObjectType ObjectType { get; set; }
        public string ObjectTypeKey { get; set; }
        public List<DbObject> DependsOn { get; set; }
        public List<DbObject> DependedOnBy { get; set; }

		public override string ToString() 
		{
			return string.Format("Name={0} Type={1}", Name, Enum.GetName(typeof(ObjectType), ObjectType));
		}

    }
}
