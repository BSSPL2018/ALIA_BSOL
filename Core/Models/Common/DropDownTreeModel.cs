using System.Collections.Generic;
using System.Linq;

namespace BSOL.Core.Models.Common
{
    public class DropDownTreeModel : DropDownModel
    {
        public long? ParentId { get; set; }
        public IReadOnlyList<DropDownTreeModel> Items { get; set; }
        public bool HasItems
        {
            get
            {
                return Items != null && Items.Any();
            }
        }
    }
}
