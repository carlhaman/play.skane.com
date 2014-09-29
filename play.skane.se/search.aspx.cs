using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;

namespace play.skane.se
{
    public partial class search : System.Web.UI.Page
    {
        index searcher = new index();
        string searchString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(Request.QueryString["index"]))
            {
                string indexString = HttpUtility.UrlDecode(Request.QueryString.GetValues("index").GetValue(0).ToString());
                if (indexString == "create")
                {
                    searcher.buildIndex();
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ContentType = "application/json";
                    Response.Charset = "UTF-8";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.Write("{index: \"created\"}");
                    Response.End();
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["query"]))
            {
                //searchString = Uri.EscapeUriString(Request.QueryString.GetValues("query").GetValue(0).ToString());
                searchString = HttpUtility.UrlDecode(Request.QueryString.GetValues("query").GetValue(0).ToString());
            }

            if (!string.IsNullOrEmpty(searchString))
            {

                Response.Clear();
                Response.ClearHeaders();
                Response.ContentType = "application/json";
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(doSearch(searchString));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ContentType = "application/json";
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write("[]");
                Response.End();
            }
        }
        private string doSearch(string query)
        {
            List<indexVideo> articles = searcher.searchArticles(query, 100);

            //DataTable aTable = new DataTable();
            //aTable.Columns.Add("bcid", typeof(string));
            //aTable.Columns.Add("score", typeof(float));
            //aTable.Columns.Add("title", typeof(string));
            //aTable.Columns.Add("shortDescription", typeof(string));
            //aTable.Columns.Add("longDescription", typeof(string));
            //aTable.Columns.Add("imageURL", typeof(string));

            //if (articles.Count > 0)
            //{
            //    foreach (indexVideo a in articles)
            //    {
            //        aTable.Rows.Add(a.bcid, a.score, a.title, a.shortDescription, a.longDescription, a.imageURL);
            //    }
            //}
            //DataView aView = aTable.DefaultView;
            //aView.Sort = "score DESC";
            //DataTable sortedTable = aView.ToTable();

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //sb.AppendLine("<div class=\"searchResults\">");

            //if ((articles.Count == 0))
            //{
            //    sb.AppendLine("<div class=\"searchArticle\"><p>Inga sökträffar för <span class=\"searchQuery\">" + searchString + "</span></p></div>");
            //}

            //if (sortedTable.Rows.Count >= 1)
            //{
            //    for (int i = 0; i <= (sortedTable.Rows.Count - 1); i++)
            //    {
            //        DataRow r = sortedTable.Rows[i];
            //        renderSearchArticle(sb, r["bcid"].ToString(), r["title"].ToString(), r["shortDescription"].ToString(), r["longDescription"].ToString(), r["imageURL"].ToString());
            //    }
            //}




            //sb.AppendLine("</div>");

            //string response = sb.ToString();
            string response = JsonConvert.SerializeObject(articles);
            return response;
        }

        private void renderSearchArticle(System.Text.StringBuilder sb, string bcid, string title, string shortDescription, string longDescription, string imageURL)
        {
            sb.AppendLine("<div class=\"searchArticle\">");
            sb.AppendLine("<h1>" + title + "</h1>");
            sb.AppendLine("<img src=\"" + imageURL + "\"/>");
            sb.AppendLine("<p>" + shortDescription + "</p>");
            sb.AppendLine("<p>" + longDescription + "</p>");
            sb.AppendLine("<div class=\"links\"><a href=\"default.aspx?bctid=" + bcid + "\">Video &raquo;</a></div>");
            sb.AppendLine("</div>");

        }
    }
}
