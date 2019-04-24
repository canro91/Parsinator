using System;
using System.Data;

namespace Parsinator
{
    public static class DataSetExtensions
    {
        public static DataTable WithColumn(this DataTable table, String name, MappingType type = MappingType.Attribute)
        {
            table.Columns.Add(new DataColumn(name, typeof(string)) { ColumnMapping = type });
            return table;
        }

        public static DataTable Empty(this DataTable table)
        {
            table.Columns.Add(new DataColumn(table.NameForEmptyColumn(), typeof(string))
            {
                ColumnMapping = MappingType.Hidden,
                DefaultValue = "_"
            });

            return table;
        }

        private static String NameForEmptyColumn(this DataTable table)
        {
            return $"{table.TableName}-Empty";
        }

        public static DataSet WithRelation(this DataSet dataSet, String parentTableName, String childTableName)
        {
            if (!dataSet.Tables.Contains(parentTableName))
                throw new ArgumentNullException($"DataSet [{dataSet.DataSetName}] doesn't contain table [{parentTableName}]");
            if (!dataSet.Tables.Contains(childTableName))
                throw new ArgumentNullException($"DataSet [{dataSet.DataSetName}] doesn't contain table [{childTableName}]");

            // HACK Create a column with a default value in each table,
            // so we can create a relation from the two tables
            var relationName = $"{parentTableName}-{childTableName}";
            var @default = 1;
            var parentColumn = new DataColumn($"{relationName}-relationColumn")
            {
                ColumnMapping = MappingType.Hidden,
                DefaultValue = @default
            };
            var childColumn = new DataColumn($"{relationName}-relationColumn")
            {
                ColumnMapping = MappingType.Hidden,
                DefaultValue = @default
            };

            var parent = dataSet.Tables[parentTableName];
            parent.Columns.Add(parentColumn);

            var child = dataSet.Tables[childTableName];
            child.Columns.Add(childColumn);

            // HACK Since we want a parent node without attributes, we create
            // a table with a dummy column. So we check if the parent table
            // contains the dummy column and add a row, there isn't already one
            if (parent.Columns.Contains(parent.NameForEmptyColumn())
                    && parent.Rows.Count == 0)
            {
                parent.Rows.Add(parent.NewRow());
            }

            if (child.Columns.Contains(child.NameForEmptyColumn()))
            {
                child.Rows.Add(child.NewRow());
            }

            var relation = new DataRelation(relationName, parentColumn, childColumn)
            {
                Nested = true
            };

            dataSet.Relations.Add(relation);
            return dataSet;
        }

        public static DataSet WithTable(this DataSet dataSet, DataTable table)
        {
            dataSet.Tables.Add(table);
            return dataSet;
        }
    }
}
