namespace Server;

public static class Program
{
    public static void Main()
    {
        string ipAddress = "localhost";
        Commands commands = new Commands(ipAddress);
        commands.ExecuteFile(@"C:\Users\dellx\Desktop\FileSystemD\FileSystemD\FileNodes\commands_100.txt");
        
        /*
        while (true)
        {
            Console.WriteLine("enter command: ");
            var line = Console.ReadLine();
            commands.ParseCommand(line);
        }
    */
    }
} 