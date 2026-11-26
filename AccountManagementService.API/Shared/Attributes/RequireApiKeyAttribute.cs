namespace AccountManagementService.API.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireApiKeyAttribute : Attribute
    {
        public string[] AllowedClients { get; }

        public RequireApiKeyAttribute(params string[] allowedClients)
        {
            AllowedClients = allowedClients ?? Array.Empty<string>();
        }
    }
}
