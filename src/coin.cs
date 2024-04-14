using Newtonsoft.Json;

public class coin {
    public int count {get;set;} // count of coin
    public string name {get;set;} // name of coin
    public string symbol {get;set;} // symbol of coin (NOV) (NOV?) || Double :)
    public string certificate {get;set;} // certificate of coin

    public override string ToString()
    {
        var responsed = new{
            count = count,
            name = name,
            symbol = symbol,
            certificate = certificate
        };
        return JsonConvert.SerializeObject(responsed);
    }
}