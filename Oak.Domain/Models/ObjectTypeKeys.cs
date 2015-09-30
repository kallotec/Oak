using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models 
{
	public static class ObjectTypeKeys 
	{
		public const string DBKEY_STOREDPROC = "P";
		public const string DBKEY_VIEW = "V";
		public const string DBKEY_TABLE = "U";
		public const string DBKEY_FUNCTION1 = "IF";
		public const string DBKEY_FUNCTION2 = "FN";
		public const string DBKEY_FUNCTION3 = "TF";

        public const string VIEWKEY_STOREDPROC = "P";
		public const string VIEWKEY_VIEW = "V";
		public const string VIEWKEY_TABLE = "T";
		public const string VIEWKEY_FUNCTION = "F";

        public const string VIEWKEY_UNKNOWN = "XXX";
    }
}
