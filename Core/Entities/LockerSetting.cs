using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace BSOL.Core.Entities
{
    public class LockerSetting : BaseObject
    {
        #region columns
        public long ID { get; set; }
        public string LockerName { get; set; }
        public int RackNo { get; set; }
        public int NoofBin { get; set; }
        public int NoofLevel { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public bool Active { get; set; }

        protected override async Task Validate()
        {
            if (await _Webcontext.LockerSettings.AnyAsync(x => x.LockerName == this.LockerName && x.ID != this.ID))
                AddMessage("Same Locker Name already exists");
        }
        protected override async Task Add()
        {
            _Webcontext.LockerSettings.Add(this);
            LogUpdate("Locker Setting", this.LockerName, this.RackNo, this.NoofBin, this.NoofLevel);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Update()
        {
            var existing = await _Webcontext.LockerSettings.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Locker Setting", this.LockerName, this.RackNo, this.NoofBin, this.NoofLevel);
            await _Webcontext.SaveChangesAsync();
        }
        protected override async Task Delete()
        {
            var existing = await _Webcontext.LockerSettings.FindAsync(ID);
            existing.Active = false;
            LogDelete("Locker Setting", this.LockerName, this.RackNo, this.NoofBin, this.NoofLevel);
            await _Webcontext.SaveChangesAsync();
        }


        #endregion
    }
}
