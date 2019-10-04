using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Five_Seconds.Helpers
{
    public class SearchTag
    {

        // Tag
        public ObservableCollection<TagItem> TagItems { get; set; } = new ObservableCollection<TagItem>();

        public void RemoveTag(TagItem tagItem)
        {
            if (tagItem == null)
                return;

            TagItems.Remove(tagItem);
        }

        public TagItem ValidateAndReturn(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return null;

            if (TagItems.Any(v => v.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                return null;

            return new TagItem()
            {
                Name = tag.ToLower()
            };
        }

        public class TagItem
        {
            public string Name { get; set; }
        }
    }
}
