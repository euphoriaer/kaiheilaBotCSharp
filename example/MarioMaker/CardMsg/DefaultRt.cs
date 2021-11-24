using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsharpBot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;


namespace MarioMaker
{
    public  class DefaultRt
    {
        private  string RegDefault = "[{\"type\":\"card\",\"theme\":\"danger\",\"size\":\"lg\",\"modules\":[{\"type\":\"header\",\"text\":{\"type\":\"plain-text\",\"content\":\"出现错误!\"}},{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"#{msg}\"}},{\"type\":\"divider\"}]}]";
        
        public  void DefaultR(JToken jObject,Bot _bot, JToken rem, string targetID)
        {

            JToken js2 = JsonConvert.DeserializeObject<JToken>(RegDefault);
            js2[0]["modules"][1]["text"]["content"] = rem["msg"];
            string json2 = JsonConvert.SerializeObject(js2);//初始化注册失败的卡片消息
            JObject dic2 = new JObject();
            dic2.Add("type", "10");
            dic2.Add("content", json2);
            dic2.Add("target_id", targetID);
            string Ress2 = JsonConvert.SerializeObject(dic2);
            _bot.SendMessage.Post("https://www.kaiheila.cn/api/v3/message/create", Ress2);
        }
    }
}
