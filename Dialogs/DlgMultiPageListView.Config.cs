using System.Windows.Forms;
using System.Collections.Generic;

namespace Dialogs
{
    #region data
    /// <summary>The data which will show in ListView.</summary>
    public class LsvData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Remarks { get; set; }
    }

    public class ListViewDatas
    {
        private List<LsvData> _datas;

        public LsvData[] GetDatas(int startIdx, int count)
        {
            LsvData[] datas = new LsvData[count];
            _datas.CopyTo(0, datas, 0, count);
            return datas;
        }
        public void Sort()
        {}
    }
    #endregion

    public partial class DlgMultiPageListView : Form
    {}

}