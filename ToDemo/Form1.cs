using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ToDemo.Data;
using mwSkills.Dialogs;

namespace ToDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region output
        private void Log(string msg, bool newline = true)
        {
            if (newline)
                TbxOutput.AppendText($"{msg}{Environment.NewLine}");
            else
                TbxOutput.AppendText(msg);
        }
        #endregion

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
            List<TestMember> members = TestMember.GenerateRandomMembers(10);
            foreach (TestMember m in members)
                Log(m.ToString());

            using (DlgMultiPageListView<TestMember> dlg = new DlgMultiPageListView<TestMember>(func))
            {
                dlg.ShowDialog();
            }
        }
    }
}
