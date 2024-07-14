using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Division : BaseObject
    {
        #region Columns

        public long ID { get; set; }
        public long EntityID { get; set; }
        [Column("Division")]
        public string DivisionName { get; set; }
        public long? InchargeID { get; set; }
        public long? AssistantInchargeID { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public string Entity { get; set; }
        [NotMapped]
        public string Incharge { get; set; }
        [NotMapped]
        public string AssistantIncharge { get; set; }

        #endregion

        protected override async Task Validate()
        {
            if (await (from d in _Webcontext.Divisions
                       where d.EntityID == this.EntityID && d.DivisionName == this.DivisionName && d.ID != this.ID
                       select d.ID).AnyAsync())
                AddMessage("Same Division already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.Divisions.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Divisions.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Division", this.DivisionName);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Departments.AnyAsync(x => x.DivisionID == this.ID))
                AddMessage("Departments added.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Division", this.DivisionName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
