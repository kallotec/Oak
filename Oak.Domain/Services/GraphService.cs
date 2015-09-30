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


        public async Task<DbObject> GetCallTree(string spName)
        {
            var graph = await GenerateGraph();
            var tested = new List<string>();

            foreach (var obj in graph.Objects)
            {
                if (obj.Name == spName)
                    return obj;
            }

            return null;
        }

		public Task<string> GetDefinition(string objName) 
		{
			return schemaGenerator.GetDefinition(objName);
		}

		public async Task<List<string>> GetAutocompleteObjectList()
		{
            var graph = await GenerateGraph();

			//only return SP's in autocomplete list
            var filteredToSps = graph.Objects
									 .Where(o => o.ObjectType == ObjectType.StoredProc || o.ObjectType == ObjectType.Function)
									 .Select(o => o.Name)
									 .ToList();
			return filteredToSps;
		}

        public async Task<DbGraph> GenerateGraph()
        {
            //get schema
            var schema = await getSchema();
            List<DbObjectInfo> objectInfos = schema.Objects;
            List<DbObjectReferenceRow> refTable = schema.References;

            List<DbObject> graph = new List<DbObject>();

            //collect all db objects
            foreach (var obj in objectInfos)
            {
                //ensure isnt already inserted
                var source = graph.FirstOrDefault(r => r.Name == obj.fullName);
                if (source == null)
                {
                    source = Mapper.Map(obj);
                    graph.Add(source);
                }
            }

            //merge into mapped lists
            foreach (var reference in refTable)
            {
                //try get both source and target from graph
                var source = graph.FirstOrDefault(r => r.Name == reference.fromFullName);
                var target = graph.FirstOrDefault(r => r.Name == reference.toFullName);

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
                Objects = graph,
            };

            return graphObj;
        }

        async Task<DbSchema> getSchema()
        {
			var schema = await schemaGenerator.BuildSchemaAsync();
            return schema;
        }


	}
}
