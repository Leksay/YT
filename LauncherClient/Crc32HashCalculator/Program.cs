using Mono.Options;
using Shared.Hash;

public static class Program
{
    public static int Main(params string[] args)
    {
        string rootDirectory = string.Empty;
        string savePath = string.Empty;

        var p = new OptionSet
        {
            {"r|root=", "root directory", v => rootDirectory = v},
            {"s|savePath=", "output directory", v => savePath = v}
        };

        p.Parse(args);

        if (string.IsNullOrEmpty(rootDirectory) || !Directory.Exists(rootDirectory))
        {
            Console.WriteLine($"Directory {rootDirectory} doesn't exist");
            return -1;
        }

        if (string.IsNullOrEmpty(savePath))
        {
            Console.WriteLine("Save path is not specified");
            return -1;
        }
        
        try
        {
            new Crc32HashCalculator().CalculateAndSaveHash(rootDirectory, savePath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Can't calculate hash {e.Message}");
            Console.WriteLine(e);
            return -1;
        }

        return 0;
    }
}