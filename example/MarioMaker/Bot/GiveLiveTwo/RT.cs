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
            var msgs = wholeMsg.Substring(spaceIndex).Split(@"\");//空格后面的是参数

            JObject msgJobj = new JObject();
            msgJobj.Add("levelType", msgs[0]);
        
            string msgJson = JsonConvert.SerializeObject(msgJobj);
        }
    }
}