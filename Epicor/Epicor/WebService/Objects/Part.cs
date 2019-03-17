using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epicor.WebService.PartService;
using Epicor.PartServiceReference.Erp;

namespace Epicor.Objects
{
    public class Part
    {
        #region Properties
        public string PartNumber { get; private set; }
        public string Description { get; private set; }
        public decimal UnitPrice { get; private set; }
        #endregion

        #region Constructors
        public Part(string partNumber)
        {
            if (!IsValidPartNumber(partNumber))
            {
                throw new ArgumentException("The part number is invalid.", "partNumber");
            }

            PartDataSetTypePart part = PartObject(partNumber);

            this.PartNumber = part.PartNum;
            this.Description = part.PartDescription;
            this.UnitPrice = part.UnitPrice;
            
            //this.PartObject = part;
        }

        private Part()
        {

        }
        #endregion

        public static PartDataSetTypePart PartObject(string partNumber)
        {
            PartDataSetTypePart mapped;

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Erp.BO.PartSvc", "Parts", $"Company eq '{Parameters.CompanyID}' and PartNum eq '{partNumber.Trim()}'", typeof(List<PartServiceReference.Erp.Part>));

                mapped = (PartDataSetTypePart)(new MappingHelper((ajax.Value as List<PartServiceReference.Erp.Part>).FirstOrDefault(), new PartDataSetTypePart()).Value);

            }
            catch (Exception ex)
            {
                mapped = null;
                throw new ArgumentOutOfRangeException("There are no matching parts with the part number " + partNumber + ".", ex);
            }
            return mapped;
        }

        private bool IsValidPartNumber(string partNumber)
        {
            return (!string.IsNullOrEmpty(partNumber) && partNumber.Trim() != "");
        }
    }
}
