using System;

public class BlogItem
{
    private Blog blog;
    private string message;

    public BlogItem(Blog blog)
    {
        this.blog = blog;
        message = "";
        if (DateTime.Now - GetLastUpdateTime() >= TimeSpan.FromDays(30))
        {
            message = "一ヶ月以上更新なし";
        }
    }

    public Blog GetBlog()
    {
        return blog;
    }

    private DateTime GetLastUpdateTime()
    {
        if (blog.Articles.Count > 0)
        {
            return blog.Articles[0].Date;
        }
        else
        {
            return new DateTime(1900, 1, 1);
        }
    }

    public string RssUri
    {
        get
        {
            return blog.RssUri;
        }
    }

    public string Title
    {
        get
        {
            return blog.Title;
        }
    }

    public string LastUpdate
    {
        get
        {
            return GetLastUpdateTime().ToString("yyyy/MM/dd");
        }
    }

    public string Message
    {
        get
        {
            return message;
        }
    }
}
