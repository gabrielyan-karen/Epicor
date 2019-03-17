using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Epicor.WebService.WarrantyRegistrationService;
using Epicor.WarrantyRegistrationServiceReference.Ice;
using System.Data.Services.Client;
using System.Net;
using Newtonsoft.Json;

namespace Epicor.Objects
{
    public abstract class UD01Unit
    {
        public string SerialNumber { get; protected set; }
        public int CustomerNumber { get; protected set; }
        public string Model { get; protected set; }
        public string EngineNumber { get; protected set; }
        public string OwnerName { get; protected set; }
        public DateTime WarrantyStart { get; protected set; }
        public DateTime WarrantyEnd { get; protected set; }
        public bool HasExtendedWarranty { get; protected set; }
        protected UD01ListDataSetTypeUD01List UnitObject { get; private set; }
        public bool IsRegistered { get { return (this.OwnerName != ""); } }

        public UD01Unit(int customerNumber, string serialNumber)
        {
            UD01ListDataSetTypeUD01List mapped;

            try
            {

                AjaxServiceHelper ajax = new AjaxServiceHelper("Ice.BO.UD01Svc", "UD01s", $"Company eq '{Parameters.CompanyID}' and Key1 eq '{serialNumber}' and Key2 eq '{customerNumber.ToString()}'", typeof(List<UD01>));
                mapped = (UD01ListDataSetTypeUD01List)(new MappingHelper((ajax.Value as List<UD01>).FirstOrDefault(), new UD01ListDataSetTypeUD01List()).Value);

                this.SerialNumber = mapped.Key1;
                int cn = 0;
                int.TryParse(mapped.Key2, out cn);
                this.CustomerNumber = cn;
                this.Model = mapped.ShortChar20; //GetModel(mapped.Character01);
                this.EngineNumber = mapped.ShortChar01;
                this.OwnerName = mapped.ShortChar04;
                this.WarrantyStart = mapped.Date01;
                this.WarrantyEnd = mapped.Date02;
                this.HasExtendedWarranty = mapped.CheckBox20;
                this.UnitObject = mapped;
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException("There are no matching warranty registrations with the customer number " + customerNumber.ToString() + " and serial number " + serialNumber + ".", ex);
            }
        }


        public static List<UD01ListDataSetTypeUD01List> GetDataSet(int customerNumber, string serialNumber)
        {
            List<UD01ListDataSetTypeUD01List> list = new List<UD01ListDataSetTypeUD01List>();
            List<UD01> response;


            AjaxServiceHelper ajax;

            try
            {
                if (string.IsNullOrEmpty(serialNumber))
                {
                    ajax = new AjaxServiceHelper("Ice.BO.UD01Svc", "UD01s", $"Key2 eq '{customerNumber}' and ShortChar04 eq ''", typeof(List<UD01>));
                }
                else
                {
                    ajax = new AjaxServiceHelper("Ice.BO.UD01Svc", "UD01s", $"Key1 eq {serialNumber} and Key2 eq '{customerNumber}' and ShortChar04 eq ''", typeof(List<UD01>));
                }

                foreach (UD01 item in (List<UD01>)ajax.Value)
                {
                    list.Add((UD01ListDataSetTypeUD01List)(new MappingHelper(item, new UD01ListDataSetTypeUD01List()).Value));
                }
            }
            catch (Exception ex)
            {

            }

            return list;
        }

        public static List<UD01ListDataSetTypeUD01List> GetDataSetNew(string dealerId, string serialNumber, bool isRegistratedUnit = false)
        {
            List<UD01ListDataSetTypeUD01List> list = new List<UD01ListDataSetTypeUD01List>();

            char output = dealerId.Substring(dealerId.Length - 1, 1)[0];

            if(!Char.IsNumber(output))
            {
                dealerId = dealerId.TrimEnd(output);
            }

            AjaxServiceHelper ajax;

            try
            {
                string condition = (isRegistratedUnit) ? "ne" : "eq";

                string url = $"(year(Date01) eq {DateTime.Now.Year} or year(Date01) eq {DateTime.Now.Year - 1}) and {(!string.IsNullOrEmpty(serialNumber) ? $"Key1 eq {serialNumber} and" : "")} ShortChar03 eq '{dealerId}' and ShortChar04 {condition} ''";// and ShortChar04 eq '{string.Empty}'

                ajax = new AjaxServiceHelper("Ice.BO.UD01Svc", "UD01s", url, typeof(List<UD01>));

                foreach (UD01 item in (List<UD01>)ajax.Value)
                {
                    list.Add((UD01ListDataSetTypeUD01List)(new MappingHelper(item, new UD01ListDataSetTypeUD01List()).Value));
                }
            }
            catch (Exception ex)
            {

            }

            return list;
        }


        public UD01Unit()
        {

        }
    }
}
