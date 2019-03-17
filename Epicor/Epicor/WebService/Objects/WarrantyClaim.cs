using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Epicor.WebService.WarrantyClaimService;
using Epicor.WarrantyClaimServiceReference.Ice;

namespace Epicor.Objects
{
    public class WarrantyClaim
    {
        #region Properties
        public const char ClaimsDealerIdSuffix = 'P';

        public int WarrantyClaimID { get; private set; }
        public string LeeBoyID { get; private set; }
        public string DealerID { get; private set; }
        public int ClaimStatusID { get; private set; }
        public string DealerEmail { get; private set; }
        public string ShortDescription { get; private set; }
        public char ClaimType { get; private set; }
        public DateTime WarrantyStart { get; private set; }
        public DateTime WarrantyEnd { get; private set; }
        public DateTime DateReceived { get; private set; }
        public DateTime DateProcessed { get; private set; }
        public string DealerWorkOrderNumber { get; private set; }
        public DateTime DateOfRepair { get; private set; }
        public int HoursOperated { get; private set; }
        public string Model { get; private set; }
        public string UnitSerialNumber { get; private set; }
        public string MajorComponent_EngineSN { get; private set; }
        public string UnitOwnerName { get; private set; }
        public string DealerName { get; private set; }
        public string DealerServiceLocation { get; private set; }
        public string DealerPhone { get; private set; }
        public string DefineCauseOfProblem { get; private set; }
        public string DealerComments { get; private set; }
        public string FactoryApprovalNumber { get; private set; }
        public string ServiceManager { get; private set; }
        public string ServiceManagerPhone { get; private set; }
        public bool FactoryConsulted { get; private set; }
        public string FactoryContact { get; private set; }
        public string LeeBoyComments { get; private set; }
        public decimal CustomerTotalClaimed { get; private set; }
        public decimal LeeBoyTotalPaid { get; private set; }
        public decimal AdjustedPartsAmount { get; private set; }
        public decimal AdjustedLaborAmount { get; private set; }
        public decimal TotalAdjustedAmount
        {
            get
            {
                return AdjustedPartsAmount + AdjustedLaborAmount;
            }
        }
        public DateTime PartHoldSubmitDate { get; private set; }
        public int CampaignID { get; private set; }
        public string GoodwillApprovalNumber { get; private set; }

        public List<WarrantyClaimPart> Parts
        {
            get
            {
                return WarrantyClaimPart.WarrantyClaimPartsForWarrantyClaim(LeeBoyID);
            }
        }

        public List<WarrantyClaimLabor> Labors
        {
            get
            {
                return WarrantyClaimLabor.WarrantyClaimLaborsForWarrantyClaim(LeeBoyID);
            }
        }

        public List<WarrantyClaimAttachment> Attachments
        {
            get
            {
                return WarrantyClaimAttachment.WarrantyClaimAttachmentsForWarrantyClaim(LeeBoyID);
            }
        }
        #endregion

        private WarrantyClaim()
        {

        }

        public WarrantyClaim(string leeBoyId)
        {
            UD100ListDataSetTypeUD100List mapped;

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD100Svc", "UD100s", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{leeBoyId}'", typeof(List<UD100>));

                mapped = (UD100ListDataSetTypeUD100List)(new MappingHelper((ajax.Value as List<UD100>).FirstOrDefault(), new UD100ListDataSetTypeUD100List()).Value);


                WarrantyClaim claim = PopulateWarrantyClaimBasedOnUD100ListDataSetTypeUD100List(mapped);

