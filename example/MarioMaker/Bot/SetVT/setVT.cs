using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".sv.GROUP")]
        public static void ssetVT(JToken jObject)
        {
            var msgs = jObject["content"].ToString().Split(" ");
            string kaiheilaId = jObject["author_id"].ToString();
            string targetId = jObject["target_id"].ToString();
           // var com = msgs[1].Split(@"\");//字符分割为数组

            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", msgs[1]);
            msgJobj.Add("video", msgs[3]);
            msgJobj.Add("tag", msgs[2]);
            msgJobj.Add("kaiheilaId", kaiheilaId);
            string msgJson = JsonConvert.SerializeObject(msgJobj);



            using (var client = new HttpClient())//转发到鼠宝
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Cfg.Read("Sv"));
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage); //返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();

                JToken rem = JsonConvert.DeserializeObject<JToken>(res.Result);//解析返回的json
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
                    //JToken js2 = JsonConvert.DeserializeObject<JToken>(RegDefault);
                    //js2[0]["modules"][1]["text"]["content"] = rem["msg"];
                    //string json2 = JsonConvert.SerializeObject(js2);//初始化注册失败的卡片消息
                    //JObject dic2 = new JObject();
                    //dic2.Add("type", "10");
                    //dic2.Add("content", json2);
                    //dic2.Add("target_id", targetID );
                    //string Ress2 = JsonConvert.SerializeObject(dic2);
                    //_bot.SendMessage.Post(_baseUrl + "/api/v3/message/create", Ress2);

                }
            }
        }
    }
}