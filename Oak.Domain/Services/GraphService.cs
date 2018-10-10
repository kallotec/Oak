using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oak.Domain.Data;
using Oak.Domain.Mapping;
using Oak.Domain.Models;

namespace Oak.Domain.Services
{

    public class GraphService
    {
		public GraphService(ISchemaFactory SchemaGenerator)
        {
			schemaGenerator = SchemaGenerator;
        }

        ISchemaFactory schemaGenerator;


        public async Task<DbGraph> GenerateDbObjectGraph()
        {
            var graphItems = new List<DbObject>();

            //get schema
            var schema = await schemaGenerator.BuildSchemaAsync();

            //collect all db objects
            foreach (var obj in schema.Objects)
            {
                //ensure isnt already inserted
                var source = graphItems.FirstOrDefault(r => r.Name == obj.fullName);
                if (source == null)
                {
                    source = Mapper.Map(obj);
                    graphItems.Add(source);
                }
            }

            //merge into mapped lists
            foreach (var reference in schema.References)
            {
                //try get both source and target from graph
                var source = graphItems.FirstOrDefault(r => r.Name == reference.fromFullName);
                var target = graphItems.FirstOrDefault(r => r.Name == reference.toFullName);

                //ensure both from and to objects exist in object list
                //and that the source isnt referencing the target (causes infinite loop)
                if (source == null || target == null || source.Name == target.Name)
                    continue;

                //create reference from source to target and vice versa
                if (!source.DependsOn.Any(r => r.Name == target.Name))
                {
                    source.DependsOn.Add(target);
                    target.DependedOnBy.Add(source);
                }

            }

            var graphObj = new DbGraph
            {
                Objects = graphItems,
                CapturedUtc = DateTime.UtcNow,
            };

            return graphObj;
        }

		public Task<string> GetDefinition(string objName)
		{
			return schemaGenerator.GetDefinitionAsync(objName);
		}
        
	}
}
