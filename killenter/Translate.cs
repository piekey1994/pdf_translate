using Ivony.Html;
using Ivony.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using System.IO;
using Newtonsoft.Json.Linq;

namespace killenter
{
    class Translate
    {
        static VsaEngine Engine = VsaEngine.CreateEngine();
        static object EvalJScript(string JScript)
        {
            object Result = null;
            try
            {
                Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return Result;

        }
        private static string ExecuteScript(string sExpression, string sCode)
        {
            MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
            scriptControl.UseSafeSubset = true;
            scriptControl.Language = "JScript";
            scriptControl.AddCode(sCode);
            try
            {
                string str = scriptControl.Eval(sExpression).ToString();
                return str;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }
        private static String getTKKScript()
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.165063 Safari/537.36 AppEngine-Google.");
                string html = webClient.DownloadString("https://translate.google.cn/");
                var parser = new JumonyParser();
                var htmlSource = parser.Parse(html);
                var allinfo = htmlSource.Find("script");
                String[] sA = { "TKK" };
                foreach (var info in allinfo)
                {
                    var scriptText = info.OuterHtml();
                    if (scriptText.IndexOf("TKK") > 0)
                    {

                        var res = scriptText.Split(new string[] { "TKK" }, StringSplitOptions.None)[1];
                        res = res.Split(new string[] { ");" }, StringSplitOptions.None)[0];
                        return "var TKK" + res + ");";
                    }
                }
                return "none";
            }
        }

        private static String getTK(String oldText)
        {
            String TKKScript = getTKKScript();
            string tkk = EvalJScript(TKKScript).ToString();
            string path = AppDomain.CurrentDomain.BaseDirectory + "tk.js";
            string str2 = File.ReadAllText(path);
            string fun = string.Format(@"tk('{0}','{1}')", oldText, tkk);
            Console.WriteLine(fun);
            string result = ExecuteScript(fun, str2);
            return result;
        }

        public static String translate(String oldText)
        {
            try
            {
                String tk = getTK(oldText);
                using (var webClient = new WebClient())
                {
                    webClient.Encoding = System.Text.Encoding.GetEncoding("GB2312");
                    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.165063 Safari/537.36 AppEngine-Google.");
                    String url = "https://translate.google.cn/translate_a/single?client=t&sl=en&tl=zh-CN&hl=zh-CN&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&ie=UTF-8&oe=UTF-8&pc=1&otf=1&ssel=0&tsel=0&kc=1&tk=" + tk + "&q=" + oldText;
                    string html = Encoding.UTF8.GetString(webClient.DownloadData(url));
                    JArray jsonArray = JArray.Parse(html);
                    String result = "";
                    foreach (var sent in jsonArray[0])
                    {
                        if (sent[0] != null)
                        {
                            result += sent[0].ToString();
                        }
                    }
                    return result;
                }
            }
            catch(Exception e)
            {
                return "您的操作太频繁，请稍后再试。（当然也可能是你的电脑访问不了谷歌翻译呵呵）";
            }
            
        }
    }
}
