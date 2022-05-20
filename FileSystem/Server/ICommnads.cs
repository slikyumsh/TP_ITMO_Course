namespace Server;

public interface ICommands
{
    void AddNode(string nodeName, int port, long allSize);
    void AddFile(string allPath, string partialPath);
    void RemoveFile(string partialPath);
    void ExecuteFile(string allPath);
    void CleanNode(string nodeName);
    void BalanceNode();
}
