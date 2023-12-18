using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            using (DlgMultiPageListView dlg = new DlgMultiPageListView())
            {}
        }
    }
}
