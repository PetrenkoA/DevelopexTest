using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace DevelopexTest
{
    public class WebAccessHelper
    {

        public static string MakeGetRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Thread.Sleep(500); //Imitate network latency
            using (StreamReader wr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                return wr.ReadToEnd();
            }
        }

        public static bool CheckConnection()
        {
            using (Ping p = new Ping())
            {
                string data = "Checking network availability";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                try
                {
                    PingReply reply = p.Send("8.8.8.8", timeout, buffer);
                    return (reply.Status == IPStatus.Success);
                }
                catch (Exception) { return false; }
            }
        }

    }
}
