using DocumentFormat.OpenXml.Features;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public static class HtmlExtensions
    {
        #region Grid

        public static GridBuilder<T> EnableFilter<T>(this GridBuilder<T> Grid) where T : class
        {
            return Grid.Filterable(x => x.Extra(false).Operators(o => o.ForString(s =>
            {
                s.Clear(); s.Contains("Contains").StartsWith("StartsWith").EndsWith("EndsWith").IsEqualTo("IsEqualTo").IsNotEqualTo("IsNotEqualTo").DoesNotContain("DoesNotContain");
            })));
        }

        public static GridBuilder<T> EnableEdit<T>(this GridBuilder<T> Grid, IHtmlHelper Helper, bool FitToHeight = true) where T : class
        {
            if (Helper.HasAccess(Rights.Edit))
                return Grid.HtmlAttributes(new { @class = "grid-edit" + (FitToHeight ? " h-grid v-grid" : "") });

            if (FitToHeight)
                return Grid.HtmlAttributes(new { @class = "h-grid v-grid" });
            else
                return Grid;
        }

        [Obsolete("Use EditButtonColumn/EditUrlColumn")]
        public static GridBoundColumnBuilder<TModel> EditColumn<TModel>(this GridColumnFactory<TModel> Column, string URL, string OnClick = "app.editItem('#:uid#')") where TModel : class
        {
            return Column.Bound("").ClientTemplate(string.Format("<a class='btn-grid btn-grid-edit fa fa-edit' target='_self' href=\"{0}\"></a>", URL.IsValid() ? URL : "javascript:" + OnClick)).Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(41).Sortable(false).Filterable(false).Visible(Column.Container.ViewContext.HasAccess(Rights.Edit));
        }
        public static GridBoundColumnBuilder<TModel> EditButtonColumn<TModel>(this GridColumnFactory<TModel> Column, string OnClick = "app.editItem('#:uid#')") where TModel : class
        {
            return Column.Bound("").ClientTemplate(string.Format("<a class='btn-grid btn-grid-edit fa fa-edit' target='_self' href=\"{0}\"></a>", "javascript:" + OnClick)).Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(41).Sortable(false).Filterable(false).Visible(Column.Container.ViewContext.HasAccess(Rights.Edit));
        }
        public static GridBoundColumnBuilder<TModel> EditUrlColumn<TModel>(this GridColumnFactory<TModel> Column, string URL) where TModel : class
        {
            return Column.Bound("").ClientTemplate(string.Format("<a class='btn-grid btn-grid-edit fa fa-edit' title='Edit' target='_self' href=\"{0}\"></a>", URL)).Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(41).Sortable(false).Filterable(false).Visible(Column.Container.ViewContext.HasAccess(Rights.Edit));
        }
        public static GridBoundColumnBuilder<TModel> ViewColumn<TModel>(this GridColumnFactory<TModel> Column, string URL) where TModel : class
        {
            return Column.Bound("").ClientTemplate(string.Format("<a class='btn-grid btn-grid-view fa fa-eye' target='_self' href='{0}'></a>", URL)).Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(40).Sortable(false).Filterable(false);
        }

        public static GridBoundColumnBuilder<TModel> DeleteColumn<TModel>(this GridColumnFactory<TModel> column, string onClick = "app.deleteItem('#:uid#')") where TModel : class
        {
            return column.Bound("").ClientTemplate(string.Format("<a class='btn-grid btn-grid-delete fa fa-trash-o' title='Delete' href=\"javascript:{0}\"></a>", onClick)).Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(45).Sortable(false).Filterable(false).Visible(column.Container.ViewContext.HasAccess(Rights.Delete));
        }

        public static GridBoundColumnBuilder<TModel> PrintColumn<TModel>(this GridColumnFactory<TModel> Column, string OnClick = "app.printItemapp.printItem('#:uid#')") where TModel : class
        {
            return Column.Bound("").ClientTemplate(string.Format("<a class='btn-grid btn-grid-print fa fa-print' title='Print' target='_self' href=\"{0}\"></a>", "javascript:" + OnClick)).Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(41).Sortable(false).Filterable(false);
        }

        public static GridBoundColumnBuilder<TModel> DeleteColumn<TModel, TValue>(this GridColumnFactory<TModel> column, Expression<Func<TModel, TValue>> expression, string onClick = "app.deleteItem('#:uid#')", bool childEdit = false) where TModel : class
        {
            var columnBuilder = column.Bound(expression);// column.Container.Editable.Enabled ? column.Bound(expression) : column.Bound("");
            return columnBuilder.ClientTemplate(string.Format("<a class='btn-grid btn-grid-delete fa fa-trash-o' title='Delete' href=\"javascript:{0}\"></a>", onClick)).Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(45).Sortable(false).Filterable(false).Visible(column.Container.ViewContext.HasAccess(childEdit ? Rights.Modify : Rights.Delete));
        }

        public static GridBoundColumnBuilder<TModel> ApproveColumn<TModel, TValue>(this GridColumnFactory<TModel> column, Expression<Func<TModel, TValue>> expression, string funcApprove = "app.approveItem('#:uid#')", string funcReject = "app.rejectItem('#:uid#')") where TModel : class
        {
            return column.Bound("").ClientTemplate(string.Format("#if(!{0}){{#<a class='btn-grid btn-approve fa fa-check' title='Approve' href=\"javascript:{1}\"></a>#}}"
                                                                + "else{{# <a class='btn-grid btn-reject fa fa-times' title='Un-Approve' href=\"javascript:{2}\"></a> #}}#",
                                                                (expression.Body as MemberExpression).Member.Name, funcApprove, funcReject))
                .Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(45).Sortable(false).Filterable(false).Visible(column.Container.ViewContext.HasAccess(Rights.Approve));
        }

        public static GridBoundColumnBuilder<TModel> VerifyColumn<TModel, TValue>(this GridColumnFactory<TModel> column, Expression<Func<TModel, TValue>> expression, string funcApprove = "app.verifyItem('#:uid#')", string funcReject = "app.revertItem('#:uid#')") where TModel : class
        {
            //var columnBuilder = column.Container.Editable.Enabled ? column.Bound(expression) : column.Bound("");
            var columnBuilder = column.Bound(expression);
            return columnBuilder.ClientTemplate(string.Format("#if(!{0}){{#<a class='btn-grid btn-verify fa fa-check' title='Verify' href=\"javascript:{1}\"></a>#}}"
                                                                + "else{{# <a class='btn-grid btn-revert fa fa-times' title='Revert' href=\"javascript:{2}\"></a> #}}#",
                                                                (expression.Body as MemberExpression).Member.Name, funcApprove, funcReject))
                .Title('\0'.ToString()).HtmlAttributes(new { style = "text-align:center;" }).Width(45).Sortable(false).Filterable(false).Visible(column.Container.ViewContext.HasAccess(Rights.Approve));
        }

        public static GridBuilder<T> Editable<T>(this GridBuilder<T> gridBuilder, IHtmlHelper htmlHelper, string Title = "", bool AllowAdd = true, bool AllowSave = true, GridInsertRowPosition NewRowPosition = GridInsertRowPosition.Top) where T : class
        {
            if (!htmlHelper.HasAccess(Rights.Modify))
                return gridBuilder;

            return gridBuilder.Editable(editable => editable.Mode(GridEditMode.InCell).CreateAt(NewRowPosition)).ToolBar(toolbar =>
            {
                if (AllowAdd)
                    toolbar.Create().Text('\0'.ToString()).HtmlAttributes(new { style = htmlHelper.HasAccess(Rights.Add) ? "" : "display:none;" });
                if (AllowSave)
                    toolbar.Save().SaveText("Save").HtmlAttributes(new { style = htmlHelper.HasAccess(Rights.Modify) ? "" : "display:none;" });
                if (Title.IsValid())
                    toolbar.Custom().Text(Title).HtmlAttributes(new { @class = "k-tool-text" });
            });
        }

        public static GridBoundColumnBuilder<TModel> DateColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).Format("{0:" + Format.Date.Description() + "}");
        }

        public static GridBoundColumnBuilder<TModel> DateTimeColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).Format("{0:" + Format.DateTime.Description() + "}").Width(150);
        }

        public static GridBoundColumnBuilder<TModel> MonthColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).Format("{0:MMM-yyyy}");
        }

        public static GridBoundColumnBuilder<TModel> TimeColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).ClientTemplate(string.Format("#: TimeSpanToTime({0}) #", (Expression.Body as MemberExpression).Member.Name)).Filterable(false).Sortable(false);
        }

        public static GridBoundColumnBuilder<TModel> MoneyColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression, int digits = 2) where TModel : class
        {
            return Column.Bound(Expression).Format("{0:n" + digits + "}").HtmlAttributes(new { style = "text-align:right;" }).HeaderHtmlAttributes(new { style = "text-align:right;" });
        }

        public static GridBoundColumnBuilder<TModel> EmailColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).ClientTemplate(string.Format("#if({0}){{#<a href='mailto:#={0}#'>#={0}#</a>#}}#", (Expression.Body as MemberExpression).Member.Name));
        }

        public static GridBoundColumnBuilder<TModel> TelColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).ClientTemplate(string.Format("#if({0}){{#<a href='tel:#={0}#'>#={0}#</a>#}}#", (Expression.Body as MemberExpression).Member.Name));
        }

        public static GridBoundColumnBuilder<TModel> CheckedColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).ClientTemplate(string.Format("#if({0}){{# <i class='fa fa-check'></i> #}}#", (Expression.Body as MemberExpression).Member.Name)).HtmlAttributes(new { style = "text-align:center;" }).HeaderHtmlAttributes(new { style = "text-align:center;" });
        }

        public static GridBoundColumnBuilder<TModel> NumberColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).HtmlAttributes(new { style = "text-align:center;" }).HeaderHtmlAttributes(new { style = "text-align:center;" });
        }

        public static GridBoundColumnBuilder<TModel> DescriptionColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression) where TModel : class
        {
            return Column.Bound(Expression).HtmlAttributes(new { @class = "grid-desc" });
        }

        public static GridBoundColumnBuilder<TModel> Center<TModel>(this GridBoundColumnBuilder<TModel> Column) where TModel : class
        {
            return Column.HtmlAttributes(new { style = "text-align:center;" }).HeaderHtmlAttributes(new { style = "text-align:center;" });
        }

        public static GridBoundColumnBuilder<TModel> ClearTitle<TModel>(this GridBoundColumnBuilder<TModel> Column) where TModel : class
        {
            return Column.Title('\0'.ToString());
        }

        public static GridBoundColumnBuilder<TModel> SelectColumn<TModel, TValue>(this GridColumnFactory<TModel> Column, Expression<Func<TModel, TValue>> Expression, string id = "chkGrid", bool enableSelectAll = true, string onSelectAll = "onSelectAll()") where TModel : class
        {
            var col = Column.Bound(Expression)
                         .ClientTemplate(string.Format("<input class='k-checkbox chkcol' id='{0}_#:{1}#' value='#:{1}#' type='checkbox'><label class='k-checkbox-label' for='{0}_#:{1}#''></label>", id, (Expression.Body as MemberExpression).Member.Name))
                         .HtmlAttributes(new { style = "text-align:center;" })
                         .Width(40).Filterable(false).Sortable(false);

            if (enableSelectAll)
                col.ClientHeaderTemplate(string.Format("<input class='k-checkbox chkhdr' id='{0}' onchange='{1}' type='checkbox'><label class='k-checkbox-label' for='{0}'></label>", id, onSelectAll))
                   .HeaderHtmlAttributes(new { style = "text-align:center;" });

            return col;
        }
        #endregion

        #region Button
        //public static HtmlString AddUrlButton(this IHtmlHelper helper, string url)
        //{
        //    return new HtmlString(helper.HasAccess(Rights.Add) ? string.Format("<a class='i-button b-icon' href='{0}'><i class='fa fa-plus'></i></a>", url) : "");
        //}
        public static HtmlString AddUrlButton(this IHtmlHelper helper, string url)
        {
            return new HtmlString(helper.HasAccess(Rights.Add) ? string.Format("<a class='i-button b-icon btn-violet' href='{0}'><i class='fa fa-plus-circle'></i>Add New</a>", url) : "");
        }
        public static HtmlString AddButton(this IHtmlHelper helper, string onClick = "app.addItem()")
        {
            return new HtmlString(helper.HasAccess(Rights.Add) ? string.Format("<a class='i-button b-icon' href='javascript:{0}'><i class='fa fa-plus'></i></a>", onClick) : "");
        }
        public static HtmlString SaveButton(this IHtmlHelper helper, string onClick = null, string title = "Save", string text = "Save", string type = "submit")
        {
            return new HtmlString(helper.HasAccess(Rights.Modify) ? string.Format("<button type='{3}' class='btn btn-save btn-cntrls' OnClick='{0}' title='{1}'><i class='fa fa-check-circle'></i>{2}</button>", onClick, title, text, type) : "");
        }
        public static HtmlString CancelButton(this IHtmlHelper helper, string onClick = null)
        {
            return new HtmlString(string.Format("<a class='rnd-btn btn-close' href='javascript:{0}()'><i class='fa fa-times'></i></a>", onClick));
        }
        public static HtmlString BackButton(this IUrlHelper helper, string pageName)
        {
            return new HtmlString(string.Format("<a class='btn btn-back' href='{0}'><i class='fa fa-angle-double-left'></i>Back</a>", helper.Page(pageName)));
        }
        public static HtmlString DeleteButton(this IHtmlHelper helper, string onClick = "app.deleteItem()")
        {
            return new HtmlString(helper.HasAccess(Rights.Delete) ? string.Format("<button class='btn btn-delete btn-cntrls' type='button' onclick='{0}'><i class='fa fa-times'></i>Delete</button>", onClick) : "");
        }
        public static HtmlString ExcelButton(this IHtmlHelper helper, string onClick = "ExportToExcel()")
        {
            return new HtmlString(helper.HasAccess(Rights.View) ? string.Format("<button type='button' class='export-icon excel' onclick='{0}' title='Export to Excel'><i class='fa fa-file-excel-o'></i></button>", onClick) : "");
        }
        public static HtmlString ExcelButtonWithTooltip(this IHtmlHelper helper, string flow = "up", string onClick = "ExportToExcel()", string toolTipText = "")
        {
            return new HtmlString(helper.HasAccess(Rights.View) ? string.Format("<span onClick='{1}' class='export-icon excel' tooltip=\"{2}\" flow='{0}'><i class='fa fa-file-excel-o'></i></span>", flow, onClick, toolTipText) : "");
        }
        public static HtmlString PdfButtonWithTooltip(this IHtmlHelper helper, string flow = "up", string onClick = "ExportToExcel()", string toolTipText = "")
        {
            return new HtmlString(helper.HasAccess(Rights.View) ? string.Format("<span onClick='{1}' class='export-icon pdf' tooltip=\"{2}\" flow='{0}'><i class='fa fa-file-pdf-o'></i></span>", flow, onClick, toolTipText) : "");
        }

        public static HtmlString ImportButton(this IHtmlHelper helper, string onClick = "ImportFromExcel()")
        {
            return new HtmlString(helper.HasAccess(Rights.Modify) ? string.Format("<button id='btnImport' type='button' class='btn btn-success btn-icon' onclick='{0}' title='Import From Excel'><i class='fa fa-download'></i> <span>Import From Excel</span></button>", onClick) : "");
        }
        public static HtmlString DownloadTemplate(this IHtmlHelper Helper, ExcelTemplate excelTemplate)
        {
            return new HtmlString(Helper.HasAccess(Rights.Modify) ? string.Format("<a href='/api/Common/DownloadTemplate?id={0}' class='btn-template fa fa-question-circle' title='Download Template'></a>", excelTemplate.Description()) : "");
        }
        public static HtmlString PrintButton(this IHtmlHelper helper, string onClick = "PrintDoc()", string text = "Print")
        {
            return new HtmlString(helper.HasAccess(Rights.View) ? string.Format("<button type='button' class='btn btn-print' onclick='{0}' title='{1}'><i class='fa fa-print'></i>{1}</button>", onClick, text) : "");
        }
        //public static HtmlString ToggleButton(this IHtmlHelper helper, Orientation direction = Orientation.Horizontal, string target = "#editor", string toggleChange = "toggle_Change")
        //{
        //    return new HtmlString(
        //        (helper.HasAccess(Rights.Add) ? string.Format("<a class='toggle-btn btn-open' href='#' data-target='{0}' data-click='{1}' data-orientation='{2}'><i class='fa fa-plus'></i></a>", target, toggleChange, direction) : "")
        //        +
        //        string.Format("<a class='toggle-btn btn-minus' href='#' data-target='{0}' data-click='{1}' data-orientation='{2}' style='display:none'><i class='fa fa-minus'></i></a>", target, toggleChange, direction)
        //        );
        //}
        public static HtmlString ToggleButton(this IHtmlHelper helper, Orientation direction = Orientation.Horizontal, string target = "#editor", string toggleChange = "toggle_Change")
        {
            return new HtmlString(
                (helper.HasAccess(Rights.Add) ? string.Format("<a class='toggle-btn btn-open btn-violet ' href='#' data-target='{0}' data-click='{1}' data-orientation='{2}'><i class='fa fa-plus-circle'></i>Add New</a>", target, toggleChange, direction) : "")
                +
                string.Format("<a class='toggle-btn btn-minus btn-violet' href='#' data-target='{0}' data-click='{1}' data-orientation='{2}' style='display:none'><i class='fa fa-minus'></i>Close</a>", target, toggleChange, direction)
                );
        }

        public static HtmlString VerifyButton(this IHtmlHelper helper, bool Verify = false, string onClick = null, string text = "Verify", string type = "submit")
        {
            return new HtmlString(helper.HasAccess(Rights.Approve) ? Verify ? string.Format("<button type='{1}' class='btn btn-verify btn-cntrls' OnClick='{0}' title='{2}'><i class='fa fa-check'></i>{2}</button>", onClick, type, text) :
                                                                              string.Format("<button type='{1}' class='btn btn-cancel btn-cntrls' OnClick='{0}' title='{2}'><i class='fa fa-times'></i>{2}</button>", onClick, type, text) : "");
        }
        public static HtmlString EditURLIcon(this IHtmlHelper helper, string URL = "")
        {
            return new HtmlString(helper.HasAccess(Rights.Edit) ? string.Format("<a class='btn-grid btn-grid-edit fa fa-edit' title='Edit' target='_self' href=\"{0}\"></a>", URL) : "");
        }
        public static HtmlString DeleteIcon(this IHtmlHelper helper, string onClick = "app.deleteItem('#:Id#','#:RefNoFormatted#')")
        {
            return new HtmlString(helper.HasAccess(Rights.Delete) ? string.Format("<a class='btn-grid btn-grid-delete fa fa-trash-o' title='Delete' href=\"javascript:{0}\"></a>", onClick) : "");
        }
        public static HtmlString PrintIcon(this IHtmlHelper helper, string onClick = "app.printItem('#:Id#')")
        {
            return new HtmlString(helper.HasAccess(Rights.View) ? string.Format("<a class='btn-grid btn-grid-print fa fa-print' title='Print' target='_self' href=\"{0}\"></a>", "javascript:" + onClick) : "");
        }
        public static HtmlString VerifyIcon(this IHtmlHelper helper, bool Verify = false, string funcApprove = "app.verifyItem('#:Id#','#:RefNoFormatted#')", string funcReject = "app.revertItem('#:Id#','#:RefNoFormatted#')")
        {
            return new HtmlString(helper.HasAccess(Rights.Approve) ? Verify ? string.Format("<a class='btn-grid btn-verify fa fa-check' title='Verify' href=\"javascript:{0}\"></a>", funcApprove) :
                                                                 string.Format("<a class='btn-grid btn-revert fa fa-times' title='Revert' href=\"javascript:{0}\"></a>", funcReject) : "");
        }
        //private static string GetUserRights(this IHtmlHelper Helper, EventType EventType)
        //{
        //    return Helper.HasAccess(EventType) ? "" : "style='display: none;'";
        //}

        #endregion

        #region DatePicker

        public static DatePickerBuilder DatePickerFor<T>(this IHtmlHelper<T> Helper, Expression<System.Func<T, System.DateTime?>> expression)
        {
            return Helper.Kendo().DatePickerFor(expression).Format(Format.Date.Description()).ParseFormats(new[] { "dd-MMM-yyyy" }).HtmlAttributes(new { type = "text" });
        }

        public static DatePickerBuilder DatePickerFor<T>(this IHtmlHelper<T> Helper, Expression<System.Func<T, System.DateTime>> expression)
        {
            return Helper.Kendo().DatePickerFor(expression).Format(Format.Date.Description()).ParseFormats(new[] { "dd-MMM-yyyy" }).HtmlAttributes(new { type = "text" });
        }

        public static DatePickerBuilder DatePicker<T>(this IHtmlHelper<T> Helper)
        {
            return Helper.Kendo().DatePicker().Format(Format.Date.Description()).ParseFormats(new[] { "dd-MMM-yyyy" }).HtmlAttributes(new { type = "text" });
        }


        #endregion

        #region DateTimePicker

        public static DateTimePickerBuilder DateTimePickerFor<T>(this IHtmlHelper<T> Helper, Expression<System.Func<T, System.DateTime?>> expression)
        {
            return Helper.Kendo().DateTimePickerFor(expression).Format(Format.DateTime.Description()).ParseFormats(new[] { "dd-MMM-yyyy hh:mm tt" }).HtmlAttributes(new { type = "text" });
        }

        public static DateTimePickerBuilder DateTimePickerFor<T>(this IHtmlHelper<T> Helper, Expression<System.Func<T, System.DateTime>> expression)
        {
            return Helper.Kendo().DateTimePickerFor(expression).Format(Format.DateTime.Description()).ParseFormats(new[] { "dd-MMM-yyyy hh:mm tt" }).HtmlAttributes(new { type = "text" });
        }

        public static DateTimePickerBuilder DateTimePickerFor<TModel>(this IHtmlHelper<TModel> Helper)
        {
            return Helper.Kendo().DateTimePicker().Format(Format.DateTime.Description()).ParseFormats(new[] { "dd-MMM-yyyy hh:mm tt" }).HtmlAttributes(new { type = "text" });
        }

        #endregion
        public static void AddModelError(this ModelStateDictionary modelState, List<string> errors)
        {
            modelState.AddModelError("ERROR", string.Join("\n", errors));
        }
        public static void AddModelError(this ModelStateDictionary modelState, string error)
        {
            modelState.AddModelError("ERROR", error);
        }
        public static string GetContentType(string filePath)
        {
            string contentType;
            if (new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType))
                return contentType;
            else
                return MediaTypeNames.Application.Octet;
        }
        public static string GetRandomFileName(string directory, string extension)
        {
            string fileName = string.Format("{0}.{1}", Guid.NewGuid().ToString(), extension.Replace(".", ""));
            if (File.Exists(Path.Combine(directory, fileName)))
                return GetRandomFileName(directory, extension);
            else
                return fileName;
        }
        public static Dictionary<string, object> GetFilters(this IList<IFilterDescriptor> gridFilters)
        {
            if (gridFilters == null)
                return new Dictionary<string, object>();

            Dictionary<string, object> lstfilter = new Dictionary<string, object>();
            Action<IEnumerable<IFilterDescriptor>> loadAllFilters = null;
            loadAllFilters = (filters) =>
            {
                foreach (var filter in filters)
                {
                    var descriptor = filter as FilterDescriptor;
                    if (descriptor != null)
                        lstfilter.Add(descriptor.Member, descriptor.Value);
                    else if (filter is CompositeFilterDescriptor)
                        loadAllFilters(((CompositeFilterDescriptor)filter).FilterDescriptors);
                }
            };
            loadAllFilters(gridFilters);
            return lstfilter;
        }
        public static void SortInterceptor(this IList<SortDescriptor> sortDescriptors, string property)
        {
            if (sortDescriptors.Any(x => x.Member == property + "Formatted"))
                sortDescriptors[0].Member = property;
        }
        #region User Rights
        public static bool HasAccess(this IHtmlHelper helper, Rights eventType)
        {
            return helper.ViewContext.HasAccess(eventType);
        }

        public static bool HasAccess(this ViewContext context, Rights eventType)
        {
            var access = context.ViewData[eventType.ToString()];
            return access == null ? true : (bool)context.ViewData[eventType.ToString()];
        }

        public static async Task<bool> HasAccess(this IHtmlHelper helper, string formName, Rights eventType = Rights.View)
        {
            var items = helper.ViewContext.ViewData;
            var response = helper.ViewContext.HttpContext.Response;

            AppUser appUser = (AppUser)helper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppUser));

            if (appUser.IsPowerUser)
            {
                items[Rights.View.ToString()] = true;
                items[Rights.Add.ToString()] = true;
                items[Rights.Edit.ToString()] = true;
                items[Rights.Modify.ToString()] = true;
                items[Rights.Delete.ToString()] = true;
                items[Rights.Approve.ToString()] = true;
                items[Rights.Full.ToString()] = true;

                return true;
            }

            var rights = await appUser.GetRights(formName);
            if (!await appUser.HasAccess(formName, eventType, rights))
            {
                response.Redirect("/Error/403");
                return false;
            }

            items[Rights.View.ToString()] = rights.View;
            items[Rights.Add.ToString()] = rights.Add;
            items[Rights.Edit.ToString()] = rights.Edit;
            items[Rights.Modify.ToString()] = rights.Add || rights.Edit;
            items[Rights.Delete.ToString()] = rights.Delete;
            items[Rights.Approve.ToString()] = rights.Approve;
            items[Rights.Full.ToString()] = rights.View && rights.Add && rights.Edit && rights.Delete && rights.Approve;

            return true;
        }
        //public static GridBoundColumnBuilder<TModel> HasAccess<TModel>(this GridBoundColumnBuilder<TModel> Column, EventType EventType) where TModel : class
        //{
        //    return Column.Visible(Column.Column.Grid.ViewContext.HasAccess(EventType));
        //}
        #endregion
        #region Mobile
        public static HtmlString MobileHeader(this IHtmlHelper helper, IEnumerable<string> columns)
        {
            return new HtmlString(string.Format("@media only screen and (max-width: 760px), (min-device-width: 768px) and (max-device-width: 1024px){{{0}}}",
                string.Join("", columns.Select((x, index) => string.Format(".r-table td:nth-of-type({0}):before {{content: \"{1}\";}}", index + 1, x)))));
        }
        #endregion
    }
}
