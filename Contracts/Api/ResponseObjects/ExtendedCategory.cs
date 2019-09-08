using System.Collections.Generic;

namespace Contracts.Api.ResponseObjects
{
    public class ExtendedCategory
    {
        public int Id { get; set; }
        public string CategoryString { get; set; }
        public string Display { get; set; }
        public string Icon { get; set; }
        public List<ExtendedCategory> SubCategories { get; set; }
        public bool IsMainCategory { get; set; }

        //logic
        public bool FSK_Relevat
        {
            get
            {
                return
                  CategoryString.StartsWith("Girls") ||
                  CategoryString.StartsWith("Men") ||
                  CategoryString == string.Empty ||
                  CategoryString == "All";
            }
        }

    }
}
