using System.Windows.Forms;
using System.Collections.Generic;

namespace Dialogs
{
    public partial class DlgMultiPageListView<T>
    {
        public DlgMultiPageListView(IEnumerable<T> objs)
        {
            objects = new List<T>(objs);
        }
    }
}