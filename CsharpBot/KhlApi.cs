﻿namespace CsharpBot
{//error 改写一个可复用的Http get 和Set
    internal static class KhlApi
    {
        internal static string BaseUrl = "https://www.kaiheila.cn";
        internal static string ApiGateway = "/api/v3/gateway/index";
        internal static string ChannelCreate = "/api/v3/message/create";
        internal static string ChatCreate = "/api/v3/direct-message/create";
    }
}