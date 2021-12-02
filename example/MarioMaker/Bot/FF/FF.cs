using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".F.GROUP")]
        public static void FF(JToken jObject)
        {

            string wholeMsg = jObject["content"].ToString();
            int spaceIndex = wholeMsg.IndexOf(" ");//定位第一个空格
            var msgs = wholeMsg.Substring(spaceIndex).Split(@"\");//空格后面的是参数

            string kaiheilaId = jObject["author_id"].ToString();
            string targetID = jObject["target_id"].ToString();

            JToken js1 = JsonConvert.DeserializeObject<JToken>(RegSuccess);
            string m = "F****************************************************************k";
            js1[0]["modules"][0]["text"]["content"] = m;
            string json1 = JsonConvert.SerializeObject(js1);//初始化注册成功的卡片消息

            JObject dic1 = new JObject();
            dic1.Add("type", "10");
            dic1.Add("content", json1);
            dic1.Add("target_id", targetID);
            string Ress1 = JsonConvert.SerializeObject(dic1);
            _bot.SendMessage.Post(_baseUrl + "/api/v3/message/create", Ress1);

        }
    }
}