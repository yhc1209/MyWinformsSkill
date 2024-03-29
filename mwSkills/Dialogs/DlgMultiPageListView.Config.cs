using System.Windows.Forms;
using System.Collections.Generic;

namespace mwSkills.Dialogs;
public partial class DlgMultiPageListView<T>
{
    private TableLayoutPanel TlpDlg;
    private ListView LsvData;
    /// <summary>到第一頁的連結。</summary>
    private LinkLabel LklFstPg;
    /// <summary>到上一頁的連結。</summary>
    private LinkLabel LklPrevPg;
    /// <summary>到下一頁的連結。</summary>
    private LinkLabel LklNextPg;
    /// <summary>到最後頁的連結。</summary>
    private LinkLabel LklLstPg;
    /// <summary>頁數說明。</summary>
    private Label LblPgDscp;
    private Label LblItemsPerPg;
    private ComboBox CbxItemPerPg;
    private Button BtnOk;
    
    private void InitializeComponent()
    {
        this.TlpDlg = new TableLayoutPanel();
        this.LsvData = new ListView();
        this.BtnOk = new Button();
        this.TlpDlg.SuspendLayout();
        this.SuspendLayout();

        // BtnOk
        BtnOk.Name = "BtnOk";
        BtnOk.AutoSize = true;
        BtnOk.Text = "好啊";
        BtnOk.DialogResult = DialogResult.OK;
        // LsvData
        LsvData.Name = "LsvData";
        LsvData.Dock = DockStyle.Fill;
        LsvData.View = View.Details;
        LsvData.FullRowSelect = true;
        LsvData.Columns.AddRange(T.ColumnHeaders);
        // LklFstPg
        LklFstPg.Name = "LklFstPg";
        LklFstPg.Text = "|<";
        LklFstPg.Margin = new Padding(3);
        // LklFstPg.VisitedLinkColor = LklFstPg.LinkColor;
        LklFstPg.Click += new System.EventHandler(GoToFstPg);
        // LklPrevPg
        LklPrevPg.Name = "LklPrevPg";
        LklPrevPg.Text = "|<";
        LklPrevPg.Margin = new Padding(3);
        // LklPrevPg.VisitedLinkColor = LklPrevPg.LinkColor;
        LklPrevPg.Click += new System.EventHandler(GoToPrevPg);
        // TlpDlg
        TlpDlg.Name = "TlpDlg";
        TlpDlg.Dock = DockStyle.Fill;
        TlpDlg.ColumnCount = 8;
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        TlpDlg.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        TlpDlg.RowCount = 3;
        TlpDlg.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        TlpDlg.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        TlpDlg.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        TlpDlg.Controls.Add(LsvData, 0, 0);
        TlpDlg.Controls.Add(LklFstPg, 1, 1);
        TlpDlg.Controls.Add(LklPrevPg, 2, 1);
        TlpDlg.Controls.Add(LklNextPg, 3, 1);
        TlpDlg.Controls.Add(LklLstPg, 4, 1);
        TlpDlg.Controls.Add(LblPgDscp, 5, 1);
        TlpDlg.Controls.Add(LblItemsPerPg, 6, 1);
        TlpDlg.Controls.Add(CbxItemPerPg, 7, 1);
        TlpDlg.Controls.Add(BtnOk, 7, 2);
        TlpDlg.SetColumnSpan(LsvData, TlpDlg.ColumnCount);
        // dlg
        Name = "DlgMultiPageListView";
        this.Controls.Add(TlpDlg);
        
        TlpDlg.ResumeLayout();
        this.ResumeLayout();
    }

    // ------ delegate ------
    public delegate List<T> UpdateDataCallback();
}