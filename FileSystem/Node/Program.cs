namespace Node;

static class Program
{
    private static void Main()
    {
        var ipAddress = "127.0.0.1";
        var directoryPath = @"C:\Users\dellx\Desktop\FileSystemD\FileSystemD\Nodes";
        
        Console.WriteLine("enter port:");
        var port = int.Parse(Console.ReadLine());
        var node = new Node(ipAddress, port, directoryPath);
        
        Console.WriteLine("start node");
        
        while (true)
        {
            node.Start();
        }
    }
}