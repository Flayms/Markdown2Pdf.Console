using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Markdown2Pdf.Options;

namespace Markdown2Pdf.Console;
internal class CommandLineHelper(string[] args) {

  public bool TryCreateOptions(out Options cliOptions, out Markdown2PdfOptions options) {
    options = new Markdown2PdfOptions();
    cliOptions = new Options();
    var helpShown = false;

    var rootCommand = _CreateCommandLineOptions(cliOptions, options); // Assigns properties to cliOptions and options
    var parser = new CommandLineBuilder(rootCommand)
      .UseDefaults()
      .UseHelp(context => helpShown = true)
      .Build();


    var result = (ExitCode)parser.Invoke(args);
    if (result != ExitCode.Success || helpShown) {
      cliOptions = null!;
      options = null!;
      return false;
    }

    return true;
  }

  private static RootCommand _CreateCommandLineOptions(Options cliOptions, Markdown2PdfOptions options) {
    var inputFileArg = new Argument<FileInfo>(
      name: "input-path",
      description: "The path to the markdown file to parse."
    );

    var outputFileArg = new Argument<FileInfo?>(
      name: "output-path",
      description: "Path where the PDF file should be generated. If not set, defaults to <markdown-filename>.pdf."
    ) { Arity = ArgumentArity.ZeroOrOne };

    var headerPathOption = new Option<string?>(
      aliases: ["-h", "--header-path"],
      description: "Path to an html-file to use as the document-header."
    );
    var footerPathOption = new Option<string?>(
      aliases: ["-f", "--footer-path"],
      description: "Path to an html-file to use as the document-footer."
    );
    var openAfterConversionOption = new Option<bool>(
      aliases: ["-o", "--open-after-conversion"],
      description: "If enabled, opens the generated pdf after execution."
    );
    var marginOptionsOption = new Option<MarginOptions?>(
      aliases: ["-m", "--margin-options"],
      description: "(Default: 50px) Css-Margins for the content in the pdf to generate. Values must be comma-separated."
    );
    var chromePathOption = new Option<string?>(
      aliases: ["-c", "--chrome-path"],
      description: "Path to chrome or chromium executable. Downloads it by itself if not set."
    );
    var keepHtmlOption = new Option<bool?>(
      aliases: ["-k", "--keep-html"],
      description: "If this is set, the temporary html file does not get deleted."
    );
    var themeOption = new Option<string?>(
      aliases: ["-t"],
      description: "The theme to use for styling the document. Can either be a predefined value (github, latex) or a path to a custom css."
    );
    var codeHighlightThemeOption = new Option<string?>(
      aliases: ["--code-highlight-theme"],
      description: "The theme to use for styling the markdown code-blocks. " +
      "Valid Values: See https://github.com/Flayms/Markdown2Pdf/blob/main/Markdown2Pdf/Options/CodeHighlightTheme.cs for an overview of all themes." // TODO: switch to wiki
    );
    var documentTitleOption = new Option<string?>(
      aliases: ["--document-title"],
      description: "The title of this document. " +
      "Can be injected into the header / footer by adding the class document-title to the element."
    );
    var customHeadContentOption = new Option<string?>(
      aliases: ["--custom-head-content"],
      description: "A string containing any content valid inside a html <head> to apply extra scripting / styling to the document."
    );
    var isLandscapeOption = new Option<bool?>(
      aliases: ["-l", "--is-landscape"],
      description: "Paper orientation."
    );
    var formatOption = new Option<string?>(
      aliases: ["--format"],
      description: "The paper format for the PDF. " +
        "Valid values: Letter, Legal, Tabloid, Ledger, A0-A6"
    );
    var scaleOption = new Option<decimal?>(
      aliases: ["-s", "--scale"],
      description: "(Default: 1) Scale of the content. Must be between 0.1 and 2."
    );

    var rootCommand = new RootCommand("Command-line application for converting Markdown to Pdf.") {
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
      scaleOption
    };

    rootCommand.SetHandler((inputFile, outputFile, openAfterConversion, markdown2PdfOptions) => {
      cliOptions.InputPath = inputFile;
      cliOptions.OutputPath = outputFile ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, "pdf"));
      cliOptions.OpenAfterConversion = openAfterConversion;
      options = markdown2PdfOptions;
    },
    inputFileArg,
    outputFileArg,
    openAfterConversionOption,
    new OptionBinder(
      headerPathOption,
      footerPathOption,
      marginOptionsOption,
      chromePathOption,
      keepHtmlOption,
      themeOption,
      codeHighlightThemeOption,
      documentTitleOption,
      customHeadContentOption,
      isLandscapeOption,
      formatOption,
      scaleOption
      ));

    return rootCommand;
  }

}
