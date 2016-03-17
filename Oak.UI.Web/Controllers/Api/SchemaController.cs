using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Oak.Domain.Models;
using Oak.Domain.Services;
using Oak.UI.Web.Models.Api;

namespace Oak.UI.Web.Controllers.Api
{
    [RoutePrefix("api/schema")]
    public class SchemaController : ApiController
    {
        public SchemaController(GraphService GraphService)
        {
            graphService = GraphService;
		}

		GraphService graphService;


		// GET api/schema/dependencytree/?objName={value}
        [HttpGet]
        [Route("dependencytree")]
		public async Task<IHttpActionResult> GetDependencyTree(string objName)
        {
			try 
			{
				//generate graph table
				var graph = await graphService.GenerateGraph();
				var callTree = new CallTreeData();

				if (string.IsNullOrEmpty(objName))
					return Ok(callTree);

				//get call tree from object if found
				var obj = await graphService.GetCallTree(objName);
				if (obj != null) {
					var dic = new Dictionary<string, string[]>();
					buildDependencyDic(obj, dic);

					foreach (var entry in dic) {
						//add object entry
						callTree.objects.Add(entry.Key, entry.Value);

						//add meta data entry for object
						var item = graph.Objects.FirstOrDefault(o => o.Name == entry.Key);
						if (item != null)
							callTree.metadata.Add(entry.Key, new ObjectData { type = item.ObjectTypeKey });
					}
				}

				return Ok(callTree);
			} 
			catch (Exception ex) 
			{
				return InternalServerError(ex);
			}
        }

		// GET api/schema/defintion/?objName={value}
		[HttpGet]
		[Route("definition")]
		public async Task<IHttpActionResult> GetDefinition(string objName) 
		{
			try 
			{
				//generate graph table
				var definition = new ObjectDefinition();

				if (string.IsNullOrEmpty(objName))
					return Ok(definition);

				//get object definition from db
				definition.DefinitionText = await graphService.GetDefinition(objName);

				return Ok(definition);
			} 
			catch (Exception ex) 
			{
				return InternalServerError(ex);
			}
		}


        // GET api/schema/autocomplete
        [HttpGet]
        [Route("autocomplete")]
        public async Task<IHttpActionResult> GetAutocomplete()
        {
			try 
			{
				//get results of search
				var results = await graphService.GetAutocompleteObjectList();

				return Ok(results);
			} 
			catch (Exception ex) 
			{
				return InternalServerError(ex);
			}
        }


        // GET api/schema/environments
        [HttpGet]
        [Route("environments")]
        public async Task<IHttpActionResult> GetEnvironments() {
			try 
			{ 
				var conns = getAllConnectionStringNames();
				return Ok(conns);
			} 
			catch (Exception ex) 
			{
				return InternalServerError(ex);
			}
        }


		void buildDependencyDic(DbObject obj, Dictionary<string, string[]> dic)
        {
			Debug.WriteLine("Processing: " + obj.Name);

            if (dic.ContainsKey(obj.Name) == false)
                dic.Add(obj.Name, obj.DependsOn.Select(d => d.Name).ToArray());
            else
                return;

            foreach (var dependency in obj.DependsOn) 
			{
				buildDependencyDic(dependency, dic);
            }

        }

        //// GET api/graphing/calltree/?spName={value}
        //[HttpGet]
        //[Route("calltree")]
        //public async Task<IHttpActionResult> GetCallTree(string spName)
        //{
        //    //config values
        //    var sqlConnString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        //    //generate graph table
        //    var sp = await graphService.GetCallTree(spName);

        //    //handle no results found
        //    if (sp == null)
        //        return Ok(new { });

        //    var top = new CallTreeItem(sp.Name);
        //    top.children = getChildren(sp);

        //    return Ok(top);
        //}

        //[HttpGet]
        //[Route("schema2")]
        //public async Task<IHttpActionResult> GetSchema2()
        //{
        //    //config values
        //    var sqlConnString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        //    //generate graph table
        //    var schema = await graphService.GenerateSchemaAsync();


        //    var nodes = new List<NodeResult>();
        //    var rels = new List<object>();
        //    int i = 0, sourceIndex;

        //    foreach (var item in schema.Objects)
        //    {
        //        nodes.Add(new NodeResult { title = item.fullName, label = "SP" });
        //        sourceIndex = i;
        //        i++;

        //        var references = schema.References.Where(r => r.from == item.name);

        //        foreach (var refr in references)
        //        {
        //            var targetIndex = nodes.FindIndex(n => n.title == refr.to);
        //            if (targetIndex == -1)
        //            {
        //                nodes.Add(new NodeResult { title = refr.to, label = "actor" });
        //                targetIndex = i;
        //                i += 1;
        //            }

        //            rels.Add(new { source = sourceIndex, target = targetIndex });
        //        }
        //    }

        //    return Ok(new { nodes = nodes, links = rels });
        //}

        //// GET api/graphing/schema
        //[HttpGet]
        //[Route("schema3")]
        //public async Task<IHttpActionResult> GetSchema3()
        //{
        //    //generate graph table
        //    var schema = await graphService.GenerateSchemaAsync();

        //    var nodes = new List<Node>();

        //    foreach (var item in schema.Objects)
        //    {
        //        var node = new Node(item.fullName, "SP");

        //        var dependencies = schema.References.Where(r => r.fromFullName == item.fullName);
        //        var dependedOnBy = schema.References.Where(r => r.toFullName == item.fullName);

        //        node.depends.AddRange(dependencies.Select(r => r.toFullName));
        //        node.dependedOnBy.AddRange(dependedOnBy.Select(r => r.fromFullName));

        //        nodes.Add(node);
        //    }

        //    var dataDic = new Dictionary<string, object>();

        //    foreach (var node in nodes)
        //        dataDic.Add(node.name, node);

        //    var data = new { data = dataDic, errors = new List<string>() };

        //    return Ok(data);
        //}

        List<string> getAllConnectionStringNames()
        {
            var conns = new List<string>();

            foreach (var conn in ConfigurationManager.ConnectionStrings)
            {
                break;
            }

            return conns;
        }

        public class NodeResult
        {
            public string title { get; set; }
            public string label { get; set; }
        }
    }
}
