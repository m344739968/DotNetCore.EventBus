using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace DotNetCore.EventBus.Infrastructure.Json
{
    public static class JsonExtension
    {
                /// <summary>
        /// 对象转json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            if (obj == null) return string.Empty;
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            var jsonstr = JsonSerializer.Serialize(obj, options: options);
            return jsonstr;
        }

        /// <summary>
        /// json转对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ToObj<T>(this string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);
            var optionsJson = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            var obj = JsonSerializer.Deserialize<T>(json, optionsJson);
            return obj;
        }
    }
}