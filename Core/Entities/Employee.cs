using System.Threading.Tasks;

namespace BSOL.Core.Entities
{
    public class Employee : BaseObject
    {

        #region Column
        public long ID { get; set; }
        public long CompanyID { get; set; }
        public long EntityID { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public bool Active { get; set; }
        public string EmailID { get; set; }
        public string PersonalEmailID { get; set; }
        public string ImagePath { get; set; }

        #endregion
        protected override async Task Add()
        {
            _Webcontext.Employees.Add(this);
            //LogAdd("Item");
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Update()
        {
            var existing = await _Webcontext.Employees.FindAsync(ID);
            _Webcontext.Entry(existing).CurrentValues.SetValues(this);
            LogUpdate("Shops", this.ID, this.FullName);
            await _Webcontext.SaveChangesAsync();
        }

        protected override async Task Delete()
        {
            _Webcontext.Remove(this);
            await _Webcontext.SaveChangesAsync();
        }
    }
}
