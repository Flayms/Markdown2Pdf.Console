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

//todo: better error handling!
if (options.InputPath == null)
  return;

Console.WriteLine("Converting markdown to pdf...");

var outputPath = options.OutputPath ?? Path.ChangeExtension(options.InputPath, "pdf");
var currentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "node_modules");

var markdown2PdfOptions = _CreateMarkdown2PdfOptions(options);
var converter = new Markdown2PdfConverter(markdown2PdfOptions);
await converter.Convert(Path.GetFullPath(options.InputPath), Path.GetFullPath(outputPath));

Console.WriteLine($"Generated pdf at: {outputPath}");

//todo: what if started from different directory?
//todo: needs to work on linux too
if (options.OpenAfterConversion)
  Process.Start("cmd", $"/c start {outputPath}");

static Markdown2PdfOptions _CreateMarkdown2PdfOptions(Options options) {
  var markdown2PdfOptions = new Markdown2PdfOptions {
    ModuleOptions = ModuleOptions.Remote,
    //ModuleOptions = ModuleOptions.FromLocalPath(currentDir),
    KeepHtml = options.KeepHtml
  };

  if (options.HeaderPath != null) {
    if (!File.Exists(options.HeaderPath))
      throw new Exception($"The file '{options.HeaderPath}' doesn't exist!");

    markdown2PdfOptions.HeaderHtml = File.ReadAllText(options.HeaderPath);
  }

  if (options.FooterPath != null) {
    if (!File.Exists(options.FooterPath))
      throw new Exception($"The file '{options.FooterPath}' doesn't exist!");

    markdown2PdfOptions.FooterHtml = File.ReadAllText(options.FooterPath);
  }

  var marginOptions = options.MarginOptions;
  if (marginOptions != null) {
    markdown2PdfOptions.MarginOptions = new Markdown2Pdf.Options.MarginOptions {
      Top = marginOptions.Top,
      Bottom = marginOptions.Bottom,
      Left = marginOptions.Left,
      Right = marginOptions.Right
    };
  }

  return markdown2PdfOptions;
}
