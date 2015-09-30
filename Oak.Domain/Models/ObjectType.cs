using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models 
{
	public enum ObjectType {
		Unknown = 0,
		StoredProc = 1,
		Table = 2,
		View = 3,
		Function = 4,
	}
}
