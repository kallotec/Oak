using Oak.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oak.UI.Web.Models.Api
{
    public class AutocompleteResult
    {
        public string Name { get; set; }
        public ObjectType Type { get; set; }
        public string TypeName { get; set; }
    }
}