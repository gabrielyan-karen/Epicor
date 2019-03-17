using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Design;
using Microsoft.Web.Services3.Security.Tokens;

using System;
using System.Collections.Generic;
using System.Net;
using System.Configuration;
using System.Web;

namespace Epicor
{
    public static class Parameters
    {
        public static string CompanyID
        {
            get
            {
                return Properties.Settings.Default.WebServiceCompanyID;
            }
        }

        public static string EpicorUrl
        {
            get
            {
                return Properties.Settings.Default.EpicorAPIUrl;
            }
        }

        public static string EpicorUserName
        {
            get
            {
                return Properties.Settings.Default.EpicorAPIUserName;
            }
        }

        public static string EpicorPassword
        {
            get
            {
                return Properties.Settings.Default.EpicorAPIPassword;
            }
        }

        private static NetworkCredential Credentials
        {
            get
            {
                return new NetworkCredential(ConfigurationManager.AppSettings["WebServiceUserName"], ConfigurationManager.AppSettings["WebServicePassword"]);
            }
        }

        public static Policy Policy
        {
            get
            {
                UsernameOverTransportAssertion item = new UsernameOverTransportAssertion();
                item.UsernameTokenProvider = new UsernameTokenProvider(Epicor.Parameters.Credentials.UserName, Epicor.Parameters.Credentials.Password);
                Policy policy = new Policy(item);

                return policy;
            }
        }

        private static Uri WebServiceBaseUrl
        {
            get
            {
                return new Uri(ConfigurationManager.AppSettings["WebServiceBaseUrl"]);
            }
        }

        public static Uri CustomerServiceUrl
        {
            get
            {
                return new Uri(WebServiceBaseUrl.ToString() + Properties.Settings.Default.CustomerServicePage);
            }
        }

        public static Uri PartServiceUrl
        {
            get
            {
                return new Uri(WebServiceBaseUrl.ToString() + Properties.Settings.Default.PartServicePage);
            }
        }

        public static Uri SalesRepServiceUrl
        {
            get
            {
                return new Uri(WebServiceBaseUrl.ToString() + Properties.Settings.Default.SalesRepServicePage);
            }
        }

        public static Uri WarrantyRegistrationServiceUrl
        {
            get
            {
                return new Uri(WebServiceBaseUrl.ToString() + Properties.Settings.Default.WarrantyRegistrationServicePage);
            }
        }

        public static Uri WarrantyClaimServiceUrl
        {
            get
            {
                return new Uri(WebServiceBaseUrl.ToString() + Properties.Settings.Default.WarrantyClaimServicePage);
            }
        }

        public static Uri AttachmentServiceUrl
        {
            get
            {
                return new Uri(WebServiceBaseUrl.ToString() + Properties.Settings.Default.AttachmentServicePage);
            }
        }

        public static Uri CampaignServiceUrl
        {
            get
            {
                return new Uri(WebServiceBaseUrl.ToString() + Properties.Settings.Default.CampaignServicePage);
            }
        }
    }
}