using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Department : BaseObject
    {
        #region Columns

        public long ID { get; set; }
        public long DivisionID { get; set; }
        [Column("Department")]
        public string DepartmentName { get; set; }
        public long? InchargeID { get; set; }
        public long? AssistantInchargeID { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public string Entity { get; set; }
        [NotMapped]
        public string Division { get; set; }
        [NotMapped]
        public long EntityID { get; set; }
        [NotMapped]
        public string Incharge { get; set; }
        [NotMapped]
        public string AssistantIncharge { get; set; }

        #endregion

        protected override async Task Validate()
        {
            if (await (from d in _Webcontext.Departments
                       where d.DivisionID == this.DivisionID && d.DepartmentName == this.DepartmentName && d.ID != this.ID
                       select d.ID).AnyAsync())
                AddMessage("Same Department already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.Departments.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Departments.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Department", this.DepartmentName);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Sections.AnyAsync(x => x.DepartmentID == this.ID))
                AddMessage("Sections added.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Departments", this.DepartmentName);
            await _Webcontext.SaveChangesAsync();
        }

    }
}
