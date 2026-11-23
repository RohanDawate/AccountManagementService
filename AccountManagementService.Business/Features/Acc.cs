namespace AccountManagementService.Business
{
    public interface IAcc
    {
        string GetData(string s1, int i1);
    }

    public class Acc : IAcc
    {
        public string GetData(string s1, int i1)
        {
            return "Hello from Acc class!";
        }
    }
}
