namespace API.Implementations
{
    public class JwtOptions
    {
        /// <summary>
        /// Название секции файла конфигурации, где находятся перечисленные опции. Нужна для того, чтобы не пришлось хардкодить ее в program.cs
        /// </summary>
        public const string SectionName = "JwtOptions";

        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        /// <summary>
        /// Секретный ключ, известный только серверу. Используется для шифровки генерируемых токенов и валидации токенов, пришедших с запросом.
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// Через сколько дней токен перестанет быть валидным
        /// </summary>
        public int ExpireDays { get; set; } = 0;

    }
}
