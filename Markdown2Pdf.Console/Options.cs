using CommandLine;
using Markdown2Pdf.Options;
using PuppeteerSharp.Media;

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

  [Option('m', "margin-options", HelpText = "(Default: 50px) Css-Margins for the content in the pdf to generate. Values must be comma-separated.")]
  public MarginOptions? MarginOptions { get; set; } = new("50px");

  [Option('c', "chrome-path", HelpText = "Path to chrome or chromium executable or self-downloads it if null.")]
  public string? ChromePath { get; set; }

  [Option('k', "keep-html", HelpText = "If this is set, the temporary html file does not get deleted.")]
  public bool KeepHtml { get; set; }

  // TODO: theme
  [Option('t', "theme", Default = "github", HelpText = "The theme to use for styling the document.\r\n" +
    "Can either be a predefined value (github, latex) or a path to a custom css.")]
  public string Theme { get; set; }
  // TODO: code-highlight-theme

  [Option("code-highlight-theme", HelpText = "(Default: Github) The theme to use for styling the markdown code-blocks.")]
  public CodeHighlightTheme CodeHighlightTheme { get; set; } = CodeHighlightTheme.Github;
  // TODO: document-title
  public string? DocumentTitle { get; set; }
  // TODO: custom-css
  // TODO: make nullbable in library
  [Option("custom-css", HelpText = "A string containing CSS to apply extra styling to the document.")]
  public string CustomCss { get; set; } = string.Empty;
  // TODO: is-landscape
  [Option('l', "is-landscape", Default = false, HelpText = "Paper orientation.")]
  public bool IsLandscape { get; set; }
  // TODO: format
  [Option('p', "format", HelpText = "(Default: A4) The paper format for the PDF.")]
  public PaperFormat Format { get; set; } = PaperFormat.A4;
  // TODO: scale
  [Option('s', "scale", HelpText = "(Default: 1) Scale of the content. Must be between 0.1 and 2.")]
  public decimal Scale { get; set; } = 1;
  // TODO: table-of-contents
  [Option("table-of-contents", HelpText = "If set, a table of contents will be generated.")]

  //todo: maybe make this a verb
  public TableOfContentsType? TableOfContents { get; set; }

  [Option("table-of-contents-max-depth", Default = 3, HelpText = "The maximum depth of the table of contents.")]
  public int TableOfContentsMaxDepth { get; set; }
}

public class MarginOptions {

  // TODO: solve better
  public MarginOptions(string parameter) {
    var splitted = parameter.Split(',');

    switch (splitted.Length) {
      case 1:
        this.Top = this.Right = this.Bottom = this.Left = splitted[0];
        break;

      case 2:
        this.Top = this.Bottom = splitted[0];
        this.Right = this.Left = splitted[1];
        break;

      case 3:
        this.Top = splitted[0];
        this.Right = this.Left = splitted[1];
        this.Bottom = splitted[2];
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

public enum TableOfContentsType {
  Ordered,
  Unordered,
}
