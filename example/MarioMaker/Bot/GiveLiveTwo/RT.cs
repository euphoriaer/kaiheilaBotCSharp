using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".rt.GROUP")]
        public static void RandomRT(JToken jObject)
        {
            string wholeMsg = jObject["content"].ToString();
            int spaceIndex = wholeMsg.IndexOf(" ");//定位第一个空格
            var msgs = wholeMsg.Substring(spaceIndex + 1).Split(@"\");//空格后面的是参数

            string kaiheilaId = jObject["author_id"].ToString();
            string targetID = jObject["target_id"].ToString();

            JObject msgJobj = new JObject();
            msgJobj.Add("levelType", msgs[0]);

            string msgJson = JsonConvert.SerializeObject(msgJobj);

            string msgShu = SendShu(Cfg.Read("Rt"), msgJson);

            var msgShus = JsonConvert.DeserializeObject<JToken>(msgShu);
            DefaultRt dd = new DefaultRt();
            if (msgShus["code"].ToString() == "500")
            {
                dd.DefaultR(jObject, _bot, msgShus, targetID);
            }
            else if (msgShus["code"].ToString() == "0")
            {
                string id = msgShus["data"]["id"].ToString();
                string levelId = msgShus["data"]["levelId"].ToString();
                string levelName = msgShus["data"]["levelName"].ToString();
                string creator = msgShus["data"]["creator"].ToString();
                string difficulty = msgShus["data"]["difficulty"].ToString();
                string levelTypeStr = msgShus["data"]["levelTypeStr"].ToString();
                string levelType = msgShus["data"]["levelType"].ToString();
                string video = msgShus["data"]["video"].ToString();
                string tag = msgShus["data"]["tag"].ToString();
                string levelStatus = msgShus["data"]["levelStatus"].ToString();
                string levelStatusStr = msgShus["data"]["levelStatusStr"].ToString();
                string difficultyVote = msgShus["data"]["difficultyVote"].ToString();
                string clear = msgShus["data"]["clear"].ToString();
                string like = msgShus["data"]["like"].ToString();

                string pinzhuangContent1 = $"**你随机到了 ***#{levelId}***,以下是关卡信息： **";

                string pinzhuangContent2 =
                    $"**名字：**#{levelName}\n**作者：**#{creator}\n**类型：**#{levelTypeStr}\n**难度/平均难度：**#{difficulty}/#{difficultyVote}\n**过关人数：**#{clear}\n**喜欢：**#{like}\n**视频地址：**[#{video}](#{video})\n**简介：**#{tag}\n**状态：**#{levelStatusStr}\n";

                var json1 = JsonConvert.DeserializeObject<JToken>(RTSucess);//初始化注册成功的卡片消息
                json1[0]["modules"][0]["text"]["content"] = pinzhuangContent1;
                json1[1]["modules"][0]["text"]["content"] = pinzhuangContent2;

                JObject dic1 = new JObject();
                dic1.Add("type", "10");
                dic1.Add("content", json1);
                dic1.Add("target_id", targetID);
                string Ress1 = JsonConvert.SerializeObject(dic1);
                _bot.SendMessage.Post(_baseUrl + "/api/v3/message/create", Ress1);
            }
        }
    }
}