using Microsoft.International.Converters.PinYinConverter;
using shortid;
using shortid.Configuration;
using Snowflake.Core;

namespace DotNetCore.EventBus.Infrastructure.IdGenerate;

public class IdGenerateExtension
{
    /// <summary>
        /// 生成订单号
        /// https://github.com/bolorundurowb/shortid
        /// </summary>
        /// <param name="prefix">订单前缀</param>
        /// <returns></returns>
        public static string GenerateUniqueOrderId(string prefix = "")
        {
            var options = new GenerationOptions(useSpecialCharacters: false, useNumbers: true, length: 9);
            var id = ShortId.Generate(options).ToUpper();
            var tt = DateTime.Now.TimeOfDay.ToString("hhmmss");
            var orderId = prefix + DateTime.Now.ToString("yyMMdd") + id;
            return orderId;
        }

        /// <summary>
        /// 生成随机短编码
        /// https://github.com/bolorundurowb/shortid
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public static string GenerateNumberCode(string prefix = "", int length = 8)
        {
            var options = new GenerationOptions(useSpecialCharacters: false, useNumbers: true, length: length);
            var id = ShortId.Generate(options).ToUpper();
            return id;
        }

        /// <summary>
        /// 生成雪花算法id
        /// </summary>
        /// <returns></returns>
        public static string GenerateSnowflakeId()
        {
            var worker = new IdWorker(1, 1);
            long id = worker.NextId();
            return id.ToString();
        }

        /// <summary>
        /// 生成有序GUID
        /// </summary>
        /// <returns></returns>
        public static string NewSequentialGuid()
        {
            var result = SequentialGuidGeneratorExtension.NewSequentialGuid().ToString("N");
            return result;
        }

        /// <summary>
        /// 汉字转化为拼音
        /// </summary>
        /// <param name="str">汉字</param>
        /// <returns>全拼</returns>
        public static string GetPinyin(string str)
        {
            string r = string.Empty;
            foreach (char obj in str)
            {
                try
                {
                    ChineseChar chineseChar = new ChineseChar(obj);
                    string t = chineseChar.Pinyins[0].ToString();
                    r += t.Substring(0, t.Length - 1);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }

        /// <summary>
        /// 获取拼音首字母
        /// </summary>
        /// <param name="str">汉字</param>
        /// <returns>首字母</returns>
        public static string GetFirstPinyin(string str)
        {
            string r = string.Empty;
            foreach (char obj in str)
            {
                try
                {
                    ChineseChar chineseChar = new ChineseChar(obj);
                    string t = chineseChar.Pinyins[0].ToString();
                    r += t.Substring(0, 1);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }

        /// <summary>
        /// 获取字符的第一个单词
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetFirstCode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            var chararry = str.ToCharArray();
            try
            {

                ChineseChar chineseChar = new ChineseChar(chararry[0]);
                string t = chineseChar.Pinyins[0].ToString();
                return t.Substring(0, 1);
            }
            catch
            {
                return chararry[0].ToString().ToUpper();
            }
        }
}
