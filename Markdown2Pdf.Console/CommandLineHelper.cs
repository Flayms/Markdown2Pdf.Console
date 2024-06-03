using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Markdown2Pdf.Options;

namespace Markdown2Pdf.Console;
internal class CommandLineHelper(string[] args) {

  public delegate Task<ExitCode> Handler(CliOptions cliOptions, Markdown2PdfConverter options);

  private readonly Argument<FileInfo> _inputFileArg = new(
    name: "input-path",
    description: "The path to the markdown file to parse."
    );

  private readonly Argument<FileInfo?> _outputFileArg = new(
    name: "output-path",
    description: "Path where the PDF file should be generated. If not set, defaults to <markdown-filename>.pdf."
    ) { Arity = ArgumentArity.ZeroOrOne };

  private readonly Option<bool?> _fromYamlOption = new(
    aliases: ["-y", "--options-from-yaml-front-matter"],
    description: "If set, loads the options from a YAML front matter block. See https://github.com/Flayms/Markdown2Pdf/wiki/Markdown2Pdf.Markdown2PdfConverter#-createwithinlineoptionsfromfilestring"
    );

  private readonly Option<string?> _chromePathOption = new(
       aliases: ["-c", "--chrome-path"],
          description: "Path to chrome or chromium executable. Downloads it by itself if not set."
       );

  private readonly Option<string?> _codeHighlightThemeOption = new(
       aliases: ["--code-highlight-theme"],
          description: "The theme to use for styling the markdown code-blocks. " +
       "Valid Values: See  https://github.com/Flayms/Markdown2Pdf/wiki/Markdown2Pdf.Options.CodeHighlightTheme for an overview of all themes.");

  private readonly Option<string?> _customHeadContentOption = new(
          aliases: ["--custom-head-content"],
                   description: "A string containing any content valid inside an html <head> to apply extra scripting / styling to the document.");

  private readonly Option<string?> _documentTitleOption = new(
    aliases: ["--document-title"],
                   description: "The title of this document. " +
          "Can be injected into the header / footer by adding the class document-title to the element.");

  private readonly Option<bool?> _enableAutoLanguageDetectionOption = new(
       aliases: ["--enable-auto-language-detection"],
                         description: "Auto detect the language for code blocks without specfied language.");

  private readonly Option<string?> _footerPathOption = new( // TODO: maybe use fileinfo
    aliases: ["-f", "--footer-path"],
    description: "Path to an html-file to use as the document-footer."
    );

  private readonly Option<string?> _formatOption = new(
       aliases: ["--format"],
          description: "The paper format for the PDF. Valid values: Letter, Legal, Tabloid, Ledger, A0-A6."
       );

  private readonly Option<string?> _headerPathOption = new(
    aliases: ["-h", "--header-path"],
       description: "Path to an html-file to use as the document-header."
       );

  private readonly Option<bool?> _isLandscapeOption = new(
       aliases: ["-l", "--is-landscape"],
             description: "Paper orientation."
       );

  private readonly Option<bool?> _keepHtmlOption = new(
    aliases: ["-k", "--keep-html"],
       description: "If this is set, the temporary html file does not get deleted.");

  private readonly Option<MarginOptions?> _marginOptionsOption = new(
    aliases: ["-m", "--margins"],
       description: "Css-Margins for the content in the pdf to generate. Values must be comma-separated."
       );

  private readonly Option<string?> _metadataTitleOption= new(
    aliases: ["--metadata-title"],
       description: "The title of the document. Can be injected into the header / footer by adding the class document-title to the element."
       );

  private readonly Option<bool?> _openAfterConversionOption = new(
    aliases: ["-o", "--open-after-conversion"],
       description: "If enabled, opens the generated pdf after execution."
       );

  private readonly Option<decimal?> _scaleOption = new(
    aliases: ["-s", "--scale"],
          description: "Scale of the content. Must be between 0.1 and 2."
          );

  private readonly Option<string?> _themeOption = new(
    aliases: ["-t", "--theme"],
          description: "The theme to use for styling the document. Can either be a predefined value (github, latex) or a path to a custom css."
          );

  private readonly Option<ListStyle?> _tocListStyleOption = new(
    aliases: ["--toc-list-style"],
             description: "Decides which characters to use before the TOC items."
             );

  private readonly Option<int?> _tocMinDepthOption = new(
    aliases: ["--toc-min-depth"],
          description: "The minimum level of heading depth to include in the TOC (e.g. 1 will only include headings greater than or equal to <h1>). Range: 1 to 6."
          );

  private readonly Option<int?> _tocMaxDepthOption = new(
       aliases: ["--toc-max-depth"],
          description: "The maximum level of heading depth to include in the TOC (e.g. 3 will include headings less than or equal to <h3>). Range: 1 to 6."
       );

