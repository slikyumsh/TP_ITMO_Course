using System.Buffers;
using Server.Entities;

namespace Server;

public class Commands : ICommands
{
    private readonly List<NodeInformation> _nodes;
    private readonly Transfer _transfer;
    
    public Commands(string ipAddress)
    {
        _nodes = new List<NodeInformation>();
        var client = new Client(ipAddress, 1111);
        _transfer = new Transfer(client, ipAddress);
    }

    public void AddNode(string nodeName, int port, long allSize)
    { 
        var node = new NodeInformation(nodeName, port, allSize);
        _nodes.Add(node);
        ChangeNode(node.Name);
        _transfer.CreateNode(nodeName);
    }

    public void AddFile(string allPath, string partialPath)
    {
        var nodeName = Path.GetDirectoryName(partialPath);
        var nodeInfo = FindNode(nodeName);
        var fileObject = new FileInformation(Path.GetFileName(partialPath), new FileInfo(allPath).Length);
        if (nodeInfo.AddFile(fileObject))
        {
            ChangeNode(nodeInfo.Name);
            _transfer.CreateFile(allPath, nodeName);
        }
    }

    public void RemoveFile(string partialPath)
    {
        var nodeName = Path.GetDirectoryName(partialPath);
        var nodeInfo = FindNode(nodeName);
        var fileObject = nodeInfo.FindFileInformation(Path.GetFileName(partialPath));
        if (!nodeInfo.RemoveFile(fileObject)) return;
        
        ChangeNode(nodeInfo.Name);
        _transfer.RemoveFile(partialPath);
    }

    public void ExecuteFile(string allPath)
    {
        string line;
        using (var f = new StreamReader(allPath))
        {
            while ((line = f.ReadLine()) != null)
            {
                ParseCommand(line);
            }
        }
        //  f.Close();
    }

    public void CleanNode(string nodeName)
    {
        var cleanNode = FindNode(nodeName);
        foreach (var node in _nodes)
        {
            if (node.Name == nodeName) continue;
            while (cleanNode.FileObjects.Count > 0 && node.CanPutFile(cleanNode.FileObjects[0]))
            {
                MoveFile(cleanNode.FileObjects[0].FileName, cleanNode.Name, node.Name);
            }
        }
    }

    public void BalanceNode()
    {
        foreach (var currentNode in _nodes)
        foreach (var node in _nodes)
        {
            if (currentNode.Name == node.Name) continue;
            for (var i = 0; i < currentNode.FileObjects.Count; ++i)
            {
                var fileObject = currentNode.FileObjects[i];
                if (!node.CanPutFile(fileObject) || node.CurrentSize + fileObject.FileSize >= currentNode.CurrentSize)
                    continue;
        
                MoveFile(fileObject.FileName, currentNode.Name, node.Name);
            }
        }
    }

    private void ParseCommand(string line)
    {
        var args = line.Split(' ').ToList();
        string command = args[0];
        
        switch (command)
        {
            case "add-node":
                string name = args[1];
                var port = int.Parse(args[2]);
                long size = long.Parse(args[3]);
                Console.WriteLine($"add-node {name} {port} {size}");
                AddNode(name, port, size);
                break;
            
            case "add-file":
                var allPath = args[1];
                var partialPath = args[2];
                Console.WriteLine($"add-file {allPath} {partialPath}");
                AddFile(allPath, partialPath);
                break;
            
            case "remove-file":
                partialPath = args[1];
                RemoveFile(partialPath);
                break;
            
            case "execute":
                var path = args[1];
                ExecuteFile(path);
                break;
                
            case "balance-node":
                BalanceNode();
                break;
            
            case "clean-node":
                name = args[1];
                CleanNode(name);
                break;
        }
    }
    
    private void MoveFile(string fileName, string oldNodeName, string newNodeName)
    {
        var oldNode = FindNode(oldNodeName);
        var newNode = FindNode(newNodeName);

        var fileObject = oldNode.FindFileInformation(fileName);
        if (fileObject is not null)
        {
            ChangeNode(oldNode.Name);
            var dataFile = _transfer.GetDataFile(oldNode.Name, fileObject.FileName);
            _transfer.RemoveFile(Path.Combine(oldNode.Name, fileObject.FileName));
            
            ChangeNode(newNode.Name);
            _transfer.CreateFile(dataFile, Path.Combine(newNode.Name, fileObject.FileName));

            newNode.AddFile(fileObject);
            oldNode.RemoveFile(fileObject); 
            ArrayPool<byte>.Shared.Return(dataFile);
        }
    }

    private NodeInformation FindNode(string name)
    {
        return _nodes.FirstOrDefault(n => n.Name.Equals(name));
    }

    private void ChangeNode(string nodeName)
    {
        var nodeInfo = FindNode(nodeName);
        _transfer.ChangeNode(nodeInfo.Port);
    }
}
