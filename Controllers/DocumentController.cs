using BSOL.Core.Models.Common;
using BSOL.Core;
using BSOL.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using BSOL.Core.Entities;
using BSOL.Core.Helpers;
using Constant = BSOL.Helpers.Constant;

namespace BSOL.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly ICommonHelper _commonHelper;
        public DocumentController(BSOLWebContext Webcontext, AppUser appUser, ICommonHelper commonHelper) : base(Webcontext, appUser)
        {
            _commonHelper = commonHelper;
        }

        [HttpPost]
        public async Task<List<Document>> GetDocuments([FromForm] string reference, [FromForm] string referenceId)
        {
            string refId = "";
            //if (!string.IsNullOrEmpty(referenceId) && !long.TryParse(referenceId, out refId))
            if (!string.IsNullOrEmpty(referenceId) && referenceId.StartsWith("P"))
                refId = (IdentityDecryptor.DecryptParam(referenceId));
            else
                refId = referenceId;

            var data = await _Webcontext.Documents.Where(x => x.CompanyId == _appUser.CompanyId && x.Reference == reference && x.ReferenceId == refId).ToListAsync();
            var docRoot = Path.Combine(_commonHelper.GetDocumentRoot(), reference);
            List<Document> result = data.Select(x => new Document
            {
                Id = x.Id,
                ReferenceId = x.ReferenceId,
                Category = x.Category,
                DisplayName = x.DisplayName,
                FileName = x.FileName,
                FileSize = System.IO.File.Exists(Path.Combine(docRoot, x.FileName)) ? Utilities.FormatBytes(new FileInfo(Path.Combine(docRoot, x.FileName)).Length) : ""
            }).ToList();
            return result;
        }

        [HttpPost]
        public async Task<List<Document>> ReadDocument([FromForm] string referenceId, [FromForm] string Reference)
        {
            string refId = "";
            if (!string.IsNullOrEmpty(referenceId) && referenceId.StartsWith("P"))
                refId = IdentityDecryptor.DecryptParam(referenceId);
            else
                refId = referenceId;

            var data = await _Webcontext.Documents.Where(x => x.CompanyId == _appUser.CompanyId && x.ReferenceId == refId && x.Reference == Reference).ToListAsync();
            List<Document> result = data.Select(x => new Document
            {
                Id = x.Id,
                ReferenceId = x.ReferenceId,
                Category = x.Category,
                DisplayName = x.DisplayName,
                FileName = x.FileName,
                Reference = x.Reference
            }).ToList();
            return result;
        }

        [HttpPost]
        public async Task<List<string>> ReadCategories()
        {
            List<string> result = await _Webcontext.Documents.Where(x => x.CompanyId == _appUser.CompanyId && !string.IsNullOrEmpty(x.Category)).Select(x => x.Category).Distinct().ToListAsync();
            return result;
        }

        [HttpGet]
        public async Task<ActionResult> Download([FromQuery] Guid id, [FromQuery] bool download = true)
        {
            var data = await _Webcontext.Documents.FindAsync(id);
            if (data == null)
                return Content("Invalid Request", MediaTypeNames.Text.Plain);

            string filePath = "";
            //if (data.Reference.StartsWith("Accounts-"))
            //    filePath = Path.Combine(_commonHelper.GetDocumentRootDB(), "ACCOUNTS", data.FileName);
            //else
            filePath = Path.Combine(_commonHelper.GetDocumentRoot(), data.Reference, data.FileName);

            if (!System.IO.File.Exists(filePath))
                return Content("File not found", MediaTypeNames.Text.Plain);

            if (download)
                return PhysicalFile(filePath, HtmlExtensions.GetContentType(filePath), data.DisplayName + Path.GetExtension(data.FileName));
            else
                return PhysicalFile(filePath, HtmlExtensions.GetContentType(filePath));
        }

        [HttpPost]
        public async Task<ActionResult> Preview([FromForm] Guid id)
        {
            return await Download(id, false);
        }


        [HttpPost]
        public async Task<ReturnMessage> Upload([FromForm] Document data, IFormFile file, [FromForm] string referenceId)
        {
            string refId = "";
            if (!string.IsNullOrEmpty(referenceId) && referenceId.StartsWith("P"))
                refId = (IdentityDecryptor.DecryptParam(referenceId));
            else
                refId = referenceId;

            data.ReferenceId = refId;
            data.CompanyId = _appUser.CompanyId;
            data.Category = data.Category == "null" ? null : data.Category;
            string dirPath = Path.Combine(_commonHelper.GetDocumentRoot(), data.Reference);
            data.FileName = Utilities.GetRandomFileName(dirPath, Path.GetExtension(file.FileName));
            if (!await data.SaveAsync())
                return SaveError(data.ErrorMessage);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (var fileStream = new FileStream(Path.Combine(dirPath, data.FileName), FileMode.Create))
                await file.CopyToAsync(fileStream);

            return Message("Document saved.");
        }

        [HttpPost]
        public async Task<ReturnMessage> Delete([FromForm] Document data)
        {
            if (data.Reference == "Accounts-REMITTANCE")
            {

            }
            data.DocumentRoot = _commonHelper.GetDocumentRoot();
            if (!await data.RemoveAsync())
                return DeleteError(data.ErrorMessage);

            return Message("Document deleted.");
        }
    }
}
