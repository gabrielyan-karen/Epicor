using System;
using System.Linq;

namespace Epicor
{
    public class ListMappingHelper
    {
        /// <summary>
        /// Reurn oject
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Object mapper
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public ListMappingHelper(object source, object destination)
        {
            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destionationProperties = destinationType.GetProperties();

            var commonProperties = from sp in sourceProperties
                                   join dp in destionationProperties on new { sp.Name, sp.PropertyType } equals
                                       new { dp.Name, dp.PropertyType }
                                   select new { sp, dp };

            foreach (var match in commonProperties)
            {
                match.dp.SetValue(destination, match.sp.GetValue(source, null), null);
            }

            Value = destination;
        }
    }
}
