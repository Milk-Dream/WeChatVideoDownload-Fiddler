using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fiddler视频号助手
{
    public class MyExtention : IFiddlerExtension,IAutoTamper
    {
        private TabPage tabPage;
        private MyControl myCtrl;
        public static String runPath = System.IO.Directory.GetCurrentDirectory() + "\\Scripts\\xbdrunconfig\\";
        public string xbdUrl = "";
        public MyExtention() { 
            this.tabPage = new TabPage("小白视频号助手");//选项卡的名字为小白视频号助手
            this.myCtrl = new MyControl();
            
        }
        public void OnBeforeUnload()
        {
            string checkVal = myCtrl.check_start.Checked ? "1" : "";
            string saveVal = myCtrl.text_save.Text;
            Utils.writeIni("start", checkVal);
            Utils.writeIni("save", saveVal);  
        }

        public void InitSetting()
        {
            if (!System.IO.Directory.Exists(runPath))
            {
                System.IO.Directory.CreateDirectory(runPath);
            }

            myCtrl.check_start.Checked = Utils.readIni("start") != "" ? true : false;
            myCtrl.text_save.Text = Utils.readIni("save");
            
        }

        public void OnLoad()
        {
            //初始化配置，读取配置项
            InitSetting();
            
            this.myCtrl.Dock = DockStyle.Fill;
            //将用户控件添加到选项卡中
            this.tabPage.Controls.Add(this.myCtrl);
            //为选项卡添加icon图标，这里使用Fiddler 自带的
            //this.tabPage.ImageIndex = (int)Fiddler.SessionIcons.Timeline;
            FiddlerApplication.UI.tabsViews.ImageList.Images.Add("FiddlersphIcon", WeChatVideoDownload.Properties.Resources.sph);
            this.tabPage.ImageIndex = FiddlerApplication.UI.tabsViews.ImageList.Images.IndexOfKey("FiddlersphIcon");
            //将tabTage选项卡添加到Fidder UI的Tab 页集合中
            FiddlerApplication.UI.tabsViews.TabPages.Add(this.tabPage);
        }

        public bool isExistsUrl(string url)
        {
            for(int i = 0;i < myCtrl.listView1.Items.Count;i++)
            {
                
                if (myCtrl.listView1.Items[i].SubItems[2].Text == url)
                {
                    return true;
                }
            }
            return false;
        }

        public void AutoTamperRequestBefore(Session oSession)
        {
            //请求之前
            if (myCtrl.check_start.Checked)
            {
                if (!oSession.fullUrl.Contains("xW21") && oSession.host == "finder.video.qq.com" && oSession.fullUrl.Contains("&web=1") && oSession.fullUrl.Contains("stodownload?"))
                {
                    var header = oSession.oRequest["Range"];
                    string url = oSession.fullUrl;
                    
                    if (header.StartsWith("bytes=0-") && !isExistsUrl(url) && xbdUrl != url)
                    {
                        xbdUrl = url;
                        string mp4Url = url.Replace("20300", "20304").Replace("20301", "20304").Replace("20302", "20304").Replace("20303", "20304").Replace("20305", "20304").Replace("20306", "20304").Replace("20307", "20304").Replace("20308", "20304").Replace("20309", "20304");
                        ListViewItem listView = new ListViewItem();
                        listView.Checked = true;
                        listView.SubItems.Add("视频号视频" + myCtrl.listView1.Items.Count);
                        listView.SubItems.Add(mp4Url);
                        listView.SubItems.Add("待下载");
                        myCtrl.listView1.Items.Add(listView);
                    }

                }
            }
        }
        public void AutoTamperRequestAfter(Session oSession)
        {
            //请求之后
        }

        public void AutoTamperResponseBefore(Session oSession)
        {
            //响应之前
        }

        public void AutoTamperResponseAfter(Session oSession)
        {
            //响应之后
        }

        public void OnBeforeReturningError(Session oSession)
        {
            //返回异常之前
        }
    }
}
