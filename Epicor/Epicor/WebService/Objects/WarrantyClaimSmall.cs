using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Epicor.WebService.WarrantyClaimService;
using Epicor.WarrantyClaimServiceReference.Ice;

namespace Epicor.Objects
{
    public class WarrantyClaimSmall
    {
        public string LeeBoyID { get; set; }
        public DateTime DateReceived { get; set; }
        public string UnitSerialNumber { get; set; }
        public char ClaimType { get; set; }
        public string Model { get; set; }
        public string ShortDescription { get; set; }
        public int ClaimStatusID { get; set; }
        public bool IsReturnRequired { get; set; }
        public int CampaignID { get; set; }

        public static List<WarrantyClaimSmall> WarrantyClaimsForDealer(string dealerId)
        {
            WarrantyClaimService claimsService = new WarrantyClaimService();

            CallContextDataSetType callContextIn = null;
            bool morePages = false;
            CallContextDataSetType callContextOut = null;
            string whereClause = "Character08 = '" + Dealer.ScrubDealerId(dealerId) + "'";

            UD100ListDataSetTypeUD100List[] ds = null;

            try
            {
                ds = claimsService.GetList(Epicor.Parameters.CompanyID, whereClause, 0, 0, callContextIn, out morePages, out callContextOut).UD100ListDataSet;
            }
            catch
            {
                return new List<WarrantyClaimSmall>();
            }

            List<WarrantyClaimSmall> claims = new List<WarrantyClaimSmall>();

            foreach (UD100ListDataSetTypeUD100List c in ds)
            {
                WarrantyClaimSmall claim = new WarrantyClaimSmall();

                claim.LeeBoyID = c.Key1;
                claim.DateReceived = c.Date04;
                claim.UnitSerialNumber = c.ShortChar01;
                claim.ClaimType = WarrantyClaim.GetClaimType(c.Key3);
                claim.Model = c.ShortChar03;
                claim.ShortDescription = c.Character04;
                claim.ClaimStatusID = WarrantyClaim.GetClaimStatusID(c.Character10);
                claim.IsReturnRequired = false;
                claim.CampaignID = (int)c.Number08;

                claims.Add(claim);
            }

            claims = GetIsReturnRequired(claims);

            return claims;
        }

        public static List<WarrantyClaimSmall> WarrantyClaimPartsForWarrantyClaim(string leeBoyId, List<WarrantyClaimSmall> claims)
        {
            List<UD100DataSetTypeUD100> mapped = new List<UD100DataSetTypeUD100>();
            List<WarrantyClaimSmall> newClaims = new List<WarrantyClaimSmall>();

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD100Svc", "UD100As", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{leeBoyId}' and ChildKey1 eq 'Part'", typeof(List<UD100>));

                foreach (UD100 item in (List<UD100>)ajax.Value)
                {
                    mapped.Add((UD100DataSetTypeUD100)(new MappingHelper(item, new UD100DataSetTypeUD100()).Value));
                }

                foreach (WarrantyClaimSmall claim in claims)
                {
                    var returnRequireds = mapped.Where(o => o.Key1 == claim.LeeBoyID && o.CheckBox03);

                    var newClaim = claim;
                    newClaim.IsReturnRequired = (returnRequireds != null && returnRequireds.Count() > 0);
                    newClaims.Add(newClaim);
                }

                return newClaims;
            }
            catch (Exception ex)
            {
                return claims;
            }
        }

        private static List<WarrantyClaimSmall> GetIsReturnRequired(List<WarrantyClaimSmall> claims)
        {
            WarrantyClaimService claimsService = new WarrantyClaimService();

            CallContextDataSetType callContextIn = null;
            bool morePages = false;
            CallContextDataSetType callContextOut = null;
            string whereClauseUD100 = BuildIsReturnRequiredWhereClause(claims);
            string whereClauseUD100A = "ChildKey1 = 'Part'";

            UD100DataSetType ds = null;

            try
            {
                ds = claimsService.GetRows(Epicor.Parameters.CompanyID, whereClauseUD100, "", whereClauseUD100A, 0, 0, callContextIn, out morePages, out callContextOut);
            }
            catch
            {
                return claims;
            }

            if (ds == null || ds.UD100DataSet == null)
            {
                return claims;
            }

            var matches = ds.UD100DataSet.Where(o => o.GetType() == typeof(UD100DataSetTypeUD100A)).Select(o => (UD100DataSetTypeUD100A)o);

            if (matches == null || matches.Count() <= 0)
            {
                return claims;
            }


            List<WarrantyClaimSmall> newClaims = new List<WarrantyClaimSmall>();

            foreach (WarrantyClaimSmall claim in claims)
            {
                var returnRequireds = matches.Where(o => o.Key1 == claim.LeeBoyID && o.CheckBox03);

                var newClaim = claim;
                newClaim.IsReturnRequired = (returnRequireds != null && returnRequireds.Count() > 0);
                newClaims.Add(newClaim);
            }

            return newClaims;
        }

        private static string BuildIsReturnRequiredWhereClause(List<WarrantyClaimSmall> claims)
        {
            if (claims == null || claims.Count <= 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < claims.Count; i++)
            {
                var claim = claims[i];
                sb.Append("Key1 = '" + claim.LeeBoyID + "'");

                if (i < claims.Count - 1)
                {
                    sb.Append(" OR ");
                }
            }

            return sb.ToString();
        }
    }
}
