using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Epicor.WebService.WarrantyRegistrationService;

namespace Epicor.Objects
{
    public class RegisteredUnit : UD01Unit
    {
        public RegisteredUnit(int customerNumber, string serialNumber)
            : base(customerNumber, serialNumber)
        {
        }

        private RegisteredUnit()
            : base()
        {

        }

        public static List<RegisteredUnit> RegisteredUnitsForDealer(int customerNumber)
        {
            return GetList(customerNumber, string.Empty);
        }

        public static List<RegisteredUnit> RegisteredUnitsForDealerNew(string dealerId)
        {
            return GetListNew(dealerId, string.Empty);
        }

        public static List<RegisteredUnit> SearchForRegisteredUnitsForDealer(int customerNumber, string partialSerialNumber)
        {
            return GetList(customerNumber, partialSerialNumber);
        }

        public static List<RegisteredUnit> SearchForRegisteredUnitsForDealerNew(string dealerId, string partialSerialNumber)
        {
            return GetListNew(dealerId, partialSerialNumber);
        }

        private static List<RegisteredUnit> GetList(int customerNumber, string serialNumber)
        {
            List<UD01ListDataSetTypeUD01List> ds = GetDataSet(customerNumber, serialNumber);

            List<RegisteredUnit> units = new List<RegisteredUnit>();

            foreach (UD01ListDataSetTypeUD01List u in ds)
            {
                RegisteredUnit unit = new RegisteredUnit();

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

        private static List<RegisteredUnit> GetListNew(string dealerId, string serialNumber)
        {
            List<UD01ListDataSetTypeUD01List> ds = GetDataSetNew(dealerId, serialNumber, true);

            List<RegisteredUnit> units = new List<RegisteredUnit>();

            foreach (UD01ListDataSetTypeUD01List u in ds)
            {
                RegisteredUnit unit = new RegisteredUnit();

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
