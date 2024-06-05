using System.CommandLine.Binding;
using System.CommandLine;
using Markdown2Pdf.Options;
using PuppeteerSharp.Media;
using Markdown2Pdf.Services;
using System.CommandLine.Parsing;

namespace Markdown2Pdf.Console;
internal class OptionBinder(CliSymbols symbols) : BinderBase<Markdown2PdfOptions> {
  private ParseResult? _parseResult;

  public Markdown2PdfOptions GetValue(BindingContext bindingContext) => this.GetBoundValue(bindingContext);

  protected override Markdown2PdfOptions GetBoundValue(BindingContext bindingContext) {
    var parseResult = this._parseResult = bindingContext.ParseResult;
    var modulesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "node_modules"); // Always the same for the Console App

    var options = new Markdown2PdfOptions {
      ModuleOptions = ModuleOptions.FromLocalPath(modulesDir)
    };

    this._HandleOption(symbols.HeaderPathOption, value => options.HeaderHtml = File.ReadAllText(value));
    this._HandleOption(symbols.FooterPathOption, value => options.FooterHtml = File.ReadAllText(value));


    this._HandleOption(symbols.MarginOptionsOption, value => {
      options.MarginOptions = new Markdown2Pdf.Options.MarginOptions() {
        Bottom = value.Bottom,
        Left = value.Left,
        Right = value.Right,
        Top = value.Top
      };
    });

    this._HandleOption(symbols.MetadataTitleOption, value => options.MetadataTitle = value);
    this._HandleOption(symbols.ChromePathOption, value => options.ChromePath = value);
    this._HandleOption(symbols.KeepHtmlOption, value => options.KeepHtml = value!.Value);
    this._HandleOption(symbols.ThemeOption, value => {
        options.Theme = PropertyService.TryGetPropertyValue<Theme>(value, out var theme)
      ? theme
      : Theme.Custom(value);
    });
    this._HandleOption(symbols.CodeHighlightThemeOption, codeHighlightThemeName => {
      options.CodeHighlightTheme = PropertyService.TryGetPropertyValue<CodeHighlightTheme>(codeHighlightThemeName, out var codeHighlightTheme)
        ? codeHighlightTheme
        : throw new ArgumentException(); // TODO: better error message
    });

    this._HandleOption(symbols.DocumentTitleOption, value => options.DocumentTitle = value);
    this._HandleOption(symbols.EnableAutoLanguageDetectionOption, value => options.EnableAutoLanguageDetection = value!.Value);
    this._HandleOption(symbols.CustomHeadContentOption, value => options.CustomHeadContent = value);
    this._HandleOption(symbols.IsLandscapeOption, value => options.IsLandscape = value!.Value);
    this._HandleOption(symbols.FormatOption, value => {
      options.Format = PropertyService.TryGetPropertyValue<PaperFormat>(value, out var paperFormat)
        ? paperFormat
        : throw new Exception(); // TODO: support
    });
    this._HandleOption(symbols.ScaleOption, value => options.Scale = value!.Value);
    this._HandleToc(options);

    return options;
  }

  private void _HandleToc(Markdown2PdfOptions options) {
    TableOfContentsOptions GetToc() => options.TableOfContents ??= new TableOfContentsOptions();
    PageNumberOptions GetPageNumberOptions() => GetToc().PageNumberOptions ??= new PageNumberOptions();

    this._HandleOption(symbols.TocMinDepthOption, value => GetToc().MinDepthLevel = value!.Value);
    this._HandleOption(symbols.TocMaxDepthOption, value => GetToc().MaxDepthLevel = value!.Value);
    this._HandleOption(symbols.TocListStyleOption, value => GetToc().ListStyle = value!.Value);
    this._HandleOption(symbols.TocHasColoredLinksOption, value => GetToc().HasColoredLinks = value!.Value);
    this._HandleOption(symbols.TocPageNumberTabLeaderOption, value => GetPageNumberOptions().TabLeader = value!.Value);
  }

  private void _HandleOption<T>(Option<T?> option, Action<T> setter) {
    var value = this._parseResult!.GetValueForOption(option);

    if (value != null)
      setter.Invoke(this._parseResult.GetValueForOption(option)!);
  }

}
