using System.Diagnostics;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using Markdown2Pdf;
using Markdown2Pdf.Console;
using Markdown2Pdf.Options;
using PuppeteerSharp.Media;

var parser = new Parser(settings => {
  settings.CaseInsensitiveEnumValues = true;
  settings.CaseSensitive = false;
  settings.HelpWriter = null;
});

var result = parser.ParseArguments<Options>(args);
var options = result.Value;

if (result.Tag == ParserResultType.NotParsed) {
  DisplayHelp(result, result.Errors);
  return;
}

if (options.InputPath == null)
  return;

Console.WriteLine("Converting markdown to pdf...");

var outputPath = options.OutputPath ?? Path.ChangeExtension(options.InputPath, "pdf");
var currentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "node_modules");

var markdown2PdfOptions = _CreateMarkdown2PdfOptions(options);
var converter = new Markdown2PdfConverter(markdown2PdfOptions);
await converter.Convert(Path.GetFullPath(options.InputPath), Path.GetFullPath(outputPath));

Console.WriteLine($"Generated pdf at: {outputPath}");

// TODO: what if started from different directory?
if (options.OpenAfterConversion)
  Process.Start(new ProcessStartInfo { FileName = outputPath, UseShellExecute = true });

static Markdown2PdfOptions _CreateMarkdown2PdfOptions(Options options) {
  var markdown2PdfOptions = new Markdown2PdfOptions {
    ModuleOptions = ModuleOptions.Remote,
    // ModuleOptions = ModuleOptions.FromLocalPath(currentDir),
    KeepHtml = options.KeepHtml
  };

  if (options.HeaderPath != null) {
    if (!File.Exists(options.HeaderPath))
      throw new Exception($"The file '{options.HeaderPath}' doesn't exist!");

    markdown2PdfOptions.HeaderHtml = File.ReadAllText(options.HeaderPath);
  }

  if (options.FooterPath != null) {
    if (!File.Exists(options.FooterPath))
      throw new Exception($"The file '{options.FooterPath}' doesn't exist!");

    markdown2PdfOptions.FooterHtml = File.ReadAllText(options.FooterPath);
  }

  var marginOptions = options.MarginOptions;
  if (marginOptions != null) {
    markdown2PdfOptions.MarginOptions = new Markdown2Pdf.Options.MarginOptions {
      Top = marginOptions.Top,
      Bottom = marginOptions.Bottom,
      Left = marginOptions.Left,
      Right = marginOptions.Right
    };
  }

  markdown2PdfOptions.Theme = options.Theme.ToLower() switch {
    "github" => Theme.Github,
    "latex" => Theme.Latex,
    "" => Theme.None,
    _ => Theme.Custom(options.Theme),
  };

  markdown2PdfOptions.CodeHighlightTheme = GetPropertyValue<CodeHighlightTheme>(options.CodeHighlightTheme);

  markdown2PdfOptions.DocumentTitle = options.DocumentTitle;
  markdown2PdfOptions.CustomCss = options.CustomCss;
  markdown2PdfOptions.IsLandscape = options.IsLandscape;
  markdown2PdfOptions.Format = GetPropertyValue<PaperFormat>(options.Format);
  markdown2PdfOptions.Scale = options.Scale;

  if (options.TableOfContents != null) {
    var isOrdered = options.TableOfContents == TableOfContentsType.Ordered;
    markdown2PdfOptions.TableOfContents = new TableOfContents(isOrdered, options.TableOfContentsMaxDepth);
  }

  return markdown2PdfOptions;
}

static T GetPropertyValue<T>(string propertyName) {
  var property = typeof(T).GetProperty(propertyName,
  BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);

  if (property == null) {
    // TODO: dipslay help
    throw new ArgumentException();
  }

  return (T)property.GetValue(null, null)!;
}

static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors) {
  var helpText = HelpText.AutoBuild(result, h => {
    HelpText.DefaultParsingErrorsHandler(result, h);
    h.AddEnumValuesToHelpText = true;
    h.AddDashesToOption = true;
    return h;
  }, e => e, verbsIndex: true);

  Console.WriteLine(helpText);
}
