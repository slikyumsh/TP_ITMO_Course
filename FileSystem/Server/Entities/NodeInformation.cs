namespace Server.Entities;

public record NodeInformation(string Name, int Port, long _allSize)
{
    private readonly long _allSize = _allSize;
    public long CurrentSize { get; private set; } = 0;
    public List<FileInformation> FileObjects { get; } = new();

    public readonly int Port = Port;
    public readonly string Name = Name;

    public bool AddFile(FileInformation fileObject)
    {
        if (!CanPutFile(fileObject)) return false;
        CurrentSize += fileObject.FileSize;
        FileObjects.Add(fileObject);
        return true;
    }
    
    public bool RemoveFile(FileInformation fileInformation)
    {
        if (FileObjects.All(file => file.FileName != fileInformation.FileName)) 
            return false;
        CurrentSize -= fileInformation.FileSize;
        FileObjects.Remove(fileInformation);
        return true;
    }

    public FileInformation FindFileInformation(string fileName) => 
        FileObjects.FirstOrDefault(fileObject => fileName == fileObject.FileName);

    public bool CanPutFile(FileInformation fileInformation) => 
        CurrentSize + fileInformation.FileSize <= _allSize;
}
