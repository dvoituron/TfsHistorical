using System;
using System.Diagnostics;
using System.Xml;

namespace TfsHistorical.Data
{
    [DebuggerDisplay("{Path} - {StartDate} - {EndDate}")]
    public class IterationItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="node"></param>
        internal IterationItem(string projectName, XmlNode node)
        {
            string projectIteration = "\\" + projectName + "\\Iteration";

            // Name
            if (node.Attributes["Name"] != null)
            {
                this.Name = node.Attributes["Name"].Value;
            }
            else
            {
                this.Name = string.Empty;
            }

            // Path
            if (node.Attributes["Path"] != null)
            {
                this.Path = node.Attributes["Path"].Value.Replace(projectIteration, projectName);
            }
            else
            {
                this.Path = string.Empty;
            }

            // StartDate
            if (node.Attributes["StartDate"] != null)
            { 
                this.StartDate = DateTime.Parse(node.Attributes["StartDate"].Value);
            }
            else
            {
                this.StartDate = null;
            }

            // FinishDate
            if (node.Attributes["FinishDate"] != null)
            {
                this.EndDate = DateTime.Parse(node.Attributes["FinishDate"].Value);
            }
            else
            {
                this.EndDate = null;
            }
            
        }

        /// <summary>
        /// Gets the iteration name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the iteration full path
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the Iteration Start date
        /// </summary>
        public DateTime? StartDate { get; private set; }

        /// <summary>
        /// Gets the Iteration Finish date
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// Returns True if this node is a valid Iteration item
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsValid(XmlNode node)
        {
            return node.Attributes["Name"] != null;
        }
    }
}
