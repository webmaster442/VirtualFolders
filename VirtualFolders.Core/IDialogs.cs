using System.Diagnostics.CodeAnalysis;

namespace VirtualFolders.Core;

public interface IDialogs
{
    bool TrySaveFile(string title, string filter, [NotNullWhen(true)] out string? file);
    bool TryOpenFiles(string title, string filter, [NotNullWhen(true)] out IEnumerable<string>? files);
    bool TryOpenFile(string title, string filter, [NotNullWhen(true)] out string? file);
    bool TryOpenFolder(string title, [NotNullWhen(true)] out string? folder);
    bool TryEditText(string title, string initialText, int maxChars, [NotNullWhen(true)] out string? result);
    void Error(string message, string title);
    bool Confirm(string message, string title);
    bool YesNo(string message, string title);
    void ShellPropertiesDialog(string file);
    IFileOperationProgress ShowFileOperationProgress(string title);
}
