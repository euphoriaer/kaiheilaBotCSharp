using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".add.GROUP")]
        public static void AddGuanqia(JToken jObject)
        {
            string wholeMsg = jObject["content"].ToString();
            int spaceIndex = wholeMsg.IndexOf(" ");//定位第一个空格
            var msgs = wholeMsg.Substring(spaceIndex).Split(@"\");//空格后面的是参数

            string kaiheilaId = jObject["author_id"].ToString();
            string targetID = jObject["target_id"].ToString();

            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", msgs[0]);
            msgJobj.Add("levelName", msgs[1]);
            msgJobj.Add("levelType", msgs[2]);
            msgJobj.Add("kaiheilaId", kaiheilaId);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            var res = SendShu(Cfg.Read("Add"), msgJson);
            JToken rem = JsonConvert.DeserializeObject<JToken>(res);//解析返回的json
            if (rem["code"].ToString() == "0")
            {
                JToken js1 = JsonConvert.DeserializeObject<JToken>(RegSuccess);
                string m = "**关卡id：" + rem["data"]["levelId"] + "**\n**关卡名字：" + rem["data"]["levelName"] + "**\n**关卡类型：" + rem["data"]["levelTypeStr"] + "**\n***                                   成  功  上  传  ！！！！！***";
                js1[0]["modules"][0]["text"]["content"] = m;
                string json1 = JsonConvert.SerializeObject(js1);//初始化注册成功的卡片消息

                JObject dic1 = new JObject();
                dic1.Add("type", "10");
                dic1.Add("content", json1);
                dic1.Add("target_id", targetID);
                string Ress1 = JsonConvert.SerializeObject(dic1);
                _bot.SendMessage.Post(_baseUrl + "/api/v3/message/create", Ress1);
            }
            else if (rem["code"].ToString() == "500")
            {
                DefaultRt dd = new DefaultRt();
                dd.DefaultR(jObject, _bot, rem, targetID);
            }

            if (string.IsNullOrEmpty(res))
            {
                _bot.SendMessage.Channel(targetID, "回调错误，post返回为空");
                return;
            }
        }
    }
}