using System.Diagnostics;
using CommandLine;
using MarkdownToPdf;
using MarkdownToPdf.Console;

//todo: error handling
var result = Parser.Default.ParseArguments<Options>(args);
var options = result.Value;

if (result.Tag == ParserResultType.NotParsed)
  return;

if (options.InputPath == null)
  return;

Console.WriteLine("Converting markdown to pdf...");

var outputPath = options.OutputPath ?? Path.ChangeExtension(options.InputPath, "pdf");

//todo: hmp test project


var settings = new MarkdownToPdfSettings {
  HeaderUrl = options.HeaderPath,
  FooterUrl = options.FooterPath,
};

var marginOptions = options.MarginOptions;
if (marginOptions != null) {
  settings.MarginOptions = new MarkdownToPdf.MarginOptions {
    Top = marginOptions.Top,
    Bottom = marginOptions.Bottom,
    Left = marginOptions.Left,
    Right = marginOptions.Right
  };
}

var converter = new MarkdownToPdfConverter(settings);
converter.Convert(options.InputPath, outputPath);

Console.WriteLine($"Generated pdf at: {outputPath}");

//todo: what if started from different directory?
if (options.OpenAfterConversion)
  Process.Start("cmd", $"/c start {outputPath}");