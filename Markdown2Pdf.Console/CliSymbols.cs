using Markdown2Pdf.Options;
using System.CommandLine;

namespace Markdown2Pdf.Console;
internal class CliSymbols {

  // TODO: maybe nullability is not needed
  public Argument<FileInfo> InputFileArg { get; } = new(
  name: "input-path",
  description: "The path to the markdown file to parse."
  );

  public Argument<FileInfo?> OutputFileArg{ get; } = new(
    name: "output-path",
    description: "Path where the PDF file should be generated. If not set, defaults to <markdown-filename>.pdf."
    ) { Arity = ArgumentArity.ZeroOrOne };

  public Option<bool?> FromYamlOption{ get; } = new(
    aliases: ["-y", "--options-from-yaml-front-matter"],
    description: "If set, loads the options from a YAML front matter block. See https://github.com/Flayms/Markdown2Pdf/wiki/Markdown2Pdf.Markdown2PdfConverter#-createwithinlineoptionsfromfilestring"
    );

  public Option<string?> ChromePathOption{ get; } = new(
       aliases: ["-c", "--chrome-path"],
          description: "Path to chrome or chromium executable. Downloads it by itself if not set."
       );

  public Option<string?> CodeHighlightThemeOption{ get; } = new(
       aliases: ["--code-highlight-theme"],
          description: "The theme to use for styling the markdown code-blocks. " +
       "Valid Values: See  https://github.com/Flayms/Markdown2Pdf/wiki/Markdown2Pdf.Options.CodeHighlightTheme for an overview of all themes.");

  public Option<string?> CustomHeadContentOption{ get; } = new(
          aliases: ["--custom-head-content"],
                   description: "A string containing any content valid inside an html <head> to apply extra scripting / styling to the document.");

  public Option<string?> DocumentTitleOption{ get; } = new(
    aliases: ["--document-title"],
                   description: "The title of this document. " +
          "Can be injected into the header / footer by adding the class document-title to the element.");

  public Option<bool?> EnableAutoLanguageDetectionOption{ get; } = new(
       aliases: ["--enable-auto-language-detection"],
                         description: "Auto detect the language for code blocks without specfied language.");

  public Option<string?> FooterPathOption{ get; } = new( // TODO: maybe use fileinfo
    aliases: ["-f", "--footer-path"],
    description: "Path to an html-file to use as the document-footer."
    );

  public Option<string?> FormatOption{ get; } = new(
       aliases: ["--format"],
          description: "The paper format for the PDF. Valid values: Letter, Legal, Tabloid, Ledger, A0-A6."
       );

  public Option<string?> HeaderPathOption{ get; } = new(
    aliases: ["-h", "--header-path"],
       description: "Path to an html-file to use as the document-header."
       );

  public Option<bool?> IsLandscapeOption{ get; } = new(
       aliases: ["-l", "--is-landscape"],
             description: "Paper orientation."
       );

  public Option<bool?> KeepHtmlOption{ get; } = new(
    aliases: ["-k", "--keep-html"],
       description: "If this is set, the temporary html file does not get deleted.");

  public Option<MarginOptions?> MarginOptionsOption{ get; } = new(
    aliases: ["-m", "--margins"],
       description: "Css-Margins for the content in the pdf to generate. Values must be comma-separated."
       );

  public Option<string?> MetadataTitleOption{ get; } = new(
    aliases: ["--metadata-title"],
       description: "The title of the document. Can be injected into the header / footer by adding the class document-title to the element."
       );

  public Option<bool?> OpenAfterConversionOption{ get; } = new(
    aliases: ["-o", "--open-after-conversion"],
       description: "If enabled, opens the generated pdf after execution."
       );

  public Option<decimal?> ScaleOption{ get; } = new(
    aliases: ["-s", "--scale"],
          description: "Scale of the content. Must be between 0.1 and 2."
          );

  public Option<string?> ThemeOption{ get; } = new(
    aliases: ["-t", "--theme"],
          description: "The theme to use for styling the document. Can either be a predefined value (github, latex) or a path to a custom css."
          );

  public Option<ListStyle?> TocListStyleOption{ get; } = new(
    aliases: ["--toc-list-style"],
             description: "Decides which characters to use before the TOC items."
             );

  public Option<int?> TocMinDepthOption{ get; } = new(
    aliases: ["--toc-min-depth"],
          description: "The minimum level of heading depth to include in the TOC (e.g. 1 will only include headings greater than or equal to <h1>). Range: 1 to 6."
          );

  public Option<int?> TocMaxDepthOption{ get; } = new(
       aliases: ["--toc-max-depth"],
          description: "The maximum level of heading depth to include in the TOC (e.g. 3 will include headings less than or equal to <h3>). Range: 1 to 6."
       );

  public Option<bool?> TocHasColoredLinksOption{ get; } = new(
    aliases: ["--toc-has-colored-links"],
             description: "Determines if the TOC links should have the default link color (instead of looking  like normal text)."
             );

  public Option<Leader?> TocPageNumberTabLeaderOption{ get; } = new(
    aliases: ["--toc-page-numbers-tab-leader"],
             description: "Generate TOC Page Numbers and use the given character to lead from the TOC title to the page number.");

  public CliSymbols() {
    this.InputFileArg.AddValidator(Utils.ValidateFileInfo);
    this.OutputFileArg.AddValidator(Utils.ValidateFileInfo);
    this.ChromePathOption.AddValidator(Utils.ValidateFilePath);
    this.CodeHighlightThemeOption.FromAmong(Utils.GetAllPublicPropertyNames<CodeHighlightTheme>());
    this.CodeHighlightThemeOption.ArgumentHelpName = "code-highlight-theme";
    this.FooterPathOption.AddValidator(Utils.ValidateFilePath);
    this.HeaderPathOption.AddValidator(Utils.ValidateFilePath);
    this.MarginOptionsOption.SetDefaultValue(new MarginOptions("50px"));
    this.TocMinDepthOption.AddValidator(r => Utils.ValidateBounds(r, 1, 6));
    this.TocMaxDepthOption.AddValidator(r => Utils.ValidateBounds(r, 1, 6));
  }

}
