using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TfsHistorical.Data
{
    public static class TfsExtensions
    {
        /// <summary>
        /// Returns all iteration items for a specified project name
        /// </summary>
        /// <param name="projectCollection"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static Iterations GetIterations(this TfsTeamProjectCollection projectCollection, string projectName)
        {
            return new Data.Iterations(projectCollection, projectName);
        }

        /// <summary>
        /// Returns all work items for a specific iteration path and dates range.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="iterationPath"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static WorkItemTimeCollection GetWorkItems(this WorkItemStore store, string iterationPath, DateTime startDate, DateTime endDate)
        {
            return new Data.WorkItemTimeCollection(store, iterationPath, startDate, endDate);
        }

        /// <summary>
        /// Returns the collection items to a DataTable
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this WorkItemCollection collection)
        {
            DataTable table = new DataTable();

            foreach (FieldDefinition field in collection.DisplayFields)
	        {
                DataColumn newCol = new DataColumn(field.Name, field.SystemType);
                newCol.AllowDBNull = true;
                table.Columns.Add(newCol);
	        }

            foreach (WorkItem wi in collection)
            {
                DataRow row = table.NewRow();

                foreach (DataColumn column in table.Columns)
	            {
                    object value = wi.Fields.Contains(column.ColumnName) ? wi.Fields[column.ColumnName].Value : null;
                    row[column.ColumnName] = value == null ? DBNull.Value : value;    
	            }
                
                table.Rows.Add(row);
            }

            return table;
            
        }
    }
}
