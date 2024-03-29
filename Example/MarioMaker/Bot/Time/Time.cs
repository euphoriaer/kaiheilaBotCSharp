﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MarioMaker
{
    internal partial class Program
    {
        public static bool CanSend = true;

        [AttrMario(".777.GROUP")]
        public static void ShuTest(JToken jObject = null)
        {
            try
            {
                string targetId = jObject["target_id"].ToString();
                SendTimeMsg(targetId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           
        }

        public static void TimeSend()
        {
            int curHour = DateTime.Now.Hour;
            Console.WriteLine("当前时间为 ：" + curHour+"当前是否可推送"+CanSend);
            if (curHour == 6 && CanSend)
            {
                SendTimeMsg("9237630566057358");
                SendTimeMsg("9960624620226581");
                CanSend = false;
                //排行榜 9237630566057358
                //闲聊 9960624620226581
            }

            if (curHour == 1)
            {
                CanSend = true;
            }
        }

        private static void SendTimeMsg(string targetId)
        {

            string msgShu = SendShu(Cfg.Read("Time"), "");
            var msgShus = JsonConvert.DeserializeObject<JToken>(msgShu);

            string dayId = msgShus["data"][0]["dayId"].ToString();
            string levelNum = msgShus["data"][0]["levelNum"].ToString();
            string levelSumAll = msgShus["data"][0]["levelSumAll"].ToString();
            string levelSub = msgShus["data"][0]["levelSub"].ToString();

            JToken js1 = JsonConvert.DeserializeObject<JToken>(Time);

            string pinzhuangContent1 =
                $"今天是服务器运行的第 {dayId} 天\n服务器共计 {levelNum} 个关卡，较昨日新增了 {levelSub} 关 \n昨日大家共计过了 {levelSumAll} 关";
            js1[0]["modules"][1]["text"]["content"] = pinzhuangContent1;
            
            string json1 = JsonConvert.SerializeObject(js1);//初始化注册成功的卡片消息

            JObject dic1 = new JObject();
            dic1.Add("type", "10");
            dic1.Add("content", json1);
            dic1.Add("target_id", targetId);//error 要发送的频道ID

            string Ress1 = JsonConvert.SerializeObject(dic1);
            _log.Record("发送每日推送" + Ress1);
            _bot.SendMessage.Post(_baseUrl + "/api/v3/message/create", Ress1);
        }
    }
}