using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TfsHistorical.Data
{
    /// <summary>
    /// Manage all iterations from a TFS project name
    /// </summary>
    public class Iterations
    {
        private string _projectName = string.Empty;
        private List<IterationItem> _items = new List<IterationItem>();

        /// <summary>
        /// Initializes a new instance of Iteration class for a TFS projet name.
        /// </summary>
        /// <param name="tfs"></param>
        /// <param name="projectName"></param>
        internal Iterations(TfsTeamProjectCollection tfs, string projectName)
        {
            this._projectName = projectName;

            ICommonStructureService css = (ICommonStructureService)tfs.GetService(typeof(ICommonStructureService));
            ProjectInfo projectInfo = css.GetProjectFromName(projectName);
            NodeInfo[] nodes = css.ListStructures(projectInfo.Uri);
            NodeInfo node = nodes.Where(item => item.StructureType == "ProjectLifecycle").Single();
            XmlElement IterationsTree = css.GetNodesXml(new string[] { node.Uri }, true);

            this.FillItemsList(IterationsTree.FirstChild);

        }

        /// <summary>
        /// Gets a list with all iterations items
        /// </summary>
        public IterationItem[] Items 
        { 
            get
            {
                return _items.ToArray();
            }
        }

        /// <summary>
        /// Fill the internal list of iterations items
        /// </summary>
        /// <param name="node"></param>
        private void FillItemsList(XmlNode node)
        {
            if (node != null)
            {
                if (IterationItem.IsValid(node))
                {
                    _items.Add(new IterationItem(this._projectName, node));
                }

                if (node.HasChildNodes)
                {
                    foreach (XmlNode subNode in node.ChildNodes)
                    {
                        this.FillItemsList(subNode);
                    }
                }
            }
        }

    }


}
