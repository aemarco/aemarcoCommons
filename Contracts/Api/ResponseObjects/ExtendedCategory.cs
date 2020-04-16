using System;
using System.Collections.Generic;

namespace Contracts.Api.ResponseObjects
{
    public class ExtendedCategory
    {
        /// <summary>
        /// Id of the category, refered to in filters etc.
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
        /// List of Subcategories
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
        public bool FSK_Relevat
        {
            get
            {
                //TODO: nicht gerade der hingucker.... (All und "" sind irrelevant?)
                return
                  CategoryString.StartsWith("Girls") ||
                  CategoryString.StartsWith("Men") ||
                  CategoryString == string.Empty ||
                  CategoryString == "All";
            }
        }

    }
}
