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

  [Option('m', "margin-options", HelpText = "(Default: 50px) Css-Margins for the content in the pdf to generate. Values must be comma-separated.")]
  public MarginOptions? MarginOptions { get; set; } = new("50px");

  [Option('c', "chrome-path", HelpText = "Path to chrome or chromium executable or self-downloads it if null.")]
  public string? ChromePath { get; set; }

  [Option('k', "keep-html", HelpText = "If this is set, the temporary html file does not get deleted.")]
  public bool KeepHtml { get; set; }

  [Option('t', Default = "github", HelpText = "The theme to use for styling the document. " +
    "Can either be a predefined value (github, latex) or a path to a custom css.")]
  public string Theme { get; set; } = string.Empty;

  [Option("code-highlight-theme", Default = "github", HelpText = "The theme to use for styling the markdown code-blocks. " +
    "Valid Values: See https://github.com/Flayms/Markdown2Pdf/blob/main/Markdown2Pdf/Options/CodeHighlightTheme.cs for an overview of all themes.")]
  public string CodeHighlightTheme { get; set; } = string.Empty;

  [Option("document-title", HelpText = "The title of this document. " +
    "Can be injected into the header / footer by adding the class document-title to the element.")]
  public string? DocumentTitle { get; set; }

  [Option("custom-head-content", HelpText = "A string containing any content valid inside a html <head> to apply extra scripting / styling to the document.")]
  public string? CustomHeadContent { get; set; }

  [Option('l', "is-landscape", Default = false, HelpText = "Paper orientation.")]
  public bool IsLandscape { get; set; }

  // TODO: support custom values e.g 8.5m,11 (in inches)
  [Option("format", Default = "A4", HelpText = "The paper format for the PDF. " +
    "Valid values: Letter, Legal, Tabloid, Ledger, A0-A6")]
  public string Format { get; set; } = string.Empty;

  [Option('s', "scale", HelpText = "(Default: 1) Scale of the content. Must be between 0.1 and 2.")]
  public decimal Scale { get; set; } = 1;

  // TODO: test
  // TODO: maybe make this a verb
  [Option("toc", Default = null, HelpText = "If set, Creates a TOC out of the markdown headers " +
    "and writes it into a <!--TOC--> comment within the markdown document.")]
  public TableOfContentsType? TableOfContents { get; set; }

  // TODO: test
  [Option("toc-max-depth", Default = 3, HelpText = "The maximum depth of the table of contents. " +
    "Requires --toc to be set.")]
  public int TableOfContentsMaxDepth { get; set; }
}
