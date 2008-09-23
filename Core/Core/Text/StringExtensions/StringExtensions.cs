﻿using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Diagnostics.ToText;

namespace Remotion.Text.StringExtensions
{
  public static class StringExtensions
  {
    private static readonly Dictionary<char, string> _escapeStringMapping = new Dictionary<char, string> { { '"', "\\\"" }, { '\n', "\\n" }, { '\r', "\\r" }, { '\t', "\\t" }, { '\\', "\\\\" }, { '\b', "\\b" }, { '\v', "\\v" }, { '\f', "\\f" } };

    /// <summary>
    /// Returns the substring of the passed string starting at the first character and 
    /// ending at the passed seperator character, excluding the seperator character.
    /// </summary>
    public static string LeftUntilChar (this string s, char separator)
    {
      int iSeparator = s.IndexOf (separator);
      if (iSeparator >= 0)
      {
        return s.Substring (0, iSeparator);
      }
      else
      {
        return s;
      }
    }

    /// <summary>
    /// Returns the substring of the passed string starting at the last character and ending at 
    /// the passed seperator character, excluding the seperator character.
    /// </summary>
    public static string RightUntilChar (this string s, char separator)
    {
      int iSeparator = s.LastIndexOf (separator);
      if (iSeparator >= 0)
      {
        return s.Substring (iSeparator + 1, s.Length - iSeparator - 1);
      }
      else
      {
        return s;
      }
    }


  /// <summary>
  /// Appends the passed string to the passed <see cref="StringBuilder"/>, replacing all tabs,newlines, linefeeds, etc 
  /// with their escaped C# string representation. E.g. tabulator => \t .
  /// See also <see cref="EscapeString(string)"/>.
  /// </summary>
  public static void EscapeString (this string s, StringBuilder stringBuilder)
    {
      foreach (char c in s)
      {
        string mappedString;
        _escapeStringMapping.TryGetValue (c, out mappedString);
        if (mappedString == null)
        {
          stringBuilder.Append (c);
        }
        else
        {
          stringBuilder.Append (mappedString);
        }
      }
    }

    /// <summary>
    /// Returns the passed string with all tabs, newlines, linefeeds, etc 
    /// replaced with their escaped C# string representation. E.g. tabulator => \t .
    /// See also <see cref="EscapeString(string,StringBuilder)"/>.
    /// </summary>
    public static string EscapeString (this string s)
    {
      var stringBuilder = new StringBuilder();
      foreach (char c in s)
      {
        string mappedString;
        _escapeStringMapping.TryGetValue (c, out mappedString);
        if (mappedString == null)
        {
          stringBuilder.Append (c);
        }
        else
        {
          stringBuilder.Append (mappedString);
        }
      }
      return stringBuilder.ToString();
    }
  }
}