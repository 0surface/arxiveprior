using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace arx.Extract.Data.Common
{
    public static class QueryFilterUtil
    {
        public static TableQuery<T> AndQueryFilters<T>(List<QueryFilterCondition> filterConditions)
            where T : TableEntity
        {
            if (filterConditions == null || filterConditions.Count() == 0)
            {
                return new TableQuery<T>();
            }
            else if (filterConditions.Count == 1)
            {
                return new TableQuery<T>().Where(GetFilterString(filterConditions[0]));
            }

            string combined = GetFilterString(filterConditions[0]);
            foreach (var item in filterConditions.Skip(1))
            {
                var filter = GetFilterString(item);
                combined = TableQuery.CombineFilters(combined, TableOperators.And, filter);
            }

            return new TableQuery<T>().Where(combined);
        }

        private static string GetFilterString(QueryFilterCondition item)
        {
            switch (item.PrimitiveTypeName)
            {
                case "int":
                    return TableQuery.GenerateFilterConditionForInt
                        (item.PropertyName, item.Operation, (int)item.GivenValue);

                case "datetime":
                    var value = new DateTimeOffset((DateTime)item.GivenValue);
                    return TableQuery.GenerateFilterConditionForDate
                        (item.PropertyName, item.Operation, value);

                case "bool":
                    return TableQuery.GenerateFilterConditionForBool
                        (item.PropertyName, item.Operation, (bool)item.GivenValue);

                case "double":
                    return TableQuery.GenerateFilterConditionForDouble
                        (item.PropertyName, item.Operation, (double)item.GivenValue);

                case "guid":
                    return TableQuery.GenerateFilterConditionForGuid
                        (item.PropertyName, item.Operation, (Guid)item.GivenValue);

                default:
                    return TableQuery.GenerateFilterCondition
                        (item.PropertyName, item.Operation, (string)item.GivenValue);
            }
        }
    }

    public class QueryFilterCondition
    {
        public string PrimitiveTypeName { get; set; }
        public string PropertyName { get; set; }
        public string Operation { get; set; }
        public object GivenValue { get; set; }

        public QueryFilterCondition(string primitiveTypeName, string propertyName, string operation, object value)
        {
            PrimitiveTypeName = primitiveTypeName;
            PropertyName = propertyName;
            Operation = operation;
            GivenValue = value;
        }

    }

}
