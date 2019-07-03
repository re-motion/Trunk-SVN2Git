// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.MessageFormatter
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Collections.Generic;

namespace Remotion.Validation.Implementation
{
  /// <summary>Assists in the construction of validation messages.</summary>
  public class MessageFormatter
  {
    private readonly Dictionary<string, object> _placeholderValues = new Dictionary<string, object>();
    private object[] _additionalArgs;

    /// <summary>Adds a value for a validation message placeholder.</summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public MessageFormatter AppendArgument (string name, object value)
    {
      _placeholderValues[name] = value;
      return this;
    }

    /// <summary>Appends a property name to the message.</summary>
    /// <param name="name">The name of the property</param>
    /// <returns></returns>
    public MessageFormatter AppendPropertyName (string name)
    {
      return AppendArgument ("PropertyName", name);
    }

    /// <summary>
    /// Adds additional arguments to the message for use with standard string placeholders.
    /// </summary>
    /// <param name="additionalArgs">Additional arguments</param>
    /// <returns></returns>
    public MessageFormatter AppendAdditionalArguments (params object[] additionalArgs)
    {
      _additionalArgs = additionalArgs;
      return this;
    }

    /// <summary>
    /// Constructs the final message from the specified template.
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>The message with placeholders replaced with their appropriate values</returns>
    public string BuildMessage (string messageTemplate)
    {
      var str = messageTemplate;
      foreach (var placeholderValue in _placeholderValues)
        str = ReplacePlaceholderWithValue (str, placeholderValue.Key, placeholderValue.Value);

      if (ShouldUseAdditionalArgs)
        return string.Format (str, _additionalArgs);

      return str;
    }

    private bool ShouldUseAdditionalArgs
    {
      get
      {
        if (_additionalArgs != null)
          return _additionalArgs.Length > 0;

        return false;
      }
    }

    private string ReplacePlaceholderWithValue (string template, string key, object value)
    {
      var oldValue = "{" + key + "}";
      return template.Replace (oldValue, value?.ToString());
    }
  }
}