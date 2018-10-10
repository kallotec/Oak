using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Oak.Domain.Models
{
    public class DbGraph
    {
        public DbGraph()
        {
            Objects = new List<DbObject>();
        }

        public IReadOnlyList<DbObject> Objects { get; set; }
        public DateTime CapturedUtc { get; set; }

        public List<DbObject> GetCallTree(string objName, CallTreeDirection direction)
        {
            var tested = new List<string>();

            if (direction == CallTreeDirection.Up)
            {
                var newObjectList = trimForUpwardOnlyReferences(this.Objects, objName);
                return newObjectList;
            }
            else
            {
                var rootObj = this.Objects.FirstOrDefault(o => string.Equals(o.Name, objName, StringComparison.OrdinalIgnoreCase));
                if (rootObj != null)
                    return new List<DbObject> { rootObj };
                else
                    return new List<DbObject>();
            }

        }

        List<DbObject> trimForUpwardOnlyReferences(IReadOnlyList<DbObject> list, string objectName)
        {
            if (list == null)
                return new List<DbObject>();

            var target = list.FirstOrDefault(l => string.Equals(l.Name, objectName, StringComparison.OrdinalIgnoreCase));
            if (target == null)
                return new List<DbObject>();

            var dependentObjNames = new List<string>();
            getDependentObjectsNames(target, dependentObjNames);

            // Remove all redundant dependent links
            foreach (var item in list)
            {
                for (var x = 0; x < item.DependsOn.Count; x++)
                {
                    var obj = item.DependsOn[x];
                    if (!dependentObjNames.Contains(obj.Name))
                    {
                        item.DependsOn.RemoveAt(x);
                        x--;
                    }
                }
            }

            // Only get dependent links
            var results = list.Where(l => dependentObjNames.Contains(l.Name)).ToList();

            return results;
        }

        void getDependentObjectsNames(DbObject obj, List<string> collector)
        {
            collector.Add(obj.Name);
            obj.DependedOnBy.ForEach(d => getDependentObjectsNames(d, collector));
        }

        public enum CallTreeDirection { Down, Up }
    }
}
