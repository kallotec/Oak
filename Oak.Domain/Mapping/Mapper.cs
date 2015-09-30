using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oak.Domain.Models;

namespace Oak.Domain.Mapping
{
    public static class Mapper
    {
        public static DbObject Map(DbObjectInfo from)
        {
            if (from == null)
                return null;

            DbObject to = new DbObject();
            to.Name = from.fullName;

            if (!string.IsNullOrEmpty(from.objectType))
            {
                //determine object type from objType key
                switch (from.objectType.Trim())
                {
                    case ObjectTypeKeys.DBKEY_STOREDPROC:
                        to.ObjectType = ObjectType.StoredProc;
                        to.ObjectTypeKey = ObjectTypeKeys.VIEWKEY_STOREDPROC;
                        break;
                    case ObjectTypeKeys.DBKEY_TABLE:
                        to.ObjectType = ObjectType.Table;
                        to.ObjectTypeKey = ObjectTypeKeys.VIEWKEY_TABLE;
						break;
					case ObjectTypeKeys.DBKEY_VIEW:
						to.ObjectType = ObjectType.View;
						to.ObjectTypeKey = ObjectTypeKeys.VIEWKEY_VIEW;
						break;
					case ObjectTypeKeys.DBKEY_FUNCTION1:
					case ObjectTypeKeys.DBKEY_FUNCTION2:
					case ObjectTypeKeys.DBKEY_FUNCTION3:
						to.ObjectType = ObjectType.Function;
						to.ObjectTypeKey = ObjectTypeKeys.VIEWKEY_FUNCTION;
						break;
                }

            }

            return to;
        }

    }
}
