using Newtonsoft.Json;

class lcc
{
    private Blockchain luna; // get luna blockchain
    public lcc(Blockchain luna) => this.luna = luna; // stupid 

    // get balance of account if was transaction with sender or receiver || this fast method, than in python. lol. it's slow. (logically)
    public float getFastBalance(string id)
    {
        var balance = 0;
        var chain = luna.getFullChain();
        var model = new
        {
            chain = new List<block>(),
            length = 0
        };
        var trans = JsonConvert.DeserializeAnonymousType(chain, model).chain;
        for (int j = 0; j < trans.Count; j++)
            for (int i = 0; i < trans[j].transactions.Count; i++){
                if (trans[j].transactions[i].sender == id)
                    balance -= trans[j].transactions[i].amount.count;
                if (trans[j].transactions[i].recipient == id)
                    balance += trans[j].transactions[i].amount.count;
            }

        Console.WriteLine(balance);
        return balance;
    }

    // transaction info from id, if was transaction with sender or receiver || this fast method, than in python. lol. it's slow.  (logically)
    public string getInfoTransaction(string id){
        var chain = luna.getFullChain();
        transaction bblock = new transaction();
        var model = new
        {
            chain = new List<block>(),
            length = 0
        };
        var trans = JsonConvert.DeserializeAnonymousType(chain, model).chain;
        for (int j = 0; j < trans.Count; j++)
            for (int i = 0; i < trans[j].transactions.Count; i++)
                if (trans[j].transactions[i].sender == id || trans[j].transactions[i].recipient == id)
                    bblock = trans[j].transactions[i];

        Console.WriteLine(bblock);
        return JsonConvert.SerializeObject(bblock);
    }
}