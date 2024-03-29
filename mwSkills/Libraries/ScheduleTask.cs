using System;
using System.Threading;

namespace Libraries
{
    public class ScheduleTask
    {
        private Timer tmrStart;
        private Timer tmrStop;
    }

    public class ScheduleTaskEventArgs : EventArgs
    {}
}
