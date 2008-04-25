using System;
using System.Text;

namespace Remotion.Text.CommandLine
{

public class CommandLineFlagArgument: CommandLineArgument
{
  // fields

  private readonly bool? _defaultValue;
  private bool? _value;

  // construction and disposal

  public CommandLineFlagArgument (string name, bool? defaultValue)
    : base (name, true)
  {
    _defaultValue = defaultValue;
  }

  public CommandLineFlagArgument (string name)
    : base (name, true)
  {
    _defaultValue = null;
  }

  // properties and methods

  public bool? DefaultValue
  {
    get { return _defaultValue; }
  }

  protected internal override void SetStringValue (string value)
  {
    if (value == null) throw new ArgumentNullException ("value");

    switch (value)
    {
      case "": 
        _value = true;
        break;

      case "+":
        _value = true;
        break;
      
      case "-":
        _value = false;
        break;

      default:
        throw new InvalidCommandLineArgumentValueException (this, "Flag parameters support only + and - as arguments.");
    }

    base.SetStringValue (value);
  }


  public override object ValueObject
  {
    get { return Value; }
  }
  
  public bool? Value
  {
    get { return _value ?? _defaultValue; }
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (IsOptional && _defaultValue == false)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
    }
    else if (IsOptional && _defaultValue == true)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      sb.Append ("-");
    }
    else
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      sb.Append ("+ | /");
      sb.Append (Name);
      sb.Append ("-");
    }
  }
}

}
