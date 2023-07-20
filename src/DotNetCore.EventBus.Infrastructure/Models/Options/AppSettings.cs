namespace DotNetCore.EventBus.Infrastructure.Models.Options
{
 public class AppSettings
    {
        /// <summary>
        /// 应用id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 应用密钥
        /// </summary>
        public string ClientSecret { get; set; }
    }
}