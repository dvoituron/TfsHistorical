using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsHistorical.Data
{
    public class TfsProject
    {
        private Data.Iterations _iterations = null;
        private TfsTeamProjectCollection _projectCollection = null;
        private string _projectName = string.Empty;

        /// <summary>
        /// Gets a reference to the current TFS Projects Collection
        /// </summary>
        public TfsTeamProjectCollection ProjectCollection 
        {
            get
            {
                return _projectCollection;
            }
            private set
            {
                _projectCollection = value;
                _iterations = null;
            }
        }

        /// <summary>
        /// Gets or sets the name of current project
        /// </summary>
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            private set
            {
                _projectName = value;
                _iterations = null;
            }
        }

        /// <summary>
        /// Gets all iterations for the current TFS project
        /// </summary>
        public Data.Iterations Iterations
        {
            get
            {
                if (_iterations == null)
                {
                    _iterations = new Data.Iterations(this.ProjectCollection, this.ProjectName);
                }
                return _iterations;
            }
        }

        /// <summary>
        /// Go to the specified URI to open a new connection to this TFS Server Collection
        /// </summary>
        /// <param name="serverUri"></param>
        /// <param name="projectName"></param>
        public void Open(string serverUri, string projectName)
        {
            this.ProjectCollection = new TfsTeamProjectCollection(new Uri(serverUri));
            this.ProjectName = projectName;            
        }

        /// <summary>
        /// Show the TFS Dialog Picker to selected a project in a project collection
        /// </summary>
        public void ShowDialog()
        {            
            TeamProjectPicker tfsDialog = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false);
            if (tfsDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ProjectCollection = tfsDialog.SelectedTeamProjectCollection;

                if (tfsDialog.SelectedProjects.Any())
                {
                    this.ProjectName = tfsDialog.SelectedProjects[0].Name;
                }
            }
        }

    }
}
