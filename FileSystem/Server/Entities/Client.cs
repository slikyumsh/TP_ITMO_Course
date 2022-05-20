using System.Net.Sockets;

namespace Server.Entities;

public record Client(string Address, int Port)
{
    private string Address { get; } = Address;
    private int Port { get; } = Port;
    private TcpClient _client;

    public NetworkStream? NetworkStream { get; private set; }

    public void Connect()
    {
        _client = new TcpClient(Address, Port);
        NetworkStream = _client.GetStream();
    }
}
