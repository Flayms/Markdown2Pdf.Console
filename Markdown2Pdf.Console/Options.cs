namespace Markdown2Pdf.Console;

internal class Options {
  public FileInfo InputPath { get; set; } = null!;
  public FileInfo OutputPath { get; set; } = null!;
  public bool OpenAfterConversion { get; set; }
}
