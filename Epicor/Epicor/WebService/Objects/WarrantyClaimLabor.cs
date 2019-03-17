using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epicor.WebService.WarrantyClaimService;
using Epicor.WarrantyClaimServiceReference.Ice;

namespace Epicor.Objects
{
    public class WarrantyClaimLabor
    {
        #region Properties
        public int LaborID { get; private set; }
        public string LeeBoyID { get; private set; }
        public decimal ActualHours { get; private set; }
        public string Description { get; private set; }
        public decimal Rate { get; private set; }
        public string LeeBoyLaborComment { get; private set; }
        public decimal LeeBoyExtendedAmount
        {
            get
            {
                return AdjustedRate * AdjustedHours;
            }
        }
        public decimal AdjustedRate { get; private set; }
        public decimal AdjustedHours { get; private set; }
        public bool LeeBoyAdjusted
        {
            get
            {
                return (ActualHours != AdjustedHours || Rate != AdjustedRate);
            }
        }

        #endregion

        private WarrantyClaimLabor()
        {

        }

        public WarrantyClaimLabor(string leeBoyId, int laborId)
        {
            List<WarrantyClaimLabor> labors = WarrantyClaimLaborsForWarrantyClaim(leeBoyId);

            if (labors == null || labors.Count() <= 0)
            {
                throw new ArgumentOutOfRangeException("There are no matching labor items for the claim with LeeBoy ID " + leeBoyId + " and internal labor ID " + laborId.ToString() + ".");
            }

            var matches = labors.Where(l => l.LaborID == laborId);

            if (matches == null || matches.Count() <= 0)
            {
                throw new ArgumentOutOfRangeException("There are no matching labor items for the claim with LeeBoy ID " + leeBoyId + " and internal labor ID " + laborId.ToString() + ".");
            }

            if (matches.Count() != 1)
            {
                throw new ArgumentOutOfRangeException("More than one matching labor item was found for the claim with LeeBoy ID " + leeBoyId + " and internal labor ID " + laborId.ToString() + ".");
            }

            WarrantyClaimLabor labor = matches.Take(1).Single();
            this.LaborID = labor.LaborID;
            this.LeeBoyID = labor.LeeBoyID;
            this.Description = labor.Description;
            this.LeeBoyLaborComment = labor.LeeBoyLaborComment;
            this.ActualHours = labor.ActualHours;
            this.Rate = labor.Rate;
            this.AdjustedHours = labor.AdjustedHours;
            this.AdjustedRate = labor.AdjustedRate;
        }

        public static List<WarrantyClaimLabor> WarrantyClaimLaborsForWarrantyClaim(string leeBoyId)
        {
            List<UD100DataSetTypeUD100> mapped = new List<UD100DataSetTypeUD100>();
            List<WarrantyClaimLabor> labors = new List<WarrantyClaimLabor>();

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD100Svc", "UD100As", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{leeBoyId}' and ChildKey1 eq 'Labor'", typeof(List<UD100>));

                foreach (UD100 item in (List<UD100>)ajax.Value)
                {
                    mapped.Add((UD100DataSetTypeUD100)(new MappingHelper(item, new UD100DataSetTypeUD100()).Value));
                }

                foreach (UD100DataSetTypeUD100 o in mapped)
                {
                    WarrantyClaimLabor labor = new WarrantyClaimLabor();

                    int id = 0;
                    labor.LaborID = id;

                    labor.LeeBoyID = o.Key1;

                    labor.Description = o.Character01;
                    labor.LeeBoyLaborComment = o.Character02;

                    labor.ActualHours = o.Number03;
                    labor.Rate = o.Number04;
                    labor.AdjustedHours = o.Number05;
                    labor.AdjustedRate = o.Number06;

                    labors.Add(labor);
                }

                return labors;
            }
            catch (Exception ex)
            {
                return new List<WarrantyClaimLabor>();
            }
        }
    }
}
