using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oak.UI.Web.Models.Api 
{
	public class Node
	{
		public Node(string Name, string Type)
        {
            name = Name;
            type = Type;
            depends = new List<string>();
            dependedOnBy = new List<string>();
        }

        public string name { get; set; }
        public string type { get; set; }
        public List<string> depends { get; set; }
        public List<string> dependedOnBy { get; set; }
	}
}