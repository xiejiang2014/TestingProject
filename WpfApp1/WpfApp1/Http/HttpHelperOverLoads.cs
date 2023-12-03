using System.Collections.Specialized;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WpfApp1.Http
{
    //本文件中的方法都是对真正http调用的重载,这些重载用于处理各种参数的转换


    public static partial class HttpHelper
    {
        #region Get

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<T> HttpGetAsync<T>(string url,
                                              bool   debug = true)
        {
            return HttpGetAsync<T>(url, new NameValueCollection(), debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static Task<T> HttpGetAsync<T>(string                    url,
                                              string                    token,
                                              bool                      debug    = true,
                                              [CallerMemberName] string funcName = ""
        )
        {
            return HttpGetAsync<T>(url, new NameValueCollection(), token, debug: debug, funcName);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <returns></returns>
        public static Task<T> HttpGetAsync<T>(string url,
                                              string paramName1,
                                              string paramValue1
        )
        {
            return HttpGetAsync<T>(url, new NameValueCollection
            {
                {paramName1, paramValue1}
            });
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static Task<T> HttpGetAsync<T>(string                    url,
                                              string                    paramName1,
                                              string                    paramValue1,
                                              string                    token,
                                              bool                      debug    = true,
                                              [CallerMemberName] string funcName = ""
        )
        {
            return HttpGetAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1}
                },
                token,
                debug: debug,
                funcName
            );
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<T> HttpGetAsync<T>(string url,
                                              string paramName1,
                                              string paramValue1,
                                              string paramName2,
                                              string paramValue2,
                                              bool   debug = true
        )
        {
            return HttpGetAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static Task<T> HttpGetAsync<T>(string                    url,
                                              string                    paramName1,
                                              string                    paramValue1,
                                              string                    paramName2,
                                              string                    paramValue2,
                                              string                    token,
                                              bool                      debug    = true,
                                              [CallerMemberName] string funcName = ""
        )
        {
            return HttpGetAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                token,
                debug,
                funcName
            );
        }

        public static Task<T> HttpGetAsync<T>(string                    url,
                                              NameValueCollection       @params,
                                              bool                      debug    = true,
                                              [CallerMemberName] string funcName = ""
        )
        {
            return HttpGetAsync<T>(url, @params, string.Empty, debug: debug, funcName);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string                    url,
                                                    NameValueCollection       @params,
                                                    string                    token,
                                                    bool                      debug    = true,
                                                    [CallerMemberName] string funcName = ""
        )
        {
            var json = await HttpGetAsync(url, @params, token, debug: debug, funcName);

            return ParesJson<T>(json);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string url)
        {
            return HttpGetAsync(url, new NameValueCollection());
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string url,
                                                string token
        )
        {
            return HttpGetAsync(url, new NameValueCollection(), token);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string url,
                                                string paramName,
                                                string paramValue
        )
        {
            return HttpGetAsync(
                url,
                new NameValueCollection
                {
                    {paramName, paramValue}
                });
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string url,
                                                string paramName,
                                                string paramValue,
                                                string token
        )
        {
            return HttpGetAsync(
                url,
                new NameValueCollection
                {
                    {paramName, paramValue}
                },
                token);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string url,
                                                string paramName1,
                                                string paramValue1,
                                                string paramName2,
                                                string paramValue2
        )
        {
            return HttpGetAsync(url, new NameValueCollection
            {
                {paramName1, paramValue1},
                {paramName2, paramValue2}
            });
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string url,
                                                string paramName1,
                                                string paramValue1,
                                                string paramName2,
                                                string paramValue2,
                                                string token,
                                                bool   debug = true
        )
        {
            return HttpGetAsync(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                token, debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string              url,
                                                NameValueCollection @params,
                                                bool                debug = true
        )
        {
            return HttpGetAsync(url, @params, string.Empty, debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string                    url,
                                                NameValueCollection       @params,
                                                string                    token,
                                                bool                      debug    = true,
                                                [CallerMemberName] string funcName = ""
        )
        {
            return HttpGetAsync(url, @params, null, null, null, 300000, token, debug: debug, funcName);
        }

        public static async Task<string> HttpGetAsync(
            string                    url,
            NameValueCollection       @params,
            CookieCollection?         cookies,
            NameValueCollection?      headers,
            string?                   userAgent,
            int                       timeout,
            string                    token,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            return await HttpGetAsync(url,   @params, cookies, headers, userAgent, timeout, token, ResponseConverter,
                                      debug, funcName);

            string ResponseConverter(string json)
            {
                return json;
            }
        }

        #endregion

        #region Post

        //public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url)
        //{
        //    var @params = new NameValueCollection();

        //    return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, string.Empty);
        //}

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string                    url,
                                                                       string                    token,
                                                                       bool                      debug    = true,
                                                                       [CallerMemberName] string funcName = ""
        )
        {
            var @params = new NameValueCollection();

            return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, token, true, funcName);
        }

        //public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string                    url,
        //                                                               string                    paramName1,
        //                                                               string                    paramValue1,
        //                                                               [CallerMemberName] string funcName = ""
        //)
        //{
        //    return await HttpPostXWwwFormUrlencodedAsync<T>(url, paramName1, paramValue1, string.Empty, funcName);
        //}

        //public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string                     url,
        //                                                               string                     paramName1,
        //                                                               string                     paramValue1,
        //                                                               string                     token,
        //                                                               [CallerMemberName] string? funcName = ""
        //)
        //{
        //    var @params = new NameValueCollection
        //    {
        //        {paramName1, paramValue1}
        //    };

        //    return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, token, funcName);
        //}

        //public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string                     url,
        //                                                               string                     paramName1,
        //                                                               string                     paramValue1,
        //                                                               string                     paramName2,
        //                                                               string                     paramValue2,
        //                                                               [CallerMemberName] string? funcName = ""
        //)
        //{
        //    return await HttpPostXWwwFormUrlencodedAsync<T>(url, paramName1, paramValue1, paramName2, paramValue2,
        //                                                    string.Empty, funcName);
        //}

        //public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string                     url,
        //                                                               string                     paramName1,
        //                                                               string                     paramValue1,
        //                                                               string                     paramName2,
        //                                                               string                     paramValue2,
        //                                                               string                     token,
        //                                                               [CallerMemberName] string? funcName = ""
        //)
        //{
        //    var @params = new NameValueCollection
        //    {
        //        {paramName1, paramValue1},
        //        {paramName2, paramValue2}
        //    };

        //    return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, token, funcName);
        //}

        //public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url, NameValueCollection @params,
        //                                                               [CallerMemberName] string funcName = ""
        //)
        //{
        //    return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, string.Empty, funcName);
        //}


        //public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
        //                                                                 string paramName,
        //                                                                 string paramValue
        //)
        //{
        //    return await HttpPostXWwwFormUrlencodedAsync(url, paramName, paramValue, string.Empty);
        //}

        //public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
        //                                                                 string paramName,
        //                                                                 string paramValue,
        //                                                                 string token
        //)
        //{
        //    var @params = new NameValueCollection
        //    {
        //        {paramName, paramValue}
        //    };

        //    return await HttpPostXWwwFormUrlencodedAsync(url, @params, token);
        //}

        //public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
        //                                                                 string paramName1,
        //                                                                 string paramValue1,
        //                                                                 string paramName2,
        //                                                                 string paramValue2
        //)
        //{
        //    return await HttpPostXWwwFormUrlencodedAsync(url, paramName1, paramValue1, paramName2, paramValue2,
        //                                                 string.Empty);
        //}

        //public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
        //                                                                 string paramName1,
        //                                                                 string paramValue1,
        //                                                                 string paramName2,
        //                                                                 string paramValue2,
        //                                                                 string token
        //)
        //{
        //    var @params = new NameValueCollection
        //    {
        //        {paramName1, paramValue1},
        //        {paramName2, paramValue2}
        //    };

        //    return await HttpPostXWwwFormUrlencodedAsync(url, @params, token);
        //}

        //public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string              url,
        //                                                                 NameValueCollection @params
        //)
        //{
        //    return await HttpPostXWwwFormUrlencodedAsync(url, @params, string.Empty);
        //}


        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string                    url,
                                                                       NameValueCollection       @params,
                                                                       string                    token,
                                                                       bool                      debug    = true,
                                                                       [CallerMemberName] string funcName = ""
        )
        {
            var json = await HttpPostXWwwFormUrlencodedAsync(url, @params, null, null, null, 300000, token, debug,
                                                             funcName);

            var result = ParesJson<T>(json);

            return result;
        }

        //public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string              url,
        //                                                                 NameValueCollection @params,
        //                                                                 string              token
        //)
        //{
        //    return await HttpPostXWwwFormUrlencodedAsync(url, @params, null, null, null, 300000, token);
        //}


        public static Task HttpPostAsync(
            string                    url,
            string                    token,
            int                       timeOut  = 300000,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            return HttpPostJsonAsync(url, "", token, timeOut, debug, funcName);
        }

        public static Task<T> HttpPostAsync<T>(
            string                    url,
            string                    token,
            int                       timeOut  = 300000,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            return HttpPostJsonAsync<T>(url, "", null, null, null, timeOut, token, debug, funcName);
        }


        public static Task<T> HttpPostJsonAsync<T>(
            string url,
            string json,
            int    timeOut = 300000
        )
        {
            return HttpPostJsonAsync<T>(url, json, "", timeOut);
        }

        public static Task<T> HttpPostJsonAsync<T>(
            string                    url,
            string                    json,
            string                    token    = "",
            int                       timeOut  = 300000,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            return HttpPostJsonAsync<T>(url, json, null, null, null, timeOut, token, debug, funcName);
        }

        #endregion


        #region Put

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns></returns>
        public static Task<T> HttpPutAsync<T>(string url)
        {
            return HttpPutAsync<T>(url, new NameValueCollection());
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<T> HttpPutAsync<T>(string url,
                                              string token,
                                              bool   debug = true
        )
        {
            return HttpPutAsync<T>(url, new NameValueCollection(), token, debug: debug);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <returns></returns>
        public static Task<T> HttpPutAsync<T>(string url,
                                              string paramName1,
                                              string paramValue1
        )
        {
            return HttpPutAsync<T>(url, new NameValueCollection
            {
                {paramName1, paramValue1}
            });
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<T> HttpPutAsync<T>(string url,
                                              string paramName1,
                                              string paramValue1,
                                              string token,
                                              bool   debug = true
        )
        {
            return HttpPutAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1}
                },
                token, debug: debug
            );
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<T> HttpPutAsync<T>(string url,
                                              string paramName1,
                                              string paramValue1,
                                              string paramName2,
                                              string paramValue2,
                                              bool   debug = true
        )
        {
            return HttpPutAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                debug: debug);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<T> HttpPutAsync<T>(string url,
                                              string paramName1,
                                              string paramValue1,
                                              string paramName2,
                                              string paramValue2,
                                              string token
        )
        {
            return HttpPutAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                token
            );
        }

        public static Task<T> HttpPutAsync<T>(string              url,
                                              NameValueCollection @params,
                                              bool                debug = true
        )
        {
            return HttpPutAsync<T>(url, @params, string.Empty, debug: debug);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static async Task<T> HttpPutAsync<T>(string              url,
                                                    NameValueCollection @params,
                                                    string              token,
                                                    bool                debug = true
        )
        {
            var json = await HttpPutAsync(url, @params, token, debug: debug);

            return ParesJson<T>(json);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string url)
        {
            return HttpPutAsync(url, new NameValueCollection());
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string url,
                                                string token
        )
        {
            return HttpPutAsync(url, new NameValueCollection(), token);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string url,
                                                string paramName,
                                                string paramValue
        )
        {
            return HttpPutAsync(
                url,
                new NameValueCollection
                {
                    {paramName, paramValue}
                });
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string url,
                                                string paramName,
                                                string paramValue,
                                                string token
        )
        {
            return HttpPutAsync(
                url,
                new NameValueCollection
                {
                    {paramName, paramValue}
                },
                token);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string url,
                                                string paramName1,
                                                string paramValue1,
                                                string paramName2,
                                                string paramValue2
        )
        {
            return HttpPutAsync(url, new NameValueCollection
            {
                {paramName1, paramValue1},
                {paramName2, paramValue2}
            });
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string url,
                                                string paramName1,
                                                string paramValue1,
                                                string paramName2,
                                                string paramValue2,
                                                string token,
                                                bool   debug = true
        )
        {
            return HttpPutAsync(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                token, debug: debug);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string              url,
                                                NameValueCollection @params,
                                                bool                debug = true
        )
        {
            return HttpPutAsync(url, @params, string.Empty, debug: debug);
        }

        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="token"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static Task<string> HttpPutAsync(string              url,
                                                NameValueCollection @params,
                                                string              token,
                                                bool                debug = true
        )
        {
            return HttpPutAsync(url, @params, null, null, null, 300000, token, debug: debug);
        }


        public static async Task<string> HttpPutAsync(
            string               url,
            NameValueCollection  @params,
            CookieCollection?    cookies,
            NameValueCollection? headers,
            string?              userAgent,
            int                  timeout,
            string               token,
            bool                 debug = true
        )
        {
            return await HttpPutAsync(url, @params, cookies, headers, userAgent, timeout, token, ResponseConverter,
                                      debug);

            string ResponseConverter(string json)
            {
                return json;
            }
        }

        #endregion
    }
}