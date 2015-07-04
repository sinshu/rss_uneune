using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public partial class Form1 : Form
{
    private Dictionary<string, List<string>> rssChunk;

    public Form1()
    {
        InitializeComponent();

        InitRssList();
    }

    private void InitRssList()
    {
        rssChunk = new Dictionary<string, List<string>>();
        string currentCategory = null;
        var list = new List<string>();
        foreach (var line in File.ReadLines("rsslist.txt", Encoding.GetEncoding("UTF-8")))
        {
            if (line.StartsWith("["))
            {
                if (currentCategory != null)
                {
                    list.Sort();
                    rssChunk.Add(currentCategory, list);
                }
                currentCategory = line.Substring(1, line.Length - 2);
                list = new List<string>();
            }
            else
            {
                list.Add(line);
            }
        }
        list.Sort();
        rssChunk.Add(currentCategory, list);

        foreach (var pair in rssChunk)
        {
            listBox1.Items.Add(pair.Key);
            listBox3.Items.Add(pair.Key);
        }
    }

    private void RefreshListBox()
    {
        listBox2.Items.Clear();
        foreach (var rss in rssChunk[listBox1.SelectedItem.ToString()])
        {
            listBox2.Items.Add(rss);
        }
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshListBox();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        var targetRss = listBox2.SelectedItem.ToString();
        rssChunk[listBox1.SelectedItem.ToString()].Remove(targetRss);
        rssChunk[listBox3.SelectedItem.ToString()].Add(targetRss);
        RefreshListBox();
    }

    private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        listBox4.Items.Clear();
        var blog = new Blog(listBox2.SelectedItem.ToString());
        foreach (var article in blog.Articles)
        {
            listBox4.Items.Add(article.Title);
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        using (var writer = new StreamWriter("rsslist.txt", false, Encoding.GetEncoding("UTF-8")))
        {
            foreach (var pair in rssChunk)
            {
                writer.WriteLine("[" + pair.Key + "]");
                pair.Value.Sort();
                foreach (var rss in pair.Value)
                {
                    writer.WriteLine(rss);
                }
            }
        }
    }
}
