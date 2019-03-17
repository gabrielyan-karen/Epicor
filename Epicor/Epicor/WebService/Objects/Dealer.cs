using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Epicor.WebService.CustomerService;
using Epicor.SalesRepServiceReference.Erp;

namespace Epicor.Objects
{
    public class Dealer
    {
        #region Properties
        public string WCDealerID { get; private set; }
        public string DealerID { get; private set; }
        public int CustomerNumber { get; private set; }
        public string Name { get; private set; }
        public string Address1 { get; private set; }
        public string Address2 { get; private set; }
        public string Address3 { get; private set; }
        public string City { get; private set; }
        public string StateProvince { get; private set; }
        public string Zip { get; private set; }
        public string Country { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public decimal DiscountRate { get; private set; }
        public string SalesRepEmail { get; private set; }
        #endregion

        #region Constructors
        public Dealer(string dealerId)
        {
            CustomerDataSetTypeCustomer customer = CustomerObject(dealerId);

            this.WCDealerID = dealerId;
            this.DealerID = customer.CustID;
            this.CustomerNumber = customer.CustNum;
            this.Name = customer.Name;
            this.Address1 = customer.Address1;
            this.Address2 = customer.Address2;
            this.Address3 = customer.Address3;
            this.City = customer.City;
            this.StateProvince = customer.State;
            this.Zip = customer.Zip;
            this.Country = customer.Country;
            this.Phone = customer.PhoneNum;
            this.Email = CleanEmailAddress(customer.EMailAddress);
            this.DiscountRate = customer.DiscountPercent;
            this.SalesRepEmail = GetSalesRepEmail(customer.SalesRepCode);
        }

        public Dealer(int customerNumber, char idSuffix)
        {
            CustomerDataSetTypeCustomer customer = CustomerObject(customerNumber);

            this.WCDealerID = customer.CustID.ToString() + idSuffix;
            this.DealerID = customer.CustID;
            this.CustomerNumber = customer.CustNum;
            this.Name = customer.Name;
            this.Address1 = customer.Address1;
            this.Address2 = customer.Address2;
            this.Address3 = customer.Address3;
            this.City = customer.City;
            this.StateProvince = customer.State;
            this.Zip = customer.Zip;
            this.Country = customer.Country;
            this.Phone = customer.PhoneNum;
            this.Email = CleanEmailAddress(customer.EMailAddress);
            this.DiscountRate = customer.DiscountPercent;
            this.SalesRepEmail = GetSalesRepEmail(customer.SalesRepCode);
        }

        private Dealer()
        {

        }
        #endregion

        private string GetSalesRepEmail(string salesRepCode)
        {
            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Erp.BO.SalesRepSvc", "SalesReps", $"Company eq '{Parameters.CompanyID}' and SalesRepCode eq '{salesRepCode}'", typeof(List<SalesRep>));

                return (ajax.Value as List<SalesRep>).FirstOrDefault().EMailAddress;
            }
            catch
            {
                return string.Empty;
            }
        }

        private CustomerDataSetTypeCustomer CustomerObject(string customerId)
        {
            CustomerDataSetTypeCustomer mapped;

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Erp.BO.CustomerSvc", "Customers", $"Company eq '{Parameters.CompanyID}' and CustID eq '{ScrubDealerId(customerId)}'", typeof(List<CustomerServiceReference.Erp.Customer>));

                mapped = (CustomerDataSetTypeCustomer)(new MappingHelper((ajax.Value as List<CustomerServiceReference.Erp.Customer>).FirstOrDefault(), new CustomerDataSetTypeCustomer()).Value);
            }
            catch (Exception ex)
            {
                mapped = null;
                throw new ArgumentOutOfRangeException("There are no matching dealers with the dealer ID " + customerId + ".", ex);
            }

            return mapped;
        }

        private CustomerDataSetTypeCustomer CustomerObject(int customerNumber)
        {
            CustomerDataSetTypeCustomer mapped;

            try
            {
                AjaxServiceHelper ajax = new AjaxServiceHelper("Erp.BO.CustomerSvc", "Customers", $"Company eq '{Parameters.CompanyID}' and CustNum eq '{customerNumber}'", typeof(List<CustomerServiceReference.Erp.CustomerListItem>));

                mapped = (CustomerDataSetTypeCustomer)(new MappingHelper((ajax.Value as List<CustomerServiceReference.Erp.CustomerListItem>).FirstOrDefault(), new CustomerDataSetTypeCustomer()).Value);

            }
            catch (Exception ex)
            {
                mapped = null;
                throw new ArgumentOutOfRangeException("There are no matching dealers with the customer number " + customerNumber.ToString() + ".", ex);
            }

            return mapped;
        }

        private string CleanEmailAddress(string email)
        {
            try
            {
                MailAddress addr = new MailAddress(email);

                return addr.Address.ToLower().Trim();
            }
            catch
            {
                return "";
            }
        }

        internal static string ScrubDealerId(string dealerId)
        {
            return dealerId.ToLower().Replace("e", "").Replace("p", "");
        }
    }
}
