using Newtonsoft.Json.Linq;

namespace MarioMaker
{
    internal partial class Program
    {

        [AttrMario(".help.PERSON")]
        private static void HelpChat(JToken jObject)
        {
            string kaiheilaId = jObject["author_id"].ToString();
            Bot.SendMessage.Chat(kaiheilaId, ChatHelp);
        }

        [AttrMario(".help.GROUP")]
        private static void HelpChannel(JToken jObject)
        {
            Bot.SendMessage.Channel(jObject["target_id"].ToString(), ChannelHelp);
        }
    }
}