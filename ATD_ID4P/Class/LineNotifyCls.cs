using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ATD_ID4P.Class
{
    public class LineNotifyCls
    {
        private string Token = "dKVYZfVMq3p0HcfklJgnVQ6NXf4pXoo0pv5ogOIzXuF";
        //private bool IsEnable = false;

        public LineNotifyCls(string _Token)
        {
            Token = _Token;
        }

        public bool SendNotify(string msg)
        {
            bool IsComplete = true;

            try
            {
                if (string.IsNullOrEmpty(msg))
                    return true;

                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
                var postData = string.Format("message={0}", msg);
                var data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + Token);

                using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception)
            {
                IsComplete = false;
                //Console.WriteLine(ex.ToString());
            }
            return IsComplete;
        }

    }
}
