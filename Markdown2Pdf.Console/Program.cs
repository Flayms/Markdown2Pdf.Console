using System.Diagnostics;
using Markdown2Pdf;
using Markdown2Pdf.Console;
using Markdown2Pdf.Options;

var commandLineHelper = new CommandLineHelper(args);

return (int) await commandLineHelper.Run(Handler);

static async Task<ExitCode> Handler(Options cliOptions, Markdown2PdfOptions options) {
  Console.WriteLine("Converting markdown to pdf...");

  var converter = new Markdown2PdfConverter(options);
  await converter.Convert(cliOptions.InputFile.FullName, cliOptions.OutputFile.FullName);

  Console.WriteLine($"Generated pdf at: {cliOptions.OutputFile}");

  // TODO: what if started from different directory?
  if (cliOptions.OpenAfterConversion)
    _ = Process.Start(new ProcessStartInfo { FileName = cliOptions.OutputFile.FullName, UseShellExecute = true });

  return ExitCode.Success;
}
