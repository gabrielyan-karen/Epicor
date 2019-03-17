using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Net;
using Epicor.WebService.CampaignService;
using Epicor.CampaignServiceReference.Ice;
using System.Data.Services.Client;

namespace Epicor.Objects
{
    public class Campaign
    {
        #region Properties
        public string CampaignID { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        #endregion

        private Campaign()
        {

        }

        public Campaign(string campaignId)
        {
            if (!IsValidCampaignID(campaignId))
            {
                throw new ArgumentException("The campaign ID is invalid.", "campaignId");
            }

            var campaign = CampaignObject(campaignId);

            this.CampaignID = campaign.Key1;
            this.StartDate = campaign.Date01;
            this.EndDate = campaign.Date02;
        }

        private static UD02DataSetTypeUD02 CampaignObject(string campaignId)
        {

            UD02DataSetTypeUD02 mapped;

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD02Svc", "UD02s", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{campaignId}'", typeof(List<UD02>));
                mapped = (UD02DataSetTypeUD02)(new MappingHelper((ajax.Value as List<UD02>).FirstOrDefault(), new UD02DataSetTypeUD02()).Value);

            }
            catch (Exception ex)
            {
                mapped = null;
                throw new ArgumentOutOfRangeException("There are no matching campaigns with the campaign ID " + campaignId + ".", ex);
            }

            return mapped;
        }

        private bool IsValidCampaignID(string campaignId)
        {
            return (!string.IsNullOrEmpty(campaignId) && campaignId.Trim() != "");
        }
    }
}
