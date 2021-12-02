﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".reg.PERSON")]
        public static void SendRegister(JToken jObject)
        {
            string wholeMsg = jObject["content"].ToString();
            int spaceIndex = wholeMsg.IndexOf(" ");//定位第一个空格
            var msgs = wholeMsg.Substring(spaceIndex).Split(@"\");//空格后面的是参数

            string kaiheilaId = jObject["author_id"].ToString();

            JObject msgJobj = new JObject();
            msgJobj.Add("kaiheilaId", kaiheilaId);
            msgJobj.Add("playerName", msgs[1]);
            msgJobj.Add("password", msgs[2]);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Cfg.Read("Reg"));
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage); //返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();

                JToken rem = JsonConvert.DeserializeObject<JToken>(res.Result);
                if (rem["code"].ToString() == "0")
                {
                    JToken js1 = JsonConvert.DeserializeObject<JToken>(RegSuccess);
                    string json1 = JsonConvert.SerializeObject(js1);//初始化注册成功的卡片消息

                    JObject dic1 = new JObject();
                    dic1.Add("type", "10");
                    dic1.Add("content", json1);
                    dic1.Add("target_id", kaiheilaId);
                    string Ress1 = JsonConvert.SerializeObject(dic1);
                    _bot.SendMessage.Post("https://www.kaiheila.cn/api/v3/direct-message/create", Ress1);
                }
                else if (rem["code"].ToString() == "500")
                {
                    JToken js2 = JsonConvert.DeserializeObject<JToken>(RegDefault);
                    js2[0]["modules"][1]["text"]["content"] = rem["msg"];
                    string json2 = JsonConvert.SerializeObject(js2);//初始化注册失败的卡片消息
                    JObject dic2 = new JObject();
                    dic2.Add("type", "10");
                    dic2.Add("content", json2);
                    dic2.Add("target_id", kaiheilaId);
                    string Ress2 = JsonConvert.SerializeObject(dic2);
                    _bot.SendMessage.Post("https://www.kaiheila.cn/api/v3/direct-message/create", Ress2);
                }

                if (string.IsNullOrEmpty(res.Result))
                {
                    _bot.SendMessage.Chat(kaiheilaId, "回调错误，post返回为空");
                }
            }
        }
    }
}