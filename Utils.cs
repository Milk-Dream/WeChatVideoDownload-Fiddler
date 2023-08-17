using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiddler视频号助手
{
    class Utils
    {
        /// <summary>
        /// 读取配置项，没有配置项返回空，有配置项返回内容
        /// </summary>
        /// <param name="code">配置项名字</param>
        /// <returns></returns>
        public static string readIni(string code)
        {
            string path = MyExtention.runPath + code + ".txt";
            if (!System.IO.File.Exists(path))
            {
                return "";
            }
            string result = System.IO.File.ReadAllText(path);
            return result;
        }
        /// <summary>
        /// 写出配置项
        /// </summary>
        /// <param name="code">配置项名字</param>
        /// <param name="val">配置项值</param>
        public static void writeIni(string code,string val) 
        {
            string path = MyExtention.runPath + code + ".txt";
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.Create(path);
            }
            System.IO.File.WriteAllText(path, val);
        }
    }
}
