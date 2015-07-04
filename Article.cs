using System;

public class Article
{
    private Blog blog;
    private string uri;
    private DateTime date;
    private string title;

    internal Article(Blog blog, string uri, DateTime date, string title)
    {
        this.blog = blog;
        this.uri = uri;
        this.date = date;
        this.title = title;
    }

    public Blog Blog
    {
        get
        {
            return blog;
        }
    }

    public string Uri
    {
        get
        {
            return uri;
        }
    }

    public DateTime Date
    {
        get
        {
            return date;
        }
    }

    public string Title
    {
        get
        {
            return title;
        }
    }
}
