using System.Diagnostics;
using Markdown2Pdf;
using Markdown2Pdf.Console;

var commandLineHelper = new CommandLineHelper(args);

if (!commandLineHelper.TryCreateOptions(out var cliOptions, out var options))
  return (int)ExitCodes.Error;

Console.WriteLine("Converting markdown to pdf...");

var converter = new Markdown2PdfConverter(options);
await converter.Convert(Path.GetFullPath(cliOptions.InputPath!), Path.GetFullPath(cliOptions.OutputPath!));

Console.WriteLine($"Generated pdf at: {cliOptions.OutputPath}");

// TODO: what if started from different directory?
if (cliOptions.OpenAfterConversion)
  _ = Process.Start(new ProcessStartInfo { FileName = cliOptions.OutputPath, UseShellExecute = true });

return (int)ExitCodes.Success;
