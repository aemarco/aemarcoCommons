using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts.Api.ResponseObjects;

namespace Extensions.contentExtensions
{
    public static class ApiResponseExtensions
    {


        /// <summary>
        /// Treeify Categories, Main and subs
        /// </summary>
        /// <param name="categories">categories to treeify</param>
        /// <returns>treeified category list</returns>
        public static List<ExtendedCategory> TreeifyCategories(this List<ExtendedCategory> categories)
        {

            var result = new List<ExtendedCategory>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var entry in categories.Where(x => x.IsMainCategory))
            {
                var subs = categories
                    .Where(x =>
                        x.CategoryString.Contains("_") &&
                        x.CategoryString.StartsWith(entry.CategoryString))
                    .ToList();

                entry.SubCategories = subs;
                result.Add(entry);
            }
            return result;
        }


        



    }
}
