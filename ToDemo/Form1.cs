using System;
using System.Diagnostics;
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
            Random r = new Random(235436);
            int count = 0, max = r.Next(120, 125);
            DlgMultiPageListView<TestMember>.UpdateDataCallback func = () => {
                if (count < max)
                {
                    int newCount = r.Next(3, 10);
                    var members = TestMember.GenerateRandomMembers(newCount, count - 1);
                    Debug.WriteLine($"[demoMultiPageListView] 新增{newCount}個成員資料。");
                    count += newCount;
                    return members;
                }
                else
                {
                    Debug.WriteLine("[demoMultiPageListView] 沒再增加了。");
                    return Array.Empty<TestMember>();
                }
            };

            using (DlgMultiPageListView<TestMember> dlg = new DlgMultiPageListView<TestMember>(func))
            {
                dlg.ShowDialog();
            }
        }
    }
}
