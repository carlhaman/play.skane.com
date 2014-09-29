using System;
using System.Collections.Generic;

using System.Web.Caching;

namespace play.skane.se
{
    public class index
    {
        System.IO.DirectoryInfo indexDirectory = System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("/index"));
        Lucene.Net.Store.Directory videoIndex = Lucene.Net.Store.FSDirectory.Open(System.Web.HttpContext.Current.Server.MapPath("/index"));
        Lucene.Net.Analysis.Analyzer analyser = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        private videoArchive _archive;
        Cache _cache = System.Web.HttpContext.Current.Cache;

        public index()
        {

        }

        public indexMessage buildIndex(indexMessage message)
        {
            if (_archive == null) { _archive = getVideoArchive(); }
            buildIndex(_archive, message);
            return message;
        }
        private videoArchive getVideoArchive()
        {
            _archive = (videoArchive)_cache["Archive"];
            if (_archive == null)
            {
                buildVideoArchive builder = new buildVideoArchive();
                _archive = builder.render();
                _cache.Insert("Archive", _archive, null, DateTime.UtcNow.AddMinutes(10), Cache.NoSlidingExpiration);
            }
            return _archive;
        }
        private void buildIndex(videoArchive archive, indexMessage message)
        {
            Lucene.Net.Index.IndexWriter writer;

            try
            {
                writer = new Lucene.Net.Index.IndexWriter(videoIndex, analyser, Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED);
                foreach (videoCategory cat in archive.categories)
                {
                    foreach (videoItem video in cat.videos)
                    {
                        string title = "";
                        if (!string.IsNullOrEmpty(video.name)) { title = video.name; }
                        string bcid = "";
                        if (!string.IsNullOrEmpty(video.id)) { bcid = video.id; }
                        string shortDescription = "";
                        if (!string.IsNullOrEmpty(video.shortDescription)) { shortDescription = video.shortDescription; }
                        string longDescription = "";
                        if (!string.IsNullOrEmpty(video.longDescription)) { longDescription = video.longDescription; }
                        string imageURL = "";
                        if (!string.IsNullOrEmpty(video.thumbnailURL)) { imageURL = video.videoStillURL; }

                        addVideoToIndex(bcid, title, shortDescription, longDescription, imageURL, writer, message);
                    }
                }
                message.success = true;
                writer.Optimize();
                message.message = "Index built and optimized!";
                writer.Dispose();
            }

            catch (Exception ex)
            {
                message.success = false;
                message.message = ex.Message;
            }


        }
        private bool videoIndexExists()
        {
            return Lucene.Net.Index.IndexReader.IndexExists(videoIndex);
        }
        private bool videoExistsInIndex(string id)
        {
            bool exist = false;
            Lucene.Net.Search.TermQuery termQuery = new Lucene.Net.Search.TermQuery(new Lucene.Net.Index.Term("bcid", id));
            Lucene.Net.Search.Searcher termSearcher = new Lucene.Net.Search.IndexSearcher(videoIndex, true);

            Lucene.Net.Search.TopScoreDocCollector termCollector = Lucene.Net.Search.TopScoreDocCollector.Create(1, true);
            termSearcher.Search(termQuery, termCollector);
            int termResults = termCollector.TopDocs().TotalHits;

            if (termResults > 0) { exist = true; }
            return exist;
        }
        private void addVideoToIndex(string bcid, string title, string shortDescription, string longDescription, string imageURL, Lucene.Net.Index.IndexWriter writer, indexMessage message)
        {
            Lucene.Net.Documents.Document video = new Lucene.Net.Documents.Document();

            video.Add(new Lucene.Net.Documents.Field("title", title, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.YES));
            video.Add(new Lucene.Net.Documents.Field("shortDescription", shortDescription, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.YES));
            video.Add(new Lucene.Net.Documents.Field("longDescription", longDescription, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.YES));
            video.Add(new Lucene.Net.Documents.Field("bcid", bcid, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NOT_ANALYZED));
            video.Add(new Lucene.Net.Documents.Field("imageURL", imageURL, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            if (videoExistsInIndex(bcid))
            {
                writer.UpdateDocument(new Lucene.Net.Index.Term("bcid", video.Get("bcid")), video);
                message.updatedVideo++;
            }
            else
            {
                writer.AddDocument(video);
                message.newVideo++;
            }

        }


        public List<indexVideo> searchArticles(string queryString, int numberOfResults)
        {
            List<indexVideo> resultsList = new List<indexVideo>();

            if (!string.IsNullOrEmpty(queryString))
            {

                Lucene.Net.QueryParsers.QueryParser parser = new Lucene.Net.QueryParsers.MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "title", "shortDescription", "longDescription" }, analyser);
                try
                {
                    Lucene.Net.Search.Query query = parser.Parse(queryString + "~");


                    Lucene.Net.Search.Searcher searcher = new Lucene.Net.Search.IndexSearcher(videoIndex, true);

                    Lucene.Net.Search.TopScoreDocCollector collector = Lucene.Net.Search.TopScoreDocCollector.Create(numberOfResults, true);

                    searcher.Search(query, collector);

                    Lucene.Net.Search.ScoreDoc[] hits = collector.TopDocs().ScoreDocs;

                    if (hits.Length >= 1)
                    {
                        for (int i = 0; i < hits.Length; i++)
                        {
                            indexVideo video = new indexVideo();

                            int docId = hits[i].Doc;
                            float score = hits[i].Score;

                            Lucene.Net.Documents.Document doc = searcher.Doc(docId);

                            video.bcid = doc.Get("bcid");
                            video.title = doc.Get("title");
                            video.score = score;
                            video.shortDescription = doc.Get("shortDescription");
                            video.longDescription = doc.Get("longDescription");
                            video.imageURL = doc.Get("imageURL");


                            resultsList.Add(video);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return resultsList;
        }
    }
    public class indexVideo
    {
        public string bcid { get; set; }
        public string title { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public string imageURL { get; set; }
        public float score { get; set; }
    }
    public class indexMessage
    {
        public indexMessage()
        {
            newVideo = 0;
            updatedVideo = 0;
            success = false;
            message = string.Empty;
        }
        public int newVideo { get; set; }
        public int updatedVideo { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }

}