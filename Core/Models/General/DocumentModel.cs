namespace BSOL.Core.Models.General
{
    public class DocumentModel
    {
        public string Reference { get; set; }
        public string ReferenceID { get; set; }
        public string[] Items { get; set; }

        public DocumentModel()
        {
            Items = new string[0];
        }
    }
}
