using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;


namespace play.skane.se
{
    public class videoArchive
    {
        public List<videoCategory> categories { get; set; }
    }
    public class videoCategory
    {
        public string name { get; set; }
        public List<videoItem> videos { get; set; }
    }
    public class videoItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public string videoStillURL { get; set; }
        public string thumbnailURL { get; set; }
        public string length { get; set; }
        public string playsTotal { get; set; }
        public string publishedDate { get; set; }
        public string startDate { get; set; }
    }

    public class buildVideoArchive
    {

        public videoArchive render()
        {
            return build();
        }
        private static string RSReadToken = "CIwPy3lSHFgnn3XNZBXpl1shqmtTfnHovkR3wdmyIil9k31SoLMy6g..";

        private videoArchive build()
        {
            videoArchive archive = new videoArchive();
            archive.categories = new List<videoCategory>();

            string BRSArchivePlayerBcId = "3222810931001";

            string videoFields = "id,name,shortDescription,longDescription,videoStillURL,thumbnailURL,length,playsTotal,publishedDate,startDate";

            var BRSRequest = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://api.brightcove.com/services/library?command=find_playlists_for_player_id&player_id={0}&video_fields={1}&token={2}", BRSArchivePlayerBcId, videoFields, RSReadToken));
            BRSRequest.Method = "POST";

            //Get BRS Account Items
            try
            {
                var response = BRSRequest.GetResponse();
                Stream dataStream = response.GetResponseStream();
                string BCResponseString = string.Empty;
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    BCResponseString = reader.ReadToEnd();

                    if (BCResponseString != null && BCResponseString != "null")
                    {
                        var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(BCResponseString);

                        foreach (dynamic category in results.items)
                        {
                            videoCategory cat = new videoCategory();
                            cat.name = category.name;
                            cat.videos = new List<videoItem>();
                            foreach (dynamic video in category.videos)
                            {
                                videoItem item = new videoItem();
                                item.id = video.id;
                                item.name = video.name.ToString().Replace("\"", "&quot");
                                item.length = video.length;
                                item.playsTotal = video.playsTotal;
                                item.thumbnailURL = video.thumbnailURL;
                                item.videoStillURL = video.videoStillURL;
                                item.publishedDate = video.publishedDate;
                                item.startDate = video.startDate;
                                item.shortDescription = video.shortDescription.ToString().Replace("\"", "&quot");
                                item.longDescription = video.longDescription.ToString().Replace("\"", "&quot");
                                cat.videos.Add(item);
                            }
                            archive.categories.Add(cat);
                        }
                    }
                }
            }
            catch { }
            return archive;
        }
    }
}