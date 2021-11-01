using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Program myObj = new Program();
            String AuthKey = myObj.getAuthKey();

            myObj.GetHourlyData(AuthKey);
            Console.ReadLine();
        }

        public string getAuthKey()
        {
            string _authKey = "";
            string  userName = "fa470f7b-7310-4f8d-be9c-040dbcabbb59";
            string password = "61c9f489-0d82-4ba4-b74e-b96122c3c10a";
            var client = new RestClient("https://timeseries.sepa.org.uk/KiWebPortal/rest/auth/oidcServer/token");
            client.Authenticator = new HttpBasicAuthenticator(userName, password);
            var request = new RestRequest(Method.POST);
           
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials", ParameterType.RequestBody);
           
            IRestResponse response = client.Execute(request);
            var _content = response.Content;
            dynamic data = JObject.Parse(_content);
            _authKey = data.access_token;
            return _authKey;
        }

        public class TimedWebClient : WebClient
        {
            // Timeout in milliseconds, default = 600,000 msec
            public int Timeout { get; set; }

            public TimedWebClient()
            {
                this.Timeout = 120000;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var objWebRequest = base.GetWebRequest(address);
                objWebRequest.Timeout = this.Timeout;
                return objWebRequest;
            }
        }

        public void GetHourlyData(string _APIKEY)
        {

            var apiString = "https://timeseries.sepa.org.uk/KiWIS/KiWIS?service=kisters&type=queryServices&returnfields=Timestamp,Value,Quality%20Code&datasource=0&request=GetTimeseriesValues&ts_path=*/*/RE/Hour.Total&format=dajson&dateformat=yyyy-MM-dd%20HH:mm:ss&metadata=TRUE";
            string _json = "";
           

            using (TimedWebClient wc = new TimedWebClient())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization,
                "Bearer "+ _APIKEY);
               _json = wc.DownloadString(apiString);
            }
            List<HourlyData> _Items = JsonConvert.DeserializeObject<List<HourlyData>>(_json);
           
            int count = 0;
            foreach (var item in _Items)
            {
                Console.WriteLine("Station Name : " + item.station_name);

                string station_no = item.station_no;
                if (item.data.Any())
                {
                    var _count = item.data.Count();

                    for (int i = 0; i < _count; i++)
                    {
                        var _data = item.data[i];
                        var itemDate = _data[0].ToString();
                        var itemValue = "";
                        if (_data[1] != null)
                        {
                            itemValue = _data[1].ToString();
                        }

                      

                    }

                }
                Console.WriteLine("Count : " + count);
                Console.WriteLine("---------------------------------");
                count++;
            }

        

        }


        public class HourlyData
        {
            public string ts_id { get; set; }
            public string station_name { get; set; }
            public string station_latitude { get; set; }
            public string station_longitude { get; set; }
            public string parametertype_name { get; set; }
            public string ts_name { get; set; }
            public string ts_unitname { get; set; }
            public string ts_unitsymbol { get; set; }
            public string station_no { get; set; }
            public string station_id { get; set; }
            public string rows { get; set; }
            public string columns { get; set; }
            public List<List<object>> data { get; set; }
        }


    }
}
