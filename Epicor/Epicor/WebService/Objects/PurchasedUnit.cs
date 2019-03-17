using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Epicor.WebService.WarrantyRegistrationService;

namespace Epicor.Objects
{
    public class PurchasedUnit : UD01Unit
    {      
        public PurchasedUnit(int customerNumber, string serialNumber) : base(customerNumber, serialNumber)
        {
            
        }

        private PurchasedUnit() : base()
        {

        }

        public static List<PurchasedUnit> PurchasedUnitsForDealer(int customerNumber)
        {
            return GetList(customerNumber, string.Empty);
        }

        public static List<PurchasedUnit> PurchasedUnitsForDealerNew(string dealerId)
        {
            return GetListNew(dealerId, string.Empty);
        }

        public static List<PurchasedUnit> SearchForPurchasedUnitsForDealer(int customerNumber, string partialSerialNumber)
        {
            return GetList(customerNumber, partialSerialNumber);
        }

        public static List<PurchasedUnit> SearchForPurchasedUnitsForDealerNew(string dealerId, string partialSerialNumber)
        {
            return GetListNew(dealerId, partialSerialNumber);
        }

        private static List<PurchasedUnit> GetList(int customerNumber, string serialNumber)
        {
            List<UD01ListDataSetTypeUD01List> ds = GetDataSet(customerNumber, serialNumber);

            List<PurchasedUnit> units = new List<PurchasedUnit>();

            foreach (UD01ListDataSetTypeUD01List u in ds)
            {
                PurchasedUnit unit = new PurchasedUnit();

                unit.SerialNumber = u.Key1;
                int key2CustomerNumber = 0;
                int.TryParse(u.Key2, out key2CustomerNumber);
                unit.CustomerNumber = key2CustomerNumber;
                unit.Model = u.ShortChar20;
                unit.EngineNumber = u.ShortChar01;
                unit.OwnerName = u.ShortChar04;
                unit.WarrantyStart = u.Date01;
                unit.WarrantyEnd = u.Date02;
                unit.HasExtendedWarranty = u.CheckBox20;

                units.Add(unit);
            }

            return units;
        }

        private static List<PurchasedUnit> GetListNew(string dealerId, string serialNumber)
        {
            List<UD01ListDataSetTypeUD01List> ds = GetDataSetNew(dealerId, serialNumber);

            List<PurchasedUnit> units = new List<PurchasedUnit>();

            foreach (UD01ListDataSetTypeUD01List u in ds)
            {
                PurchasedUnit unit = new PurchasedUnit();

                unit.SerialNumber = u.Key1;
                int key2CustomerNumber = 0;
                int.TryParse(u.Key2, out key2CustomerNumber);
                unit.CustomerNumber = key2CustomerNumber;
                unit.Model = u.ShortChar20;
                unit.EngineNumber = u.ShortChar01;
                unit.OwnerName = u.ShortChar04;
                unit.WarrantyStart = u.Date01;
                unit.WarrantyEnd = u.Date02;
                unit.HasExtendedWarranty = u.CheckBox20;

                units.Add(unit);
            }

            return units;
        }
    }
}
