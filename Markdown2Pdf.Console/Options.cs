namespace Markdown2Pdf.Console;

internal class Options {
  public FileInfo InputFile { get; set; } = null!;
  public FileInfo OutputFile { get; set; } = null!;
  public bool OpenAfterConversion { get; set; }
}
