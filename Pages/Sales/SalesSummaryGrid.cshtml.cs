using BSOL.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace BSOL.Pages.Sales
{
    [IgnoreAntiforgeryToken(Order = 1001)]//TODO: Implement AFT -> https://stackoverflow.com/questions/48373229/400-bad-request-when-post-ing-to-razor-page
    public class SalesSummaryGridModel : PageModel
    {
        private readonly BSOLWebContext _context;
        public SalesSummaryGridModel(BSOLWebContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }

        public async Task OnPostAsync(DateTime? fromDate = null)
        {
            DataTable dataTable;
            dataTable = await _context.ExecuteDataTableAsync("spPOS_ItemSummary", new { FDate = fromDate });

            int i = 1;
            foreach (DataColumn col in dataTable.Columns)
            {
                var colName = col.ColumnName;
                col.ColumnName = $"Column{i++}";
                col.Caption = colName;
            }

            ViewData["GRID_DATA"] = dataTable;
        }
    }
}
