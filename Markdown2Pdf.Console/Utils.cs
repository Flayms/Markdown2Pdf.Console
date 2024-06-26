﻿using System.CommandLine.Parsing;
using System.Reflection;

namespace Markdown2Pdf.Console;
internal static class Utils {
  public static void ValidateFileInfo(ArgumentResult result) {
    var file = result.GetValueOrDefault<FileInfo>();
    if (!file.Exists)
      result.ErrorMessage = $"File '{file.FullName}' does not exist.";
  }

  public static void ValidateFilePath(OptionResult result) {
    var filePath = result.GetValueOrDefault<string>()!;
    var fullPath = Path.GetFullPath(filePath);
    if (!File.Exists(fullPath))
      result.ErrorMessage = $"File '{fullPath}' does not exist.";
  }

  public static void ValidateBounds(OptionResult result, int lowerBound, int upperBound) {
    var value = result.GetValueOrDefault<int?>();
    if (value.HasValue && (value.Value < lowerBound || value.Value > upperBound))
      result.ErrorMessage = $"Value '{value}' is out of bounds. Must be between {lowerBound} and {upperBound}.";
  }

  public static string[] GetAllPublicPropertyNames<T>() {
    return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static).Select(p => p.Name).ToArray();
  }
}
