using System;

namespace Libraries
{
    /// <summary>用來防止洗版(flooding)的類別。</summary>
    /// <remarks></remarks>
    public class Dam
    {
        /// <summary>前次放行項目的Hash。</summary>
        private int prev_hash {get; set;} = 0;
        /// <summary>前次放行時間。</summary>
        private DateTime last_time {get; set;} = new DateTime(1970, 1, 1);
        /// <summary>同個物件的可放行最短間隔時間，單位：分鐘。</summary>
        private int min_period {get; set;} = 30;

        /// <param name="interval">同個物件可放行的最短間隔時間，單位：分鐘。</param>
        public Dam(int interval)
        {
            prev_hash = 0;
            last_time = new DateTime(1970, 1, 1);
            min_period = interval;
        }

        public bool CanDo(int hash)
        {
            DateTime now = DateTime.Now;
            if (hash == prev_hash)
            {
                if (now <= last_time.AddMinutes(min_period))
                    return false;
            }

            prev_hash = hash;
            last_time = now;
            return true;
        }
        public bool CanDo(string idstr)
        {
            int hash = idstr.GetHashCode();
            return CanDo(hash);
        }
    }
}