using CommandLine;

namespace Markdown2Pdf.Console;
public class MarginOptions {

  // TODO: solve better
  public MarginOptions(string parameter) {
    var splitted = parameter.Split(',');

    switch (splitted.Length) {
      case 1:
        this.Top = this.Right = this.Bottom = this.Left = splitted[0];
        break;

      case 2:
        this.Top = this.Bottom = splitted[0];
        this.Right = this.Left = splitted[1];
        break;

      case 3:
        this.Top = splitted[0];
        this.Right = this.Left = splitted[1];
        this.Bottom = splitted[2];
        break;

      case 4:
        this.Top = splitted[0];
        this.Right = splitted[1];
        this.Bottom = splitted[2];
        this.Left = splitted[3];
        break;

      default:
        break;
    }
  }

  [Option]
  public string? Top { get; set; }

  [Option]
  public string? Right { get; set; }

  [Option]
  public string? Bottom { get; set; }

  [Option]
  public string? Left { get; set; }
}
