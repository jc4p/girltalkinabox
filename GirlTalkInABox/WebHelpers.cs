using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GirlTalkInABox
{
    class WebHelpers
    {
        // Adapted from http://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data
        public static async Task<string> UploadFile(string url, Dictionary<string, string> formData, Stream fileStream, string fileParamName)
        {
            var request = await Task.Run(() =>
            {
                string boundary = "---------------------------" + DateTime.UtcNow.Ticks.ToString("x");
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                var wr = (HttpWebRequest)WebRequest.Create(url);
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = CredentialCache.DefaultCredentials;

                using (Stream rs = wr.GetRequestStream())
                {
                    foreach (var key in formData.Keys)
                    {
                        string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                        rs.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, formData[key]);
                        byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                        rs.Write(formitembytes, 0, formitembytes.Length);
                        rs.Write(boundarybytes, 0, boundarybytes.Length);
                    }

                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n";
                    string header = string.Format(headerTemplate, fileParamName);
                    byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                    rs.Write(headerbytes, 0, headerbytes.Length);

                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        rs.Write(buffer, 0, bytesRead);
                    }


                    byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    rs.Write(trailer, 0, trailer.Length);
                }
                using (var response = wr.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                {
                    var data = new StreamReader(responseStream);
                    return data.ReadToEnd();
                }
            });
            return request;
        }
    }
}
