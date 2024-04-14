using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net;
class Blockchain
{
    private List<block> chain = new List<block>();
    private List<transaction> transactions = new List<transaction>();
    public List<node> nodes = new List<node>();
    private block lastBlock => chain.Last();
    private coin privateCoin = new coin();
    public string nodeID { get; set; }
    private int difficulty = 1;
    public Blockchain()
    {
        var certificated = new
        {
            title = "luna",
            read = "this-coin-is-not-false-and-you-can-(gsx)-close",
            stamp = "12.04.24",
            bank = "luna.fsf.00.24"
        };
        if (File.Exists("luna.cert"))
        {
            privateCoin.name = "LunaCosmicCoin";
            privateCoin.symbol = "LCC";
            privateCoin.count = 1;
            privateCoin.certificate = JsonConvert.DeserializeAnonymousType(File.ReadAllText("luna.cert"), certificated).ToString();
        }
        else
        {
            Console.WriteLine("No certificate found. Create a new one...");
            File.WriteAllText("luna.cert", "{'title' : 'luna','read' : 'this-coin-is-not-false-and-you-can-(gsx)-close','stamp' : '12.04.24','bank' : 'luna.fsf.00.24'}");
        }

        Console.WriteLine("Blockchain class created");
        if (nodeID == null)
            this.nodeID = Guid.NewGuid().ToString().Replace("-", "");
        //Console.WriteLine(nodeID);
        if (File.Exists("projectluna.dat") && File.ReadAllText("projectluna.dat") != "")
        {
            string json = File.ReadAllText("projectluna.dat");
            var model = new
            {
                chain = new List<block>(),
                length = 0
            };
            var data = JsonConvert.DeserializeAnonymousType(json, model);
            chain = data.chain;
            var hoster = File.ReadAllLines(@"address.dat")[0].Split(':')[0];
            var porter = File.ReadAllLines(@"address.dat")[0].Split(':')[1];
            Console.WriteLine($"Chain length: {chain.Count}\nLater blocks will be saved in projectluna.dat, and check if they are valid http://" + hoster + ":" + porter + "/nodes/resolve");

        }
        else
        {
            Console.WriteLine("Blockchain not in projectluna.dat and will be created from genesis block");
            createBlock(proof: 100, previousHash: "1"); // genesis block. First block in the chain. Proof of work...
        }

        Console.WriteLine("Your id is: " + nodeID + " your id - wallet, which is connected to the blockchain and node. Save it.");
    }

    public void changeNodeID(string _nodeID) => nodeID = _nodeID;

    internal int createTransaction(string description, string NOV, string sender, string recipient, coin amount)
    {
        transaction newTransaction = new transaction();
        newTransaction.description = description;
        newTransaction.NOV = NOV;
        newTransaction.sender = sender;
        newTransaction.recipient = recipient;
        newTransaction.amount = amount;
        transactions.Add(newTransaction);
        return lastBlock != null ? lastBlock.index + 1 : 0;

    }

    private block createBlock(int proof, string previousHash = null)
    {
        block newBlock = new block();
        newBlock.index = chain.Count;
        newBlock.previousHash = previousHash ?? getHash(chain.Last());
        newBlock.proof = proof;
        newBlock.timestamp = DateTime.Now;
        newBlock.transactions = transactions.ToList();
        newBlock.hash = previousHash ?? getHash(newBlock);
        transactions.Clear();
        chain.Add(newBlock);
        return newBlock;
    }

    private string getHash(block _block)
    {
        string blockText = JsonConvert.SerializeObject(_block);
        return GetSha256(blockText);
    }

    private bool validChain(List<block> chain)
    {
        block blocki = new block();
        block lastBlock = chain.First();
        int currentIndex = 1;
        while (currentIndex < chain.Count)
        {
            blocki = chain.ElementAt(currentIndex);
            Console.WriteLine($"{lastBlock}");
            Console.WriteLine($"{blocki}");
            Console.WriteLine("----------------------------");

            //Check that the hash of the block is correct
            if (blocki.previousHash != getHash(lastBlock))
                return false;

            //Check that the Proof of Work is correct
            if (!validProofOfWork(lastBlock.proof, blocki.proof, lastBlock.previousHash))
                return false;

            lastBlock = blocki;
            currentIndex++;
        }

        return true;
    }

    public string GetSha256(string text)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            var hashBuilder = new StringBuilder();

            byte[] bytes = Encoding.Unicode.GetBytes(text);
            byte[] hash = sha256.ComputeHash(bytes);

            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");

