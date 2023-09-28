// See https://aka.ms/new-console-template for more information
using Fiddler;
using log4net;
using log4net.Config;
using log4net.Core;
using QluSportHelper.Token;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
namespace QluSportHelper;
public class MainProgram
{
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("主程序");
    public static async Task Main(String[] args)
    {
        //XmlConfigurator.Configure();
        logger.Info("未检测到token数据,接下来将开始抓取");
        logger.Info("如果您知道您的token,请关闭程序并在程序目录下的token文件中输入");
        logger.Info("如果您不知道,请输入y");
        var wait = Console.ReadLine();
        if (wait.ToLower() == "y")
        {
            logger.Info("即将开始抓取");
            logger.Warn("请在接下来弹出的提示框中选择 是 ,否则将无法抓取");
            TokenCapture token = new TokenCapture(8998);
        }
        await Task.Delay(-1);
    }
}