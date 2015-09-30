using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oak.UI.Web.Models.Api
{
    public class CallTreeData
    {
        public CallTreeData() 
        {
			objects = new Dictionary<string, string[]>();
			metadata = new Dictionary<string, ObjectData>();
        }

		public Dictionary<string, string[]> objects { get; set; }
		public Dictionary<string, ObjectData> metadata { get; set; }

    }
}