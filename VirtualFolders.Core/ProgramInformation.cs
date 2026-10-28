namespace VirtualFolders.Core;

public sealed record ProgramInformation
{
    public required string ProgramPath { get; init; }
    public required string DisplayName { get; init; }
    public required bool IsAutoCreated { get; init; }
}
