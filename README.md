<h1 align="center">Markdown2Pdf.Console</h1>

<p align="center">
  <img src="./assets/md2pdf-console.svg" alt="Logo" Width=128px/>
  <br>
</p>

Command-line application for converting Markdown to PDF, using [Markdown2Pdf](https://github.com/Flayms/Markdown2Pdf).

<!-- TOC -->

## Download

Download build for Windows or Linux at [Releases](https://github.com/Flayms/Markdown2Pdf.Console/releases/latest)

## Getting started

Convert a Markdown-File:
```sh
md2pdf "README.md" # Outputs README.pdf
```

Open Help:
```sh
md2pdf --help # Displays all options
```

For detailed information please refer to the [Markdown2Pdf Wiki](https://github.com/Flayms/Markdown2Pdf/wiki).

## Usage

```
md2pdf <input-path> [<output-path>] [options]
```

## Arguments

```
Arguments:
  <input-path>   The path to the markdown file to parse.
  <output-path>  Path where the PDF file should be generated. If not set, defaults to <markdown-filename>.pdf.
```

## Options

```
-h, --header-path <header-path>                             Path to an html-file to use as the document-header.
-f, --footer-path <footer-path>                             Path to an html-file to use as the document-footer.
-o, --open-after-conversion                                 If enabled, opens the generated pdf after execution.
-m, --margins <margins>                                     Css-Margins for the content in the pdf to generate. Values must be comma-separated. [default: 50px]
-c, --chrome-path <chrome-path>                             Path to chrome or chromium executable. Downloads it by itself if not set.
-k, --keep-html                                             If this is set, the temporary html file does not get deleted.
-t, --theme <theme>                                         The theme to use for styling the document. Can either be a predefined value (github, latex) or a path to a custom css.
--code-highlight-theme <code-highlight-theme>               The theme to use for styling the markdown code-blocks. Valid Values: See https://github.com/Flayms/Markdown2Pdf/wiki/Markdown2Pdf.Options.CodeHighlightTheme for an overview of all themes.
--document-title <document-title>                           The title of this document. Can be injected into the header / footer by adding the class document-title to the element.
--custom-head-content <custom-head-content>                 A string containing any content valid inside an html <head> to apply extra scripting / styling to the document.
-l, --is-landscape                                          Paper orientation.
--format <format>                                           The paper format for the PDF. Valid values: Letter, Legal, Tabloid, Ledger, A0-A6
-s, --scale <scale>                                         Scale of the content. Must be between 0.1 and 2.
--toc-min-depth <toc-min-depth>                             The minimum level of heading depth to include in the TOC (e.g. 1 will only include headings greater than or equal to <h1>). Range: 1 to 6.
--toc-max-depth <toc-max-depth>                             The maximum level of heading depth to include in the TOC (e.g. 3 will include headings less than or equal to <h3>). Range: 1 to 6.
--toc-list-style <Decimal|None|OrderedDefault|Unordered>    Decides which characters to use before the TOC items.
--toc-has-colored-links                                     Determines if the TOC links should have the default link color (instead of looking  like normal text).
--toc-page-numbers-tab-leader <Dashes|Dots|None|Underline>  Generate TOC Page Numbers and use the given character to lead from the TOC title to the page number.
--version                                                   Show version information
-?, -h, --help                                              Show help and usage information
```

> [!NOTE]  
> Note: setting any of the --toc options will cause a TOC to be generated within the placeholders.

> [!TIP]
> All the options can also be directly embedded in the Markdown with a YAML front matter block. See [Example](https://github.com/Flayms/Markdown2Pdf/wiki/Markdown2Pdf.Markdown2PdfConverter#examples-1).

