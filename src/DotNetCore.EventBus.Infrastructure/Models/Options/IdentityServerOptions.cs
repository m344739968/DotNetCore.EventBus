namespace DotNetCore.EventBus.Infrastructure.Models.Options
{
    public class IdentityServerOptions
    {
        /// <summary>
        /// 授权服务地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; }
    }
}