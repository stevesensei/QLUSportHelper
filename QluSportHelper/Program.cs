using QluSportHelper.Signin;
using QluSportHelper.Token;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
namespace QluSportHelper;
public class MainProgram
{
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("主程序");
    public static async Task Main(String[] args)
    {
        //情怀
        PrintIntro();
        CheckTokenFile();
        await StartRunning();
        //XmlConfigurator.Configure();
        await Task.Delay(-1);
    }

    async static Task StartRunning()
    {
        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token")))
        {
            SigninRunning runner =
                new SigninRunning(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token")));
            await runner.GetLine();
        }
    }

    static void CheckTokenFile()
    {
        if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "token"))||String.IsNullOrEmpty(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "token"))))
        {
            
            logger.Info("未检测到token数据,接下来将开始抓取");
            logger.Info("如果您知道您的token,请输入任意字符或关闭程序并在程序目录下的token文件中输入");
            logger.Info("如果您不知道,请输入n");
            var wait = Console.ReadLine();
            if (wait.ToLower() == "n")
            {
                Console.Clear();
                logger.Info("即将开始抓取");
                logger.Warn("=========Warning=========");
                logger.Warn("请在此之前确保以下内容");
                logger.Warn("1.请退出您的代理软件,如clash,v2ray等");
                logger.Warn("2.当前的代理地址为,IP:127.0.0.1,端口:8998;请确保上述地址没有被占用");
                logger.Warn("3.请仔细阅读项目wiki,防止意外发生");
                logger.Warn("4.请退出杀毒软件,如果您不放心,这是查毒报告:");
                logger.Warn("https://s.threatbook.com/report/file/719bc9c1546de3e1feb88b0c523e1d1fa022a1c9dac3030b6c96bff58c8e4c6e");
                logger.Warn("=========Warning=========");
                Console.WriteLine("请按任意键继续");
                Console.ReadLine();
                Console.Clear();
                logger.Warn("请在接下来弹出的提示框中选择 是 ,并允许防火墙,否则将无法抓取");
                TokenCapture token = new TokenCapture(8998);
            }
            else
            {
                logger.Info("请您输入token");
                var waitToken = Console.ReadLine();
                var str = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token"));
                str.Close();
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token"),waitToken);
                logger.Info("写入完毕");
            }
        }
        else
        {
            logger.Info("token存在,继续下一步");
        }
    }

    static void PrintIntro()
    {
        string[] intro = new string[] { };
        try
        {
            intro = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "intro.txt"));
        }
        catch (Exception e)
        {
            logger.Error("我的字符画咋没了捏?一定是日富美干的!");
            return;
        }

        foreach (var lines in intro)
        {
            Console.WriteLine(lines);
        }
    }
}