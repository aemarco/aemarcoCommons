using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Contracts.Api.ResponseObjects
{
    public class ExtendedCategory
    {
        /// <summary>
        /// Id of the category, referred to in filters etc.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// path of category ex. "Girls_Bikes" is main Category Girls, subcategory Bikes 
        /// </summary>
        public string CategoryString { get; set; }
        /// <summary>
        /// Name of Category for user interface
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// Icon as a Base64 string (only root categories)
        /// </summary>
        public string Icon { get; set; }
        
        /// <summary>
        /// List of Subcategories (only if tree version is requested)
        /// </summary>
        public List<ExtendedCategory> SubCategories { get; set; }
        /// <summary>
        /// true if this category is a root category
        /// </summary>
        public bool IsMainCategory { get; set; }
        
        
        /// <summary>
        /// true if this category is adult relevant
        /// </summary>
        //logic
        [Obsolete]
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once InconsistentNaming
        public bool FSK_Relevat =>
            
            CategoryString.StartsWith("Girls") ||
            CategoryString.StartsWith("Men") ||
            CategoryString == string.Empty ||
            CategoryString == "All";



        /// <summary>
        /// true if this category is adult relevant
        /// </summary>
#pragma warning disable 612
        public bool FskRelevant => FSK_Relevat;
#pragma warning restore 612


    }
}
