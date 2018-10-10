using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oak.Domain.Models;

namespace Oak.Domain.Data
{
    public interface ISchemaFactory
    {
        Task<DbSchema> BuildSchemaAsync();
		Task<string> GetDefinitionAsync(string objectName);
    }
}
