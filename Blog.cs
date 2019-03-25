using System;
using System.Collections.Generic;
using System.Xml;

public class Blog
{
    private string rssUri;
    private string uri;
    private string title;
    private List<Article> articles;

    private Blog(string rssUri, string uri, string title)
    {
        this.rssUri = rssUri;
        this.uri = uri;
        this.title = title;
        articles = new List<Article>();
    }

    public Blog(string rssUri)
    {
        this.rssUri = rssUri;
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(rssUri);
        uri = xmlDocument.GetElementsByTagName("link")[0].InnerText;
        title = xmlDocument.GetElementsByTagName("title")[0].InnerText;
        articles = new List<Article>();
        foreach (var article in GetArticlesFromXmlDocument(xmlDocument))
        {
            articles.Add(article);
        }
    }

    public static Blog FromRssUriOrEmpty(string rssUri)
    {
        try
        {
            return new Blog(rssUri);
        }
        catch (Exception e)
        {
            return new Blog(rssUri, "", "[ERROR]" + e.Message);
        }
    }

    private IEnumerable<Article> GetArticlesFromXmlDocument(XmlDocument xmlDocument)
    {
        var isRss2 = xmlDocument.DocumentElement.Name == "rss";
        var itemNodes = xmlDocument.GetElementsByTagName("item");
        foreach (var itemNode in itemNodes)
        {
            var itemElement = (XmlElement)itemNode;
            var articleUri = itemElement.GetElementsByTagName("link")[0].InnerText;
            var dateNodes = itemElement.GetElementsByTagName(isRss2 ? "pubDate" : "dc:date");
            var articleDate = DateTime.Parse(dateNodes[0].InnerText);
            var articleTitle = itemElement.GetElementsByTagName("title")[0].InnerText;
            var article = new Article(this, articleUri, articleDate, articleTitle);
            yield return article;
        }
    }

    public string RssUri
    {
        get
        {
            return rssUri;
        }
    }

    public string Uri
    {
        get
        {
            return uri;
        }
    }

    public string Title
    {
        get
        {
            return title;
        }
    }

    public IList<Article> Articles
    {
        get
        {
            return articles;
        }
    }
}
