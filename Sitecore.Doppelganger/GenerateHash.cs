using System;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;

namespace Sitecore.Doppelganger
{
    public class GenerateHash
    {
        protected void OnItemSaved(object sender, EventArgs args)
        {
            if (args == null)
                return;

            var item = Event.ExtractParameter(args, 0) as Item;
            Assert.IsNotNull(item, "No item in parameters");

            if (item.Paths.IsMediaItem)
            {
                using (new EventDisabler())
                {
                    item.Editing.BeginEdit();
                    try
                    {
                        if (item.Fields["hash"] != null)
                        {
                            item["hash"] = HashGenerator.GetHash(item);
                        }
                        item.Editing.EndEdit();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to update the media hash for item with ID {item.ID}", ex);
                        item.Editing.CancelEdit();
                    }
                }
            }
        }
    }
}