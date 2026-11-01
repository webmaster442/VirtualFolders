using System.Text;

namespace VirtualFolders.Core;

public static class ExtensionMethods
{
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
