public class block
{
    public int index { get; set; }
    public int gen { get; set; }
    public string previousHash { get; set; } // hash of the previous block
    public DateTime timestamp { get; set; } // timestamp of the block
    public string hash { get; set; } // hash of the block
    public int proof { get; set; } // proof of work of the block
    public List<transaction> transactions { get; set; } // list of transactions in the block, each transaction is a dictionary with the following keys: {sender, receiver, amount, timestamp}

    public override string ToString()
    {
        return "{" + $"'index' : '{index}', 'timestamp' : '{timestamp.ToString("yyyy-MM-dd HH:mm:ss")}', 'proof' : '{proof}', 'hash' : '{hash}', 'previus_hash': '{previousHash}', 'transactions': '{transactions.Count}'"  + "}";
    }
    
}

// бляяя бляя гигакод такой классный, я хотел написать коммент кода, а он сам все написал.. Круто, не правда ли?) (этот  комент тоже он на половину написал)