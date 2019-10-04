using Five_Seconds.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Five_Seconds.Helpers
{
    public static class SearchTag
    {

        // Tag
        public static List<TagItem> StaticTagItems { get; set; } = new List<TagItem>();

        public static void RemoveTag(TagItem tagItem)
        {
            if (tagItem == null)
                return;

            StaticTagItems.Remove(tagItem);
        }

        public static TagItem ValidateAndReturn(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return null;

            if (StaticTagItems.Any(v => v.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                return null;

            var _tagItem = new TagItem()
            {
                Name = tag.ToLower()
            };

            StaticTagItems.Add(_tagItem);

            return _tagItem;
        }

        public class TagItem
        {
            public string Name { get; set; }
        }
    }
}
