using BSOL.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace BSOL.Pages.HirePurchase
{
    [IgnoreAntiforgeryToken(Order = 1001)]//TODO: Implement AFT -> https://stackoverflow.com/questions/48373229/400-bad-request-when-post-ing-to-razor-page
    public class MonthUnitStatusGridModel : PageModel
    {
        private readonly BSOLContext _context;
        public MonthUnitStatusGridModel(BSOLContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }

        public async Task OnPostAsync(DateTime? FromMonth = null, DateTime? ToMonth = null, long CustomerId = 0, string Product = null, bool Proforma = false, bool AdvanceBooking = false, bool CreditEvaluation = false, bool Agreement = false)
        {
            DataTable dataTable;
            dataTable = await _context.ExecuteDataTableAsync("spPOS_HPStatus", new { FromMonth, ToMonth, CustomerId, Product, Proforma, AdvanceBooking, CreditEvaluation, Agreement });

            int i = 1;
            foreach (DataColumn col in dataTable.Columns)
            {
                var colName = col.ColumnName;
                col.Caption = colName;
            }

            ViewData["GRID_DATA"] = dataTable;
        }

    }
}
