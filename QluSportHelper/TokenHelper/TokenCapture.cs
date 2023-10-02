using Fiddler;
using log4net;

namespace QluSportHelper.Token;

public class TokenCapture
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger("Token抓取器");
    public TokenCapture(int port)
    {
        FiddlerCoreStartupSettingsBuilder builder = new FiddlerCoreStartupSettingsBuilder();
        builder.ListenOnPort((ushort)port);
        builder.AllowRemoteClients();
        builder.RegisterAsSystemProxy();
        builder.DecryptSSL();
        var config = builder.Build();
        FiddlerApplication.Startup(config);
        InstallCertificate();
        _logger.Warn("证书安装成功!注意此时您无法正常浏览网页,抓取结束后方可恢复");
        _logger.Info("成功打开代理服务器!");
        _logger.Warn("=========Warning=========");
        _logger.Warn("接下来请在登录界面点击右上角的小齿轮,开启使用代理,地址和端口分别填入127.0.0.1和8998随后");
        _logger.Warn("请登录微信,打开觅幂小程序,随意点击几处,程序将自动抓取token");
        _logger.Warn("=========Warning=========");
        FiddlerApplication.BeforeRequest += CaptureToken;
    }

    public void Uninstall()
    {
        UninstallCertificate();
    }

    public static bool InstallCertificate()
    {
        if (!CertMaker.rootCertExists())
        {
            if (!CertMaker.createRootCert())
                return false;

            if (!CertMaker.trustRootCert())
                return false;
        }

        return true;
    }
    
    public static bool UninstallCertificate()
    {
        if (CertMaker.rootCertExists())
        {
            if (!CertMaker.removeFiddlerGeneratedCerts(true))
                return false;
        }
        return true;
    }

    public void CaptureToken(Session session)
    {
        if (session.hostname == "admin.report.mestallion.com")
        {
            if (!String.IsNullOrEmpty(session.RequestHeaders["token"]))
            {
                FiddlerApplication.BeforeRequest -= CaptureToken;
                _logger.Info("已经抓取到token!正在写入文件");
                _logger.Warn("请在下一步的移除窗口中选择确定,否则将无法正确浏览网页!");
                if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "token")))
                {
                    var str = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token"));
                    str.Close();
                }
                Console.WriteLine(session.RequestHeaders["token"]);
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token"),session.RequestHeaders["token"]);
                Uninstall();
                Fiddler.FiddlerApplication.oProxy.Detach();
                FiddlerApplication.Shutdown();
                Console.Clear();
                MainProgram.Main(null);
            }
        }
    }
}