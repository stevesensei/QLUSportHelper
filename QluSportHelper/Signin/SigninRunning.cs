using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
using LitJson;
using QluSportHelper.models;
using RestSharp;
using Random = System.Random;

namespace QluSportHelper.Signin;
[Serializable]
public struct SubmitPosition
{
    public string lat { get; set; }
    public string lng { get; set; }
}
[Serializable]
public struct SubmitSignin
{
    public string ble { get; set; }
    public string gps { get; set; }
    public string lat { get; set; }
    public string lng { get; set; }
    public string bs_id { get; set; }
    public string bs_name { get; set; }
    public string id { get; set; }
}

public class SigninRunning
{
    public string Token;
    private RestClient _client;
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger("打卡");
    private SubmitPosition _position = new SubmitPosition()
    {
        lat = "36.55358",
        lng = "116.75199"
    };
    public string BaseUrl = "https://admin.report.mestallion.com/api/mini/";
    public string GetLineUrl = "https://admin.report.mestallion.com/api/mini/sport/getline";
    public string GetMapUrl = "https://admin.report.mestallion.com/api/mini/sport/today";
    public string SigninUrl = "https://admin.report.mestallion.com/api/mini/sport/daka";
    
    public SigninRunning(string token)
    {
        Token = token;
        _client = new RestClient(BaseUrl);
    }
    /// <summary>
    /// 获取路线
    /// </summary>
    public async Task GetLine()
    {
        Encoding encoding = Encoding.GetEncoding("utf-8");
        RestRequest req = new RestRequest("sport/getline", Method.Post);
        //添加请求头
        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36 MicroMessenger/7.0.9.501 NetType/WIFI MiniProgramEnv/Windows WindowsWechat");
        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        req.AddHeader("Token", Token);
        req.AddHeader("Referer", "https://servicewechat.com/wx5069fcccc8151ce3/28/page-frame.html");
        //添加请求数据
        req.AddJsonBody(JsonMapper.ToJson(_position), ContentType.FormUrlEncoded);
        var response = await _client.ExecutePostAsync(req, CancellationToken.None);
        if (response.IsSuccessStatusCode)
        {
            switch (JsonMapper.ToObject<SigninData>(encoding.GetString(response.RawBytes)).code)
            {
                case 500:
                    _logger.Error("获取路线达到上限!请明天再来吧");
                    //await GetMap();
                    break;
                case -10001:
                    _logger.Error("Token过期或出现错误,请检查或删除文件重新获取");
                    break;
                case 200:
                    _logger.Info("开始打卡,正在获取路线");
                    await GetMap();
                    break;
            }
        }
    }

    public async Task GetMap()
    {
        Encoding encoding = Encoding.GetEncoding("utf-8");
        RestRequest req = new RestRequest("sport/today", Method.Post);
        //添加请求头
        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36 MicroMessenger/7.0.9.501 NetType/WIFI MiniProgramEnv/Windows WindowsWechat");
        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        req.AddHeader("Token", Token);
        req.AddHeader("Referer", "https://servicewechat.com/wx5069fcccc8151ce3/28/page-frame.html");
        var response = await _client.ExecutePostAsync(req, CancellationToken.None);
        if (response.IsSuccessStatusCode)
        {
            SigninData data = JsonMapper.ToObject<SigninData>(encoding.GetString(response.RawBytes));
            if (data.data != null)
            {
                _logger.Info(String.Format("欢迎,{0}同学",data.data.user.name));
                _logger.Info("==========================================");
                _logger.Info(String.Format("本次打卡共{0}米,以下是本次打卡的信息:",data.data.line.distence));
                _logger.Info("------------------------------------");
                foreach (var point in data.data.line.lines)
                {
                    _logger.Info($"位置: {point.point_name}");
                    _logger.Info($"区间距离: {point.distence}");
                    _logger.Info("------------------------------------");
                }
                //实际上幂觅提供了starttime与endtime,但是显示完这个直接就把第一个卡打了,所以创建的时间就是开始时间,时长30分钟
                DateTime validTime = Convert.ToDateTime(data.data.line.create_time);
                validTime = validTime.AddMinutes(30);
                validTime = validTime.AddSeconds(15);
                _logger.Info($"到期时间: {validTime.ToString()}");
                _logger.Info("==========================================");
                //十五秒后打卡第一个点
                _logger.Info("15秒后开始打卡!");
                await StartTimer(data.data.line.lines);
            }
        }
    }

    public Task StartTimer(List<Line2> lines)
    {
        Random random = new Random();
        for (int i = 0; i < lines.Count; i++)
        {
            float time = 15 + 330 * i;
            if (i != 0)
            {
                time+=random.Next(1, 35);
            }

            var temp = i;
            Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
            {
                int index = temp;
                //这里用于提交打卡
                Encoding encoding = Encoding.GetEncoding("utf-8");
                RestRequest req = new RestRequest("sport/daka", Method.Post);
                //添加请求头
                req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36 MicroMessenger/7.0.9.501 NetType/WIFI MiniProgramEnv/Windows WindowsWechat");
                req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                req.AddHeader("Token", Token);
                req.AddHeader("Referer", "https://servicewechat.com/wx5069fcccc8151ce3/28/page-frame.html");
                SubmitSignin data = new SubmitSignin();
                data.ble = "false";
                data.gps = "false";
                data.lat = lines[index].lat.ToString();
                data.lng = lines[index].lng.ToString();
                data.bs_id = "";
                data.bs_name = "";
                data.id = lines[index].id.ToString();
                req.AddJsonBody(JsonMapper.ToJson(data));
                Console.WriteLine(JsonMapper.ToJson(data));
                var response = _client.ExecutePost(req);
                if (response.IsSuccessStatusCode)
                {
                    //尼玛为什么这个Data会返回字符串啊
                    switch (JsonMapper.ToObject<SigninDataDaka>(encoding.GetString(response.RawBytes)).code)
                    {
                        case 500:
                            _logger.Error("获取路线达到上限或找不到打卡点!");
                            break;
                        case -10001:
                            _logger.Error("Token过期或出现错误,请检查或删除文件重新获取");
                            break;
                        case 200:
                            _logger.Info("==============");
                            _logger.Info($"第{index+1}次打卡成功!");
                            _logger.Info("==============");
                            break; 
                    }
                }
            });
        }

        return Task.CompletedTask;
    }
}