namespace AccountManagementService.API.Shared.Attributes
{
    public enum ResponseStyle
    {
        Uniform,
        Standard
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ResponseStyleAttribute : Attribute
    {
        public ResponseStyle Style { get; }

        public ResponseStyleAttribute(ResponseStyle style)
        {
            Style = style;
        }
    }
}
