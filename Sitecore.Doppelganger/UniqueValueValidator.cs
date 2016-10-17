using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
using Sitecore.Data.Validators;
using Sitecore.Search;

namespace Sitecore.Doppelganger
{
    public class UniqueValueValidator : StandardValidator
    {
        protected override ValidatorResult Evaluate()
        {
            string value = GetControlValidationValue();
            var root = Context.ContentDatabase.GetItem(ItemIDs.MediaLibraryRoot);
            var item = GetItem();

            //var duplicates = root.Axes.GetDescendants()
            //    .Where(x => x.ID != item.ID)
            //    .Where(x => !string.IsNullOrEmpty(x["hash"]))
            //    .Where(x => string.Equals(x["hash"], value, StringComparison.CurrentCultureIgnoreCase))
            //    .ToArray();

            var duplicates = FindMatchingHashes(item);

            if (duplicates.Any())
            {
                //set the error message
                var paths = string.Join(", ", duplicates.Select(x => x.Paths.MediaPath).ToArray());
                Text = GetText($"Value must be unique. Item has the same file hash as items {paths}.");

                return ValidatorResult.Error;
            }
            
            return ValidatorResult.Valid;
        }

        public Item[] FindMatchingHashes(Item item)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");
            using (var context = index.CreateSearchContext())
            {
                var predicate = PredicateBuilder.True<IndexItem>()
                    .And(x => x.Id != item.ID.Guid)
                    .And(x => x.Paths.Contains(ItemIDs.MediaLibraryRoot.Guid))
                    .And(x => x.Hash == item["hash"]);

                return context.GetQueryable<IndexItem>()
                    .Where(predicate)
                    .GetResults()
                    .Hits
                    .Select(x => Context.ContentDatabase.GetItem(new ID(x.Document.Id)))
                    .ToArray();
            }
        }

        private class IndexItem
        {
            [IndexField("_group")]
            public Guid Id { get; set; }

            [TypeConverter(typeof(IndexFieldEnumerableConverter))]
            [IndexField("_path")]
            public virtual IEnumerable<Guid> Paths { get; set; }

            [IndexField("hash")]
            public string Hash { get; set; }
        }
        protected override ValidatorResult GetMaxValidatorResult()
        {
            return GetFailedResult(ValidatorResult.Warning);
        }

        public override string Name => "UniqueValue";
    }
}