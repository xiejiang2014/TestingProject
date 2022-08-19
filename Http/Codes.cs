namespace ShareDrawing.HttpClient.Http
{
    /// <summary>
    /// http通信状态代码
    /// </summary>
    public enum Codes
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,


        /// <summary>
        /// 授权未通过 appkey 不存在或被禁用
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 权限不足
        /// </summary>
        NoPermission = 403,

        /// <summary>
        /// 授权码无效
        /// </summary>
        InvalidAuthorizationCode = 1104,

        /// <summary>
        /// 短信验证码无效
        /// </summary>
        InvalidSmsVerificationCode = 1105,

        /// <summary>
        /// 业务参数问题（以返回具体msg为主）
        /// </summary>
        ParameterError = 1106,


        /// <summary>
        /// 数据不存在
        /// </summary>
        DataNotExist = 1201,


        /// <summary>
        /// 签名错误
        /// </summary>
        SignatureError = 1301,

        //------------------------以下为通用错误代码

        /// <summary>
        /// 业务失败
        /// </summary>
        Failure = 400,

        /// <summary>
        /// 服务器错误
        /// </summary>
        ServerError = 500,
    }
}