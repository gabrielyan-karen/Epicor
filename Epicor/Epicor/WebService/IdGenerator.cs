using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epicor
{
    public static class IdGenerator
    {
        public static int MinValue
        {
            get
            {
                return 1000000;
            }
        }

        /// <summary>
        /// Generates a random integer between 1,000,000 and int.MaxValue, inclusive.
        /// </summary>
        /// <returns></returns>
        public static int Generate()
        {
            Random r = new Random();

            return r.Next(MinValue, int.MaxValue);
        }
    }
}
