namespace APIConsume.Models
{
    public class StateModel
    {
        public int? StateID { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public int CountryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
