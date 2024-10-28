using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenapiDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

//有道翻译实现

namespace all_on_whatsapp
{
    public static class YoudaoTranslator
    {
        // 您的应用ID
        private static string APP_KEY = "26865035ffe9b6d8";
        // 您的应用密钥
        private static string APP_SECRET = "HHgkdtSEh8xMHNggKJNcUnFr3xgjZJi7";

        public static string Execute(string q)
        {
            // 添加请求参数
            Dictionary<String, String[]> paramsMap = createRequestParams(q);
            // 添加鉴权相关参数
            AuthV3Util.addAuthParams(APP_KEY, APP_SECRET, paramsMap);
            Dictionary<String, String[]> header = new Dictionary<string, string[]>() { { "Content-Type", new String[] { "application/x-www-form-urlencoded" } } };
            // 请求api服务
            byte[] result = HttpUtil.doPost("https://openapi.youdao.com/api", header, paramsMap, "application/json");
            // 打印返回结果
            if (result == null)
            {
                return string.Empty;
            }
            string resStr = System.Text.Encoding.UTF8.GetString(result);

            try
            {
                JObject parsedJson = JObject.Parse(resStr);
                JArray translations = (JArray)parsedJson["translation"]!;
                string translatedText = translations[0].ToString();
                Debug.WriteLine("翻译结果: " + translatedText);
                return translatedText;
            }
            catch (JsonException e)
            {
                Logger.Error("解析 JSON 时出错: " + e.Message);
                return string.Empty;
            }


        }
        private static Dictionary<String, String[]> createRequestParams(string q)
        {
            string from = "auto";
            string to = "EN";

            return new Dictionary<string, string[]>() {
                { "q", new string[]{q}},
                {"from", new string[]{from}},
                {"to", new string[]{to}},
            };
        }
    }
}
