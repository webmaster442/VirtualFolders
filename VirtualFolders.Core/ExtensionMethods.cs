using System.Text;

using VirtualFolders.Core.Exporters;

namespace VirtualFolders.Core;

internal static class ExtensionMethods
{
    extension(IHasSizeLimit hasSizeLimit)
    {
        public bool IsWithinSizeLimit(IReadOnlyList<VirtualFile> files, IDialogs dialogs)
        {
            var size = files.Sum(f => f.FileSize.TotalBytes);
            if (size > hasSizeLimit.MaxSizeLimit.TotalBytes)
            {
                dialogs.Error($"The total size of the selected files ({new FileSize(size)}) exceeds the maximum allowed size of {hasSizeLimit.MaxSizeLimit}. Please select fewer files or smaller files.", "Error");
                return false;
            }
            return true;
        }
    }

    extension(VirtualFile virtualFile)
    {
        public async Task WriteAsUrlFileAsync(Stream target, CancellationToken cancellationToken)
        {
            Encoding encoding = new UTF8Encoding(false); // UTF-8 without BOM
            using StreamWriter writer = new(target, encoding, leaveOpen: true);
            await writer.WriteLineAsync($"""
            [InternetShortcut]
            URL={virtualFile.FullPath}
            Comment={virtualFile.Notes}
            """);
        }

        public byte[] WriteAsBytes()
        {
            string content= $"""
            [InternetShortcut]
            URL={virtualFile.FullPath}
            Comment={virtualFile.Notes}
            """;
            var encoding = new UTF8Encoding(false); // UTF-8 without BOM
            return encoding.GetBytes(content);
        }

        public string NameWithExtension(string? overrideExtension = null)
        {
            return !string.IsNullOrEmpty(overrideExtension)
                ? $"{virtualFile.Name}.{overrideExtension}"
                : $"{virtualFile.Name}.{virtualFile.Extension}";
        }
    }

    extension (string str)
    {
        public FileSize CalculateSize()
        {
            long result = 0;
            for (int i=0; i < str.Length; i++)
            {
                if (char.IsHighSurrogate(str[i]))
                {
                    result += 4; // Surrogate pair takes 4 bytes in UTF-8
                    i++; // Skip the low surrogate
                }
                else
                {
                    result += str[i] <= 0x7F ? 1 : str[i] <= 0x7FF ? 2 : 3; // UTF-8 encoding size
                }
            }
            return new FileSize(result);
        }
    }
}