  private readonly Option<bool?> _tocHasColoredLinksOption = new(
    aliases: ["--toc-has-colored-links"],
             description: "Determines if the TOC links should have the default link color (instead of looking  like normal text)."
             );

  private readonly Option<Leader?> _tocPageNumberTabLeaderOption = new(
    aliases: ["--toc-page-numbers-tab-leader"],
             description: "Generate TOC Page Numbers and use the given character to lead from the TOC title to the page number.");

  public async Task<ExitCode> Run(Handler handler) {
    var rootCommand = _CreateCommand(handler);
    var parser = new CommandLineBuilder(rootCommand)
      .UseDefaults()
      .Build();

    return (ExitCode) await parser.InvokeAsync(args);
  }

  // TODO: maybe nullability is not needed
  private RootCommand _CreateCommand(Handler handler) {
    this._inputFileArg.AddValidator(Utils.ValidateFileInfo);
    this._outputFileArg.AddValidator(Utils.ValidateFileInfo);
    this._chromePathOption.AddValidator(Utils.ValidateFilePath);
    this._codeHighlightThemeOption.FromAmong(Utils.GetAllPublicPropertyNames<CodeHighlightTheme>());
    this._codeHighlightThemeOption.ArgumentHelpName = "code-highlight-theme";
    this._footerPathOption.AddValidator(Utils.ValidateFilePath);
    this._headerPathOption.AddValidator(Utils.ValidateFilePath);
    this._marginOptionsOption.SetDefaultValue(new MarginOptions("50px"));
    this._tocMinDepthOption.AddValidator(r => Utils.ValidateBounds(r, 1, 6));
    this._tocMaxDepthOption.AddValidator(r => Utils.ValidateBounds(r, 1, 6));

    var rootCommand = new RootCommand($"Command-line application for converting Markdown to Pdf.{Environment.NewLine}" +
      $"Note: setting any of the --toc options will cause a TOC to be generated within the placeholders.") {
      this._inputFileArg,
      this._outputFileArg,
      this._fromYamlOption,
      this._headerPathOption,
      this._footerPathOption,
      this._openAfterConversionOption,
      this._marginOptionsOption,
      this._chromePathOption,
      this._keepHtmlOption,
      this._themeOption,
      this._codeHighlightThemeOption,
      this._documentTitleOption,
      this._customHeadContentOption,
      this._isLandscapeOption,
      this._formatOption,
      this._scaleOption,
      this._tocMinDepthOption,
      this._tocMaxDepthOption,
      this._tocListStyleOption,
      this._tocHasColoredLinksOption,
      this._tocPageNumberTabLeaderOption,
    };

    // TODO: check if this works as intended
    rootCommand.AddValidator(result => {
      var yamlResult = result.Children.FirstOrDefault(s => s.Symbol == this._fromYamlOption);
      if (yamlResult is null)
        return;

      foreach (var child in result.Children) {
        // only option allowed together with yaml result is open-after-conversion
        if (child.Symbol == this._openAfterConversionOption || child == yamlResult || child.Symbol is not Option)
          continue;

        child.ErrorMessage = $"Option {child.Symbol.Name} Cannot be used together with --options-from-yaml-front-matter.";
      }
    });

    // TODO: refac
    rootCommand.SetHandler(async (context) => {
      var parseResult = context.ParseResult;
      var inputFile = parseResult.GetValueForArgument(this._inputFileArg);
      var cliOptions = new CliOptions {
        InputFile = inputFile,
        OutputFile = parseResult.GetValueForArgument(this._outputFileArg) ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, "pdf")),
        OpenAfterConversion = parseResult.GetValueForOption(this._openAfterConversionOption).Value
      };

      var isParseFromYaml = parseResult!.GetValueForOption(this._fromYamlOption);
      if (isParseFromYaml.HasValue && isParseFromYaml.Value == true) {
        var converter = Markdown2PdfConverter.CreateWithInlineOptionsFromFile(inputFile);
        var resultYaml = await handler(cliOptions, converter); // Runs actual logic here
        context.ExitCode = (int)resultYaml;
        return;
      }

      var binder = new OptionBinder(
        this._headerPathOption,
        this._footerPathOption,
        this._marginOptionsOption,
        this._metadataTitleOption,
        this._chromePathOption,
        this._keepHtmlOption,
        this._themeOption,
        this._codeHighlightThemeOption,
        this._documentTitleOption,
        this._enableAutoLanguageDetectionOption,
        this._customHeadContentOption,
        this._isLandscapeOption,
        this._formatOption,
        this._scaleOption,
        this._tocMinDepthOption,
        this._tocMaxDepthOption,
        this._tocListStyleOption,
        this._tocHasColoredLinksOption,
        this._tocPageNumberTabLeaderOption
       );

      var options = binder.GetValue(context.BindingContext);
      var result = await handler(cliOptions, new Markdown2PdfConverter(options)); // Runs actual logic here

      context.ExitCode = (int)result;
    });

    return rootCommand;
  }

}
