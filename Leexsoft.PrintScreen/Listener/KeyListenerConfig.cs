using log4net;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Leexsoft.PrintScreen.Listener
{
    public class KeyListenerConfig
    {
        private static readonly string ConfigFileName = "Listener.config";
        private static readonly string DefaultPath = @"C:\";
        private static readonly string DefaultScreen = Screen.PrimaryScreen.DeviceName;
        private static readonly Rectangle DefaultArea = Rectangle.Empty;

        public bool HasChange { get; set; }
        public string SavePath { get; set; }
        public string ScreenName { get; set; }
        public Rectangle WatchArea { get; set; }

        #region 只读属性
        public Screen WatchScreen
        {
            get
            {
                if (Screen.AllScreens.Any(s => s.DeviceName.Equals(ScreenName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return Screen.AllScreens.First(s => s.DeviceName.Equals(ScreenName, StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    return Screen.PrimaryScreen;
                }
            }
        }
        public string AreaDesc
        {
            get
            {
                if (!WatchArea.Equals(DefaultArea))
                {
                    return string.Format("{{x:{0}, y:{1}, w:{2}, h:{3}}}", WatchArea.X, WatchArea.Y, WatchArea.Width, WatchArea.Height);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        #endregion

        public KeyListenerConfig()
        {
            HasChange = false;
            SetDefault();
            ReadConfig();
        }

        private void SetDefault()
        {
            SavePath = DefaultPath;
            ScreenName = DefaultScreen;
            WatchArea = DefaultArea;
        }

        private void ReadConfig()
        {
            try
            {
                var doc = ConfigHelper.GetConfigXml(ConfigFileName);
                var configNode = doc.SelectSingleNode("/configuration/config");
                XmlNode node = null;
                if (configNode != null)
                {
                    node = configNode.SelectSingleNode("path");
                    SavePath = node.InnerText;
                    node = configNode.SelectSingleNode("screen");
                    ScreenName = node.InnerText;
                    node = configNode.SelectSingleNode("area");
                    if (node != null)
                    {
                        WatchArea = ReadArea(node.InnerText);
                    }
                    this.ValidateConfig();
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("error").Error("读取配置文件报错", ex);
                //设置默认配置并保存
                this.SetDefault();
                this.SaveConfig();
            }
        }

        private Rectangle ReadArea(string areaJson)
        {
            var anonymous = new { x = 0, y = 0, w = 0, h = 0 };
            var obj = JsonConvert.DeserializeAnonymousType(areaJson, anonymous);
            return new Rectangle(obj.x, obj.y, obj.w, obj.h);
        }        

        private void ValidateConfig()
        {
            var needSave = false;
            if(!Directory.Exists(SavePath))
            {
                needSave = true;                
            }
            if(!Screen.AllScreens.Any(i=>i.DeviceName.Equals(ScreenName, StringComparison.CurrentCultureIgnoreCase)))
            {
                needSave = true;
            }
            if(needSave)
            {
                this.SetDefault();
                this.SaveConfig();
            }
        }

        public void SaveConfig()
        {
            this.HasChange = false;

            var filePath = ConfigHelper.GetConfigFilePath(ConfigFileName, false);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("configuration");
            xmlDoc.AppendChild(root);
            var config = xmlDoc.CreateElement("config");
            root.AppendChild(config);
            var pathNode = xmlDoc.CreateElement("path");
            pathNode.InnerText = SavePath;
            config.AppendChild(pathNode);
            var screenNode = xmlDoc.CreateElement("screen");
            screenNode.InnerText = ScreenName;
            config.AppendChild(screenNode);
            if (WatchArea != DefaultArea)
            {
                var areaNode = xmlDoc.CreateElement("area");
                areaNode.InnerText = string.Format("{{x:{0}, y:{1}, w:{2}, h:{3}}}", WatchArea.X, WatchArea.Y, WatchArea.Width, WatchArea.Height);
                config.AppendChild(areaNode);
            }
            xmlDoc.Save(filePath);
        }
    }
}
