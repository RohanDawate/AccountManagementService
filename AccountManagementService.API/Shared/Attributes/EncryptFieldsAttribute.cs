namespace AccountManagementService.API.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class EncryptFieldsAttribute : Attribute
    {
        public string[] Fields { get; }

        public EncryptFieldsAttribute(params string[] fields)
        {
            Fields = fields ?? Array.Empty<string>();
        }
    }
}
