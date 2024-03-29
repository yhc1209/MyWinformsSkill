using System;
using System.Text;
using System.Text.Json;
using System.IO;
using System.IO.Pipes;

namespace mwSkills.Libraries
{
    public class NPClient
    {
        public const string PIPENAME = "yhcnamedpipe";
        public static string LastPhrase = string.Empty;
        /// <summary>前次與<see cref="NPServer"/>溝通時的回傳code。</summary>
        /// <remarks><list>
        /// <item></item>
        /// </list></remarks>
        public static int LastRespCode {get; private set;} = 0;


        public static bool CheckNamedPipe()
        {
            using (NamedPipeClientStream pipe = new NamedPipeClientStream(".", PIPENAME, PipeDirection.InOut))
            {
                try
                {
                    pipe.Connect(100);
                    return true;
                }
                catch (Exception e)
                {
                    LastPhrase = $"{e.GetType().Name} - {e.Message}";
                    return false;
                }
            }
        }

        /// <returns>回傳是否成功傳送訊息。回傳<see langword="false"/>時可以查看<see cref="LastPhrase"/>。</returns>
        private static SD3NP_PKG sendNgetMsg(SD3NP_PKG request)
        {
            using (NamedPipeClientStream pipe = new NamedPipeClientStream(".", PIPENAME, PipeDirection.InOut))
            {
                int len, len_a;
                pipe.Connect(5000);
                byte[] buf = JsonSerializer.SerializeToUtf8Bytes<SD3NP_PKG>(request);
                pipe.Write(BitConverter.GetBytes(buf.Length));  // length
                pipe.Write(buf);                                // content
                pipe.Flush();

                buf = new byte[4];
                len_a = pipe.Read(buf, 0, 4);   // length
                SD3NP_PKG response;
                if (len_a < 4)
                {
                    response = new SD3NP_PKG();
                    response.Code = -1000;
                    response.Message = $"Failed to get response length. (only got {len_a} bytes)";
                }
                len = BitConverter.ToInt32(buf, 0);
                Array.Resize<byte>(ref buf, len);
                len_a = pipe.Read(buf, 0, len); // content

                if (len_a != len)
                {
                    response = new SD3NP_PKG();
                    response.Code = -1001;
                    response.Message = $"The response length is incorrect. (It should be {len} but actually is {len_a})";
                }
                else
                {
                    response = JsonSerializer.Deserialize<SD3NP_PKG>(buf);
                    if (response.Type != request.Type)
                    {
                        response.Code = -1002;
                        response.Message = $"The type of response is wrong. ({response.Type})";
                    }
                }

                LastRespCode = response.Code;
                return response;
            }
        }
    }
}