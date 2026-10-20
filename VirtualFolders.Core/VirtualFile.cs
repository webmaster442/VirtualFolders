namespace VirtualFolders.Core;

public sealed class VirtualFile
{
    public required string Name { get; init; }
    public required string Extension { get; init; }
    public required string FullPath { get; init; }
    public required DateTime Added { get; init; }
    public bool IsAvailable { get; init; }
    public FileSize FileSize { get; init; }
    public DateTime LastModified { get; init; }
    public DateTime Created { get; init; }
    public FileAttributes Attributes { get; init; }

    internal static VirtualFile CreateFromFilePath(string filePath, DateTime? added = null)
    {
        if (!File.Exists(filePath))
        {
            return new VirtualFile
            {
                FullPath = filePath,
                Added = added ?? DateTime.MinValue.Date,
                IsAvailable = false,
                Extension = Path.GetExtension(filePath),
                FileSize = 0,
                LastModified = DateTime.MinValue.Date,
                Created = DateTime.MinValue.Date,
                Attributes = FileAttributes.Normal,
                Name = Path.GetFileNameWithoutExtension(filePath),
            };
        }

        FileInfo fileInfo = new(filePath);

        return new VirtualFile
        {
            FullPath = fileInfo.FullName,
            Name = Path.GetFileNameWithoutExtension(fileInfo.Name),
            Extension = fileInfo.Extension,
            Added = added ?? DateTime.Now,
            Attributes = fileInfo.Attributes,
            Created = fileInfo.CreationTime,
            FileSize = fileInfo.Length,
            IsAvailable = true,
            LastModified = fileInfo.LastWriteTime
        };
    }
}