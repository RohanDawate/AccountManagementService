namespace AccountManagementService.CrossCutting.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class NoLogAttribute : Attribute
    {
    }
}
