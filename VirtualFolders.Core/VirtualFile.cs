using VirtualFolders.Core.Format;

namespace VirtualFolders.Core;

public sealed class VirtualFile
{
    public required string Name { get; set; }
    public required string Extension { get; init; }
    public required string FullPath { get; init; }
    public required DateTime Added { get; init; }
    public required string Notes { get; set; }
    public required bool IsUrl { get; init; }
    public bool IsAvailable { get; init; }
    public FileSize FileSize { get; init; }
    public DateTime LastModified { get; init; }
    public DateTime Created { get; init; }
    public FileAttributes Attributes { get; init; }

    internal static VirtualFile CreateFromUrl(string url, DateTime? added = null)
    {
        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? created))
        {
            if (created.IsFile)
            {
                return CreateFromFilePath(created.LocalPath, added);
            }

            return new VirtualFile
            {
                FullPath = created.AbsoluteUri.ToString(),
                Added = added ?? DateTime.Now,
                IsAvailable = true,
                Extension = "url",
                FileSize = url.CalculateSize(),
                Notes = string.Empty,
                LastModified = added ?? DateTime.Now,
                Created = added ?? DateTime.Now,
                Attributes = FileAttributes.Normal,
                Name = Path.GetFileNameWithoutExtension(url),
                IsUrl = true
            };
        }

        throw new InvalidOperationException($"Invalid URL: {url}");
    }

    
    internal static VirtualFile CreateNotExisting(string filePath, DateTime? added = null)
    {
        return new VirtualFile
        {
            FullPath = filePath,
            Added = added ?? DateTime.MinValue.Date,
            IsAvailable = false,
            Extension = Path.GetExtension(filePath),
            FileSize = 0,
            Notes = string.Empty,
            LastModified = DateTime.MinValue.Date,
            Created = DateTime.MinValue.Date,
            Attributes = FileAttributes.Normal,
            Name = Path.GetFileNameWithoutExtension(filePath),
            IsUrl = false
        };
    }


    internal static VirtualFile CreateFromFilePath(string filePath, DateTime? added = null)
    {
        if (!File.Exists(filePath))
        {
            return CreateNotExisting(filePath, added);
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
            LastModified = fileInfo.LastWriteTime,
            Notes = string.Empty,
            IsUrl = false
        };
    }

    internal static VirtualFile CreateFromLoaded(VirtualFileV1 file)
    {
        VirtualFile created = file.IsUrl 
            ? CreateFromUrl(file.Path, file.Added) 
            : CreateFromFilePath(file.Path, file.Added);

        if (!string.IsNullOrEmpty(file.DisplayName))
        {
            created.Name = file.DisplayName;
        }

        created.Notes = file.Notes;
        return created;
    }
}