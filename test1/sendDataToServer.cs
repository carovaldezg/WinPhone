using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace test1
{
    class sendDataToServer
    {
        Uri uri;
        HttpWebRequest request;
        String url;
        String res;


        public sendDataToServer()
        {
            url = "http://beta.apisense.io/WindowsPhone/upload/data/caro/1.0/signature";
            uri = new System.Uri(url);
            request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/octet-stream";

        }

        public byte[] buffer { get; set; }



        public void sendFileHttpPost()
        {
            try
            {
                request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
            }
            catch (Exception e) 
            {
                MessageBox.Show(e.Message);
            }
        }

        void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            request = (HttpWebRequest)callbackResult.AsyncState;
            // End the stream request operation
            Stream postStream = request.EndGetRequestStream(callbackResult);

            postStream.Write(buffer, 0, buffer.Length);
            postStream.Close();

            // Start the web request
            request.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), request);

        }

         void GetResponsetStreamCallback(IAsyncResult callbackResult)
          {
                HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult);
                    HttpStatusCode responseStatus = response.StatusCode;
                    if (responseStatus != HttpStatusCode.OK)
                    
                        using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
                        {
                            res = httpWebStreamReader.ReadToEnd();
                        }

                    
                }

                catch (Exception e)
                {

                }
               
            
    
             }


         public String getResponse()
         {
             return res;
         }

  /*      void GetResponsetStreamCallback(IAsyncResult callbackResult)
        {
            //     try
            //   {
            HttpWebRequest webRequest = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse response;

            // End the get response operation

            response = (HttpWebResponse)webRequest.EndGetResponse(callbackResult);

            Stream streamResponse = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            var Response = streamReader.ReadToEnd();
            //outputbox.Text = Response.ToString();

            streamResponse.Close();
            streamReader.Close();
            response.Close();

            //       }
            //    catch (WebException e)
            //  {
            // Error treatment
            // ...
            //} 
            //  }

        }
   * */
    }    

        
}
