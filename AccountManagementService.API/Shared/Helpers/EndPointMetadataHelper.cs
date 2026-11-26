using AccountManagementService.API.Shared.Attributes;
using Microsoft.AspNetCore.Http.Features;

namespace AccountManagementService.API.Shared.Helpers
{
    public static class EndPointMetadataHelper
    {
        public static (EncryptionMode Mode, EncryptionAlgorithm Algorithm)
            GetEncryptionSettings(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<EncryptionAttribute>();

            return attribute is null
                ? (EncryptionMode.None, EncryptionAlgorithm.None)
                : (attribute.Mode, attribute.Algorithm);
        }

        public static string[] GetEncryptFields(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<EncryptFieldsAttribute>();

            return attribute?.Fields ?? Array.Empty<string>();
        }

        public static string[] GetMaskedFields(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<MaskFieldsAttribute>();

            return attribute?.Fields ?? Array.Empty<string>();
        }

        public static ResponseStyle GetResponseStyle(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ResponseStyleAttribute>();

            return attribute?.Style ?? ResponseStyle.Standard;
        }

        public static (bool IsRequired, string[] AllowedClients) GetApiKeySettings(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<RequireApiKeyAttribute>();

            return attribute is null
                ? (false, Array.Empty<string>())
                : (true, attribute.AllowedClients);
        }
    }
}
