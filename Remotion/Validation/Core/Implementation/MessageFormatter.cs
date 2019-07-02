// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.MessageFormatter
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System.Collections.Generic;

namespace Remotion.Validation.Implementation
{
  /// <summary>Assists in the construction of validation messages.</summary>
  public class MessageFormatter
  {
    private readonly Dictionary<string, object> placeholderValues = new Dictionary<string, object>();
    /// <summary>Default Property Name placeholder.</summary>
    public const string PropertyName = "PropertyName";
    private object[] additionalArgs;

    /// <summary>Adds a value for a validation message placeholder.</summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public MessageFormatter AppendArgument(string name, object value)
    {
      this.placeholderValues[name] = value;
      return this;
    }

    /// <summary>Appends a property name to the message.</summary>
    /// <param name="name">The name of the property</param>
    /// <returns></returns>
    public MessageFormatter AppendPropertyName(string name)
    {
      return this.AppendArgument("PropertyName", (object)name);
    }

    /// <summary>
    /// Adds additional arguments to the message for use with standard string placeholders.
    /// </summary>
    /// <param name="additionalArgs">Additional arguments</param>
    /// <returns></returns>
    public MessageFormatter AppendAdditionalArguments(params object[] additionalArgs)
    {
      this.additionalArgs = additionalArgs;
      return this;
    }

    /// <summary>
    /// Constructs the final message from the specified template.
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>The message with placeholders replaced with their appropriate values</returns>
    public string BuildMessage(string messageTemplate)
    {
      string str = messageTemplate;
      foreach (KeyValuePair<string, object> placeholderValue in this.placeholderValues)
        str = this.ReplacePlaceholderWithValue(str, placeholderValue.Key, placeholderValue.Value);
      if (this.ShouldUseAdditionalArgs)
        return string.Format(str, this.additionalArgs);
      return str;
    }

    private bool ShouldUseAdditionalArgs
    {
      get
      {
        if (this.additionalArgs != null)
          return this.additionalArgs.Length > 0;
        return false;
      }
    }

    private string ReplacePlaceholderWithValue(string template, string key, object value)
    {
      string oldValue = "{" + key + "}";
      return template.Replace(oldValue, value == null ? (string)null : value.ToString());
    }
  }
}