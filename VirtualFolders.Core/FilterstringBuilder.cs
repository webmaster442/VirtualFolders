using System.Text;

namespace VirtualFolders.Core;

internal sealed class FilterstringBuilder
{
    private readonly StringBuilder _builder;

    public FilterstringBuilder()
    {
        _builder = new StringBuilder();
    }

    private void Add(string description, params string[] extensons)
    {
        if (_builder.Length > 0)
            _builder.Append('|');

        _builder.Append(description);
        _builder.Append('|');
        _builder.Append(string.Join(';', extensons));
    }

    public FilterstringBuilder Clear()
    {
        _builder.Clear();
        return this;
    }

    public FilterstringBuilder WithStarFiles()
    {
        Add("All Files", "*.*");
        return this;
    }

    public FilterstringBuilder WithImageFiles()
    {
        Add("Image Files", "*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp", "*.tiff", "*.webp", "*.heic", "*.png", "*.svg", "*.psd", "*.ico");
        return this;
    }

    public FilterstringBuilder WithTextFiles()
    {
        Add("Text Files", "*.txt", "*.md", "*.csv", "*.log", "*.xml", "*.json", "*.yaml", "*.yml");
        return this;
    }

    public FilterstringBuilder WithDocumentFiles()
    {
        Add("Document Files", "*.pdf", "*.doc", "*.docx", "*.xls", "*.xlsx", "*.ppt", "*.pptx", "*.odt", "*.ods", "*.odp");
        return this;
    }

    public FilterstringBuilder WithAudioFiles()
    {
        Add("Audio Files", "*.mp3", "*.wav", "*.flac", "*.aac", "*.ogg", "*.wma", "*.m4a", "*.aiff", "*.alac", "*.pcm");
        return this;
    }

    public FilterstringBuilder WithVideoFiles()
    {
        Add("Video Files", "*.mp4", "*.avi", "*.mkv", "*.mov", "*.wmv", "*.flv", "*.webm", "*.mpeg", "*.mpg", "*.3gp");
        return this;
    }

    public FilterstringBuilder WithCompressedFiles()
    {
        Add("Compressed Files", "*.zip", "*.rar", "*.7z", "*.tar", "*.gz", "*.bz2", "*.xz");
        return this;
    }

    public FilterstringBuilder WithDiskImages()
    {
        Add("Disk Images", "*.iso", "*.img", "*.dmg", "*.vhd", "*.vhdx", "*.vmdk", "*.bin", "*.cue", "*.nrg", "*.mdf", "*.ccd");
        return this;
    }

    public FilterstringBuilder WithVirtualFolders()
    {
        Add("Virtual Folders", "*.vfolder");
        return this;
    }

    public FilterstringBuilder WithAllFiles()
    {
        WithStarFiles();
        WithImageFiles();
        WithTextFiles();
        WithDocumentFiles();
        WithAudioFiles();
        WithVideoFiles();
        WithCompressedFiles();
        WithDiskImages();
        return this;
    }

    public string Build()
        => _builder.ToString();
}
