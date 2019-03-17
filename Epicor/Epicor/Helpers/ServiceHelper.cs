using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;

namespace Epicor
{
    class ServiceHelper
    {
        /// <summary>
        /// Reurn oject
        /// </summary>
        public object Value { get; set; }

        public ServiceHelper(object svc)
        {
            Type sourceType = svc.GetType();
            NetworkCredential serviceCreds = new NetworkCredential(Parameters.EpicorUserName, Parameters.EpicorPassword);
            List<PropertyInfo> sourceProperties = sourceType.GetProperties().ToList();
            PropertyInfo credentialsProperty = sourceProperties.Where(o => o.Name == "Credentials").FirstOrDefault();
            PropertyInfo baseUriProperty = sourceProperties.Where(o => o.Name == "BaseUri").FirstOrDefault();
            CredentialCache cache = new CredentialCache { { baseUriProperty.GetValue(svc) as Uri, "Basic", serviceCreds } };

            credentialsProperty.SetValue(svc, cache);
            Value = svc;
        }
    }
}
