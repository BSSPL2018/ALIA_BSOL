namespace BSOL.Core.Entities
{
    public class EventLog 
    {
        public long ID { get; set; }
        public long? CompanyID { get; set; }
        public string Module { get; set; }
        public string ActionType { get; set; }
        public string Reference { get; set; }
        public string Changes { get; set; }
        public string ActionBy { get; set; }
        public DateTime? ActionDate { get; set; }
        public string ObjectType { get; set; }
        public long? ObjectID { get; set; }
        public string Machine { get; set; }
        public long? UserID { get; set; }

        
    }
}
