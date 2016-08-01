using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace WpfApplication1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 发送 API 请求并返回方案信息。
        /// </summary>
        /// <returns></returns>
        private static T RequestApi<T>(string origin, string origin_region, string destination, string destination_region, string mode)
        {
            string apiUrl = "http://api.map.baidu.com/direction/v1";
            //string ak = "E4805d16520de693a3fe707cdc962045";
            string apiKey = "TuwbP44P2NNGaI2GmLOo3yC4aSGuX0Db"; //
            string output = "json";
            origin_region = "北京";
            origin = "清华大学";
            destination = "北京大学";
            destination_region = "北京";
            mode = "driving";
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add("ak", apiKey);
            param.Add("output", output);
            if (mode == "driving")
            {
                param.Add("origin_region", origin_region);
                param.Add("destination_region", destination_region);
            }
            else
            {
                param.Add("region", origin_region);
            }

            param.Add("origin", origin);
            param.Add("destination", destination);
            param.Add("mode", mode);

            string result = string.Empty;

            //初始化方案信息实体类。
            T info = default(T);
            try
            {
                //以 Get 形式请求 Api 地址
                result = HttpUtils2.DoGet(apiUrl, param);
                info = JsonHelper.FromJson<T>(result);
            }
            catch (Exception)
            {
                info = default(T);
                throw;
            }

            return info;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show( NoHTML( RequestApi<Object>(null, null, null, null,null).ToJson()));
        }


        public string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([/r/n])[/s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "/", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "/xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "/xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "/xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "/xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(/d+);", "", RegexOptions.IgnoreCase);
            //替换掉 < 和 > 标记
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("/r/n", "");
            //返回去掉html标记的字符串
            return Htmlstring;
        }

    }
}
