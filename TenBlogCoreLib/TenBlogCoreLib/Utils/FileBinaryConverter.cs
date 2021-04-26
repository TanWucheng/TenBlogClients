using System;
using System.IO;

namespace TenBlogCoreLib.Utils
{
    public class FileBinaryConverter
    {
        /// <summary>
        /// 将文件转换为byte数组
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] File2Bytes(string path)
        {
            if (!File.Exists(path))
            {
                return Array.Empty<byte>();
            }

            var fi = new FileInfo(path);
            var buff = new byte[fi.Length];

            var fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return buff;
        }

        /// <summary>
        /// 将byte数组转换为文件并保存到指定地址
        /// </summary>
        /// <param name="buff">byte数组</param>
        /// <param name="savPath">保存地址</param>
        public static void Bytes2File(byte[] buff, string savPath)
        {
            if (File.Exists(savPath))
            {
                File.Delete(savPath);
            }

            var fs = new FileStream(savPath, FileMode.CreateNew);
            var bw = new BinaryWriter(fs);
            bw.Write(buff, 0, buff.Length);
            bw.Close();
            fs.Close();
        }
    }
}
