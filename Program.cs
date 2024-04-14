Console.WriteLine("Hello ProjectLuna!\nProjectLuna #lunacc - BlockChain for Luna Cosimic Coin.\nThis is program for Mining of Luna Cosimic Coin.\nAll informaiton about this program can be found in readme.md file and on site projectluna.ru");
//hot vars (wars)
var static_address = @"address.dat";
var hoster = "";
var porter = "";
// check if file exist
if (!File.Exists(@"projectluna.dat"))
{
    Console.WriteLine("File projectluna.dat not found. Creating...");
    File.Create(@"projectluna.dat").Close();
    Console.WriteLine("File projectluna.dat created.");
}
else
{
    Console.WriteLine("File projectluna.dat found.");
}
// check if static address exist (address.dat)
if (!File.Exists(static_address))
{
    try
    {
        Console.WriteLine("Write your address with port. Example: 127.0.0.1:3000");
        Console.WriteLine("Enter address:"); var address = Console.ReadLine();
        hoster = address.Split(':')[0];
        porter = address.Split(':')[1];
        File.WriteAllLines(static_address, new string[] { address.Split(":")[0] + ":" + address.Split(":")[1] });
    }
    catch
    {
        Console.WriteLine("File address.dat not found. Creating...");
        File.WriteAllLines(static_address, new string[] { "127.0.0.1:3000" });
        Console.WriteLine("File address.dat created. Content: 127.0.0.1:3000");
    }
}
else
{
    hoster = File.ReadAllLines(static_address)[0].Split(':')[0];
    porter = File.ReadAllLines(static_address)[0].Split(':')[1];
    Console.WriteLine("File address.dat found. Content: " + hoster + ":" + porter);
}

// Please, write your ID, if you don't know it. Program will generate one.
Console.WriteLine("Write your ID, if your ID is empty, program will generate one.\nID (Not write if you don't know): "); var id = Console.ReadLine();
var blockchain = new Blockchain();
if (id != "")
    blockchain.changeNodeID(id);
//start server
var server = new Server(blockchain, hoster, porter);
blockchain.registerNode(@"http://" + hoster + ":" + porter + "/");
Console.WriteLine("This node registered: http://" + hoster + ":" + porter);
//info about: CLOSE THIS PROGRAM
Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("Press Ctrl+C for close");
Console.ForegroundColor = ConsoleColor.White;

// 1 - close this program
AppDomain app = AppDomain.CurrentDomain;
app.ProcessExit += delegate
{
    node nodei = new node();
    nodei.uri = new Uri(@"http://" + hoster + ":" + porter + "/");
    blockchain.nodes.Remove(nodei);
    File.WriteAllText(@"projectluna.dat", blockchain.getFullChain());
};
// 2 - close this program
Console.CancelKeyPress += delegate
{
    node nodei = new node();
    nodei.uri = new Uri(@"http://" + hoster + ":" + porter);
    blockchain.nodes.Remove(nodei);
    File.WriteAllText(@"projectluna.dat", blockchain.getFullChain());
};
// 3 while loop for DONT CLOSE THIS PROGRAM, if you want to close this program, please type "Ctrl+C" in console
for (; ; ) { }
// what is this????
Console.Read(); // >_<