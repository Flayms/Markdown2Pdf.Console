namespace Markdown2Pdf.Console;

internal class CliOptions {
  public FileInfo InputFile { get; set; } = null!;
  public FileInfo OutputFile { get; set; } = null!;
  public bool OpenAfterConversion { get; set; }
}
