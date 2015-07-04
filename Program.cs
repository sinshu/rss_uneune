using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        using (var form = new Form1())
        {
            Application.Run(form);
        }
    }
}
