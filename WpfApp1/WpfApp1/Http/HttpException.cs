using System;
using System.Net;

namespace WpfApp1.Http
{
    /// <summary>
    /// 包含友好提示信息的异常
    /// </summary>
    public class HttpException : ApplicationException
    {
        /// <summary>
        /// 不希望用户看到的异常信息
        /// </summary>
        public string? LogMessage { get; set; }

        public int Code { get; set; }


        public HttpStatusCode StatusCode { get; set; }

        public HttpException()
        {
            LogMessage = "创建 HttpException 没有指定 Message";
        }

        public HttpException(string message,
            int code,
            string logMessage
        ) : this(message, logMessage)
        {
            Code = code;
        }

        public HttpException(string message,
            string logMessage
        ) : base(message)
        {
            LogMessage = logMessage;
        }

        public HttpException(string message,
            string logMessage,
            HttpStatusCode statusCode
        ) : base(message)
        {
            LogMessage = logMessage;
            StatusCode = statusCode;
        }

        public HttpException(string message,
            int code
        ) : base(message)
        {
            Code = code;
        }

        public HttpException(string message) : base(message)
        {
        }

        public HttpException(string message,
            string logMessage,
            Exception innerException
        ) : base(message, innerException)
        {
            LogMessage = logMessage;
        }

        public HttpException(string message,
            Exception innerException
        ) : base(message, innerException)
        {
        }
    }
}