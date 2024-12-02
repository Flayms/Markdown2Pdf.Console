using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace Markdown2Pdf.Console;
internal class CommandLineHelper(string[] args) {

  public delegate Task<ExitCode> Handler(CliOptions cliOptions, Markdown2PdfConverter options);
  private readonly CliSymbols _symbols = new();

  public async Task<ExitCode> Run(Handler handler) {
    var rootCommand = _CreateCommand(handler);
    var parser = new CommandLineBuilder(rootCommand)
      .UseDefaults()
      .Build();

    return (ExitCode)await parser.InvokeAsync(args);
  }

  private RootCommand _CreateCommand(Handler handler) {
    var symbols = this._symbols;

    var rootCommand = new RootCommand($"Command-line application for converting Markdown to Pdf.{Environment.NewLine}" +
      $"Note: setting any of the --toc options will cause a TOC to be generated within the placeholders.") {
      symbols.InputFileArg,
      symbols.OutputFileArg,
      symbols.FromYamlOption,
      symbols.HeaderPathOption,
      symbols.FooterPathOption,
      symbols.OpenAfterConversionOption,
      symbols.MarginOptionsOption,
      symbols.ChromePathOption,
      symbols.KeepHtmlOption,
      symbols.ThemeOption,
      symbols.CodeHighlightThemeOption,
      symbols.DocumentTitleOption,
      symbols.CustomHeadContentOption,
      symbols.IsLandscapeOption,
      symbols.FormatOption,
      symbols.ScaleOption,
      symbols.TocMinDepthOption,
      symbols.TocMaxDepthOption,
      symbols.TocListStyleOption,
      symbols.TocHasColoredLinksOption,
      symbols.TocPageNumberTabLeaderOption,
    };

    rootCommand.AddValidator(this._ValidateRootCommand);
    rootCommand.SetHandler(async (context) => await this._HandleCommand(context, handler));

    return rootCommand;
  }

  private void _ValidateRootCommand(CommandResult result) {
    var symbols = this._symbols;
    var yamlResult = result.Children.FirstOrDefault(s => s.Symbol == symbols.FromYamlOption);
    if (yamlResult is null)
      return;

    foreach (var child in result.Children) {
      // only option allowed together with yaml result is open-after-conversion
      if (child.Symbol == symbols.OpenAfterConversionOption || child == yamlResult || child.Symbol is not Option)
        continue;

      child.ErrorMessage = $"Option {child.Symbol.Name} Cannot be used together with --options-from-yaml-front-matter.";
    }
  }

  private async Task _HandleCommand(InvocationContext context, Handler handler) {
    var symbols = this._symbols;
    var parseResult = context.ParseResult;
    var inputFile = parseResult.GetValueForArgument(symbols.InputFileArg);
    var cliOptions = new CliOptions {
      InputFile = inputFile,
      OutputFile = parseResult.GetValueForArgument(symbols.OutputFileArg)
      ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, "pdf")),
    };

    var openAfterConversion = parseResult.GetValueForOption(symbols.OpenAfterConversionOption);
    if (openAfterConversion.HasValue)
      cliOptions.OpenAfterConversion = openAfterConversion.Value;

    var isParseFromYaml = parseResult!.GetValueForOption(symbols.FromYamlOption);

    var converter = (isParseFromYaml.HasValue && isParseFromYaml.Value == true)
      ? Markdown2PdfConverter.CreateWithInlineOptionsFromFile(inputFile)
      : new Markdown2PdfConverter(new OptionBinder(symbols).GetValue(context.BindingContext));

    var result = await handler(cliOptions, converter); // Runs actual logic here
    context.ExitCode = (int)result;
  }

}
