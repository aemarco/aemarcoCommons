using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts.Api.ResponseObjects;

namespace Extensions.contentExtensions
{
    public static class ApiResponseExtensions
    {
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
