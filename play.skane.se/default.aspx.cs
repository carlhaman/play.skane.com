using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.Caching;

namespace play.skane.se
{
    public partial class _default : System.Web.UI.Page
    {
        private videoArchive _archive;
        bool frontpage = true;

        protected void Page_Load(object sender, EventArgs e)
        {

            string archiveString = "Archive";

            _archive = (videoArchive)Cache[archiveString];
            if (_archive == null)
            {
                buildVideoArchive builder = new buildVideoArchive();
                _archive = builder.render();
                Cache.Insert(archiveString, _archive, null, DateTime.UtcNow.AddMinutes(10), Cache.NoSlidingExpiration);
            }

            //För att inte indexera staging-server
            if (!Request.Url.Host.ToString().Contains("play.skane.com"))
            {
                HtmlMeta robotMeta = new HtmlMeta();
                robotMeta.Name = "ROBOTS";
                robotMeta.Content = "NOINDEX, NOFOLLOW";
                Page.Header.Controls.Add(robotMeta);
            }

            string queryId = string.Empty;

            if (Request.QueryString["bctid"] != null)
            {
                frontpage = false;
                queryId = Request.QueryString.GetValues("bctid").GetValue(0).ToString();
                findVideoInArchive(queryId);

            }
            else
            {
                getLatestVideo();
                setStandardMeta();
            }

            renderVideoArchive(_archive);

        }

        private void setStandardMeta()
        {
            string title = "play.skane.com";
            string description = "Välkommen till play.skane.com.";
            string logoURL = Request.Url.GetLeftPart(UriPartial.Authority) + "/Images/Logga.jpg";

            StringBuilder twitter = new StringBuilder();
            StringBuilder facebook = new StringBuilder();

            twitter.AppendLine("<meta name=\"twitter:title\" content=\"" + title + "\" />");
            facebook.AppendLine("<meta property=\"og:title\" content=\"" + title + "\"/>");
            twitter.AppendLine("<meta name=\"twitter:description\" content=\"" + description + "\" />");
            facebook.AppendLine("<meta property=\"og:description\" content=\"" + description + "\" />");
            twitter.AppendLine("<meta name=\"twitter:image:src\" content=\"" + logoURL + "\" />");
            facebook.AppendLine("<meta property=\"og:image\" content=\"" + logoURL + "\" />");
            facebook.AppendLine("<meta property=\"og:type\" content=\"website\"/>");
            twitter.AppendLine("<meta name=\"twitter:card\" content=\"summary\" />");
            twitter.AppendLine("<meta name=\"twitter:url\" content=\"http://play.skane.com\" />");
            facebook.AppendLine("<meta property=\"og:url\" content=\"http://play.skane.com\" />");

            fbMeta.Text = facebook.ToString();
            twMeta.Text = twitter.ToString();

        }
        private void renderVideoArchive(videoArchive archive)
        {
            string archiveString = "archiveHtml";
            string html = (string)Cache[archiveString];

            if (html == null)
            {
                StringBuilder menuBuilder = new StringBuilder();
                StringBuilder clipBuilder = new StringBuilder();

                //prepare menu
                menuBuilder.AppendLine("<div class=\"categories\">\n");
                menuBuilder.AppendLine("\t<ul>\n");

                //prepare clips
                clipBuilder.AppendLine("<div class=\"video-listing\"></div>\n");
                int categoryId = 1;

                foreach (videoCategory category in archive.categories)
                {
                    menuBuilder.AppendLine("<li><a href=\"javascript:showCategory(playlistId_" + categoryId.ToString() + ");\">" + category.name + "</a></li>\n");

                    clipBuilder.AppendLine("<div id=\"playlistId_" + categoryId.ToString() + "\" style=\"display:none;\">\n");

                    foreach (videoItem video in category.videos)
                    {
                        string description = string.Empty;
                        if (video.longDescription != null && video.longDescription != "") { description = video.longDescription; }
                        else { description = video.shortDescription; };

                        clipBuilder.AppendLine("<div class=\"post\">\n");
                        clipBuilder.AppendLine("<a href=\"?bctid=" + video.id + "\" class=\"videoBox\">");
                        clipBuilder.AppendLine("<div class=\"info-box\"><h2>" + video.name + "</h2><img src=\"" + video.videoStillURL + "\"/><p>" + description + "</p></div>");
                        clipBuilder.AppendLine("\t<div class=\"item-video\">\n");
                        //clipBuilder.AppendLine("\t\t<img src=\"" + video.thumbnailURL + "\" alt=\"" + video.shortDescription + "\">\n");
                        clipBuilder.AppendLine("\t\t<img src=\"" + video.videoStillURL + "\" alt=\"" + video.shortDescription + "\">\n");
                        clipBuilder.AppendLine("\t</div>\n");
                        clipBuilder.AppendLine("\t<h3>" + video.name + "</h3>");
                        clipBuilder.AppendLine("</a>");
                        clipBuilder.AppendLine("</div>\n");


                    }
                    clipBuilder.AppendLine("</div>\n");

                    categoryId++;
                }

                menuBuilder.AppendLine("\t</ul>\n");
                menuBuilder.AppendLine("</div>\n");

                html = menuBuilder.ToString() + clipBuilder.ToString();
                Cache.Insert("archiveHtml", html, null, DateTime.UtcNow.AddMinutes(10), Cache.NoSlidingExpiration);
            }
            wrapper.InnerHtml = html;
        }

