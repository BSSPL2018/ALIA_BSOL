using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class GSTSetting : BaseObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public long GSTInAccountId { get; set; }
        public long GSTOutAccountId { get; set; }
        public bool IsDefault { get; set; }

        #region Additional Properties
        [NotMapped]
        public string GSTInAccount { get; set; }
        [NotMapped]
        public string GSTOutAccount { get; set; }

        #endregion

        protected override async Task Validate()
        {
            if (await _Webcontext.GSTSettings.AnyAsync(x => x.Name == this.Name && x.Id != this.Id))
                AddMessage("Same GST Name (" + this.Name + ") already exists.");

            if (await _Webcontext.GSTSettings.AnyAsync(x => x.IsDefault == true && x.Id != this.Id))
                AddMessage("Default GST settings already exists for (" + this.Name + ") .");
        }

        protected override async Task Add()
        {
            _Webcontext.GSTSettings.Add(this);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.GSTSettings.FindAsync(Id);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("GST SETTINGS", this.Name);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task CheckDependency()
        {
            
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("GST SETTINGS", this.Name);
            await _Webcontext.SaveChangesAsync();
        }

    }
}
