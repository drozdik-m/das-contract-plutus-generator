using System.Xml.Linq;
using DasContract.Abstraction;
using DasContract.Blockchain.Plutus.Console;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels;
using DasContract.Blockchain.Plutus.Generators;

var arguments = new ConsoleArguments(args);

//Help
if (args.Length == 0 || arguments.FlagExists("--help"))
{
    Console.WriteLine("--help               for help");
    Console.WriteLine("--input path         for loading and translating a DasContract file");
    Console.WriteLine("--output path        for exporting translated Plutus contract into a file");
    Console.WriteLine("--output-console     for exporting translated Plutus contract into the standard output");
    Console.WriteLine("--watch              watches for changes in the input file and automatically exports the result");
    Console.WriteLine("--verbose            exceptions and errors are more verbose");
    return;
}

//Do the export
try
{
    Export(GetContract());

    //Watch
    if (arguments.FlagExists("--watch"))
    {
        Watch();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());

    if (arguments.FlagExists("--verbose"))
        Console.WriteLine(ex.StackTrace);
}

/// <summary>
/// Tries to get the plutus contract
/// </summary>
PlutusContract GetContract()
{
    if (arguments is null)
        throw new Exception("Arguments object is null");

    if (!arguments.FlagExists("--input"))
        throw new Exception("No form of DasContract input is set. Please checkout --help for help.");

    if (arguments.TryGetArgumentValue("--input", out string path))
    {
        if (!File.Exists(path))
            throw new Exception($"File does not exist {path}");

        var fileLines = File.ReadAllLines(path);
        var fileContent = string.Join(Environment.NewLine, fileLines);

        var xElement = XElement.Parse(fileContent);
        var contract = new Contract(xElement);

        var plutusContract = PlutusContractConvertor.Default.Convert(contract);

        return plutusContract;
    }
    else
        throw new Exception("Could not get input path. Did you enter exactly one input path?");

}

/// <summary>
/// Tries to export the plutus contract
/// </summary>
void Export(PlutusContract contract)
{
    if (arguments is null)
        throw new Exception("Arguments object is null");

    if (!arguments.FlagExists("--output") && !arguments.FlagExists("--output-console"))
        throw new Exception("No form of Plutus output is set. Please checkout --help for help.");

    var contractCode = PlutusContractGenerator.Default(contract).Generate();

    if (arguments.FlagExists("--output-console"))
        Console.WriteLine(contractCode.InString());

    if (arguments.TryGetArgumentValue("--output", out var path))
    {
        if (!File.Exists(path))
            File.Create(path).Close();

        File.WriteAllText(path, contractCode.InString());
    } 
}

/// <summary>
/// Watches for file changes
/// </summary>
async Task Watch()
{
    if (arguments is null)
        throw new Exception("Arguments object is null");

    if (!arguments.TryGetArgumentValue("--input", out var path))
        throw new Exception("DasContract file input is set or does not have a single path input. Please checkout --help for help.");

    var dirName = Path.GetDirectoryName(path);
    if (dirName is null)
        throw new Exception($"Could not recover directory of path {path}");

    var fileName = Path.GetFileName(path);
    if (fileName is null)
        throw new Exception($"Could not file name of path {path}");

    using FileSystemWatcher watcher = new FileSystemWatcher()
    {
        Path = dirName,
        Filter = fileName,
        NotifyFilter = NotifyFilters.LastWrite | 
            NotifyFilters.LastAccess | 
            NotifyFilters.Size | 
            NotifyFilters.FileName |
            NotifyFilters.Security,
        EnableRaisingEvents = true,
    };

    watcher.Changed += new FileSystemEventHandler(OnWatchedFileChanged);

    Console.WriteLine($"Watching file {fileName}");
    Console.WriteLine($"Write \"quit\" or \"q\" to quit");
    while (true)
    {
        var read = Console.ReadLine();
        if (read == "quit" || read == "q")
            break;
    }
}
void OnWatchedFileChanged(object source, FileSystemEventArgs e)
{
    Export(GetContract());
    var date = DateTime.Now;
    Console.WriteLine($"[{date.Hour}:{date.Minute}:{date.Second}] Output updated");
}
