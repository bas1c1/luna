using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

public class certificate
{
    public string title { get; set; } // title of the certificate
    public string read { get; set; } // date of reading the certificate
    public string stamp { get; set; } // date of signing the certificate
    public string bank { get; set; } // / name of the bank that issued the certificate / ONLY USED BY LUNA

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