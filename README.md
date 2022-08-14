# kaiheilaBotCSharp
开黑啦机器人C#版，需要机器人使用websocket ，基于.Net5.0 可以跨平台运行在服务器上。


# Quick Start

添加CsharpBot.dll到引用，[Releases](https://github.com/euphoriaer/kaiheilaBotCSharp/releases)

    private static void Main(string[] args)
        {
            bot = new Bot("机器人的token");
            bot.ChatMsg += Receive;
            bot.Run();
        }
        
    private static void Receive(string chat, string targetID)
        {
            Console.WriteLine("Bot:" + chat);
            if (chat == "你好")
            {
                bot.SendMessage(targetID, "我是机器人。你好");
            }
        }  

