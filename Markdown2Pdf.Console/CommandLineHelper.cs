using System.CommandLine;
using Markdown2Pdf.Options;

namespace Markdown2Pdf.Console;
internal class CommandLineHelper(string[] args) {

  public bool TryCreateOptions(out Options cliOptions, out Markdown2PdfOptions options) {
    options = new Markdown2PdfOptions();
    cliOptions = new Options();

    var rootCommand = _CreateCommandLineOptions(cliOptions, options); // Assigns properties to cliOptions and options

    var result = (ExitCode)rootCommand.Invoke(args);
    if (result != ExitCode.Success) {
      cliOptions = null!;
      options = null!;
      return false;
    }

    return true;
  }

  private RootCommand _CreateCommandLineOptions(Options cliOptions, Markdown2PdfOptions options) {
    // TODO: maybe use FileInfo
    // TODO: handle default values correctly
    // getDefaultValue
    var inputPathArg = new Argument<string>(
      name: "input-path",
      description: "The path to the markdown file to parse."
      );

    // TODO: maybe use FileInfo
    var outputPathArg = new Argument<string?>(
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
      getDefaultValue: () => "github",
      description: "The theme to use for styling the document. Can either be a predefined value (github, latex) or a path to a custom css."
    );
    var codeHighlightThemeOption = new Option<string?>(
      aliases: ["--code-highlight-theme"],
      getDefaultValue: () => "github",
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
      getDefaultValue: () => "A4",
      description: "The paper format for the PDF. " +
        "Valid values: Letter, Legal, Tabloid, Ledger, A0-A6"
    );
    var scaleOption = new Option<decimal?>(
      aliases: ["-s", "--scale"],
      getDefaultValue: () => 1,
      description: "(Default: 1) Scale of the content. Must be between 0.1 and 2."
    );

    var rootCommand = new RootCommand {
      inputPathArg,
      outputPathArg,
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

    rootCommand.SetHandler((inputPath, outputPath, openAfterConversion, markdown2PdfOptions) => {
      cliOptions.InputPath = inputPath;
      cliOptions.OutputPath = outputPath ?? Path.ChangeExtension(inputPath, "pdf");
      cliOptions.OpenAfterConversion = openAfterConversion;
      options = markdown2PdfOptions;
    },
    inputPathArg,
    outputPathArg,
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
