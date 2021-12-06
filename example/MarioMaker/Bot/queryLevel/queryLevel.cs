using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".d.GROUP")]
        public static void QueryLevel(JToken jObject)
        {
            string wholeMsg = jObject["content"].ToString();
            int spaceIndex = wholeMsg.IndexOf(" ");//定位第一个空格
            var msgs = wholeMsg.Substring(spaceIndex + 1).Split(@"\");

            string kaiheilaId = jObject["author_id"].ToString();
            string targetID = jObject["target_id"].ToString();

            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", msgs[0]);
            msgJobj.Add("kaiheilaId", kaiheilaId);
            string msgJson = JsonConvert.SerializeObject(msgJobj);
            Console.WriteLine("往鼠宝发送信息: " + msgJson);
            using (var client = new HttpClient())
            {
                var res = SendShu(Cfg.Read("D"), msgJson);

                JToken rem = JsonConvert.DeserializeObject<JToken>(res);//解析返回的json
                if (rem["code"].ToString() == "0")
                {
                    JToken js1 = JsonConvert.DeserializeObject<JToken>(DSuccess);
                    //string m = "**名字：" + rem["data"][0]["levelName"] + "**\n**作者：" + rem["data"][0]["creator"] + "**\n**关卡类型：" + rem["data"][0]["levelTypeStr"] + "**\n**难度/平均难度：" + rem["data"][0]["difficulty"] + "/" + rem["data"][0]["difficultyVote"] + "**\n**过关人数：" + rem["data"][0]["clear"] + "**\n**喜欢：" + rem["data"][0]["like"] + "**\n**视频地址：" + rem["data"][0]["video"] + "**\n**简介：" + rem["data"][0]["tag"] + "**\n状态：" + rem["data"][0]["leveStatusStr"];
                    string m = "**名字：**" + rem["data"][0]["levelName"] + "\n**作者：**" + rem["data"][0]["creator"] + "\n**关卡类型：**" + rem["data"][0]["levelTypeStr"] + "\n**难度/平均难度：**" + rem["data"][0]["difficulty"] + "/" + rem["data"][0]["difficultyVote"] + "\n**过关人数：**" + rem["data"][0]["clear"] + "\n**喜欢：**" + rem["data"][0]["like"] + "\n**视频地址：**" + rem["data"][0]["video"] + "\n**简介：**" + rem["data"][0]["tag"] + "\n**状态：**" + rem["data"][0]["levelStatusStr"];
                    string v = "**你查询的关卡名为 ***" + rem["data"][0]["levelId"] + "***,以下是关卡信息： **";
                    js1[0]["modules"][0]["text"]["content"] = v;

                    js1[1]["modules"][0]["text"]["content"] = m;
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
}