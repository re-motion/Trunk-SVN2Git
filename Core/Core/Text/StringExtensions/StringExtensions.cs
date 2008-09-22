using System;

namespace Remotion.Text.StringExtensions
{
  public static class StringExtensions
  {
    /// <summary>
    /// Returns the substring of the passed string starting at the first character and ending at the passed seperator character, excluding the seperator character.
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
    /// Returns the substring of the passed string starting at the last character and ending at the passed seperator character, excluding the seperator character.
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


  }
}