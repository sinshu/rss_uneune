using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private BindingList<BlogItem> bindingList;

    private bool readingRss;

    private string currentFileName;

    public MainForm()
    {
        InitializeComponent();

        splitContainer2.SplitterDistance = splitContainer2.Height - textBox2.Height;

        openFileDialog1.Filter = "RSSリスト|*.txt";
        openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        openFileDialog1.FileName = "rsslist.txt";
        saveFileDialog1.Filter = "RSSリスト|*.txt";
        saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        saveFileDialog1.FileName = "rsslist.txt";

        bindingList = new BindingList<BlogItem>();
        dgvBlogList.DataSource = bindingList;

        readingRss = false;
    }

    private async void ReadRssFromFile(string path)
    {
        if (readingRss)
        {
            MessageBox.Show("RSSリストの読み込みが完了していません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        readingRss = true;
        SetCurrentFile(path);
        bindingList.Clear();
        foreach (var line in File.ReadLines(path))
        {
            toolStripStatusLabel1.Text = "RSS取得中: " + line;
            var blog = await Task.Run(() => Blog.FromRssUriOrEmpty(line));
            bindingList.Add(new BlogItem(blog));
        }
        dgvBlogList.AutoResizeColumns();
        toolStripStatusLabel1.Text = "RSSリスト読み込み完了";
        readingRss = false;
    }

    private bool CheckFilePath(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            MessageBox.Show("指定されたファイルは存在しません。\r\nパス: " + path, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    private void SetCurrentFile(string path)
    {
        currentFileName = path;
        Text = Path.GetFileName(path) + " - " + Settings.Title;
    }

    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Copy;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    private void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
        if (CheckFilePath(paths[0]))
        {
            ReadRssFromFile(paths[0]);
        }
    }

    private void dgvBlogList_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
        if (bindingList.Count == 0) return;
        var blogItem = bindingList[e.RowIndex];
        textBox1.Clear();
        var sb = new StringBuilder();
        int i = 1;
        foreach (var article in blogItem.GetBlog().Articles)
        {
            sb.AppendLine(i.ToString("00") + " " + article.Title);
            i++;
        }
        textBox1.Text = sb.ToString();
    }

    private void openStripMenuItem_Click(object sender, EventArgs e)
    {
        var result = openFileDialog1.ShowDialog();
        if (result == DialogResult.OK)
        {
            if (CheckFilePath(openFileDialog1.FileName))
            {
                ReadRssFromFile(openFileDialog1.FileName);
            }
        }
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var result = saveFileDialog1.ShowDialog();
        if (result == DialogResult.OK)
        {
            File.WriteAllLines(saveFileDialog1.FileName, bindingList.Select(item => item.RssUri).OrderBy(line => line));
            SetCurrentFile(saveFileDialog1.FileName);
            toolStripStatusLabel1.Text = "保存完了: " + currentFileName;
        }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (currentFileName == null)
        {
            saveAsToolStripMenuItem_Click(sender, e);
        }
        else
        {
            File.WriteAllLines(currentFileName, bindingList.Select(item => item.RssUri).OrderBy(line => line));
            toolStripStatusLabel1.Text = "保存完了: " + currentFileName;
        }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Close();
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        toolStripStatusLabel1.Text = "RSS取得中: " + textBox2.Text;
        var blog = await Task.Run(() => Blog.FromRssUriOrEmpty(textBox2.Text));
        bindingList.Add(new BlogItem(blog));
        toolStripStatusLabel1.Text = "RSS取得完了";
    }
}
