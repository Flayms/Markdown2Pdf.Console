using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;
using Markdown2Pdf.Options;

namespace Markdown2Pdf.Console;
internal class CommandLineHelper(string[] args) {

  public delegate Task<ExitCode> Handler(Options cliOptions, Markdown2PdfOptions options);

  public async Task<ExitCode> Run(Handler handler) {
    var rootCommand = _CreateCommand(handler);
    var parser = new CommandLineBuilder(rootCommand)
      .UseDefaults()
      .Build();

    return (ExitCode) await parser.InvokeAsync(args);
  }

  // TODO: from yaml
  // TODO: maybe nullability is not needed
  private static RootCommand _CreateCommand(Handler handler) {
    var inputFileArg = new Argument<FileInfo>(
      name: "input-path",
      description: "The path to the markdown file to parse."
    );
    inputFileArg.AddValidator(Utils.ValidateFileInfo);

    var outputFileArg = new Argument<FileInfo?>(
      name: "output-path",
      description: "Path where the PDF file should be generated. If not set, defaults to <markdown-filename>.pdf."
    ) { Arity = ArgumentArity.ZeroOrOne };
    outputFileArg.AddValidator(Utils.ValidateFileInfo);

    var chromePathOption = new Option<string?>(
      aliases: ["-c", "--chrome-path"],
      description: "Path to chrome or chromium executable. Downloads it by itself if not set."
    );
    chromePathOption.AddValidator(Utils.ValidateFilePath);

    var codeHighlightThemeOption = new Option<string?>(
      aliases: ["--code-highlight-theme"],
      description: "The theme to use for styling the markdown code-blocks. " +
      "Valid Values: See https://github.com/Flayms/Markdown2Pdf/wiki/Markdown2Pdf.Options.CodeHighlightTheme for an overview of all themes." // TODO: switch to wiki
    );
    codeHighlightThemeOption.FromAmong(Utils.GetAllPublicPropertyNames<CodeHighlightTheme>());
    codeHighlightThemeOption.ArgumentHelpName = "code-highlight-theme";

    var customHeadContentOption = new Option<string?>(
      aliases: ["--custom-head-content"],
      description: "A string containing any content valid inside an html <head> to apply extra scripting / styling to the document."
    );
    var documentTitleOption = new Option<string?>(
      aliases: ["--document-title"],
      description: "The title of this document. " +
      "Can be injected into the header / footer by adding the class document-title to the element."
    );
    var enableAutoLanguageDetectionOption = new Option<bool?>(
      aliases: ["--enable-auto-language-detection"],
      description: "Auto detect the language for code blocks without specfied language."
    );
    var footerPathOption = new Option<string?>(
      aliases: ["-f", "--footer-path"],
      description: "Path to an html-file to use as the document-footer."
    );
    footerPathOption.AddValidator(Utils.ValidateFilePath);
    var formatOption = new Option<string?>(
      aliases: ["--format"],
      description: "The paper format for the PDF. " +
      "Valid values: Letter, Legal, Tabloid, Ledger, A0-A6"
    );
    var headerPathOption = new Option<string?>(
      aliases: ["-h", "--header-path"],
      description: "Path to an html-file to use as the document-header."
    );
   headerPathOption.AddValidator(Utils.ValidateFilePath);
    var isLandscapeOption = new Option<bool?>(
      aliases: ["-l", "--is-landscape"],
      description: "Paper orientation."
    );
    var keepHtmlOption = new Option<bool?>(
      aliases: ["-k", "--keep-html"],
      description: "If this is set, the temporary html file does not get deleted."
    );
    var marginOptionsOption = new Option<MarginOptions?>(
      aliases: ["-m", "--margins"],
      description: "Css-Margins for the content in the pdf to generate. Values must be comma-separated."
    );
    marginOptionsOption.SetDefaultValue(new MarginOptions("50px"));

    var metadataTitleOption = new Option<string?>(
      aliases: ["--metadata-title"],
      description: "The title of the document. Can be injected into the header / footer by adding the class document-title to the element."
    );
    var openAfterConversionOption = new Option<bool>(
      aliases: ["-o", "--open-after-conversion"],
      description: "If enabled, opens the generated pdf after execution."
    );
    var scaleOption = new Option<decimal?>(
      aliases: ["-s", "--scale"],
      description: "Scale of the content. Must be between 0.1 and 2."
    );
    var themeOption = new Option<string?>(
      aliases: ["-t", "--theme"],
      description: "The theme to use for styling the document. Can either be a predefined value (github, latex) or a path to a custom css."
    );

    var tocListStyleOption = new Option<ListStyle?>(
      aliases: ["--toc-list-style"],
      description: "Decides which characters to use before the TOC items."
    );

    var tocMinDepthOption = new Option<int?>(
      aliases: ["--toc-min-depth"],
      description: "The minimum level of heading depth to include in the TOC (e.g. 1 will only include headings greater than or equal to <h1>). Range: 1 to 6."
    );
    tocMinDepthOption.AddValidator(r => Utils.ValidateBounds(r, 1, 6));

    var tocMaxDepthOption = new Option<int?>(
      aliases: ["--toc-max-depth"],
      description: "The maximum level of heading depth to include in the TOC (e.g. 3 will include headings less than or equal to <h3>). Range: 1 to 6."
    );
    tocMaxDepthOption.AddValidator(r => Utils.ValidateBounds(r, 1, 6));

    var tocHasColoredLinksOption = new Option<bool?>(
      aliases: ["--toc-has-colored-links"],
      description: "Determines if the TOC links should have the default link color (instead of looking  like normal text)."
    );

    var tocPageNumberTabLeaderOption = new Option<Leader?>(
      aliases: ["--toc-page-numbers-tab-leader"],
      description: "Generate TOC Page Numbers and use the given character to lead from the TOC title to the page number."
    );

    var rootCommand = new RootCommand($"Command-line application for converting Markdown to Pdf.{Environment.NewLine}" +
      $"Note: setting any of the --toc options will cause a TOC to be generated within the placeholders.") {
      inputFileArg,
      outputFileArg,
      headerPathOption,
      footerPathOption,
      openAfterConversionOption,
      marginOptionsOption,
      chromePathOption,
      keepHtmlOption,
      themeOption,
      codeHighlightThemeOption,
      documentTitleOption,
      customHeadContentOption,
      isLandscapeOption,
      formatOption,
      scaleOption,
      tocMinDepthOption,
      tocMaxDepthOption,
      tocListStyleOption,
      tocHasColoredLinksOption,
      tocPageNumberTabLeaderOption,
    };

    rootCommand.SetHandler(async (context) => {
      var parseResult = context.ParseResult;
      var inputFile = parseResult.GetValueForArgument(inputFileArg);
      var cliOptions = new Options {
        InputFile = inputFile,
        OutputFile = parseResult.GetValueForArgument(outputFileArg) ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, "pdf")),
        OpenAfterConversion = parseResult.GetValueForOption(openAfterConversionOption)
      };

      var binder = new OptionBinder(
        headerPathOption,
        footerPathOption,
        marginOptionsOption,
        metadataTitleOption,
        chromePathOption,
        keepHtmlOption,
        themeOption,
        codeHighlightThemeOption,
        documentTitleOption,
        enableAutoLanguageDetectionOption,
        customHeadContentOption,
        isLandscapeOption,
        formatOption,
        scaleOption,
        tocMinDepthOption,
        tocMaxDepthOption,
        tocListStyleOption,
        tocHasColoredLinksOption,
        tocPageNumberTabLeaderOption
       );

      var options = binder.GetValue(context.BindingContext);
      var result = await handler(cliOptions, options); // Runs actual logic here

      context.ExitCode = (int)result;
    });

    return rootCommand;
  }

}
