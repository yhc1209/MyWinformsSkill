using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;

namespace Libraries
{
    public class WsClient
    {
        private ClientWebSocket _ws = null;
        private CancellationTokenSource _cts = null;
        private string _svrurl = null;
        /// <summary>debug訊息輸出資料夾。</summary>
        private string DBG_DIR = $"output\\";
        public bool IsOpened
        {
            get {
                Debug.WriteLineIf(_ws.State != WebSocketState.Open, $"[WsClient] check state? {_ws.State}\t\t(open狀態不顯示)");
                return (_ws.State < WebSocketState.CloseSent);
            }
        }
        public string Host
        {
            get {
                return _svrurl;
            }
        }
        public Exception LastExcp {get; private set;}
        /// <summary>建立SD3 web socket client實例。</summary>
        /// <param name="callback">監聽事件的callback function。</param>
        public WsClient(SdWsEventHandler callback)
        {
            OnServerRequest = callback;
        }

        /// <summary>連線並註冊。</summary>
        public bool Connect(string url)
        {
            if (_ws != null && ((int)_ws.State) < ((int)WebSocketState.CloseSent))
            {
                LastExcp = new Exception($"Connection exists! You need to close it before connect again. (state: {_ws.State})");
                return false;
            }

            _cts = new CancellationTokenSource();
            _ws = new ClientWebSocket();
            _ws.Options.RemoteCertificateValidationCallback = delegate { return true; };
            // todo: JWT?

            try
            {
                _ws.ConnectAsync(new Uri(url), _cts.Token).Wait();
                Debug.Write($"[WsClient] 連線完畢({url})。 state: {_ws.State}");
                _svrurl = url;

                // // register
                // SDWS_RQST_REG req = new SDWS_RQST_REG(CtlSvrConn.MyPrivateIP);
                // string jstxt = JsonSerializer.Serialize<SDWS_RQST_REG>(req);
                // sendMessage(jstxt);
                // jstxt = receiveMessage();
                // JsonSerializerOptions op = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // SDWS_RESPONSE rsp = JsonSerializer.Deserialize<SDWS_RESPONSE>(jstxt, op);
                // if (rsp.Action != req.Action)
                // {
                //     if (rsp.Action.ToLower() == "websocketerror")
                //         throw new Exception($"Failed to register. code: {rsp.Code} message: {rsp.Message}");
                //     throw new Exception($"Failed to receive the response of registering. (The action of response: {rsp.Action})");
                // }
                // if (rsp.Code != 0)
                //     throw new Exception($"Failed to register. code: {rsp.Code} message: {rsp.Message}");

                Task.Run(listenOn);
                return true;
            }
            catch (Exception excp)
            {
                LastExcp = excp;
            }
            _svrurl = null;
            return false;
        }

        public void Disconnect()
        {
            try
            {
                _cts.Cancel();

                if (((int)_ws.State) < ((int)WebSocketState.CloseSent))
                {
                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal close.", CancellationToken.None).Wait();
                    Debug.WriteLine($"[WsClient] 送出斷線。 status: {_ws.CloseStatus}");
                }
                else
                    Debug.WriteLine($"[WsClient] 斷線中，稍後。 status: {_ws.CloseStatus}");
            }
            catch (Exception excp)
            {
                // SDEventVwr.ExcpLog("Exception thrown while close web socket connection.", excp);
            }
            _ws.Dispose();
        }

        private void sendMessage(string msg)
        {
            #if DEBUG
            Directory.CreateDirectory(DBG_DIR);
            string checkfile = $"{DBG_DIR}Send_{getActionName(msg)}_{DateTime.Now.ToString("yyMMdd_HHmmss_ffff")}.txt";
            File.WriteAllText(checkfile, $"{msg}\n\nSent to \'{_svrurl}\'.");
            #endif

            byte[] bMsg = Encoding.UTF8.GetBytes(msg);
            _ws.SendAsync(bMsg, WebSocketMessageType.Text, true, _cts.Token).Wait();
            Debug.WriteLine($"[WsClient] 發送訊息({msg})to \'{_svrurl}\'。");
        }

