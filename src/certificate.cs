using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

public class certificate
{
    public string title { get; set; }
    public string read { get; set; }
    public string stamp { get; set; }
    public string bank { get; set; }

    public override string ToString()
    {

        var responsed = new
        {
            title = title,
            read = read,
            stamp = stamp,
            bank = bank
        };
        return JsonConvert.SerializeObject(responsed);
    }
}