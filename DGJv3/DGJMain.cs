using BilibiliDM_PluginFramework;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DGJv3
{
    public class DGJMain : DMPlugin
    {
        private readonly DGJWindow window;

        private VersionChecker versionChecker;

        //#################!!!程序集入口!!!#################
        public DGJMain()
        {
            //初始化插件目录结构
            try
            {
                var info = Directory.CreateDirectory(Utilities.BinDirectoryPath);//创建程序集使用的文件夹
                info.Attributes = FileAttributes.Directory | FileAttributes.Hidden;//设置属性使Directory隐藏???
            }
            catch (Exception) { }
            //增加程序集加载错误解决方法
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

            //设置插件信息
            PluginName = "点歌姬";
            PluginVer = BuildInfo.Version;
            PluginDesc = "使用弹幕点播歌曲";
            PluginAuth = "Genteure/橘子233";
            PluginCont = "https://github.com/Orange23333/DGJv3/issues";

            try
            {
                Directory.CreateDirectory(Utilities.DataDirectoryPath);//创建插件的数据文件夹
            }
            catch (Exception) { }

            //初始化窗口
            window = new DGJWindow(this);

            //检查版本更新
            versionChecker = new VersionChecker("Orange23333/AutoPictureClicker");
            Task.Run(() =>
            {
                if (versionChecker.FetchInfo())//如果能获取插件远端版本信息
                {
                    Version current = new Version(BuildInfo.Version);//获取插件当前版本信息

                    if (versionChecker.HasNewVersion(current))//比较当前插件与远端版本
                    {
                        Log("插件有新版本" + Environment.NewLine +
                            $"当前版本：{BuildInfo.Version}" + Environment.NewLine +
                            $"最新版本：{versionChecker.Version.ToString()} 更新时间：{versionChecker.UpdateDateTime.ToShortDateString()}" + Environment.NewLine +
                            versionChecker.UpdateDescription);
                    }
                }
                else
                {
                    Log("版本检查出错：" + versionChecker?.LastException?.Message);
                }
            });
        }

        public override void Admin()
        {
            window.Show();
            window.Activate();
        }

        public override void DeInit() => window.DeInit();

        /// <summary>
        /// 解决程序集加载失败的问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();//获取当前代码所处程序集
            AssemblyName assemblyName = new AssemblyName(args.Name);//获取需要解决的程序集的标识

            var path = assemblyName.Name + ".dll";//生成程序集dll文件名
            string filepath = Path.Combine(Utilities.BinDirectoryPath, path);//生成程序集完整的所在位置

            if (assemblyName.CultureInfo?.Equals(CultureInfo.InvariantCulture) == false)
            { path = string.Format(@"{0}\{1}", assemblyName.CultureInfo, path); }//用区域语言加载程序集???

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))//???
            {
                if (stream == null) { return null; }

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);//读入本地的程序集
                try
                {
                    System.IO.File.WriteAllBytes(filepath, assemblyRawBytes);//把本地的程序集及写入到可能存在的位置的的程序集文件中
                }
                catch (Exception) { }
            }

            return Assembly.LoadFrom(filepath);//读取可能存在的未知的程序集
        }
    }
}
