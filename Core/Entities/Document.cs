using BSOL.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSOL.Core.Entities
{
    public class Document:BaseObject
    {
        #region Columns

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public long CompanyId { get; set; }
        public string Reference { get; set; }
        [BindNever]
        public string ReferenceId { get; set; }
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public string EntryBy { get; set; }
        [BindNever]
        public DateTime? EntryDate { get; set; }

        #endregion

        #region Additional Properties
        [NotMapped]
        public int UploadControlId { get; set; }
        [NotMapped]
        public string DocumentRoot { get; set; }
        [NotMapped]
        public string FileSize { get; set; }

        #endregion

        protected override async Task OnSave()
        {
            await Add();
        }

        protected override async Task Add()
        {
            this.EntryBy = GetAppUserName();
            this.EntryDate = DateTime.Now;
            _Webcontext.Documents.Add(this);
            await _Webcontext.SaveChangesAsync();
        }

        protected override Task Update()
        {
            throw new NotImplementedException();
        }

        protected override async Task Delete()
        {
            var existing = await _Webcontext.Documents.FindAsync(Id);

            string filePath = Path.Combine(this.DocumentRoot, existing.Reference, existing.FileName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            _Webcontext.Documents.Remove(existing);
            //LogDelete("DOCUMENTS", existing.Reference, existing.DisplayName);
            await _Webcontext.SaveChangesAsync();
        }

        public async Task<bool> DeleteByReference(DocumentReference reference, string referencId)
        {
            return await ModifyAsync(async () =>
            {
                var companyId = GetCompanyId();
                var documents = await _Webcontext.Documents.Where(x => x.CompanyId == companyId && x.Reference == reference.ToString() && x.ReferenceId == referencId).ToListAsync();
                if (!documents.Any())
                    return true;

                foreach (var doc in documents)
                {
                    string filePath = Path.Combine(this.DocumentRoot, doc.Reference, doc.FileName);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }

                _Webcontext.Documents.RemoveRange(documents);

                await _Webcontext.SaveChangesAsync();
                return true;
            });
        }

        internal Task DeleteByReference(DocumentReference quotations, long id)
        {
            throw new NotImplementedException();
        }
    }
}
