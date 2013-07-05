// ------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Extensions
{
    using System.Collections.Generic;
    using HSA.InfoSys.Common.Entities;

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
        public static string ElementsToString(this IEnumerable<string> strings)
        {
            var i = 0;
            var tmp = string.Empty;

            foreach (var str in strings)
            {
                if (i == 0)
                {
                    tmp += string.Format("{0}", str);
                }
                else
                {
                    tmp += string.Format(",{0}", str);
                }

                i++;
            }

            return tmp;
        }

        /// <summary>
        /// Components to string.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <returns>A string containing all components of this list.</returns>
        public static string ComponentsToString(this IEnumerable<Component> components)
        {
            int i = 0;
            var tmp = string.Empty;

            foreach (var component in components)
            {
                if (i == 0)
                {
                    tmp += string.Format("{0}", component);
                }
                else
                {
                    tmp += string.Format(", {0}", component);
                }

                i++;
            }

            return tmp;
        }

        /// <summary>
        /// Orgs the units to string.
        /// </summary>
        /// <param name="orgUnits">The org units.</param>
        /// <returns>
        /// A string containing all org units of this list.
        /// </returns>
        public static string OrgUnitsToString(this IEnumerable<OrgUnit> orgUnits)
        {
            int i = 0;
            var tmp = string.Empty;

            foreach (var orgUnit in orgUnits)
            {
                if (i == 0)
                {
                    tmp += string.Format("{0}", orgUnit);
                }
                else
                {
                    tmp += string.Format(", {0}", orgUnit);
                }

                i++;
            }

            return tmp;
        }
    }
}