                this.LeeBoyID = claim.LeeBoyID;
                this.ClaimType = claim.ClaimType;
                this.GoodwillApprovalNumber = claim.GoodwillApprovalNumber;
                this.FactoryContact = claim.FactoryContact;
                this.FactoryApprovalNumber = claim.FactoryApprovalNumber;
                this.ShortDescription = claim.ShortDescription;
                this.DefineCauseOfProblem = claim.DefineCauseOfProblem;
                this.LeeBoyComments = claim.LeeBoyComments;
                this.DealerComments = claim.DealerComments;
                this.DealerID = claim.DealerID;
                this.CampaignID = claim.CampaignID;
                this.ClaimStatusID = claim.ClaimStatusID;
                this.CustomerTotalClaimed = claim.CustomerTotalClaimed;
                this.WarrantyClaimID = claim.WarrantyClaimID;
                this.AdjustedPartsAmount = claim.AdjustedPartsAmount;
                this.AdjustedLaborAmount = claim.AdjustedLaborAmount;
                this.LeeBoyTotalPaid = claim.LeeBoyTotalPaid;
                this.DateOfRepair = claim.DateOfRepair;
                this.DateProcessed = claim.DateProcessed;
                this.PartHoldSubmitDate = claim.PartHoldSubmitDate;
                this.DateReceived = claim.DateReceived;
                this.FactoryConsulted = claim.FactoryConsulted;
                this.UnitSerialNumber = claim.UnitSerialNumber;
                this.HoursOperated = claim.HoursOperated;
                this.Model = claim.Model;
                this.MajorComponent_EngineSN = claim.MajorComponent_EngineSN;
                this.UnitOwnerName = claim.UnitOwnerName;
                this.DealerWorkOrderNumber = claim.DealerWorkOrderNumber;
                this.DealerServiceLocation = claim.DealerServiceLocation;
                this.DealerPhone = claim.DealerPhone;
                this.ServiceManager = claim.ServiceManager;
                this.ServiceManagerPhone = claim.ServiceManagerPhone;
                this.WarrantyStart = claim.WarrantyStart;
                this.WarrantyEnd = claim.WarrantyEnd;
                this.DealerEmail = claim.DealerEmail;
                this.DealerName = claim.DealerName;
            }
            catch (Exception ex)
            {
                mapped = null;
                throw new ArgumentOutOfRangeException("There are no matching claims with the LeeBoy ID " + leeBoyId + ".", ex);
            }
        }

        private static WarrantyClaim PopulateWarrantyClaimBasedOnUD100ListDataSetTypeUD100List(UD100ListDataSetTypeUD100List c)
        {
            WarrantyClaim claim = new WarrantyClaim();

            claim.LeeBoyID = c.Key1;
            claim.ClaimType = GetClaimType(c.Key3);

            claim.FactoryApprovalNumber = c.Character01;
            claim.FactoryContact = c.Character02;
            claim.GoodwillApprovalNumber = c.Character03;
            claim.ShortDescription = c.Character04;
            claim.DefineCauseOfProblem = c.Character05;
            claim.LeeBoyComments = c.Character06;
            claim.DealerComments = c.Character07;
            string dealerId = c.Character08;
            claim.DealerID = dealerId + ClaimsDealerIdSuffix;
            claim.ClaimStatusID = GetClaimStatusID(c.Character10);

            claim.CustomerTotalClaimed = c.Number01;

            int id = 0;
            try
            {
                id = (int)c.Number02;
            }
            catch
            {

            }

            claim.WarrantyClaimID = id;
            claim.AdjustedPartsAmount = c.Number05;
            claim.AdjustedLaborAmount = c.Number06;
            claim.LeeBoyTotalPaid = c.Number07;

            claim.CampaignID = (int)c.Number08;


            claim.DateOfRepair = c.Date01;
            claim.DateProcessed = c.Date02;
            claim.PartHoldSubmitDate = c.Date03;
            claim.DateReceived = c.Date04; // aka datesubmitted

            claim.FactoryConsulted = c.CheckBox01;

            claim.UnitSerialNumber = c.ShortChar01;

            int hoursOperated = 0;
            int.TryParse(c.ShortChar02, out hoursOperated);
            claim.HoursOperated = hoursOperated;

            claim.Model = c.ShortChar03;
            claim.MajorComponent_EngineSN = c.ShortChar04;
            claim.UnitOwnerName = c.ShortChar05;
            claim.DealerWorkOrderNumber = c.ShortChar06;
            claim.DealerServiceLocation = c.ShortChar07;
            claim.DealerEmail = c.ShortChar08;
            claim.ServiceManager = c.ShortChar09;
            claim.ServiceManagerPhone = c.ShortChar10;

            RegisteredUnit unit = null;
            Dealer dealer = new Dealer(dealerId);

            try
            {
                unit = new RegisteredUnit(dealer.CustomerNumber, claim.UnitSerialNumber);
            }
            catch
            {

            }

            if (unit != null)
            {
                claim.WarrantyStart = unit.WarrantyStart;
                claim.WarrantyEnd = unit.WarrantyEnd;
            }
            else
            {
                claim.WarrantyStart = SqlDateTime.MinValue.Value;
                claim.WarrantyEnd = SqlDateTime.MinValue.Value;
            }

            //claim.DealerEmail = dealer.Email;
            claim.DealerName = dealer.Name;
            claim.DealerPhone = dealer.Phone;

            return claim;
        }

        internal static bool IsReturnRequired(string leeBoyId)
        {
            try
            {
                //RestApi restApi = (new ServiceHelper(
                //    new RestApi(new Uri(string.Format(Parameters.EpicorUrl, "Ice.BO.UD100Svc")))
                //    ).Value as RestApi);

                //List<UD100> response = restApi.UD100s.Where(x => x.Company == Parameters.CompanyID && x.Key1 == leeBoyId/* && "ChildKey1 = 'Part'"*/ && x.CheckBox03 == true).ToList();

                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD100Svc", "UD100As", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{leeBoyId}' and ChildKey1 = 'Part' and CheckBox03 eq true", typeof(List<UD100>));
                int responseCount = (ajax.Value != null) ? (ajax.Value as List<UD100>).Count : 0;

                return (ajax.Value != null && responseCount > 0);
                //return (response != null && response.Count() > 0);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal static int GetClaimStatusID(string claimStatus)
        {
            int defaultStatus = 2; // submitted

            if (string.IsNullOrEmpty(claimStatus))
            {
                return defaultStatus;
            }

            claimStatus = claimStatus.ToLower().Trim();

            switch (claimStatus)
            {
                case "submitted": return defaultStatus;
                case "accepted": return 3;
                case "rejected": return 4;
                case "adjusted": return 5;
                case "hold": return 6;
                case "paid": return 7;
                default: return defaultStatus;
            }
        }

        internal static char GetClaimType(string claimType)
        {
            if (string.IsNullOrEmpty(claimType))
            {
                return '0';
            }

            claimType = claimType.ToLower().Trim();

            if (claimType.Length <= 0)
            {
                return '0';
            }

            char ct = claimType[0];

            switch (ct)
            {
                case 'e': return 'E';
                case 'p': return 'P';
                case 'u': return 'U';
                case 'c': return 'C';
                case 'g': return 'G';
                default: return '0';
            }
        }
    }
}
