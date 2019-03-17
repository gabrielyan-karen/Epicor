using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epicor.WebService.WarrantyClaimService;
using Epicor.WarrantyClaimServiceReference.Ice;

namespace Epicor.Objects
{
    public class WarrantyClaimPart
    {
        #region Properties
        public int PartID { get; private set; }
        public string LeeBoyID { get; private set; }
        public int Quantity { get; private set; }
        public string PartNumber { get; private set; }
        public string Description { get; private set; }
        public decimal UnitCost { get; private set; }
        public decimal LeeBoyUnitCost { get; private set; }
        public decimal LeeBoyExtendedAmount
        {
            get
            {
                return AdjustedQuantity * LeeBoyUnitCost;
            }
        }        
        public string LeeBoyPartsComment { get; private set; }
        public bool LeeBoyPart { get; private set; }
        public bool ReturnedRequired { get; private set; }
        public int AdjustedQuantity { get; private set; }
        public decimal AdjustedUnitCost { get; private set; }
        public bool LeeBoyAdjusted
        {
            get
            {
                return (Quantity != AdjustedQuantity || UnitCost != AdjustedUnitCost);
            }
        }

        #endregion

        private WarrantyClaimPart()
        {

        }

        public WarrantyClaimPart(string leeBoyId, int partId)
        {
            List<WarrantyClaimPart> parts = WarrantyClaimPartsForWarrantyClaim(leeBoyId);

            if (parts == null || parts.Count() <= 0)
            {
                throw new ArgumentOutOfRangeException("There are no matching parts for the claim with LeeBoy ID " + leeBoyId + " and internal part ID " + partId.ToString() + ".");
            }

            var matches = parts.Where(p => p.PartID == partId);

            if (matches == null || matches.Count() <= 0)
            {
                throw new ArgumentOutOfRangeException("There are no matching parts for the claim with LeeBoy ID " + leeBoyId + " and internal part ID " + partId.ToString() + ".");
            }

            if (matches.Count() != 1)
            {
                throw new ArgumentOutOfRangeException("More than one matching part was found for the claim with LeeBoy ID " + leeBoyId + " and internal part ID " + partId.ToString() + ".");
            }

            WarrantyClaimPart part = matches.Take(1).Single();
            this.PartID = part.PartID;
            this.LeeBoyID = part.LeeBoyID;
            this.PartNumber = part.PartNumber;
            this.Description = part.Description;
            this.LeeBoyPartsComment = part.LeeBoyPartsComment;
            this.AdjustedQuantity = part.AdjustedQuantity;
            this.AdjustedUnitCost = part.AdjustedUnitCost;
            this.Quantity = part.Quantity;
            this.UnitCost = part.UnitCost;
            this.LeeBoyPart = part.LeeBoyPart;
            this.ReturnedRequired = part.ReturnedRequired;
            this.LeeBoyUnitCost = part.LeeBoyUnitCost;
        }
        
        public static List<WarrantyClaimPart> WarrantyClaimPartsForWarrantyClaim(string leeBoyId)
        {
            List<UD100DataSetTypeUD100> mapped = new List<UD100DataSetTypeUD100>();
            List<WarrantyClaimPart> parts = new List<WarrantyClaimPart>();

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD100Svc", "UD100As", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{leeBoyId}' and ChildKey1 eq 'Part'", typeof(List<UD100>));

                foreach (UD100 item in (List<UD100>)ajax.Value)
                {
                    mapped.Add((UD100DataSetTypeUD100)(new MappingHelper(item, new UD100DataSetTypeUD100()).Value));
                }

                foreach (UD100DataSetTypeUD100 o in mapped)
                {
                    WarrantyClaimPart part = new WarrantyClaimPart();

                    int id = 0;
                    part.PartID = id; //Epicor.IdGenerator.Generate();
                    part.LeeBoyID = o.Key1;

                    part.Description = o.Character01;
                    part.LeeBoyPartsComment = o.Character02;

                    part.AdjustedQuantity = (int)o.Number01;
                    part.AdjustedUnitCost = o.Number02;
                    part.Quantity = (int)o.Number07;
                    part.UnitCost = o.Number08;

                    part.LeeBoyPart = !o.CheckBox02;
                    part.ReturnedRequired = o.CheckBox03;

                    part.LeeBoyUnitCost = GetLeeBoyUnitCost(part.PartNumber);

                    parts.Add(part);
                }

                return parts;
            }
            catch (Exception ex)
            {
                return new List<WarrantyClaimPart>();
            }
        }

        private static decimal GetLeeBoyUnitCost(string partNumber)
        {
            try 
	        {
                return Part.PartObject(partNumber).UnitPrice;
	        }
	        catch (Exception)
	        {
                return 0;		
	        }
        }
    }
}
