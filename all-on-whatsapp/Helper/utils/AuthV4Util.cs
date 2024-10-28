using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace OpenapiDemo
{
    static class AuthV4Util
    {

        /*
            添加v4鉴权相关参数 -
            appKey : 应用ID
            salt : 随机值
            curtime : 当前时间戳(秒)
            signType : 签名版本
            sign : 请求签名
            
            @param appKey    您的应用ID
            @param appSecret 您的应用密钥
            @param paramsMap 请求参数表
        */
        public static void addAuthParams(string appKey, string appSecret, Dictionary<string, string[]> paramsMap)
        {
            string salt = System.Guid.NewGuid().ToString();
            string curtime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + "";
            string sign = calculateSign(appKey, appSecret, salt, curtime);
            paramsMap.Add("appKey", new string[] { appKey });
            paramsMap.Add("salt", new string[] { salt });
            paramsMap.Add("curtime", new string[] { curtime });
            paramsMap.Add("signType", new string[] { "v4" });
            paramsMap.Add("sign", new string[] { sign });
        }

        /*
            计算v4鉴权签名 -
            计算方式 : sign = sha256(appKey + salt + curtime + appSecret)
        
            @param appKey    您的应用ID
            @param appSecret 您的应用密钥
            @param salt      随机值
            @param curtime   当前时间戳(秒)
            @return 鉴权签名sign
        */
        public static string calculateSign(string appKey, string appSecret, string salt, string curtime)
        {
            string strSrc = appKey + salt + curtime + appSecret;
            return encrypt(strSrc);
        }

        private static string encrypt(string strSrc)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(strSrc);

            // 使用 SHA256.Create() 创建 SHA256 实例
            using (SHA256 algorithm = SHA256.Create())
            {
                byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }
    }
}