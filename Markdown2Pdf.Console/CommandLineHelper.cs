using System.Reflection;
using CommandLine;
using CommandLine.Text;
using Markdown2Pdf.Options;
using PuppeteerSharp.Media;

namespace Markdown2Pdf.Console;
internal class CommandLineHelper(string[] args) {

  private readonly string[] _args = args;
  private ParserResult<Options>? _parserResult;

  public bool TryCreateOptions(out Options cliOptions, out Markdown2PdfOptions options) {
    options = null!;

    var parser = new Parser(settings => {
      settings.CaseInsensitiveEnumValues = true;
      settings.CaseSensitive = false;
      settings.HelpWriter = null;
      settings.AutoVersion = true;
    });

    var result = this._parserResult = parser.ParseArguments<Options>(this._args);
    cliOptions = result.Value;

    if (result.Tag == ParserResultType.NotParsed) {
      this._DisplayHelp();
      return false;
    }

    if (!this._TryCleanupCliOptions(cliOptions))
      return false;

    if (!this._TryCreateMarkdown2PdfOptions(cliOptions, out options))
      return false;

    return true;
  }

  private bool _TryCleanupCliOptions(Options options) {
    if (options.InputPath == null) {
      this._DisplayHelp("The input path is required.");
      return false;
    }

    options.OutputPath ??= Path.ChangeExtension(options.InputPath, "pdf");
    // var currentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "node_modules");

    return true;
  }

  private bool _TryCreateMarkdown2PdfOptions(Options cliOptions, out Markdown2PdfOptions options) {
    options = new Markdown2PdfOptions {
      ModuleOptions = ModuleOptions.Remote,
      // ModuleOptions = ModuleOptions.FromLocalPath(currentDir),
      KeepHtml = cliOptions.KeepHtml
    };

    if (cliOptions.HeaderPath != null) {
      if (!File.Exists(cliOptions.HeaderPath)) {
        this._DisplayHelp($"The file '{cliOptions.HeaderPath}' doesn't exist!");
        return false;
      }

      options.HeaderHtml = File.ReadAllText(cliOptions.HeaderPath);
    }

    if (cliOptions.FooterPath != null) {
      if (!File.Exists(cliOptions.FooterPath)) {
        this._DisplayHelp($"The file '{cliOptions.FooterPath}' doesn't exist!");
        return false;
      }

      options.FooterHtml = File.ReadAllText(cliOptions.FooterPath);
    }

    var marginOptions = cliOptions.MarginOptions;
    if (marginOptions != null) {
      options.MarginOptions = new Markdown2Pdf.Options.MarginOptions {
        Top = marginOptions.Top,
        Bottom = marginOptions.Bottom,
        Left = marginOptions.Left,
        Right = marginOptions.Right
      };
    }

    options.Theme = cliOptions.Theme.ToLower() switch {
      "github" => Theme.Github,
      "latex" => Theme.Latex,
      "" => Theme.None,
      _ => Theme.Custom(cliOptions.Theme),
    };

    if (!this._TryGetPropertyValue<CodeHighlightTheme>(cliOptions.CodeHighlightTheme, out var codeHighlightTheme))
      return false;

    options.CodeHighlightTheme = codeHighlightTheme;
    options.DocumentTitle = cliOptions.DocumentTitle;
    options.CustomHeadContent = cliOptions.CustomHeadContent;
    options.IsLandscape = cliOptions.IsLandscape;

    if (!this._TryGetPropertyValue<PaperFormat>(cliOptions.Format, out var paperFormat))
      return false;

    options.Format = paperFormat;
    options.Scale = cliOptions.Scale;

    if (cliOptions.TableOfContents != null) {
      var isOrdered = cliOptions.TableOfContents == TableOfContentsType.Ordered;
      options.TableOfContents = new TableOfContents(isOrdered, cliOptions.TableOfContentsMaxDepth);
    }

    return true;
  }

  private void _DisplayHelp(string? errorMessage = null) {
    var result = this._parserResult!;

    var helpText = HelpText.AutoBuild(result,
      onError: h => {
        _ = HelpText.DefaultParsingErrorsHandler(result, h);
        h.AddEnumValuesToHelpText = true;
        h.AddDashesToOption = true;
        return h;
      }, onExample: e => e, verbsIndex: true);

    if (errorMessage != null)
      helpText.AddPreOptionsText($"ERROR(S):{Environment.NewLine}\t{errorMessage}");

    var appName = Assembly.GetExecutingAssembly().GetName().Name;
    helpText.Copyright = string.Empty;
    helpText.Heading = new HeadingInfo(appName, Assembly.GetExecutingAssembly().GetName().Version!.ToString(3));

    helpText.AddPreOptionsText($"Usage: {appName} <input-path> [output-path] [options]");

    System.Console.WriteLine(helpText);
  }

  private bool _TryGetPropertyValue<T>(string propertyName, out T propertyValue) {
    var property = typeof(T).GetProperty(propertyName,
    BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);

    if (property == null) {
      this._DisplayHelp($"'{propertyName}' is not a valid value for this option.");
      propertyValue = default!;
      return false;
    }

    propertyValue = (T)property.GetValue(null, null)!;
    return true;
  }

}
