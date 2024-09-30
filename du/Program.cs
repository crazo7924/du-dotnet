if (args.Length == 1 || args.Length == 2)
{
    try
    {
        if ((File.GetAttributes(args[0]) & FileAttributes.Directory) == FileAttributes.Directory)
        {
            uint? depth = null;
            if (args.Length == 2) depth = uint.Parse(args[1]);
            decimal size = GetDirectorySize(args[0], depth);
            Console.WriteLine($"In bytes: {size}");
            Console.WriteLine($"In human readable units: {GetFileSizeHumanReadable((long)size)}");
        }
        else
        {
            decimal size = GetFileSize(new FileInfo(args[0]).Length);
            Console.WriteLine($"In bytes: {size}");
            Console.WriteLine($"In human readable units: {GetFileSizeHumanReadable((long)size)}");
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e);
    }
}
else
{
    Console.Error.WriteLine("Arguments: <path of file or directory> [optional unsigned <recurse depth>]");
}

static decimal GetFileSize(long length, SizeUnit size = SizeUnit.B)
{
    return size switch
    {
        SizeUnit.B => length,
        SizeUnit.KB => length / 1000M,
        SizeUnit.MB => length / (1000 * 1000M),
        SizeUnit.GB => length / (1000 * 1000 * 1000M),

        _ => throw new NotImplementedException(),
    };
}

static string GetFileSizeHumanReadable(long length)
{
    return length switch
    {
        < 1000 => GetFileSize(length, SizeUnit.B) + " B",
        < 1000 * 1000 => Math.Round(GetFileSize(length, SizeUnit.KB), 2) + " KB",
        < 1000 * 1000 * 1000 => Math.Round(GetFileSize(length, SizeUnit.MB), 2) + " MB",
        _ => Math.Round(GetFileSize(length, SizeUnit.GB), 2) + " GB",
    };
}

static decimal GetDirectorySize(string directory, uint? depth = null)
{
    decimal size = 0M;

    foreach (var file in Directory.GetFiles(directory))
    {
        if (Directory.Exists(file))
        {
            size += GetDirectorySize(file, depth);
        }
        else if (File.Exists(file))
        {
            size += GetFileSize(new FileInfo(file).Length);
        }
    }
    if (depth != null)
    {
        if (depth == 0) return size;
        else depth--;
    }
    foreach (var file in Directory.GetDirectories(directory))
    {
        if (Directory.Exists(file))
        {
            size += GetDirectorySize(file, depth);
        }
        else if (File.Exists(file))
        {
            size += GetFileSize(new FileInfo(file).Length);
        }
    }

    return size;
}

enum SizeUnit
{
    B,
    KB,
    MB,
    GB,
}