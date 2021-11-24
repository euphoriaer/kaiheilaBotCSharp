﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".clear.GROUP")]
        public static void Clear(JToken jObject)
        {
            var msgs = jObject["content"].ToString().Split(" ");
            string kaiheilaId = jObject["author_id"].ToString();
            string 频道id = jObject["target_id"].ToString();
            var com = msgs[1].Split(@"\");//字符分割为数组

            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", com[0]);
            msgJobj.Add("difficultyVote", com[1]);
            msgJobj.Add("like", com[2]);
            msgJobj.Add("kaiheilaId", kaiheilaId);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            #region 卡片消息实例

#if DEBUG
            //注册成功Card
            string m =
                "[{\"type\":\"card\",\"theme\":\"info\",\"size\":\"lg\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"\"}},{\"type\":\"divider\"}]}]";

            JToken js = JsonConvert.DeserializeObject<JToken>(m);

            js[0]["modules"][0]["text"]["content"] = "s";

            string n = JsonConvert.SerializeObject(js);

            JObject dic2 = new JObject();//开始组装
            dic2.Add("type", "10"); //卡片消息
            dic2.Add("target_id", 频道id);//要发的频道
            dic2.Add("content", n);//鼠宝返回的内容

            //组装完毕，转Josn
            string json2 = JsonConvert.SerializeObject(dic2);

            //Bot.SendMessage.Channel(频道id, json2);
            _bot.SendMessage.Post(_baseUrl + "/api/v3/message/create", json2);

#endif

            #endregion 卡片消息实例

            using (var client = new HttpClient())//转发到鼠宝
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Cfg.Read("Clear"));
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage); //返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();
               

                


                //如果鼠宝的消息是空的，发个报错给kaiheila
                if (string.IsNullOrEmpty(res.Result))//转发消息给bot
                {
                    _bot.SendMessage.Channel(jObject["target_id"].ToString(), "回调错误，post返回为空");
                }
                else
                {
                    _bot.SendMessage.Channel(jObject["target_id"].ToString(), "" + res.Result);
                }
            }
        }
    }
}