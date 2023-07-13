using System.Diagnostics;
using CommandLine;
using Markdown2Pdf;
using Markdown2Pdf.Console;
using Markdown2Pdf.Options;

//todo: error handling
var result = Parser.Default.ParseArguments<Options>(args);
var options = result.Value;

if (result.Tag == ParserResultType.NotParsed)
  return;

if (options.InputPath == null)
  return;

Console.WriteLine("Converting markdown to pdf...");

var outputPath = options.OutputPath ?? Path.ChangeExtension(options.InputPath, "pdf");
var currentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "node_modules");

var markdown2pdfOptions = new Markdown2PdfOptions {
  HeaderUrl = options.HeaderPath,
  FooterUrl = options.FooterPath,
  ModuleOptions = ModuleOptions.FromLocalPath(currentDir),
  KeepHtml = options.KeepHtml
  };

var marginOptions = options.MarginOptions;
if (marginOptions != null) {
  markdown2pdfOptions.MarginOptions = new Markdown2Pdf.Options.MarginOptions {
    Top = marginOptions.Top,
    Bottom = marginOptions.Bottom,
    Left = marginOptions.Left,
    Right = marginOptions.Right
  };
}

var converter = new Markdown2PdfConverter(markdown2pdfOptions);
converter.Convert(Path.GetFullPath(options.InputPath), Path.GetFullPath(outputPath));

Console.WriteLine($"Generated pdf at: {outputPath}");

//todo: what if started from different directory?
if (options.OpenAfterConversion)
  Process.Start("cmd", $"/c start {outputPath}");