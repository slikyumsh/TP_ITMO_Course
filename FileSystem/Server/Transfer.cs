using System.Buffers;
using System.Text;
using Node;
using Server.Entities;

namespace Server;

public class Transfer
{
    private Client _client;
    private readonly string _ipAddress;

    public Transfer(Client client, string ipAddress)
    {
        _client = client;
        _ipAddress = ipAddress;
    }

    public void ChangeNode(int port)
    {
        _client = new Client(_ipAddress, port);
    }

    public void CreateNode(string nodeName)
    {
        _client.Connect();
        CreateNodeByte(nodeName);
    }

    public void CreateFile(string allPath, string nodeName)
    {
        _client.Connect();
        SendFileByte(allPath, Path.Combine(nodeName, Path.GetFileName(allPath)));
    }

    public void CreateFile(byte[] dataFile, string partialPath)
    {
        _client.Connect();
        SendFileByte(dataFile, partialPath);
    }

    public byte[] GetDataFile(string nodeName, string fileName)
    {
        _client.Connect();
        GetFileByte(Path.Combine(nodeName, fileName));
        
        var lengthData = new byte[sizeof(int)];
        _client.NetworkStream.Read(lengthData, 0, sizeof(int));
        
        var length = BitConverter.ToInt32(lengthData, 0);
        byte[] fileBytes = ArrayPool<byte>.Shared.Rent(length);
        _client.NetworkStream.Read(fileBytes, 0, length);
        return fileBytes;
    }

    public void RemoveFile(string partialPath)
    {
        _client.Connect();
        DeleteFileByte(partialPath);
    }
    
    private void CreateNodeByte(string nodeName)
    {
        byte[] serverOptionBytes = BitConverter.GetBytes((int)Actions.AddNode);
        byte[] lengthNameBytes = BitConverter.GetBytes(nodeName.Length);
        byte[] directoryNameBytes = Encoding.ASCII.GetBytes(nodeName);

        _client.NetworkStream.Write(serverOptionBytes, 0, sizeof(int));
        _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
        _client.NetworkStream.Write(directoryNameBytes, 0, nodeName.Length);
    }

    private void SendFileByte(string path, string partialPath)
    {
        byte[] serverOption = BitConverter.GetBytes((int)Actions.AddFile);
        byte[] lengthNameBytes = BitConverter.GetBytes(partialPath.Length);
        byte[] fileNameBytes = Encoding.ASCII.GetBytes(partialPath);
        int size = (int) new FileInfo(path).Length;
        byte[] dataBytes = ArrayPool<byte>.Shared.Rent(size);
        Array.Copy(File.ReadAllBytes(path), dataBytes, size);
        byte[] dataLengthBytes = BitConverter.GetBytes(dataBytes.Length);
        
        _client.NetworkStream.Write(serverOption, 0, sizeof(int));
        _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
        _client.NetworkStream.Write(fileNameBytes, 0, fileNameBytes.Length);
        _client.NetworkStream.Write(dataLengthBytes, 0, dataLengthBytes.Length);
        _client.NetworkStream.Write(dataBytes, 0, dataBytes.Length);
        
        ArrayPool<byte>.Shared.Return(dataBytes);
    }
    
    private void SendFileByte(byte[] fileData, string partialPath)
    {
        
        byte[] serverOption = BitConverter.GetBytes((int)Actions.AddFile);
        byte[] lengthNameBytes = BitConverter.GetBytes(partialPath.Length);
        byte[] fileNameBytes = Encoding.ASCII.GetBytes(partialPath);
        byte[] dataBytes = ArrayPool<byte>.Shared.Rent(fileData.Length);
        Array.Copy(fileData, dataBytes, fileData.Length);
        byte[] dataLengthBytes = BitConverter.GetBytes(dataBytes.Length);

        _client.NetworkStream.Write(serverOption, 0, sizeof(int));
        _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
        _client.NetworkStream.Write(fileNameBytes, 0, fileNameBytes.Length);
        _client.NetworkStream.Write(dataLengthBytes, 0, dataLengthBytes.Length);
        _client.NetworkStream.Write(dataBytes, 0, dataBytes.Length);
        
        ArrayPool<byte>.Shared.Return(dataBytes);
    }

    private void DeleteFileByte(string fileName)
    {
        byte[] serverOption = BitConverter.GetBytes((int)Actions.RemoveFile);
        byte[] lengthNameBytes = BitConverter.GetBytes(fileName.Length);
        byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileName);

        _client.NetworkStream.Write(serverOption, 0, sizeof(int));
        _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
        _client.NetworkStream.Write(fileNameBytes, 0, fileNameBytes.Length);
    }
    
    private void GetFileByte(string partialPath)
    {
        byte[] serverOption = BitConverter.GetBytes((int)Actions.GetFileByte);
        byte[] lengthNameBytes = BitConverter.GetBytes(partialPath.Length);
        byte[] partialPathBytes = Encoding.ASCII.GetBytes(partialPath);

        _client.NetworkStream.Write(serverOption, 0, sizeof(int));
        _client.NetworkStream.Write(lengthNameBytes, 0, lengthNameBytes.Length);
        _client.NetworkStream.Write(partialPathBytes, 0, partialPathBytes.Length);
    }
}
