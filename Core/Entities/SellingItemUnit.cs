
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
	public class SellingItemUnit : BaseObject
	{
		public long Id { get; set; }
		public long ItemId { get; set; }
		public long SellingUnitId { get; set; }
		public decimal SellingUnitRate { get; set; }
		public long UPC { get; set; }
		public decimal Conversion { get; set; }
        public long? ShopId { get; set; }

        [NotMapped]
		public string SellingUnit { get; set; }

		protected override async Task Validate()
		{
			if (await _Webcontext.SellingItemUnits.AnyAsync(x => x.SellingUnitId == this.SellingUnitId && x.Id != this.Id && x.ItemId == this.ItemId))
				AddMessage("Selling Unit already exists");
		}

		protected override async Task Add()
		{
			_Webcontext.SellingItemUnits.Add(this);
			await _Webcontext.SaveChangesAsync();
		}

		protected override async Task Update()
		{
			var existing = await _Webcontext.SellingItemUnits.FindAsync(Id);
			_Webcontext.Entry(existing).CurrentValues.SetValues(this);
			await _Webcontext.SaveChangesAsync();
		}

		protected override async Task CheckDependency()
		{

		}

		protected override async Task Delete()
		{
			var existing = await _Webcontext.SellingItemUnits.FindAsync(Id);
			_Webcontext.SellingItemUnits.Remove(existing);
			await _Webcontext.SaveChangesAsync();

			Log("ITEMS", ActionType.Delete, this.SellingUnit);
			await _Webcontext.SaveChangesAsync();
		}
	}
}
