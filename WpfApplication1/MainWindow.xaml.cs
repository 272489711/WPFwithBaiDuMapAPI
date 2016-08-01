using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace WpfApplication1
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]

    [System.Runtime.InteropServices.ComVisibleAttribute(true)] 
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScriptHelper sh;
        private string path = @"C:\Users\Administrator\Downloads\百度离线地图Demo-v1.3\BaiduMap_cityCenter.txt";
        private bool isCoordnOn = false;
        private DispatcherTimer timer;
        private mshtml.IHTMLDocument2 doc2;
        public MainWindow()
        {
            InitializeComponent();
            mapBrowser.Navigate(Environment.CurrentDirectory+"\\HTMLPage1.html");
            InitialTreeView();
            timer = new DispatcherTimer();
            timer.Tick += timer1_Tick;

            sh = new ScriptHelper();
            mapBrowser.ObjectForScripting = sh;
            doc2 = mapBrowser.Document as mshtml.IHTMLDocument2;

            sh.ProcMessage = (s) => { mapBrowser.Focus(); };
            
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                
                string tag_lng = ((mshtml.IHTMLElement)(doc2.all.item("mouselng",0))).innerText;
                string tag_lat = ((mshtml.IHTMLElement)(doc2.all.item("mouselat", 0))).innerText;
                double dou_lng, dou_lat;
                if (double.TryParse(tag_lng, out dou_lng) && double.TryParse(tag_lat, out dou_lat))
                {
                   cusorLbl.Content = "当前坐标：" + dou_lng.ToString("F5") + "," + dou_lat.ToString("F5");
                }
            }
            catch (Exception ee)
            { MessageBox.Show(ee.Message); }

        }

        public void InitialTreeView()
        {
            ReadCities rc = new ReadCities(path);
            JObject jobj = rc.getJObj();
            for (int i = 0; i < jobj["municipalities"].Count(); ++i)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = jobj["municipalities"][i]["n"];
                item.Tag = ((string)(jobj["municipalities"][i]["g"])).Split('|')[0];
                trItem1.Items.Add(item);
            }
           
            for (int j = 0; j < jobj["provinces"].Count(); ++j)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = jobj["provinces"][j]["n"];
                item.Tag = jobj["provinces"][j]["g"];
                for (int i = 0; i < jobj["provinces"][j]["cities"].Count(); ++i)
                {
                    TreeViewItem subItem = new TreeViewItem();
                    subItem.Header = jobj["provinces"][j]["cities"][i]["n"];
                    subItem.Tag = ((string)(jobj["provinces"][j]["cities"][i]["g"]));
                    subItem.MouseLeftButtonUp+= ItemSelectEvent;
                    item.Items.Add(subItem);
                    
                }
                trItem2.Items.Add(item);
            }

            for (int i = 0; i < jobj["other"].Count(); ++i)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = jobj["other"][i]["n"];
                item.Tag = ((string)(jobj["other"][i]["g"]));
                trItem3.Items.Add(item);
            }
 
        }





        public void ItemSelectEvent(object item,MouseButtonEventArgs e)
        {
            mshtml.IHTMLWindow2 wnd2 = doc2.parentWindow;
            string message ="你现在位于"+ ((TreeViewItem)item).Header.ToString();
            string Px = (((TreeViewItem)item).Tag.ToString()).Split('|')[0].Split(',')[0];
            string Py = (((TreeViewItem)item).Tag.ToString()).Split('|')[0].Split(',')[1];
            wnd2.execScript(String.Format("addMark({0},{1},'{2}',8)", Px, Py,message),"javascript");
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isCoordnOn)
            {
                cusorLbl.Visibility = Visibility.Hidden;
                ((Button)sender).Content = "开启实时坐标";
                isCoordnOn = false;
                timer.Stop();
            }
            else
            {
                cusorLbl.Visibility = Visibility.Visible;
                ((Button)sender).Content = "关闭实时坐标";
                isCoordnOn = true;
                timer.Start();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mshtml.IHTMLWindow2 wnd2 = doc2.parentWindow;
            wnd2.execScript("removeAllMarks()", "javascript");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mapBrowser.InvokeScript("eval", @"$('.anchorBL').hide();");
            clearRightBtn.IsEnabled = false;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            mapBrowser.InvokeScript("eval", @"myDis.open();");
        }
        


    }

    [ComVisible(true)]
    public class ScriptHelper
    {
        public Action<string> ProcMessage = null;
        public void SendMessage(string msg)
        {
            if (ProcMessage != null)
                ProcMessage(msg);
        }
        public void clearDelegate()
        {
            while (ProcMessage != null)
                ProcMessage -= this.ProcMessage;
        }

    }

}
