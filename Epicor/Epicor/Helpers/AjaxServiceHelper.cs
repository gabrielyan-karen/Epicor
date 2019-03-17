using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Epicor
{
    class AjaxServiceHelper
    {
        public object Value { get; set; }

        public AjaxServiceHelper(string service, string name, string filter, Type type)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(Parameters.EpicorUserName, Parameters.EpicorPassword);
                string url = $"{string.Format(Parameters.EpicorUrl, service)}{name}?$filter={filter}&$top={ConfigurationManager.AppSettings["Swagger.ResultRowsCount"]}";
                string result = client.DownloadString(url);

                string valueResult = JsonConvert.DeserializeObject<dynamic>(result).value.ToString();

                Value = JsonConvert.DeserializeObject(valueResult, type);

            }
        }
    }
}
