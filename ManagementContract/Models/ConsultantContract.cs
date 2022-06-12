namespace ManagementContract.Models
{
    public class ConsultantContract
    {
        public int ContractId { get; set; }

        public Contract Contract { get; set; } = new();

        public int ConsultantId { get; set; }

        public Consultant Consultant { get; set; } = new();
       
    }
}