            return hashBuilder.ToString();
        }
    }

    private int createProofOfWork(int lastProof, string previousHash)
    {
        int proof = 0;
        while (!validProofOfWork(lastProof, proof, previousHash))
        {
            proof++;
        }
        return proof;

    }

    private bool validProofOfWork(int lastProof, int proof, string previousHash)
    {
        string guess = $"{lastProof}{proof}{previousHash}{difficulty}";
        string guessHash = GetSha256(guess);
        return guessHash.StartsWith("0000");
    }

    internal string mineBlock()
    {
        if (!Consensus())
        {
            int proof = createProofOfWork(lastBlock.proof, lastBlock.previousHash);
            int blockID = createTransaction(description: "Good Work", NOV: "LCC", sender: "luna", recipient: nodeID, amount: privateCoin);
            block newBlock = createBlock(proof);

            var responsed = new
            {
                message = "Block mined",
                index = newBlock.index,
                transactions = newBlock.transactions.ToArray(),
                proof = newBlock.proof,
                previousHash = newBlock.previousHash,
                hash = newBlock.hash
            };
            Console.WriteLine(responsed);
            Console.WriteLine($"Your transaction will be included in block {blockID}");
            return JsonConvert.SerializeObject(responsed);
        }
        //Console.WriteLine("Check to connect in the internet or check to aviable other nodes");
        return "None";
    }

    internal string getFullChain()
    {
        List<block> copychain = new List<block>();
        copychain = chain.ToList();
        var response = new
        {
            chain = copychain,
            length = copychain.Count
        };

        return JsonConvert.SerializeObject(response);
    }

    internal string getNodes()
    {
        List<node> copynodes = new List<node>();
        copynodes = nodes.ToList();
        var response = new { nodes = copynodes };
        return JsonConvert.SerializeObject(response);
    }
    public void registerNode(string address)
    {
        nodes.Add(new node { uri = new Uri(address) });
    }

    internal string RegisterNodes(string[] nodes)
    {
        var builder = new StringBuilder();
        foreach (string node in nodes)
        {
            string url = $"http://{node}/";
            registerNode(url);
            builder.Append($"{url}, ");
        }

        builder.Insert(0, $"{nodes.Count()} new nodes have been added: ");
        string result = builder.ToString();
        return result.Substring(0, result.Length - 2);
    }

    internal bool Consensus()
    {
        try
        {
            foreach (node node in nodes)
            {
                if (nodes.Count == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("(INFO | WAITING) NODES AMOUNT IN 1 NODE, please add node to nodes list, if this list is empty");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    var url = new Uri(node.uri, "/");
                    var request = (HttpWebRequest)WebRequest.Create(url + "chain");
                    var response = (HttpWebResponse)request.GetResponse();
                    var request2 = (HttpWebRequest)WebRequest.Create(url + "nodes");
                    var response2 = (HttpWebResponse)request2.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (url != new Uri("http://" + File.ReadAllLines("address.dat")[0] + "/"))
                        {
                            var model = new
                            {
                                chain = new List<block>(),
                                length = 0
                            };
                            var nodemodel = new { nodes = new List<node>() };
                            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                            string jsonfornodes = new StreamReader(response2.GetResponseStream()).ReadToEnd();
                            List<block> chain = JsonConvert.DeserializeAnonymousType(json, model).chain;
                            List<node> nodes = JsonConvert.DeserializeAnonymousType(jsonfornodes, nodemodel).nodes;

                            if (chain.Count > this.chain.Count && validChain(chain))
                            {
                                this.chain = chain;
                                using (FileStream stream = new FileStream("projectluna.dat", FileMode.Truncate))
                                {
                                    StreamWriter writer = new StreamWriter(stream);
                                    writer.Write(json);
                                    writer.Close();
                                }
                            }
                            if (nodes.Count > this.nodes.Count)
                            {
                                this.nodes = nodes;
                            }
                            using (FileStream stream = new FileStream("projectluna.dat", FileMode.Truncate))
                            {
                                StreamWriter writer = new StreamWriter(stream);
                                writer.Write(json);
                                writer.Close();
                            }
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error while syncing with node " + url);
                        return false;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //Console.WriteLine("Me work in error");
            //node no = new node { uri = new Uri($"http://{e.InnerException.Message.Split("(")[1].Split(")")[0]}") };
            //Console.WriteLine(no + " " + no.uri);
            //this.nodes.RemoveAt(nodes.FindIndex(x => x.uri == no.uri));
            //Console.WriteLine(this.nodes);
            //Consensus();
            return false;
        }
        return false;
    }

    internal string consensus()
    {
        bool replaced = resolveConflicts();
        string message = replaced ? "was replaced" : "is authoritive";

        var response = new
        {
            Message = $"Our chain {message}",
            Chain = chain
        };

        return JsonConvert.SerializeObject(response);

    }

    private bool resolveConflicts()
    {
        List<block> newChain = null;
        int maxLength = chain.Count;

        foreach (node nodei in nodes)
        {
            if (nodes.Count == 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("(INFO | WAITING) NODES AMOUNT IN 1 NODE, please add node to nodes list, if this list is empty");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                var url = new Uri(nodei.uri, "/chain");
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var model = new
                    {
                        chain = new List<block>(),
                        length = 0
                    };
                    string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var data = JsonConvert.DeserializeAnonymousType(json, model);
                    if (data.chain.Count > chain.Count && validChain(data.chain))
                    {
                        maxLength = data.chain.Count;
                        newChain = data.chain;
                    }
                }
            }
        }

        if (newChain != null)
        {
            chain = newChain;
            Console.WriteLine("This chain is valid");
            return true;
        }
        Console.WriteLine("This chain is invalid");
        return false;
    }

}