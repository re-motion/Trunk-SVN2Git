using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Text.CommandLine
{
  public class CommandLineClassParser: CommandLineParser
  {
    private readonly Type _argumentClass;

    /// <summary> IDictionary &lt;CommandLineArgument, MemberInfo&gt; </summary>
    private readonly IDictionary _arguments;
    
    public CommandLineClassParser (Type argumentClass)
    {
      _argumentClass = argumentClass;
      _arguments = new ListDictionary();

      foreach (MemberInfo member in argumentClass.GetMembers (BindingFlags.Public | BindingFlags.Instance))
      {
        if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
        {
          CommandLineArgumentAttribute argumentAttribute = (CommandLineArgumentAttribute) ReflectionUtility.GetSingleAttribute (
              member, typeof (CommandLineArgumentAttribute), false, false);
          if (argumentAttribute != null)
          {
            argumentAttribute.SetMember (member);
            argumentAttribute.AddArgument (Arguments, _arguments, member);
          }
        }
      }
    }

    public new object Parse (string commandLine, bool includeFirstArgument)
    {
      return Parse (SplitCommandLine (commandLine, includeFirstArgument));
    }

    public new object Parse (string[] args)
    {
      base.Parse (args);
      object obj = Activator.CreateInstance (_argumentClass);

      foreach (DictionaryEntry entry in _arguments)
      {
        CommandLineArgument argument = (CommandLineArgument) entry.Key;
        MemberInfo fieldOrProperty = (MemberInfo) entry.Value;
        Type memberType = ReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
        object value = argument.ValueObject;
        if (argument is ICommandLinePartArgument)
          value = ((ICommandLinePartArgument)argument).Group.ValueObject;

        if (memberType == typeof (bool))
        {
          if (value == null)
            throw new ApplicationException (string.Format ("{0} {1}: Cannot convert null to System.Boolean. Use Nullable<Boolean> type for optional attributes without default values.", fieldOrProperty.MemberType, fieldOrProperty.Name));
          else if (value is bool?)
            value = ((bool?) value).Value;
        }

        if (value != null)
        {
          try
          {
            ReflectionUtility.SetFieldOrPropertyValue (obj, fieldOrProperty, value);
          }
          catch (Exception e)
          {
            throw new ApplicationException (string.Format ("Error setting value of {0} {1}: {2}", fieldOrProperty.MemberType, fieldOrProperty.Name, e.Message), e);
          }
        }
      }
      return obj;
    }
  }

  public class CommandLineClassParser<T>: CommandLineClassParser
  {
    public CommandLineClassParser ()
        : base (typeof (T))
    {
    }

    public new T Parse (string commandLine, bool includeFirstArgument)
    {
      return (T) base.Parse (commandLine, includeFirstArgument);
    }

    public new T Parse (string[] args)
    {
      return (T) base.Parse (args);
    }
  }
}
