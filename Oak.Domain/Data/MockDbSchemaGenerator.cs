using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oak.Domain.Models;

namespace Oak.Domain.Data
{
    public class MockDbSchemaGenerator : ISchemaFactory
    {
        public MockDbSchemaGenerator(string PathToSpFile, string PathToReferencesFile)
        {
            pathToSpFile = PathToSpFile;
            pathToReferencesFile = PathToReferencesFile;
        }

        string pathToSpFile;
        string pathToReferencesFile;
        DbSchema schemaCached;


        public async Task<DbSchema> BuildSchemaAsync()
        {
            if (schemaCached != null)
                return schemaCached;

            schemaCached = new DbSchema();

            //read stored procs file into the schema objects list
            var spsFileLines = File.ReadAllLines(pathToSpFile);
            foreach (var line in spsFileLines)
            {
                var obj = readCsvLineAsObject(line);
                if (obj != null)
                    schemaCached.Objects.Add(obj);
            }

            //read references file into the schema references list
            var refsFileLines = File.ReadAllLines(pathToReferencesFile);
            foreach (var line in refsFileLines)
            {
                var reff = readCsvLineAsReference(line);
                if (reff != null)
                    schemaCached.References.Add(reff);
            }

            return schemaCached;
        }

		public Task<string> GetDefinition(string objectName) 
		{
			throw new NotImplementedException();
		}

        DbObjectInfo readCsvLineAsObject(string csvLine)
        {
            if (string.IsNullOrWhiteSpace(csvLine))
                return null;

            var cells = csvLine.Split(',');

            return new DbObjectInfo
            {
                name = cells[0],
                objectId = cells[1],
                schemaName = cells[2],
                fullName = cells[3]
            };
        }

        DbObjectReferenceRow readCsvLineAsReference(string csvLine)
        {
            if (string.IsNullOrWhiteSpace(csvLine))
                return null;

            var cells = csvLine.Split(',');

            return new DbObjectReferenceRow
            {
                fromSchema = cells[0],
                fromId = cells[1],
                from = cells[2],
                toSchema = cells[3],
                toId = cells[4],
                to = cells[5],
            };
        }

	}
}
