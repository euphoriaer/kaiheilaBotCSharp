using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".sd.GROUP")]
        public static void resetDifficulty(JToken jObject)
        {
            string wholeMsg = jObject["content"].ToString();
            int spaceIndex = wholeMsg.IndexOf(" ");//定位第一个空格
            var msgs = wholeMsg.Substring(spaceIndex).Split(@"\");//空格后面的是参数

            string kaiheilaId = jObject["author_id"].ToString();
            string targetId = jObject["target_id"].ToString();
            // var com = msgs[1].Split(@"\");//字符分割为数组

            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", msgs[0]);
            msgJobj.Add("difficulty", msgs[1]);
            msgJobj.Add("kaiheilaId", kaiheilaId);

            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())//转发到鼠宝
            {
                var res = SendShu(Cfg.Read("Sd"), msgJson);

                JToken rem = JsonConvert.DeserializeObject<JToken>(res);//解析返回的json
                if (rem["code"].ToString() == "0")
                {
                    JToken js1 = JsonConvert.DeserializeObject<JToken>(SdSuccess);
                    string json1 = JsonConvert.SerializeObject(js1);//初始化注册成功的卡片消息

                    JObject dic1 = new JObject();
                    dic1.Add("type", "10");
                    dic1.Add("content", json1);
                    dic1.Add("target_id", targetId);
                    string Ress1 = JsonConvert.SerializeObject(dic1);
                    _bot.SendMessage.Post(_baseUrl + "/api/v3/message/create", Ress1);
                }
                else if (rem["code"].ToString() == "500")
                {
                    DefaultRt dd = new DefaultRt();
                    dd.DefaultR(jObject, _bot, rem, targetId);
                }
            }
        }
    }
}