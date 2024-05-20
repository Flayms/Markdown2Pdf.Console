using System.Diagnostics;
using Markdown2Pdf;
using Markdown2Pdf.Console;

var commandLineHelper = new CommandLineHelper(args);

if (!commandLineHelper.TryCreateOptions(out var cliOptions, out var options))
  return (int)ExitCode.Error;

Console.WriteLine("Converting markdown to pdf...");

var converter = new Markdown2PdfConverter(options);
await converter.Convert(cliOptions.InputPath.FullName, cliOptions.OutputPath.FullName);

Console.WriteLine($"Generated pdf at: {cliOptions.OutputPath}");

// TODO: what if started from different directory?
if (cliOptions.OpenAfterConversion)
  _ = Process.Start(new ProcessStartInfo { FileName = cliOptions.OutputPath.FullName, UseShellExecute = true });

return (int)ExitCode.Success;
