namespace AccountManagementService.API.Shared.Attributes
{
    public enum EncryptionMode
    {
        None = 0,
        Field = 1,
        Full = 2
    }

    public enum EncryptionAlgorithm
    {
        None = 0,
        AesGCM = 1,
        Aes256CBC = 2,
        RSA = 3
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class EncryptionAttribute : Attribute
    {
        public EncryptionMode Mode { get; }
        public EncryptionAlgorithm Algorithm { get; }

        public EncryptionAttribute(
            EncryptionMode mode = EncryptionMode.None,
            EncryptionAlgorithm algorithm = EncryptionAlgorithm.None)
        {
            Mode = mode;
            Algorithm = algorithm;
        }
    }
}
