namespace Spring_Hero_Bank_on_CSharp.entity
{
    public class BlockchainAddress
    {
        public BlockchainAddress()
        {
        }
        
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public double Balance { get; set; }
        public long CreateAtMLS { get; set; }
        public long UpdateAtMLS { get; set; }
    }
}