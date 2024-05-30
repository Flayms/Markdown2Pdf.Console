using System.CommandLine.Binding;
using System.CommandLine;
using Markdown2Pdf.Options;
using PuppeteerSharp.Media;
using Markdown2Pdf.Services;
using System.CommandLine.Parsing;

namespace Markdown2Pdf.Console;
internal class OptionBinder(
  Option<string?> headerPathOption,
  Option<string?> footerPathOption,
  Option<MarginOptions?> marginOptionsOption,
  Option<string?> metadataTitle,
  Option<string?> chromePathOption,
  Option<bool?> keepHtmlOption,
  Option<string?> themeOption,
  Option<string?> codeHighlightThemeOption,
  Option<string?> documentTitleOption,
  Option<bool?> enableAutoLanguageDetectionOption,
  Option<string?> customHeadContentOption,
  Option<bool?> isLandscapeOption,
  Option<string?> formatOption,  
  Option<decimal?> scaleOption,
  Option<int?> tocMinDepthOption,
  Option<int?> tocMaxDepthOption,
  Option<ListStyle?> tocListStyleOption,
  Option<bool?> tocHasColoredLinksOption,
  Option<Leader?> tocPageNumberTabLeaderOption
  )
  : BinderBase<Markdown2PdfOptions> {

  private readonly Option<string?> _headerPathOption = headerPathOption;
  private readonly Option<string?> _footerPathOption = footerPathOption;
  private readonly Option<MarginOptions?> _marginOptionsOption = marginOptionsOption;
  private readonly Option<string?> _metadataTitleOption = metadataTitle;
  private readonly Option<string?> _chromePathOption = chromePathOption;
  private readonly Option<bool?> _keepHtmlOption = keepHtmlOption;
  private readonly Option<string?> _themeOption = themeOption;
  private readonly Option<string?> _codeHighlightThemeOption = codeHighlightThemeOption;
  private readonly Option<string?> _documentTitleOption = documentTitleOption;
  private readonly Option<bool?> _enableAutoLanguageDetectionOption = enableAutoLanguageDetectionOption;
  private readonly Option<string?> _customHeadContentOption = customHeadContentOption;
  private readonly Option<bool?> _isLandscapeOption = isLandscapeOption;
  private readonly Option<string?> _formatOption = formatOption;
  private readonly Option<decimal?> _scaleOption = scaleOption;
  private readonly Option<int?> _tocMinDepthOption = tocMinDepthOption;
  private readonly Option<int?> _tocMaxDepthOption = tocMaxDepthOption;
  private readonly Option<ListStyle?> _tocListStyleOption = tocListStyleOption;
  private readonly Option<bool?> _tocHasColoredLinksOption = tocHasColoredLinksOption;
  private readonly Option<Leader?> _tocPageNumberTabLeaderOption = tocPageNumberTabLeaderOption;
  private ParseResult? _parseResult;

  public Markdown2PdfOptions GetValue(BindingContext bindingContext) => this.GetBoundValue(bindingContext);

  protected override Markdown2PdfOptions GetBoundValue(BindingContext bindingContext) {
    var parseResult = this._parseResult = bindingContext.ParseResult;
    var modulesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "node_modules"); // Always the same for the Console App

    var options = new Markdown2PdfOptions {
      ModuleOptions = ModuleOptions.FromLocalPath(modulesDir)
    };

    this._HandleOption(_headerPathOption, value => options.HeaderHtml = File.ReadAllText(value));
    this._HandleOption(_footerPathOption, value => options.FooterHtml = File.ReadAllText(value));


    this._HandleOption(_marginOptionsOption, value => {
      options.MarginOptions = new Markdown2Pdf.Options.MarginOptions() {
        Bottom = value.Bottom,
        Left = value.Left,
        Right = value.Right,
        Top = value.Top
      };
    });

    this._HandleOption(_metadataTitleOption, value => options.MetadataTitle = value);
    this._HandleOption(_chromePathOption, value => options.ChromePath = value);
    this._HandleOption(_keepHtmlOption, value => options.KeepHtml = value!.Value);
    this._HandleOption(_themeOption, value => {
        options.Theme = PropertyService.TryGetPropertyValue<Theme>(value, out var theme)
      ? theme
      : Theme.Custom(value);
    });
    this._HandleOption(_codeHighlightThemeOption, codeHighlightThemeName => {
      options.CodeHighlightTheme = PropertyService.TryGetPropertyValue<CodeHighlightTheme>(codeHighlightThemeName, out var codeHighlightTheme)
        ? codeHighlightTheme
        : throw new ArgumentException(); // TODO: better error message
    });

    this._HandleOption(_documentTitleOption, value => options.DocumentTitle = value);
    this._HandleOption(_enableAutoLanguageDetectionOption, value => options.EnableAutoLanguageDetection = value!.Value);
    this._HandleOption(_customHeadContentOption, value => options.CustomHeadContent = value);
    this._HandleOption(_isLandscapeOption, value => options.IsLandscape = value!.Value);
    this._HandleOption(_formatOption, value => {
      options.Format = PropertyService.TryGetPropertyValue<PaperFormat>(value, out var paperFormat)
        ? paperFormat
        : throw new Exception(); // TODO: support
    });
    this._HandleOption(_scaleOption, value => options.Scale = value!.Value);
    this._HandleToc(options);

    return options;
  }

  private void _HandleToc(Markdown2PdfOptions options) {
    TableOfContentsOptions GetToc() => options.TableOfContents ??= new TableOfContentsOptions();
    PageNumberOptions GetPageNumberOptions() => GetToc().PageNumberOptions ??= new PageNumberOptions();

    this._HandleOption(_tocMinDepthOption, value => GetToc().MinDepthLevel = value!.Value);
    this._HandleOption(_tocMaxDepthOption, value => GetToc().MaxDepthLevel = value!.Value);
    this._HandleOption(_tocListStyleOption, value => GetToc().ListStyle = value!.Value);
    this._HandleOption(_tocHasColoredLinksOption, value => GetToc().HasColoredLinks = value!.Value);
    this._HandleOption(_tocPageNumberTabLeaderOption, value => GetPageNumberOptions().TabLeader = value!.Value);
  }

  private void _HandleOption<T>(Option<T?> option, Action<T> setter) {
    var value = this._parseResult!.GetValueForOption(option);

    if (value != null)
      setter.Invoke(this._parseResult.GetValueForOption(option)!);
  }

}
