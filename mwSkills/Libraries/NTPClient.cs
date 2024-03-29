using System;
using System.Net.Sockets;
using System.Diagnostics;

namespace mwSkills.Libraries
{
    /// <summary>跟連線的物件。</summary>
    public class NTPClient
    {
        public static Exception LastExcp {get; private set;}

        /// <summary>試著取得NTP server時間訊息。</summary>
        [Obsolete("測試用而已。", true)]
        public static bool TryGetNtpMsg(string ntpsvr, int port, int timeloc, out NTPmsg msg)
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.SendTimeout = 100;
                    socket.ReceiveTimeout = 100;
                    socket.Connect(ntpsvr, port);

                    byte[] ntpdata = new byte[48];
                    int LI = 0, VN = 3, Mode = 3;
                    ntpdata[0] = (byte)(((LI & 0x03) << 6) + ((VN & 0x0B) << 3) + (Mode & 0x0B));

                    DateTime clnsend = DateTime.Now;
                    socket.Send(ntpdata);
                    socket.Receive(ntpdata);
                    DateTime clnrecv = DateTime.Now;

                    msg = new NTPmsg(clnsend, clnrecv, ntpdata, timeloc);
                }
                return true;
            }
            catch (SocketException se)
            {
                Debug.WriteLine($"[NTP] 取得NTP資訊失敗！ {se.GetType().Name} - {se.Message}");
            }
            catch (Exception excp)
            {
                Debug.WriteLine($"[NTP] 取得NTP資訊失敗！ {excp.GetType().Name} - {excp.Message}\n{excp.StackTrace}");
            }

            msg = null;
            return false;
        }
        
        /// <summary>試著取得NTP server時間，若取得NTP server時間失敗則會取得系統時間。</summary>
        /// <param name="ntpsvr">NTP server的URL。</param>
        /// <param name="utcbias">要取得時間的UTC時區偏移。</param>
        /// <param name="time">若回傳值為false表示索取時間失敗，則此參數會回傳系統時間。</param>
        /// <returns>回傳true表示成功取得NTP server時間；回傳false表示取得失敗。</returns>
        public static bool TryGetNtpTime(string ntpsvr, int utcbias, out DateTime time)
        {
            if (String.IsNullOrEmpty(ntpsvr))
            {
                time = DateTime.UtcNow;
                return false;
            }
            #region 解析server URL
            string host, errmsg;
            int port;
            if (!TryParseNtpSvr(ntpsvr, out host, out port, out errmsg))
            {
                Debug.WriteLine(errmsg);
                time = DateTime.UtcNow;
                return false;
            }
            #endregion
            
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SendTimeout = 100;
                socket.ReceiveTimeout = 100;
                socket.Connect(host, port);

                byte[] ntpdata = new byte[48];
                int LI = 0, VN = 3, Mode = 3;
                ntpdata[0] = (byte)(((LI & 0x03) << 6) + ((VN & 0x0B) << 3) + (Mode & 0x0B));

                DateTime clnsend = DateTime.Now;
                socket.Send(ntpdata);
                socket.Receive(ntpdata);
                DateTime clnrecv = DateTime.Now;

                NTPmsg msg = new NTPmsg(clnsend, clnrecv, ntpdata, utcbias);
                time = msg.TheTime;
                return true;
            }
            // catch (SocketException se)
            // {
            //     Debug.WriteLine($"[NTP] 取得NTP資訊失敗！ {se.GetType().Name} - {se.Message}");
            // }
            catch (Exception excp)
            {
                Debug.WriteLine($"[NTP] 取得NTP資訊失敗！ {excp.GetType().Name} - {excp.Message}\n{excp.StackTrace}");
                LastExcp = excp;
            }

            time = DateTime.Now;
            return false;
        }

        /// <summary>嘗試解析<paramref name="svrurl"/>是否為正確的NTP server URL格式。</summary>
        public static bool TryParseNtpSvr(string svrurl, out string host, out int port, out string msg)
        {
            try
            {
                if (String.IsNullOrEmpty(svrurl))
                    throw new Exception("The server was not specified.");

                string rest = svrurl;
                // scheme
                if (svrurl.Contains("://"))
                {
                    string[] comps = svrurl.Split("://");
                    if (comps.Length != 2)
                        throw new Exception("Bad URL.");
                    if (comps[0] != "ntp")
                        throw new Exception("Wrong protocol.");
                    rest = comps[1];
                }
                // host and port
                if (rest.Contains(":"))
                {
                    int idxLastColon = rest.LastIndexOf(':');
                    if (idxLastColon == rest.Length - 1)
                        throw new Exception("Bad URL.");
                    string strPort = rest.Substring(idxLastColon + 1);
                    if (!Int32.TryParse(strPort, out port))
                        throw new Exception("Bad URL.");
                    if (port > 65535 || port < 0)
                        throw new Exception("Bad prot");

                    host = rest.Substring(0, idxLastColon).TrimStart('[').TrimEnd(']');
                }
                else
                {
                    host = rest;
                    port = 123;
                }

                msg = null;
                return true;
            }
            catch (Exception excp)
            {
                msg = $"Failed to parse URL '{svrurl}'. ({excp.Message})";
                host = null;
                port = -1;
                return false;
            }
        }
    }

    public class NTPmsg
    {
        #region fields
        /// <summary>NTP client傳出訊息的時間。</summary>
        public DateTime TSclientSend;
        /// <summary>NTP server收到訊息的時間。</summary>
        public DateTime TSserverRecv;
        /// <summary>NTP server傳出訊息的時間。</summary>
        public DateTime TSserverSend;
        /// <summary>NTP client收到訊息的時間。</summary>
        public DateTime TSclientRecv;

        /// <summary>Leap Indicator</summary>
        /// <remarks>
        /// A 2-bit leap indicator. When set to 11, it warns of an alarm condition (clock unsynchronized); 
        /// when set to any other value, it is not to be processed by NTP.
        /// </remarks>
        private int LI;
        /// <summary>Version Number</summary>
        /// <remarks>A 3-bit version number that indicates the version of NTP. The latest version is version 4.</remarks>
        private int VN;
        /// <summary>Mode</summary>
        /// <remarks>A 3-bit code that indicates the work mode of NTP.</remarks>
        private int Mode;
        /// <summary>Stratum</summary>
        /// <remarks>
        /// An 8-bit integer that indicates the stratum level of the local clock, with the value ranging from 1 to 16. 
        /// Clock precision decreases from stratum 1 through stratum 16. A stratum 1 clock has the highest precision, and a stratum 
        /// 16 clock is not synchronized and cannot be used as a reference clock.
        /// </remarks>
        private int Stratum;
        /// <summary>Poll</summary>
        /// <remarks>An 8-bit signed integer that indicates the maximum interval between successive messages, which is called the poll interval.</remarks>
        private int Poll;
        /// <summary>Precision</summary>
        /// <remarks>An 8-bit signed integer that indicates the precision of the local clock.</remarks>
        private int Precision;
        /// <summary>Root Delay</summary>
        /// <remarks>Roundtrip delay to the primary reference source.</remarks>
        private int RootDelay;
        /// <summary>Root Dispersion</summary>
        /// <remarks>The maximum error of the local clock relative to the primary reference source.</remarks>
        private int RootDispersion;
        /// <summary>Reference Identifier</summary>
        /// <remarks>Identifier of the particular reference source.</remarks>
        private int ReferenceIdentifier;

        /// <summary>Reference Timestamp</summary>
        /// <remarks>The local time at which the local clock was last set or corrected.</remarks>
        private DateTime RefTS;
        /// <summary>Originate Timestamp</summary>
        /// <remarks>The local time at which the request departed from the client for the service host.</remarks>
        private DateTime OrgTS;
        /// <summary>>Authenticator</summary>
        /// <remarks>Authentication information.</remarks>
        public byte[] Authenticator;
        /// <summary>時區。</summary>
        /// <remarks>以台為為例時區為UTC+8，所以值為8。</remarks>
        public int TimeLoc;

        public TimeSpan offset;
        public TimeSpan delay;
        #endregion
        public string NtpMode
        {
            get { return ntpMode[Mode]; }
        }

        /// <summary>計算後的那個時間。</summary>
        public DateTime TheTime
        {
            get { return (TSclientSend + delay + offset); }
        }

        /// <summary>解析NTP package。</summary>
        public NTPmsg(DateTime ts_send, DateTime ts_receive, byte[] ntppackage, int utcbias)
        {
            TimeLoc = utcbias;
            
            LI = ((ntppackage[0] & 0xC0) >> 6);
            VN = ((ntppackage[0] & 0x38) >> 3);
            Mode = (ntppackage[0] & 0x07);
            Stratum = (ntppackage[1] & 0x0F);
            Poll = (ntppackage[2] & 0x0F);
            Precision = (ntppackage[3] & 0x0F);
            RootDelay = BitConverter.ToInt32(swapEndianness(ntppackage[4..8]));
            RootDispersion = BitConverter.ToInt32(swapEndianness(ntppackage[8..12]));
            ReferenceIdentifier = BitConverter.ToInt32(swapEndianness(ntppackage[12..16]));
            RefTS = NtpFormat2DateTime(ntppackage[16..24]);
            OrgTS = NtpFormat2DateTime(ntppackage[24..32]);

            TSclientSend = ts_send;                                 // t1
            TSserverRecv = NtpFormat2DateTime(ntppackage[32..40]);  // t2
            TSserverSend = NtpFormat2DateTime(ntppackage[40..48]);  // t3
            TSclientRecv = ts_receive;                              // t4
            // Debug.WriteLine(
            //     string.Format(
            //         "t1={0}, t2={1}, t3={2}, t4={3}",
            //         TSclientSend.ToString("yyyy/MM/dd-HH:mm:ss.fff"),
            //         TSserverRecv.ToString("yyyy/MM/dd-HH:mm:ss.fff"),
            //         TSserverSend.ToString("yyyy/MM/dd-HH:mm:ss.fff"),
            //         TSclientRecv.ToString("yyyy/MM/dd-HH:mm:ss.fff")
            //     )
            // );

            offset = 0.5 * ((TSserverRecv - TSclientSend) + (TSserverSend - TSclientRecv));
            delay = (TSclientRecv - TSclientSend) - (TSserverRecv - TSserverSend);
            // Debug.WriteLine($"offset=((t2-t1)+(t3-t4))/2={offset}");
            // Debug.WriteLine($"delay=((t4-t1)-(t3-t2))={delay}");

            if (ntppackage.Length > 48)
                Authenticator = ntppackage[48..];
        }
        
        private DateTime NtpFormat2DateTime(byte[] b)
        {
            ulong intPart = BitConverter.ToUInt32(swapEndianness(b[0..4]));
            ulong fractPart = BitConverter.ToUInt32(swapEndianness(b[4..8]));

            ulong ms = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            return new DateTime(1900, 1, 1).AddMilliseconds((long)ms).AddHours(TimeLoc);
        }

        private byte[] swapEndianness(byte[] x)
        {
            byte[] y = new byte[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = x[x.Length - i - 1];
            
            return y;
        }

        private string[] ntpMode = {
            "reserved",
            "symmetric active",
            "symmetric passive",
            "client",
            "server",
            "broadcast or multicast",
            "NTP control message",
            "reserved for private use.",
        };
    }
}