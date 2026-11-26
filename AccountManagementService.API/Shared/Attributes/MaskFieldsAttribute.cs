namespace AccountManagementService.API.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MaskFieldsAttribute : Attribute
    {
        public string[] Fields { get; }

        public MaskFieldsAttribute(params string[] fields)
        {
            Fields = fields ?? Array.Empty<string>();
        }
    }
}
