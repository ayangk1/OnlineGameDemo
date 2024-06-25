using GameSever.Controller;
using GameSever.Server;


public class Program
{
    static void Main(string[] args)
    {
        // string ip = "172.20.10.6";
        string ip = "192.168.0.100";
        // string ip = "10.6.0.8";
        Server server = new Server(ip, 6677);
        Console.ReadKey();
    }

}


