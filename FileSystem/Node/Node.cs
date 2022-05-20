using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Node;

public class Node
{
    private readonly TcpListener _listener;
    private readonly string _directoryPath;
    private TcpClient? _client;
    private NetworkStream? _networkStream;

    public Node(string ipAddress, int port, string directoryPath)
    {
        _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        _client = null;
        _directoryPath = directoryPath;
        Directory.CreateDirectory(_directoryPath);
        _listener.Start();
    }
    
    public void Start()
    {
        _client = _listener.AcceptTcpClient();
        _networkStream = _client.GetStream();
        ChooseAction();
        _networkStream.Close();
        _client.Close();
    }

    private void ChooseAction()
    {
        var actionBytes = new byte[sizeof(Actions)];
        do
        {
            _networkStream.Read(actionBytes, 0, sizeof(Actions));
        } while (_networkStream.DataAvailable);
        
        var action = BitConverter.ToInt32(actionBytes, 0);
        if (!Enum.IsDefined(typeof(Actions), action)) return;
        
        switch ((Actions) action)
        {
            
            case Actions.AddNode:
                Console.WriteLine("add node");
                CreateNode();
                break;
            
            case Actions.AddFile:
                Console.WriteLine("add file");
                AddFile();
                break;

            case Actions.RemoveFile:
                RemoveFile();
                break;
            
            case Actions.GetFileByte:
                GetFileBytes();
                break;
            
            default:
                return;
        }
    }

    private string GetData()
    {
        long length = GetLength();
        var dataBytes = string.Empty;
        var buffer = new byte[length];
        do
        {
            _networkStream.Read(buffer, 0, buffer.Length);
        } while (_networkStream.DataAvailable);

        dataBytes += Encoding.ASCII.GetString(buffer, 0, buffer.Length);
        return dataBytes;
    }
        
    private int GetLength()
    {
        var length = new byte[sizeof(int)];
        do
        {
            _networkStream.Read(length, 0, sizeof(int));
        } while (_networkStream.DataAvailable);

        return BitConverter.ToInt32(length, 0);
    }
    
    private void AddFile()
    {
        var fileName = GetData();
        var length = GetLength();

        var buffer = new byte[length];

        do
        {
            _networkStream.Read(buffer, 0, length);
        } while (_networkStream.DataAvailable);
        File.WriteAllBytes(Path.Combine(_directoryPath, fileName), buffer);
    }

    private void CreateNode()
    {
        var nodeName = GetData();
        Directory.CreateDirectory(Path.Combine(_directoryPath, nodeName));
    }

    private void RemoveFile()
    {
        var fileName = GetData();
        File.Delete(Path.Combine(_directoryPath, fileName));
    }
    
    private void GetFileBytes()
    {
        var partialPath = GetData();
        var allPath = Path.Combine(_directoryPath, partialPath);

        int size = (int) new FileInfo(allPath).Length;
        byte[] dataBytes = ArrayPool<byte>.Shared.Rent(size);
        Array.Copy(File.ReadAllBytes(allPath), dataBytes, size);
        
        byte[] dataLengthBytes = BitConverter.GetBytes(dataBytes.Length);

        _networkStream?.Write(dataLengthBytes, 0, dataLengthBytes.Length);
        _networkStream?.Write(dataBytes, 0,dataLengthBytes.Length);
        
        ArrayPool<byte>.Shared.Return(dataBytes);
    }
}