using System;
using System.Linq;
using AutoMapper;

namespace Epicor
{
    public class AutoMappingHelper
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
        public AutoMappingHelper(object source, object destination)
        {
            Type st = source.GetType();
            Type dt = destination.GetType();

            Value = null;
        }
    }
}
