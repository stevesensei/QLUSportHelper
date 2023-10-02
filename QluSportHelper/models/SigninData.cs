namespace QluSportHelper.models;

//这个数据命名有种不顾运维死活的美
    public class Data
    {
        public string msg { get; set; }
        public int max1km { get; set; }
        public int crontime { get; set; }
        public double center_lat { get; set; }
        public int cronstate { get; set; }
        public Line line { get; set; }
        public int zoom { get; set; }
        public int min1km { get; set; }
        public double center_lng { get; set; }
        public User user { get; set; }
    }

    public class Line
    {
        public double distence { get; set; }
        public string create_time { get; set; }
        public double lng { get; set; }
        public string? endtime { get; set; }
        public string? starttime { get; set; }
        public int clock_in_total { get; set; }
        public double total { get; set; }
        public int clock_in { get; set; }
        public int member { get; set; }
        public int id { get; set; }
        public int state { get; set; }
        public string max_end_time { get; set; }
        public double complete { get; set; }
        public List<Line2> lines { get; set; }
        public double lat { get; set; }
    }

    public class Line2
    {
        public int point_id { get; set; }
        public double distence { get; set; }
        public string create_time { get; set; }
        public double lng { get; set; }
        public double total_distence { get; set; }
        public int gps { get; set; }
        public string point_name { get; set; }
        public int line_id { get; set; }
        public string bs_id { get; set; }
        public string bs_name { get; set; }
        public int ble { get; set; }
        public object clock_in_time { get; set; }
        public int id { get; set; }
        public double lat { get; set; }
    }

    public class SigninData
    {
        public Data? data { get; set; }
        public object user { get; set; }
        public int code { get; set; }
        public string? msg { get; set; }
    }

    public class User
    {
        public double sport_count { get; set; }
        public string unionId { get; set; }
        public string create_time { get; set; }
        public string open_id { get; set; }
        public string head_img { get; set; }
        public int sex { get; set; }
        public string xuehao { get; set; }
        public int sport_days { get; set; }
        public string phone { get; set; }
        public int sport_num { get; set; }
        public string last_time { get; set; }
        public string idcard { get; set; }
        public string nickname { get; set; }
        public string name { get; set; }
        public int station { get; set; }
        public int id { get; set; }
        public int clazz { get; set; }
    }
/// <summary>
/// 傻逼
/// </summary>
public class SigninDataDaka
{
    public object data { get; set; }
    public object user { get; set; }
    public int code { get; set; }
    public string? msg { get; set; }
}