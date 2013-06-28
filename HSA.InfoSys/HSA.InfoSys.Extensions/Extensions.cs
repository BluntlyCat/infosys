// ------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// This class extends some types of c#.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Elements to string.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <returns>A string containing all elements of this list.</returns>
        public static string ElementsToString(this IList<string> strings)
        {
            int i = 0;
            var tmp = string.Empty;

            foreach (var str in strings)
            {
                if (i == 0)
                {
                    tmp += string.Format("{0}", str);
                }
                else
                {
                    tmp += string.Format(", {0}", str);
                }

                i++;
            }

            return tmp;
        }
    }
}
