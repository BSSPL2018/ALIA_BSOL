using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Section : BaseObject
    {
        #region Columns
        public long ID { get; set; }
        public long DepartmentID { get; set; }
        [Column("Section")]
        public string SectionName { get; set; }
        public long? InchargeID { get; set; }
        public long? AssistantInchargeID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan S2StartTime { get; set; }
        public TimeSpan PHStartTime { get; set; }
        public TimeSpan PHS2StartTime { get; set; }
        public TimeSpan GHStartTime { get; set; }
        public TimeSpan GHS2StartTime { get; set; }
        public TimeSpan WOStartTime { get; set; }
        public TimeSpan WOS2StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan S1EndTime { get; set; }
        public TimeSpan PHEndTime { get; set; }
        public TimeSpan PHS1EndTime { get; set; }
        public TimeSpan GHEndTime { get; set; }
        public TimeSpan GHS1EndTime { get; set; }
        public TimeSpan WOEndTime { get; set; }
        public TimeSpan WOS1EndTime { get; set; }
        public TimeSpan Break { get; set; }
        public TimeSpan HDBreak { get; set; }
        public TimeSpan PHBreak { get; set; }
        public TimeSpan PHHDBreak { get; set; }
        public TimeSpan GHBreak { get; set; }
        public TimeSpan GHHDBreak { get; set; }
        public TimeSpan WOBreak { get; set; }
        public TimeSpan WOHDBreak { get; set; }
        public int AttendanceLeniency { get; set; }

        public TimeSpan MinWorkHours { get; set; }
        public TimeSpan HDMinWorkHours { get; set; }
        public TimeSpan PHMinWorkHours { get; set; }
        public TimeSpan PHHDMinWorkHours { get; set; }
        public TimeSpan GHMinWorkHours { get; set; }
        public TimeSpan GHHDMinWorkHours { get; set; }
        public TimeSpan WOMinWorkHours { get; set; }
        public TimeSpan WOHDMinWorkHours { get; set; }
        public string WeekOff1 { get; set; }
        public string WeekOff1Policy { get; set; }
        public string WeekOff2 { get; set; }
        public string WeekOff2Policy { get; set; }
        public int RequiredPunches { get; set; }
        public int HDRequiredPunches { get; set; }
        public int PHRequiredPunches { get; set; }
        public int PHHDRequiredPunches { get; set; }
        public int GHRequiredPunches { get; set; }
        public int GHHDRequiredPunches { get; set; }
        public int WORequiredPunches { get; set; }
        public int WOHDRequiredPunches { get; set; }
        public string WorkingType { get; set; }
        public string PublicHoliday { get; set; }
        public string GovernmentHoliday { get; set; }
        public bool OverTime { get; set; }
        public int MaxStaffsOnLeave { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }


        #endregion
        #region Additional Properties 
        [NotMapped]
        public string Incharge { get; set; }
        [NotMapped]
        public string AssistantIncharge { get; set; }
        [NotMapped]
        public string Entity { get; set; }
        [NotMapped]
        public string Division { get; set; }
        [NotMapped]
        public string Department { get; set; }
        [NotMapped]
        public long EntityID { get; set; }
        [NotMapped]
        public long DivisionID { get; set; }
        [NotMapped]
        public int HDAttendanceLeniency { get; set; }
        [NotMapped]
        public int PHAttendanceLeniency { get; set; }
        [NotMapped]
        public int PHHDAttendanceLeniency { get; set; }
        [NotMapped]
        public int GHAttendanceLeniency { get; set; }
        [NotMapped]
        public int GHHDAttendanceLeniency { get; set; }
        [NotMapped]
        public int WOAttendanceLeniency { get; set; }
        [NotMapped]
        public int WOHDAttendanceLeniency { get; set; }
        #endregion

        protected override async Task Add()
        {
            _Webcontext.Sections.Add(this);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Validate()
        {
            if (await _Webcontext.Sections.AnyAsync(x => x.DepartmentID == this.DepartmentID && x.ID != this.ID && x.SectionName == this.SectionName))
                AddMessage("Same Section (" + this.SectionName + ") already exists");
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.Sections.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Sections", this.SectionName);
            await _Webcontext.SaveChangesAsync();

        }
        protected override async Task CheckDependency()
        {
            if (await _Webcontext.Designations.AnyAsync(x => x.SectionID == this.ID))
                AddMessage("Designations added.");
        }
        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogDelete("Sections", this.SectionName);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
