using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using Oak.Domain.Models;

namespace Oak.Domain.Data
{
    public class DbSchemaFactory : ISchemaFactory
    {
		public DbSchemaFactory(Uri GraphApiUrl, string SqlConnString, bool PersistToGraphDb = false)
        {
            sqlConnString = SqlConnString;
            graphApiUrl = GraphApiUrl;
            persistToGraphDb = PersistToGraphDb;
        }

        string sqlConnString;
        Uri graphApiUrl;
        bool persistToGraphDb;
		DbSchema schema;


        public async Task<DbSchema> BuildSchemaAsync()
        {
			if (schema != null)
				return schema;

            List<DbObjectInfo> objectInfoList = null;
            List<DbObjectReferenceRow> referenceTable = null;

            //get reference table from sql
            using (IDbConnection conn = new SqlConnection(sqlConnString))
            {
                objectInfoList = (await conn.QueryAsync<DbObjectInfo>(getQuery_SpData())).ToList();
                referenceTable = (await conn.QueryAsync<DbObjectReferenceRow>(getQuery_SpReferenceTable())).ToList();
            }
            
            //refTable complete
			return schema = new DbSchema 
            {
                Objects = objectInfoList,
                References = referenceTable,
            };
        }

		public async Task<string> GetDefinition(string objectName) {

			var definition = string.Empty;

			//get reference table from sql
			using (IDbConnection conn = new SqlConnection(sqlConnString)) {

				var reader = await conn.ExecuteReaderAsync(getQuery_ObjDefinition(objectName));
                var sb = new StringBuilder();

                while (reader.Read())
                    sb.Append(reader.GetString(0));

                definition = string.Join(Environment.NewLine, sb.ToString());
			}
            
			return definition;
		}

        string getQuery_SpData()
        {
            return @"
                    DECLARE @sps TABLE (name VARCHAR(255), objectType VARCHAR(3), objectId INT, schemaName VARCHAR(20), fullName VARCHAR(255))

                    --get all user created sp's in db instance
                    INSERT INTO @sps (name, objectType, objectId, schemaName, fullName)
                    SELECT so.name, LTRIM(RTRIM(so.type)), so.object_id, OBJECT_SCHEMA_NAME(so.object_id), (OBJECT_SCHEMA_NAME(so.object_id) + '.' + so.name)
                    FROM sys.objects so
                    WHERE so.type = 'P' OR so.type = 'U' OR so.type = 'V' OR so.type = 'FN' OR so.type = 'IF' OR so.type = 'TF'

                    SELECT * FROM @sps";
        }

        string getQuery_SpReferenceTable()
        {
            return @"
                    DECLARE @sps TABLE (name VARCHAR(255), objectType VARCHAR(3), objectId INT, schemaName VARCHAR(20), fullName VARCHAR(255))

                    --get all user created sp's in db instance
                    INSERT INTO @sps (name, objectType, objectId, schemaName, fullName)
                    SELECT so.name, LTRIM(RTRIM(so.type)), so.object_id, OBJECT_SCHEMA_NAME(so.object_id), (OBJECT_SCHEMA_NAME(so.object_id) + '.' + so.name)
                    FROM sys.objects so
                    WHERE so.type = 'P' OR so.type = 'U' OR so.type = 'V' OR so.type = 'FN' OR so.type = 'IF' OR so.type = 'TF'

                    --collate all references between only the above db objects
                    DECLARE @references TABLE (
	                    [fromId] int, 
	                    [fromType] VARCHAR(3),
	                    [toId] int,
	                    [toType] VARCHAR(3)
                    )

                    INSERT INTO 
	                   @references ([fromId], [fromType], [toId], [toType])
                    SELECT 
	                    d.referencing_id, 
	                    sp.objectType,
	                    sp2.objectId,
	                    sp2.objectType
                    FROM 
	                    sys.sql_expression_dependencies d
					INNER JOIN
						@sps sp ON sp.objectId = d.referencing_id --enforce that it is a user created SP
					INNER JOIN
						@sps sp2 ON sp2.name = d.referenced_entity_name --enforce that it is a user created SP
                    WHERE
	                   d.referenced_class = 1 
						AND d.referencing_class = 1
					
                    SELECT 
	                    OBJECT_SCHEMA_NAME(r.[fromId]) as [fromSchema], 
	                    r.[fromId], 
	                    OBJECT_NAME(r.[fromId]) as [from], 
	                    r.[fromType],
	                    OBJECT_SCHEMA_NAME(r.[toId]) as [toSchema], 
	                    r.[toId],
	                    OBJECT_NAME(r.[toId]) as [to],
	                    r.[toType]
                    FROM 
	                    @references r";
        }

		string getQuery_ObjDefinition(string objectName) {
			return @"select OBJECT_DEFINITION(object_id('" + objectName + "'))";
		}

    }
}
