using Newtonsoft.Json;

public class transaction
{
    public coin amount {get;set;}
    public string NOV { get; set; }
    public string description { get; set; } // description of transaction
    public string sender { get; set; } // sender of transaction (user id)
    public string recipient { get; set; } // recipient of transaction (user id)

    public override string ToString() {
        var response = new{
            amount = amount,
            NOV = NOV,
            description = description,
            sender = sender,
            recipient = recipient
        }; 
        return JsonConvert.SerializeObject(response);
    }

}
