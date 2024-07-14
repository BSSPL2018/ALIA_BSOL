namespace BSOL.Helpers
{
    public class EditorData
    {
        public string Url { get; set; }
        public string TextField { get; set; }
        public string ValueField { get; set; }
        public string DisplayMember { get; set; }
        public string AdditionalData { get; set; }
        public string DataEvent { get; set; }
        public string OnChange { get; set; }

        public EditorData()
        {

        }

        public EditorData(string Url, string DisplayMember, string TextField = "Text", string ValueField = "Id", string OnChange = "")
        {
            this.Url = Url;
            this.DisplayMember = DisplayMember;
            this.TextField = TextField;
            this.ValueField = ValueField;
            this.OnChange = OnChange;
        }
    }
}