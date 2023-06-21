using CommandLine;

namespace Markdown2Pdf.Console;

internal class Options {

  [Value(0, HelpText = "The path to the markdown file to parse.")]
  public string? InputPath { get; set; }

  [Value(1, Required = false, HelpText = "Path where the PDF file should be generated. If not set, defaults to <markdown-filename>.pdf.")]
  public string? OutputPath { get; set; }

  [Option('h', "header-path", HelpText = "Path to an html-file to use as the document-header.")]
  public string? HeaderPath { get; set; }

  [Option('f', "footer-path", HelpText = "Path to an html-file to use as the document-footer.")]
  public string? FooterPath { get; set; }

  [Option('o', "open-after-conversion", Default = false, HelpText = "If enabled, opens the generated pdf after execution.")]
  public bool OpenAfterConversion { get; set; }

  [Option('m', "margin-options", HelpText = "Css-Margin for the content in the pdf to generate. Values must be comma-separated.")]
  public MarginOptions? MarginOptions { get; set; }

  [Option('c', "chrome-path", HelpText = "If this is set, uses the provided chrome or chromium executable instead of self-downloading it.")]
  public string? ChromePath { get; set; }
}

public class MarginOptions {

  //todo: solve better
  public MarginOptions(string parameter) {
    var splitted = parameter.Split(',');

    switch (splitted.Length) {
      case 1:
        this.Top = splitted[0];
        this.Right = splitted[0];
        this.Bottom = splitted[0];
        this.Left = splitted[0];
        break;

      case 2:
        this.Top = splitted[0];
        this.Right = splitted[1];
        this.Bottom = splitted[0];
        this.Left = splitted[1];
        break;

      case 3:
        this.Top = splitted[0];
        this.Right = splitted[1];
        this.Bottom = splitted[2];
        this.Left = splitted[1];
        break;

      case 4:
        this.Top = splitted[0];
        this.Right = splitted[1];
        this.Bottom = splitted[2];
        this.Left = splitted[3];
        break;

      default:
        break;
    }
  }

  [Option]
  public string? Top { get; set; }

  [Option]
  public string? Right { get; set; }

  [Option]
  public string? Bottom { get; set; }

  [Option]
  public string? Left { get; set; }

}