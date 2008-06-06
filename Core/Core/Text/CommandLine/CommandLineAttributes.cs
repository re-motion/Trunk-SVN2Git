/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Text.CommandLine
{
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public abstract class CommandLineArgumentAttribute : Attribute
  {
    private readonly CommandLineArgument _argument;

    #region dummy constructor

    /// <summary> do not use this constructor </summary>
    /// <remarks> 
    ///   This constructor is necessary because, even in <see langword="abstract"/> attribute classes, one constructor 
    ///   must have arguments that meet the constraints of attribute declarations. 
    /// </remarks>
    [Obsolete ("Do not use this constructor.", true)]
    protected CommandLineArgumentAttribute (int doNotUseThisConstructor)
    {
      throw new NotSupportedException();
    }

    #endregion

    protected CommandLineArgumentAttribute (CommandLineArgument argument)
    {
      _argument = argument;
    }

    public string Name
    {
      get { return _argument.Name; }
      set { _argument.Name = value; }
    }

    public bool IsOptional
    {
      get { return _argument.IsOptional; }
      set { _argument.IsOptional = value; }
    }

    public string Placeholder
    {
      get { return _argument.Placeholder; }
      set { _argument.Placeholder = value; }
    }

    public string Description
    {
      get { return _argument.Description; }
      set { _argument.Description = value; }
    }

    public CommandLineArgument Argument
    {
      get { return _argument; }
    }

    public virtual void SetMember (MemberInfo fieldOrProperty)
    {
    }

    public virtual void AddArgument (CommandLineArgumentCollection argumentCollection, IDictionary dictionary, MemberInfo member)
    {
      argumentCollection.Add (this.Argument);
      dictionary.Add (this.Argument, member);
    }
  }

  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineStringArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineStringArgumentAttribute (bool isOptional)
        : base (new CommandLineStringArgument (isOptional))
    {
    }

    public CommandLineStringArgumentAttribute (string name, bool isOptional)
        : base (new CommandLineStringArgument (name, isOptional))
    {
    }
  }

  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineFlagArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineFlagArgumentAttribute (string name)
        : base (new CommandLineFlagArgument (name))
    {
    }

    public CommandLineFlagArgumentAttribute (string name, bool defaultValue)
        : base (new CommandLineFlagArgument (name, defaultValue))
    {
    }
  }


  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineInt32ArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineInt32ArgumentAttribute (string name, bool isOptional)
        : base (new CommandLineInt32Argument (name, isOptional))
    {
    }

    public CommandLineInt32ArgumentAttribute (bool isOptional)
        : base (new CommandLineInt32Argument (isOptional))
    {
    }
  }

  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineEnumArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineEnumArgumentAttribute (bool isOptional)
        : base (new CommandLineEnumArgument (isOptional, null))
    {
    }

    public CommandLineEnumArgumentAttribute (string name, bool isOptional)
        : base (new CommandLineEnumArgument (name, isOptional, null))
    {
    }

    public override void SetMember (MemberInfo fieldOrProperty)
    {
      Type enumType = ReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
      if (! enumType.IsEnum)
      {
        throw new ApplicationException (
            string.Format (
                "Attribute {0} can only be applied to enumeration fields or properties.", typeof (CommandLineEnumArgumentAttribute).FullName));
      }
      ((CommandLineEnumArgument) Argument).EnumType = enumType;
    }
  }

  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineModeArgumentAttribute : CommandLineArgumentAttribute
  {
    private Type _enumType;

    public CommandLineModeArgumentAttribute (bool isOptional)
        : base (new CommandLineModeArgument (isOptional, null))
    {
    }

    public new CommandLineModeArgument Argument
    {
      get { return (CommandLineModeArgument) base.Argument; }
    }

    public override void SetMember (MemberInfo fieldOrProperty)
    {
      _enumType = ReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
      if (! _enumType.IsEnum)
      {
        throw new ApplicationException (
            string.Format (
                "Attribute {0} can only be applied to enumeration fields or properties.", typeof (CommandLineEnumArgumentAttribute).FullName));
      }

      Argument.EnumType = _enumType;
      Argument.CreateChildren();
    }

    public override void AddArgument (CommandLineArgumentCollection argumentCollection, IDictionary dictionary, MemberInfo member)
    {
      if (_enumType == null)
        throw new InvalidOperationException ("SetMember must be called before AddArgument");

      foreach (CommandLineModeFlagArgument flag in Argument.Parts)
      {
        argumentCollection.Add (flag);
        dictionary.Add (flag, member);
      }
      argumentCollection.Add (Argument);
    }
  }

  [AttributeUsage (AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public class CommandLineModeAttribute : Attribute
  {
    public static CommandLineModeAttribute GetAttribute (FieldInfo field)
    {
      return (CommandLineModeAttribute) AttributeUtility.GetCustomAttribute (field, typeof (CommandLineModeAttribute), false);
    }

    private string _name;
    private string _description;

    public CommandLineModeAttribute (string name)
    {
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }
  }
}
