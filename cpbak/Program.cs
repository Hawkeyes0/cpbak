using System;
using System.Linq;
using System.Text;
using System.IO;

namespace cpbak
{
    class Program
    {
        static void Main(string[] args)
        {
            int days = 14;
            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            if (args.Length < 2)
            {
                printUsage();
            }
            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("指定的源目录不存在。");
                return;
            }
            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine("指定的目标目录不存在。正在尝试新建。");
                try
                {
                    Directory.CreateDirectory(args[1]);
                }
                catch
                {
                    Console.WriteLine("新建目录{0}失败。", args[1]);
                    return;
                }
            }
            if (args.Length == 3)
            {
                try
                {
                    days = Convert.ToInt32(args[2]);
                }
                catch
                {
                    Console.WriteLine("指定的失效日期错误，请输入整数。默认值为{0}。", days);
                    return;
                }
            }

            copyBackupFiles(src: args[0], dest: args[1], expired: days);
        }

        private static void copyBackupFiles(string src, string dest, int expired)
        {
            string curr, target;
            string[] srcfiles, destfiles;
            DateTime fileDate, expiredDate;
            int count = 0;

            curr = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(src);
                src = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(curr);
            }
            catch
            {
                Console.WriteLine("指定的源目录不存在。");
                return;
            }
            try
            {
                Directory.SetCurrentDirectory(dest);
                dest = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(curr);
            }
            catch
            {
                Console.WriteLine("指定的目标目录不存在。");
                return;
            }
            srcfiles = Directory.GetFiles(src);
            destfiles = Directory.GetFiles(dest);
            expiredDate = DateTime.Now.Date.AddDays(-expired);

            foreach (string s in srcfiles)
            {
                fileDate = File.GetLastWriteTime(s);
                if (fileDate < expiredDate)
                {
                    continue;
                }
                target = s.Replace(src, dest);
                if (destfiles.Contains(target))
                {
                    continue;
                }
                File.Copy(s, target);
                count++;
                Console.WriteLine("文件{0}已复制。", getFilename(s));
            }
            Console.WriteLine("共 {0} 个文件已复制。", count);

            count ^= count;
            foreach (string d in destfiles)
            {
                fileDate = File.GetLastWriteTime(d);
                if (fileDate > expiredDate)
                {
                    continue;
                }
                File.Delete(d);
                Console.WriteLine("文件{0}已过期被删除。", getFilename(d));
                count++;
            }
            Console.WriteLine("共 {0} 个文件已过期被删除。", count);
        }

        private static string getFilename(string path)
        {
            int idx = path.LastIndexOf('\\');
            return path.Substring(++idx);
        }

        private static void printUsage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("将最新的备份文件复制到指定目录。");
            sb.AppendLine();
            sb.AppendLine("cpbak [drive:]srcPath [drive:]destPath [expiredDays]");
            sb.AppendLine();
            Console.Write(sb.ToString());
        }
    }
}
