using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace TfsHistorical.Data
{
    /// <summary>
    /// Class used to export data to different formats (csv,html,etc..)
    /// </summary>
    public class Export
    {
        #region DECLARATIONS

        private DataTable _data;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Default constructor
        /// </summary>
        public Export() { }

        /// <summary>
        /// Creates a new instance by using a datatable
        /// </summary>
        /// <param name="data">Data to export</param>
        public Export(DataTable data)
        {
            _data = data;
        }

        /// <summary>
        /// Init. a new instance of the exportation class
        /// </summary>
        /// <param name="grid">Grid to export</param>
        public Export(System.Windows.Forms.DataGridView grid)
        {
            _data = new DataTable(grid.Name);

            // Create columns
            foreach (System.Windows.Forms.DataGridViewColumn col in grid.Columns)
            {
                if (col.Visible)
                {
                    _data.Columns.Add(col.HeaderText);
                }
            }

            // Add data
            foreach (System.Windows.Forms.DataGridViewRow row in grid.Rows)
            {
                List<Object> items = new List<object>();
                foreach (System.Windows.Forms.DataGridViewCell item in row.Cells)
                {
                    if (grid.Columns[item.ColumnIndex].Visible)
                    {
                        DataGridViewImageCell itemImage = item as DataGridViewImageCell;
                        if (itemImage != null)
                        {
                            items.Add(itemImage.Value);
                        }
                        else
                        {
                            items.Add(item.FormattedValue);
                        }
                    }
                }
                _data.Rows.Add(items.ToArray());
            }

        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Get or set the data to export
        /// </summary>
        public DataTable Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Export to CSV
        /// </summary>
        /// <returns>CSV string</returns>
        public string ToCsv()
        {
            if (_data == null)
            {
                // No data to export
                throw new ArgumentException("No data to be exported was found.");
            }
            else
            {
                StringBuilder csvContent = new StringBuilder();

                // add the column name
                foreach (DataColumn column in _data.Columns)
                {
                    // if the data contains a semi colon, put double quote at the beginning and the end
                    if (column.ColumnName.Contains(";"))
                    {
                        csvContent.Append("\"" + column.ColumnName + "\";");
                    }
                    // No semi colon
                    else
                    {
                        csvContent.Append(column.ColumnName + ";");
                    }
                }
                csvContent.Append("\n");

                // Loop on each datarow of the datatable
                StringBuilder rowContent = new StringBuilder();
                foreach (DataRow row in _data.Rows)
                {
                    rowContent.Remove(0, rowContent.Length);
                    for (int i = 0; i < _data.Columns.Count; i++)
                    {
                        // if the data contains a semi colon, put double quote at the beginning and the end
                        if (row[i].ToString().Contains(";"))
                        {
                            rowContent.Append("\"" + row[i].ToString() + "\";");
                        }
                        else
                        {
                            rowContent.Append(row[i].ToString() + ";");
                        }
                    }
                    csvContent.AppendLine(rowContent.ToString());
                }
                return csvContent.ToString();
            }
        }

        /// <summary>
        /// Export to HTML
        /// </summary>
        /// <returns>HTML string</returns>
        public string ToHtml()
        {
            if (_data == null)
            {
                throw new ArgumentException("No data to be exported was found.");
            }
            else
            {
                StringBuilder htmlContent = new StringBuilder();

                htmlContent.AppendLine("<html>");
                htmlContent.AppendLine("\t<style>");
                htmlContent.AppendLine("\tbr {mso-data-placement:same-cell;}");
                htmlContent.AppendLine("\t</style>");
                htmlContent.AppendLine("\t<head>");
                htmlContent.AppendLine("\t\t<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />");
                htmlContent.AppendLine("\t</head>");
                htmlContent.AppendLine("\t<body>");
                htmlContent.AppendLine("\t\t<table border='1' cellspacing='0' cellpadding='2'>");

                // Add columns
                htmlContent.AppendLine("\t\t\t<tr>");
                foreach (DataColumn column in _data.Columns)
                {
                    if (String.IsNullOrEmpty(column.ColumnName))
                    {
                        htmlContent.AppendLine("\t\t\t\t<th>&nbsp;</th>");
                    }
                    else
                    {
                        htmlContent.AppendLine("\t\t\t\t<th>" + HttpUtility.HtmlEncode(column.ColumnName) + "</th>");
                    }
                }

                htmlContent.AppendLine("\t\t\t</tr>");

                // Add rows
                foreach (DataRow row in _data.Rows)
                {
                    htmlContent.AppendLine("\t\t\t<tr>");

                    foreach (object obj in row.ItemArray)
                    {
                        if (String.IsNullOrEmpty(obj.ToString()))
                        {
                            htmlContent.AppendLine("\t\t\t\t<td>&nbsp;</td>");
                        }
                        else if (obj is DateTime)
                        {
                            DateTime dte = (DateTime)obj;
                            if (dte.Hour == 0 || dte.Minute == 0)
                            {
                                htmlContent.AppendLine("\t\t\t\t<td>" + HttpUtility.HtmlEncode(dte.ToShortDateString()) + "</td>");
                            }
                            else
                            {
                                htmlContent.AppendLine("\t\t\t\t<td>" + HttpUtility.HtmlEncode(obj.ToString()) + "</td>");
                            }
                        }
                        else if (obj.ToString().Contains("€"))
                        {
                            htmlContent.AppendLine("\t\t\t\t<td>'" + HttpUtility.HtmlEncode(obj.ToString()).Replace("\n", "<br>") + "</td>");
                        }
                        else
                        {
                            htmlContent.AppendLine("\t\t\t\t<td>" + HttpUtility.HtmlEncode(obj.ToString()).Replace("\n", "<br>") + "</td>");
                        }
                    }

                    htmlContent.AppendLine("\t\t\t</tr>");
                }

                htmlContent.AppendLine("\t\t</table>");
                htmlContent.AppendLine("\t</body>");
                htmlContent.AppendLine("</html>");
                return htmlContent.ToString();
            }
        }

        /// <summary>
        /// Export to XML
        /// </summary>
        /// <returns>XML string</returns>
        public string ToXml()
        {
            if (_data == null)
            {
                throw new ArgumentException("No data to be exported was found.");
            }
            else
            {
                DataSet xmlContent = new DataSet("Data");
                using (xmlContent)
                {
                    xmlContent.Tables.Add(_data);
                    return xmlContent.GetXml();
                }
            }
        }

        #endregion

        public void CopyToClipBoard()
        {
            Encoding enc = Encoding.UTF8;
            string html = this.ToHtml();

            string begin = "Version:0.9\r\nStartHTML:{0:000000}\r\nEndHTML:{1:000000}"
                         + "\r\nStartFragment:{2:000000}\r\nEndFragment:{3:000000}\r\n";

            string html_begin = "<html>\r\n<head>\r\n"
                              + "<meta http-equiv=\"Content-Type\""
                              + " content=\"text/html; charset=" + enc.WebName + "\">\r\n"
                              + "<title>HTML clipboard</title>\r\n</head>\r\n<body>\r\n"
                              + "<!--StartFragment-->";

            string html_end = "<!--EndFragment-->\r\n</body>\r\n</html>\r\n";

            string begin_sample = String.Format(begin, 0, 0, 0, 0);

            int count_begin = enc.GetByteCount(begin_sample);
            int count_html_begin = enc.GetByteCount(html_begin);
            int count_html = enc.GetByteCount(html);
            int count_html_end = enc.GetByteCount(html_end);

            string html_total = String.Format(begin
                , count_begin
               , count_begin + count_html_begin + count_html + count_html_end
               , count_begin + count_html_begin
               , count_begin + count_html_begin + count_html
               ) + html_begin + html + html_end;

            DataObject obj = new DataObject();
            obj.SetData(DataFormats.Html, new System.IO.MemoryStream(enc.GetBytes(html_total)));
            Clipboard.SetDataObject(obj, true);
        }
    }
}