        private string receiveMessage()
        {
            WebSocketReceiveResult wsrr = null;
            List<byte> buff = new List<byte>();
            do
            {
                byte[] buf = new byte[2048];
                wsrr = _ws.ReceiveAsync(buf, _cts.Token).Result;
                // Debug.WriteLine($"[WsClient] received: {wsrr.Count}/{buf.Length} (EndOfMessage?{wsrr.EndOfMessage}) from \'{_svrurl}\'");
                ArraySegment<byte> seg = new ArraySegment<byte>(buf, 0, wsrr.Count);
                buff.AddRange(seg.ToArray());
            }
            while (!wsrr.EndOfMessage);

            string text = Encoding.UTF8.GetString(buff.ToArray());
            Debug.WriteLine($"[WsClient] 接收訊息({text}) from \'{_svrurl}\'");
            #if DEBUG
            Directory.CreateDirectory(DBG_DIR);
            string checkfile = $"{DBG_DIR}Receive_{getActionName(text)}_{DateTime.Now.ToString("yyMMdd_HHmmss_ffff")}.txt";
            File.WriteAllText(checkfile, $"{text}\n\nReceived from \'{_svrurl}\'.");
            #endif
            return text;
        }
        /// <summary>取得訊息中的action名稱。</summary>
        private string getActionName(string msg)
        {
            string lwr = msg.ToLower();
            string pattern = "\"action\":\"";
            if (!lwr.Contains(pattern))
                return null;

            int idxStart = lwr.IndexOf(pattern) + pattern.Length;
            int idxEndQuote = lwr.IndexOf('\"', idxStart);
            int actionNameLen = idxEndQuote - idxStart;
            
            return msg.Substring(idxStart, actionNameLen);
        }

        /// <summary>開始監聽訊息。</summary>
        /// <remarks>務必在register成功後才進行。</remarks>
        private void listenOn()
        {
            Debug.WriteLine($"[WsClient] 開始監聽。");
            while (_cts != null && !_cts.Token.IsCancellationRequested)
            {
                if (((int)_ws.State) > ((int)WebSocketState.Open))
                    break;  // 若溝通將要關閉則結束監聽
                try
                {
                    string msg = receiveMessage();
                    var rcv = JsonSerializer.Deserialize<Dictionary<string, object>>(msg);
                    SdWsEventHandlerArgs arg = null;
                    JsonSerializerOptions op = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    switch (rcv["action"].ToString())
                    {
                        // case "getFolderInfos":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_NODE>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})請問{req.Path}的資訊。 (僅資料夾?{req.DirOnly})");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "ClientSetting":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_SET>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想設定你的資訊。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "CurrentClientSetting":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_CURRSET>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})請問你的設定資訊。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "hiddenCheck":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_HIDDENCHECK>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想問{req.Path}能否設為隱藏區。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "secprocCheck":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_SECPROCCHECK>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想問{req.Path}能否設為安全程序。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "createFolder":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_NEWFOLDR>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想建立資料夾{req.Fullpath}。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "ClientBatchSetting":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_BATCH>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想做批次設定。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "getSettingRequest":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_GETSETTING>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})要求你做getSetting。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "getClientPerm":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_GETCLNPERM>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想索取使用者權限資訊。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "setClientPerm":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_SETCLNPERM>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想設定使用者權限資訊。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        // case "getClientUsers":
                        // {
                        //     var req = JsonSerializer.Deserialize<SDWS_RQST_GETCLNUSERS>(msg, op);
                        //     Debug.WriteLine($"[WsClient] server({req.Account})想索取使用者名單。");
                        //     arg = new SdWsEventHandlerArgs { ServerUrl = _svrurl, Request = req, Response = null };
                        //     break;
                        // }
                        default:
                        {
                            Debug.WriteLine($"[WsClient] 收到未知訊息。 {msg}");
                            break;
                        }
                    }
                    
                    if (arg != null)
                    {
                        OnRequset(arg);
                        if (arg.Response != null)
                            sendMessage(JsonSerializer.Serialize<object>(arg.Response));
                    }
                }
                catch (Exception excp)
                {
                    Debug.WriteLine($"[WsClient] 監聽訊息時發生例外狀況。({excp.GetType().Name} - {excp.Message})");
                }
            }
            Debug.WriteLine($"[WsClient] 結束監聽。");
        }

