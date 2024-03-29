using System;
using System.Threading;

namespace mwSkills.Libraries
{
    public class ScheduleTask
    {
        private Timer tmrStart;
        private Timer tmrStop;
    }

    public class ScheduleTaskEventArgs : EventArgs
    {}
}
