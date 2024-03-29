using System;
using System.Windows.Forms;

namespace mwSkills.Dialogs;

/// <summary>
/// 方便在<see cref="ListView"/>中顯示的類別介面。
/// </summary>
public interface IListViewData
{
    public static abstract ColumnHeader[] ColumnHeaders { get; }
    public abstract ListViewItem ToLsvItem();
}