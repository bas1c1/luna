using Newtonsoft.Json;

public class coin {
    public int count {get;set;}
    public string name {get;set;}
    public string symbol {get;set;}
    public string certificate {get;set;}

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