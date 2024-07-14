using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using ExcelDataReader;
using OfficeOpenXml;

namespace BSOL.Helpers
{
    public class ExcelHelper
    {
        public DataTable ReadDataFromExcel(Stream stream)
        {
            DataSet dsExcelData;
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                var data = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                dsExcelData = data;
            }
            if (dsExcelData.Tables.Count == 0)
                return null;
            return dsExcelData.Tables[0];
        }

        public byte[] Export(DataTable dtData, string SheetName, params string[] Header)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(SheetName);
                var colCount = dtData.Columns.Count;
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#339CFF");
                Color colFromBg = System.Drawing.ColorTranslator.FromHtml("#DAF7A6");
                var endAlphabet = colCount > 26 ? 'Z' : Convert.ToChar(64 + colCount);
                if (Header.Any())
                {
                    for (int i = 0; i < Header.Length; i++)
                    {
                        var titleRow = ws.Cells["A" + (i + 1).ToString()];
                        titleRow.Value = Header[i];
                        titleRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        titleRow.Style.Font.Bold = true;
                        titleRow.Style.Fill.BackgroundColor.SetColor(colFromHex);
                        ws.SelectedRange[string.Format("A{0}:{1}{0}", i + 1, endAlphabet)].Merge = true;
                        ws.SelectedRange[string.Format("A{0}:{1}{0}", i + 1, endAlphabet)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }
                    var headerRow = ws.Cells["A" + (Header.Count() + 1).ToString()];
                    headerRow.Style.Font.Bold = true;
                    ws.Cells[string.Format("A{0}:{1}{2}", (Header.Count() + 1), endAlphabet, (Header.Count() + 1))].Style.Font.Bold = true;
                    headerRow.LoadFromDataTable(dtData, true);
                }
                else
                {
                    ws.Cells["A1"].LoadFromDataTable(dtData, true);
                }

                ws.SelectedRange["A:" + endAlphabet.ToString()].AutoFitColumns(10, 60);
                ws.Cells[string.Format("A1:{0}1", endAlphabet)].Style.Font.Bold = true;

                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }
}
