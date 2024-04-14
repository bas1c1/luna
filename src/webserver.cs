using System.Net;
using System.Configuration;
using Newtonsoft.Json;

class Server
{
    public Server(Blockchain chain, string hoster, string porter)
    {
        var settings = ConfigurationManager.AppSettings;
        string host = hoster;
        string port = porter;

        var server = new TinyWebServer.WebServer(request =>
                {
                    string path = request.Url.PathAndQuery.ToLower();
                    string query = "";
                    string json = "";
                    if (path.Contains("?"))
                    {
                        string[] parts = path.Split('?');
                        path = parts[0];
                        query = parts[1];
                    }

                    switch (path)
                    {
                        case "/styles/style.css":
                            return File.ReadAllText("src/html/styles/style.css");
                        case "/styles/style2.css":
                            return File.ReadAllText("src/html/styles/style2.css");
                        case "/assets/bootstrap/css/bootstrap.min.css":
                            return File.ReadAllText("src/html/assets/bootstrap/css/bootstrap.min.css");
                        case "assets/bootstrap/js/bootstrap.bundle.js":
                            return File.ReadAllText("src/html/assets/bootstrap/js/bootstrap.bundle.js");

                        case "/":
                            return File.ReadAllText("src/html/index.html");
                        case "/help":
                            return File.ReadAllText("src/html/help.html");
                        case "/api":
                            return File.ReadAllText("src/html/api.html");
                        case "/mine":
                            if (chain.mineBlock() != "None")
                                for (; ; )
                                    chain.mineBlock();
                            return chain.mineBlock();

                        case "/transactions/new":
                            if (request.HttpMethod != HttpMethod.Post.Method)
                            {
                                Console.WriteLine($"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}");
                                return $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";
                            }
                            json = new StreamReader(request.InputStream).ReadToEnd();
                            transaction trx = JsonConvert.DeserializeObject<transaction>(json);
                            int blockId = chain.createTransaction(trx.description, trx.NOV, trx.sender, trx.recipient, trx.amount);

                            return $"Your transaction will be included in block {blockId}";

                        case "/chain":
                            return chain.getFullChain();

                        case "/nodes/register":
                            if (request.HttpMethod != HttpMethod.Post.Method)
                                return $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";

                            json = new StreamReader(request.InputStream).ReadToEnd();
                            var urlList = new { Urls = new string[0] };
                            var obj = JsonConvert.DeserializeAnonymousType(json, urlList);
                            return chain.RegisterNodes(obj.Urls);

                        case "/nodes":
                            return chain.getNodes();

                        case "/nodes/resolve":
                            return chain.consensus();

                        case $"/balance":
                            if (request.HttpMethod != HttpMethod.Post.Method)
                            {
                                Console.WriteLine($"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}");
                                return $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";
                            }
                            json = new StreamReader(request.InputStream).ReadToEnd();
                            var modely = new { user = "" };
                            var t = JsonConvert.DeserializeAnonymousType(json, modely);
                            return new lcc(chain).getFastBalance(t.user).ToString();

                        case "/transactions/info":
                            if (request.HttpMethod != HttpMethod.Post.Method)
                            {
                                Console.WriteLine($"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}");
                                return $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";
                            }
                            json = new StreamReader(request.InputStream).ReadToEnd();
                            var modelb = new { user = "" };
                            var a = JsonConvert.DeserializeAnonymousType(json, modelb);
                            return new lcc(chain).getInfoTransaction(a.user);
                    }

                    return "";
                },
                $"http://{host}:{port}/",
                $"http://{host}:{port}/help/",
                $"http://{host}:{port}/api/",
                $"http://{host}:{port}/mine/",
                $"http://{host}:{port}/transactions/new/",
                $"http://{host}:{port}/chain/",
                $"http://{host}:{port}/nodes/register/",
                $"http://{host}:{port}/nodes/resolve/",
                $"http://{host}:{port}/nodes/sync/",
                $"http://{host}:{port}/nodes/",
                $"http://{host}:{port}/balance/",
                $"http://{host}:{port}/transactions/info/"

            );

        server.Run();
        Console.WriteLine("Server Started: " + "http://" + host + ":" + port);
    }

}