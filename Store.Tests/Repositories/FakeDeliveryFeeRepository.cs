using Store.Domain.Repositories;

namespace Store.Tests.Repositories
{
    public class FakeDeliveryFeeRepository : IDeliveryFeeRepository
    {
        public decimal Get(string zipCode)
        {
            if(zipCode == "12345678")
                return 10;
            return 0;
        }
    }
}
