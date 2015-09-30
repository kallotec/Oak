using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oak.Domain.Models;

namespace Oak.Domain.Data
{
    public class SchemaRepository
    {
        public void Insert(DbSchema schema)
        {
            ////connect to graph db
            //var db = new GraphClient(graphApiUrl);
            //db.Connect();

            ////clear graph db
            //if (persistToGraphDb)
            //    await db.Cypher.Match("(n)")
            //                    .OptionalMatch("(n)-[r]-()")
            //                    .With("n,r")
            //                    .Limit(100000)
            //                    .Delete("n,r")
            //                    .ExecuteWithoutResultsAsync();

            ////merge into mapped lists
            //foreach (var record in referenceTable)
            //{
            //    //source
            //    var source = refTable.FirstOrDefault(r => r.Name == record.name);
            //    if (source == null)
            //    {
            //        ////insert to db
            //        //if (persistToGraphDb)
            //        //    await db.Cypher.CreateUnique("(entity:Entity {newEntity})")
            //        //                   .WithParam("newEntity", source)
            //        //                   .ExecuteWithoutResultsAsync();
            //    }

            //    //target
            //    var target = refTable.FirstOrDefault(r => r.Name == record.target);
            //    if (target == null)
            //    {
            //        ////insert to db
            //        //if (persistToGraphDb)
            //        //    await db.Cypher.CreateUnique("(entity:Entity {newEntity})")
            //        //                   .WithParam("newEntity", target)
            //        //                   .ExecuteWithoutResultsAsync();
            //    }

            //    //determine relationship between nodes
            //    if (record.referencing)
            //    {
            //        //create reference to target if not exists
            //        if (!source.DependsOn.Any(r => r.Name == target.Name))
            //        {
            //            ////insert relationships to db
            //            //if (persistToGraphDb)
            //            //{
            //            //    await db.Cypher.Match("(entity1:Entity)", "(entity2:Entity)")
            //            //                   .Where((DbObject entity1) => entity1.Name == source.Name)
            //            //                   .AndWhere((DbObject entity2) => entity2.Name == target.Name)
            //            //                   .CreateUnique("entity1-[:DEPENDS_ON]->entity2")
            //            //                   .ExecuteWithoutResultsAsync();
            //            //    await db.Cypher.Match("(entity1:Entity)", "(entity2:Entity)")
            //            //                   .Where((DbObject entity1) => entity1.Name == source.Name)
            //            //                   .AndWhere((DbObject entity2) => entity2.Name == target.Name)
            //            //                   .CreateUnique("entity2-[:DEPENDED_ON_BY]->entity1")
            //            //                   .ExecuteWithoutResultsAsync();
            //            //}
            //        }
            //    }
            //    else
            //    {
            //        //create reference to target if not exists
            //        if (!source.DependsOn.Any(r => r.Name == target.Name))
            //        {
            //            ////insert relationship to db
            //            //if (persistToGraphDb)
            //            //{
            //            //    await db.Cypher.Match("(entity1:Entity)", "(entity2:Entity)")
            //            //                   .Where((DbObject entity1) => entity1.Name == source.Name)
            //            //                   .AndWhere((DbObject entity2) => entity2.Name == target.Name)
            //            //                   .CreateUnique("entity2-[:DEPENDS_ON]->entity1")
            //            //                   .ExecuteWithoutResultsAsync();
            //            //    await db.Cypher.Match("(entity1:Entity)", "(entity2:Entity)")
            //            //                   .Where((DbObject entity1) => entity1.Name == source.Name)
            //            //                   .AndWhere((DbObject entity2) => entity2.Name == target.Name)
            //            //                   .CreateUnique("entity1-[:DEPENDED_ON_BY]->entity2")
            //            //                   .ExecuteWithoutResultsAsync();
            //            //}
            //        }
            //    }

            //}

        }
    }
}
