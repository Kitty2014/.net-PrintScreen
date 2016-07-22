//===================================================================
// 文件名:		ConfigHelper.cs
// 版权:		Copyright (C) 2010 Leexsoft
// 创建人:		Leexsoft
// 创建日期:	2013-11-8
// 描述:		配置文件辅助类
// 备注:		
//===================================================================
using System;
using System.IO;
using System.Xml;

namespace Leexsoft.PrintScreen.Listener
{
    public static class ConfigHelper
	{
		public static string GetConfigFilePath(string configFileName, bool checkExists)
		{
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\" + configFileName);
			if (checkExists && !File.Exists(filePath)) throw new IOException(string.Format("文件{0}不存在", filePath));

			return filePath;
		}

		public static string[] FindConfigFiles(string patternName)
		{
			string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config");
			string[] filePaths = Directory.GetFiles(dirPath, patternName);
			return filePaths;
		}

		public static XmlDocument GetConfigXml(string configFileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(GetConfigFilePath(configFileName, true));
			return doc;
		}
	}
}
