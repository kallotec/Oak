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
    public enum CallTreeDirection { Down, Up }

    public class GraphService
    {
		public GraphService(ISchemaFactory SchemaGenerator)
        {
			schemaGenerator = SchemaGenerator;
        }

        ISchemaFactory schemaGenerator;


        public async Task<List<DbObject>> GetCallTree(string spName, CallTreeDirection direction)
        {
            var graph = await GenerateGraph();
            var tested = new List<string>();

            if (direction == CallTreeDirection.Up)
            {
                var newObjectList = trimForUpwardOnlyReferences(graph.Objects, spName);
                graph.Objects = newObjectList;
                return graph.Objects;
            }
            else
            {
                var rootObj = graph.Objects.FirstOrDefault(o => o.Name == spName);
                if (rootObj != null)
                    return new List<DbObject> { rootObj };
                else
                    return new List<DbObject>();
            }

        }

		public Task<string> GetDefinition(string objName) 
		{
			return schemaGenerator.GetDefinition(objName);
		}

        public async Task<List<DbObject>> GetAutocompleteObjectList(ObjectType? filter = null)
        {
            var graph = await GenerateGraph();

            //only return SP's in autocomplete list
            var filteredToSps = graph.Objects
                                     .Where(o => (filter != null 
                                        ? o.ObjectType == filter 
                                        : o.ObjectType != ObjectType.Unknown))
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
                CapturedUtc = DateTime.UtcNow,
            };

            return graphObj;
        }

        async Task<DbSchema> getSchema()
        {
			var schema = await schemaGenerator.BuildSchemaAsync();
            return schema;
        }

        List<DbObject> trimForUpwardOnlyReferences(List<DbObject> list, string objectName)
        {
            if (list == null)
                return new List<DbObject>();

            var target = list.FirstOrDefault(l => l.Name == objectName);
            if (target == null)
                return list;

            var dependentObjNames = new List<string>();
            getDependentObjectsNames(target, dependentObjNames);

            // Remove all redundant dependent links
            list.ForEach(r =>
            {
                for (int x = 0; x < r.DependsOn.Count; x++)
                {
                    var obj = r.DependsOn[x];
                    if (!dependentObjNames.Contains(obj.Name))
                    {
                        r.DependsOn.RemoveAt(x);
                        x--;
                    }
                }
                
            });

            // Only get dependent links
            var results = list.Where(l => dependentObjNames.Contains(l.Name)).ToList();

            return results;
        }

        void getDependentObjectsNames(DbObject obj, List<string> collector)
        {
            collector.Add(obj.Name);
            obj.DependedOnBy.ForEach(d => getDependentObjectsNames(d, collector));
        }

	}
}