        /// <summary>Web Socket server送來request的事件。</summary>
        public SdWsEventHandler OnServerRequest;

        protected virtual void OnRequset(SdWsEventHandlerArgs arg)
        {
            if (OnServerRequest != null)
                OnServerRequest(arg);
        }
    }
    public delegate void SdWsEventHandler(SdWsEventHandlerArgs args);
    public class SdWsEventHandlerArgs : EventArgs
    {
        public string ServerUrl {get; set;}
        public object Request {get; set;}
        public object Response {get; set;}
    }

    #region send class
    /// <summary>註冊的請求。</summary>
    public class SDWS_RQST_REG
    {
        [JsonPropertyName("action")]
        public string Action {get; init;}
        [JsonPropertyName("macAddr")]
        /// <remarks>已棄用。</remarks>
        public string MAC {get; init;}
        [JsonPropertyName("uniqueKey")]
        public string UniKey {get; init;}

        /// <summary>建立web socket server的連線請求封包。</summary>
        /// <param name="privip">連線到web socket server的private IP。web socket server與control server是同一台主機所以可使用<see cref="CtlSvrConn.MyPrivateIP"/>。</param>
        /// <remarks></remarks>
        public SDWS_RQST_REG(string privip)
        {
            Action = "getClientConnectInfos";
            MAC = getMac(privip);
            UniKey = getUniKey(MAC);
        }

        private string getMac(string privateIP)
        {
            string macAddrs = string.Empty;

            // 查看該IP address網卡的MAC address
            NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface network in networks)
            {
                IPInterfaceProperties prop = network.GetIPProperties();
                foreach (UnicastIPAddressInformation ip in prop.UnicastAddresses)
                {
                    if (privateIP == ip.Address.ToString())
                    {
                        byte[] addrByte = network.GetPhysicalAddress().GetAddressBytes();
                        macAddrs = BitConverter.ToString(addrByte);
                    }
                }
            }
            return macAddrs;
        }
         
        /// <summary>以MAC address加上固定運算取得unique key。</summary>
        /// <remarks>其實是ticket的算法。</remarks>
        private string getUniKey(string macaddrs)
        {
            string key = string.Empty;
            string tks =  $"{macaddrs}SDefenseGetTicket";
            byte[] ticketB = SHA256.HashData(Encoding.UTF8.GetBytes(tks));
            foreach (byte b in ticketB)
                key += $"{b:X02}";

            return key;
        }
    }
    #endregion
    /// <summary>基本的回覆訊息。</summary>
    /// <remarks>適用於下列action：<list>
    /// <item>getClientConnectInfos</item>
    /// <item>ClientSetting</item>
    /// <item>sysFolderCheck</item>
    /// <item>createFolder</item>
    /// </list></remarks>
    public class SDWS_RESPONSE
    {
        [JsonPropertyName("action")]
        public string Action {get; set;}
        [JsonPropertyName("message")]
        public string Message {get; set;}
        [JsonPropertyName("code")]
        public int Code {get; set;}
    }

    #region receive class
    public class SDWS_REQUEST
    {
        [JsonPropertyName("action")]
        public string Action {get; set;}
        [JsonPropertyName("account")]
        public string Account {get; set;}
    }

    #endregion

    public class SdWsException : Exception
    {
        public SdWsException(int res, string message) : base(message)
        {
            HResult = res;
        }
    }
}