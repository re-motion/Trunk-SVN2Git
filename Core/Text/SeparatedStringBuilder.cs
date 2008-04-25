using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace Remotion.Text
{

/// <summary>
/// Builds a string adding separators between appended strings.
/// </summary>
public class SeparatedStringBuilder
{
  /// <summary>
  /// Appends the result of selector(item) for each item in the specified list.
  /// </summary>
  public static string Build<T> (string separator, IEnumerable<T> list, Func<T, string> selector)
  {
    SeparatedStringBuilder sb = new SeparatedStringBuilder (separator);
    foreach (T item in list)
      sb.Append (selector (item));
    return sb.ToString ();
  }

  /// <summary>
  /// Appends each item in the specified list.
  /// </summary>
  public static string Build<T> (string separator, IEnumerable<T> list)
  {
    SeparatedStringBuilder sb = new SeparatedStringBuilder (separator);
    foreach (T item in list)
      sb.Append (item);
    return sb.ToString ();
  }

  /// <summary>
  /// Appends each item in the specified list.
  /// </summary>
  public static string Build<T> (string separator, IEnumerable list)
  {
    SeparatedStringBuilder sb = new SeparatedStringBuilder (separator);
    foreach (object obj in list)
      sb.Append (obj);
    return sb.ToString ();
  }

  private string _separator;
  private StringBuilder _stringBuilder;

	public SeparatedStringBuilder (string separator, int capacity)
	{
    _stringBuilder = new StringBuilder (capacity);
    _separator = separator;
	}

  public SeparatedStringBuilder (string separator)
  {
    _stringBuilder = new StringBuilder ();
    _separator = separator;
  }

  public void Append (string s)
  {
    AppendSeparator();
    _stringBuilder.Append (s);
  }

  public void Append<T> (T arg)
  {
    AppendSeparator();
    _stringBuilder.Append (arg.ToString());
  }

  public void AppendFormat (string format, params object[] args)
  {
    AppendSeparator();
    _stringBuilder.AppendFormat (format, args);
  }

  private void AppendSeparator()
  {
    if (_stringBuilder.Length > 0)
      _stringBuilder.Append (_separator);
  }

  public int Length
  {
    get { return _stringBuilder.Length; }
  }

  public override string ToString()
  {
    return _stringBuilder.ToString();
  }

}

}
