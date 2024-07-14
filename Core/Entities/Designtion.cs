using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Designtion : BaseObject
    {
        #region Columns
        public long ID { get; set; }
        public long SectionID { get; set; }
        public string Designation { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        #endregion
        #region Additional Properties
        [NotMapped]
        public string Entity { get; set; }
        [NotMapped]
        public string Division { get; set; }
        [NotMapped]
        public string Department { get; set; }
        [NotMapped]
        public string Section { get; set; }
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public long EntityID { get; set; }
        [NotMapped]
        public long DivisionID { get; set; }
        [NotMapped]
        public long DepartmentID { get; set; }
        #endregion

        protected override async Task Add()
        {
            _Webcontext.Designations.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.Designations.AnyAsync(x => x.SectionID == this.SectionID && x.ID != this.ID && x.Designation == this.Designation))
                AddMessage("Same Designation (" + this.Designation + ") already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Designations.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Designations", this.Designation);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Employments.AnyAsync(x => x.DesignationID == this.ID))
                AddMessage("Employment Details added.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Designations", this.Designation);
            await _Webcontext.SaveChangesAsync();
        }

    }
}
