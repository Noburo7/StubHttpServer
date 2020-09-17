using System;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;

namespace StubHttpServer
{
    class HttpServer
    {
        static void Main(string[] args)
        {
            try
            {
                var api1Url = ConfigurationManager.AppSettings["hostUrl"] + ConfigurationManager.AppSettings["api1addr"];
                var api2Url = ConfigurationManager.AppSettings["hostUrl"] + ConfigurationManager.AppSettings["api2addr"];

                var listener = new HttpListener();
                listener.Prefixes.Add(api1Url);
                listener.Prefixes.Add(api2Url);
                listener.Start();
                Console.WriteLine("Http serever was started.");
                Console.WriteLine($"{api1Url}");
                Console.WriteLine($"{api2Url}");

                while (true)
                {
                    var context = listener.GetContext();
                    var req = context.Request;
                    var res = context.Response;

                    //このプログラムではポストデータは使用しない。
                    string postData;
                    using (var sr = new StreamReader(req.InputStream, Encoding.UTF8))
                    {
                        postData = sr.ReadToEnd();
                    }

                    try
                    {
                        if (req.Url.LocalPath == ConfigurationManager.AppSettings["api1addr"])
                        {
                            //Process for api1
                            Console.WriteLine("API1 was called.");
                            res.StatusCode = 200;
                            byte[] content = Encoding.UTF8.GetBytes("Hello API1");
                            res.OutputStream.Write(content, 0, content.Length);
                        }
                        else if (req.Url.LocalPath == ConfigurationManager.AppSettings["api2addr"])
                        {
                            //Process for api2
                            Console.WriteLine("API2 was called.");
                            res.StatusCode = 200;
                            byte[] content = Encoding.UTF8.GetBytes("Hello API2");
                            res.OutputStream.Write(content, 0, content.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        res.StatusCode = 400;
                        byte[] content = Encoding.UTF8.GetBytes(ex.ToString());
                        res.OutputStream.Write(content, 0, content.Length);
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        res.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}