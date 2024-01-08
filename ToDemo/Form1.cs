using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dialogs;

namespace ToDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region events
        private void keyDownEvent(object sender, KeyEventArgs e)
        {}
        private void BtnTest_Click(object sender, EventArgs e)
        {
            demoMultiPageListView();
        }
        private void BtnAux_Click(object sender, EventArgs e)
        {}
        #endregion

        private void demoMultiPageListView()
        {
            // load data
            string json = File.ReadAllText(@"ToDemo\data\NumbersOfClass1.json");
            Class1[] info = JsonSerializer.Deserialize<Class1[]>(json);

            using (DlgMultiPageListView<Class1> dlg = new DlgMultiPageListView<Class1>(info))
            {
                dlg.ShowDialog();
            }
        }
    }
}
