namespace AccountManagementService.Business
{
    public class Customer
    {
        public int CustId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public interface IAcc
    {
        string GetData(string s1, int i1);
    }

    public class Acc : IAcc
    {
        public string GetData(string Message, int CustId)
        {
            return "Hello from Acc class!";
        }

        public Customer GetCustomer(int CustId)
        {
            return new Customer
            {
                CustId = 1,
                Name = "John Doe",
                Age = 30
            };
        }
    }
}
