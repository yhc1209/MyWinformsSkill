using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace mwSkills.Dialogs;
public partial class DlgMultiPageListView<T> : Form where T: IListViewData
{
    private UpdateDataCallback UpdateCallback;
    private List<T> data = new List<T>();
    private int itemsPerPage = 10;
    /// <summary>當前頁數。(zero-based)</summary>
    private int currentPage = 0;
    private object lock_data = new object();

    public DlgMultiPageListView(UpdateDataCallback callback)
    {
        InitializeComponent();
        UpdateCallback = callback;
    }
    private void updateData()
    {
        lock (lock_data) { data = UpdateCallback.Invoke(); }
    }

    private void updatePgDscp(int from, int to, int total)
    {
        LblPgDscp.Text = $"{from}-{to} of {total}";
        if (from == 1)
        {
            LklFstPg.Enabled = false;
            LklPrevPg.Enabled = false;
        }
        else
        {
            LklFstPg.Enabled = true;
            LklPrevPg.Enabled = true;
        }
        if (from == total || (to - from + 1) < itemsPerPage)
        {
            LklLstPg.Enabled = false;
            LklNextPg.Enabled = false;
        }
        else
        {
            LklLstPg.Enabled = true;
            LklNextPg.Enabled = true;
        }
    }

    /// <summary>將<see cref="LsvData"/>刷新到第<paramref name="pg"/>頁的畫面。</summary>
    /// <param name="pg">頁數(zero-based)。</param>
    /// <remarks>頁數<pararef name="pg"/>是從0開始算。</remarks>
    private void GoToPg(int pg)
    {
        T[] dataArr;
        int from = pg * itemsPerPage, to;   // zero-based
        int total;
        lock (lock_data)
        {
            total = data.Count;
            if (total <= from)
            {
                if (currentPage == 0)
                    return;
                pg = 0;
                from = 0;
            }
            if (total <= (from + itemsPerPage - 1))
                to = total - 1;
            else
                to = from + itemsPerPage - 1;
            dataArr = data.GetRange(from, to - from).ToArray();
        }
        LsvData.BeginUpdate();
        LsvData.Clear();
        foreach (T d in dataArr)
            LsvData.Items.Add(d.ToLsvItem());
        updatePgDscp(from + 1, to + 1, total);
        LsvData.EndUpdate();
        currentPage = pg;
    }

    private void GoToFstPg(object sender, EventArgs e)
    {
        if (currentPage == 0)
            return;
        GoToPg(0);
    }

    private void GoToPrevPg(object sender, EventArgs e)
    {
        if (currentPage < 1)
            return;
        GoToPg(currentPage - 1);
    }

    private void GoToNextPg(object sender, EventArgs e)
    {
        GoToPg(currentPage + 1);
    }
    
    private void GoToLstPg(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}