        private void getLatestVideo()
        {
            if (_archive != null)
            {
                long published = 0;
                videoItem latestVideo = new videoItem();

                foreach (videoCategory cat in _archive.categories)
                {
                    foreach (videoItem video in cat.videos)
                    {
                        long thisPublished = 0;
                        bool parse = long.TryParse(video.startDate, out thisPublished);
                        if (parse)
                        {
                            if (thisPublished >= published)
                            {
                                latestVideo = video;
                                published = thisPublished;
                            }
                        }
                    }
                }
                if (latestVideo != null) { parseVideo(latestVideo); }
            }
        }
        private void parseVideo(videoItem video)
        {
            string playerKey = "AQ~~%2CAAAC55oP_QE~%2C9I10eiFWoUl-RRGxGS93LW88Oyy-tUat";
            string playerId = "3222810932001";
            StringBuilder html = new StringBuilder();
            html.AppendLine("<div class=\"video\">\n");
            //VideoPlayer
            html.AppendLine("<div id=\"main-video\">\n");
            html.Append(@"
                <object id='myExperience" + video.id + @"' class='BrightcoveExperience'>
                <param name='bgcolor' value='#808080' />
                <param name='width' value='680' />
                <param name='height' value='380' />
                <param name='playerID' value='" + playerId + @"' />
                <param name='playerKey' value='" + playerKey + @"' />
                <param name='isVid' value='true' />
                <param name='isUI' value='true' />
                <param name='dynamicStreaming' value='true' />
                <param name='linkBaseURL' value='http://play.skane.com?bctid=" + video.id + @"' />
                <param name='wmode' value='opaque' />
                <param name='htmlFallback' value='true' />
                <param name='@videoPlayer' value='" + video.id + @"' />
                </object>
                <script type='text/javascript'>brightcove.createExperiences();</script>
            ");
            html.AppendLine("</div>");
            //VideoMeta
            html.AppendLine("<div class=\"information-video\">\n");

            html.AppendLine("<h2>" + video.name + "</h2>\n");
            html.AppendLine("<p>" + video.shortDescription + "</p>\n");

            //video information
            html.AppendLine("<div class=\"video-information\">");
            if (video.length != null) { html.AppendLine("\t<span class=\"label\">Längd:</span><p class=\"video-length\">" + new TimeSpan(0, 0, 0, 0, int.Parse(video.length)).ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) + "</p>"); }
            if (video.publishedDate != null)
            {
                DateTime UNIXepoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                long milli;
                bool parse = long.TryParse(video.publishedDate, out milli);
                if (parse)
                {
                    UNIXepoch = UNIXepoch.AddMilliseconds(milli);
                    html.AppendLine("\t<span class=\"label\">Publicerad:</span><p class=\"date\">" + UNIXepoch.ToShortDateString() + "</p>");
                }
            }
            if (video.playsTotal != null) { html.AppendLine("\t<span class=\"label\">Visad:</span><p class=\"views\">" + video.playsTotal + "</p>"); }
            html.AppendLine("</div>");

            //sharing
            html.AppendLine("<div class=\"share\">");
            html.Append(@"
              <div class='addthis_toolbox addthis_default_style '>
                <a class='addthis_button_facebook_like' fb:like:layout='button_count'></a>
                <a class='addthis_button_tweet'></a>
              </div>
              <script type='text/javascript' src='//s7.addthis.com/js/300/addthis_widget.js#pubid=xa-52e638551dd5b4d6'></script>

              <div class='addthis_toolbox addthis_16x16_style'>
                <a class='addthis_button_preferred_1'></a>
                <a class='addthis_button_preferred_3'></a>
              </div>
              <script type='text/javascript' src='//s7.addthis.com/js/300/addthis_widget.js#pubid=xa-52e63818548cf1ba'></script>
            ");
            html.AppendLine("</div>\n");

            html.AppendLine("</div>\n");

            html.AppendLine("</div>\n");

            videoContainer.InnerHtml = html.ToString();

            //Set social meta tags
            if (!frontpage)
            {

                StringBuilder faceBookMeta = new StringBuilder();
                StringBuilder twitterMeta = new StringBuilder();

                if (video.name != null)
                {
                    metaPageTitel.Text = video.name;
                    twitterMeta.AppendLine("<meta name=\"twitter:title\" content=\"" + video.name + "\" />");
                    faceBookMeta.AppendLine("<meta property=\"og:title\" content=\"" + video.name + "\"/>");
                }
                if (video.shortDescription != null)
                {
                    twitterMeta.AppendLine("<meta name=\"twitter:description\" content=\"" + video.shortDescription + "\" />");
                    faceBookMeta.AppendLine("<meta property=\"og:description\" content=\"" + video.shortDescription + "\" />");
                }
                if (video.videoStillURL != null)
                {
                    twitterMeta.AppendLine("<meta name=\"twitter:image:src\" content=\"" + video.videoStillURL + "\" />");
                    faceBookMeta.AppendLine("<meta property=\"og:image\" content=\"" + video.videoStillURL + "\" />");
                }
                if (video.id != null)
                {
                    twitterMeta.AppendLine("<meta name=\"twitter:card\" content=\"player\" />");
                    twitterMeta.AppendLine("<meta name=\"twitter:url\" content=\"http://play.skane.com/?bctid=" + video.id + "\" />");

                    string twitterPlayerUrl = "https://link.brightcove.com/services/player/bcpid" + playerId + "?bckey=" + playerKey + "&bctid=" + video.id + "&secureConnections=true&autoStart=false&height=100%25&width=100%25";

                    twitterMeta.AppendLine("<meta name=\"twitter:player\" content=\"" + twitterPlayerUrl + "\" />");
                    twitterMeta.AppendLine("<meta name=\"twitter:player:width\" content=\"360\" />");
                    twitterMeta.AppendLine("<meta name=\"twitter:player:height\" content=\"200\" />");


                    string facebookPlayerId = playerId;
                    faceBookMeta.AppendLine("<meta property=\"og:url\"  content=\"http://play.skane.com/?bctid=" + video.id + "\"/>");
                    faceBookMeta.AppendLine("<meta property=\"og:type\" content=\"movie\"/>");
                    faceBookMeta.AppendLine("<meta property=\"og:video:height\" content=\"270\"/>");
                    faceBookMeta.AppendLine("<meta property=\"og:video:width\" content=\"480\"/>");
                    faceBookMeta.AppendLine("<meta property=\"og:video\" content=\"http://c.brightcove.com/services/viewer/federated_f9/?isVid=1&isUI=1&playerID=" + facebookPlayerId + "&autoStart=true&videoId=" + video.id + "\">");
                    faceBookMeta.AppendLine("<meta property=\"og:video:secure_url\" content=\"https://secure.brightcove.com/services/viewer/federated_f9/?isVid=1&isUI=1&playerID=" + facebookPlayerId + "&autoStart=true&videoId=" + video.id + "&secureConnections=true\">");
                    faceBookMeta.AppendLine("<meta property=\"og:video:type\" content=\"application/x-shockwave-flash\">");

                }

                fbMeta.Text = faceBookMeta.ToString();
                twMeta.Text = twitterMeta.ToString();
            }

            getRealtedVideos(video.id);
        }

        private void getRealtedVideos(string bcid)
        {
            Dictionary<long, videoItem> relatedVideosDict = new Dictionary<long, videoItem>();

            if (_archive != null)
            {
                videoItem latestVideo = new videoItem();

                foreach (videoCategory cat in _archive.categories)
                {
                    foreach (videoItem video in cat.videos)
                    {
                        long thisPublished = 0;
                        bool parse = long.TryParse(video.startDate, out thisPublished);
                        if (parse)
                        {
                        if (!relatedVideosDict.ContainsKey(thisPublished))
                            {
                                relatedVideosDict.Add(thisPublished, video);
                            }
                        }
                    }
                }
            }



            StringBuilder html = new StringBuilder();
            html.AppendLine("<div class=\"related-videos\">");
            html.AppendLine("<h2>Relaterade videor</h2>");
            if (relatedVideosDict.Count >= 1)
            {
                html.AppendLine("<div class=\"slider-videos\">");
                foreach (var item in relatedVideosDict.OrderByDescending(i => i.Key))
                {
                    html.AppendLine("<a class=\"relatedLink\" href=\"?bctid=" + item.Value.id + "\">");
                    //html.AppendLine("<div class=\"info-box\"><h2>" + item.Value.name + "</h2><img src=\"" + item.Value.videoStillURL + "\"/><p>" + item.Value.shortDescription + "</p></div>");
                    html.AppendLine("<div class=\"item-video\">");
                    html.AppendLine("<img src=\"" + item.Value.videoStillURL + "\" height=\"110px\" width=\"200px\" alt=\"" + item.Value.shortDescription + "\" />");
                    html.AppendLine("</div>");
                    html.AppendLine("</a>");
                }
                html.AppendLine("</div>");
            }
            html.AppendLine("</div>");

            relatedVideos.InnerHtml = html.ToString();
        }


        private void findVideoInArchive(string bcid)
        {
            if (_archive != null)
            {
                foreach (videoCategory cat in _archive.categories)
                {
                    foreach (videoItem video in cat.videos)
                    {
                        if (video.id == bcid) { parseVideo(video); }
                    }
                }
            }
        }
    }
}