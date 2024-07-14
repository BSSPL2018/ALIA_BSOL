using DocumentFormat.OpenXml.Office2010.Excel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class CallLog : BaseObject
    {
        #region Columns
        public long ID { get; set; }
        public long ReferenceId { get; set; }
        public string Module { get; set; }
        public string CallType { get; set; }
        public DateTime CallDate { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        #endregion
        #region Additional Properties
        [NotMapped]
        public string QuoataionNo { get; set; }
        #endregion 

        protected override async Task Add()
        {
            _Webcontext.CallLogs.Add(this);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            LogUpdate("ACCOUNT", this.CallType, this.CallDate, this.Subject, this.Details);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.CallLogs.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("ACCOUNT", this.CallType, this.CallDate, this.Subject, this.Details);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
