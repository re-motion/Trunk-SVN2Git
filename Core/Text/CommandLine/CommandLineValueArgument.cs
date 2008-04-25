using System;
using System.Globalization;
using System.Text;

namespace Remotion.Text.CommandLine
{

public abstract class CommandLineValueArgument: CommandLineArgument
{
  public CommandLineValueArgument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineValueArgument (bool isOptional)
    : base (isOptional)
  {
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (! IsPositional)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      if (Placeholder != null)
        sb.Append (Parser.Separator);
    }
    sb.Append (Placeholder);
  }
}

public class CommandLineStringArgument: CommandLineValueArgument
{
  public CommandLineStringArgument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineStringArgument (bool isOptional)
    : base (isOptional)
  {
  }

  public override object ValueObject
  {
    get { return Value; }
  }
  
  public string Value
  {
    get { return StringValue; }
  }
}

public class CommandLineInt32Argument: CommandLineValueArgument
{
  private int? _value;

  public CommandLineInt32Argument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineInt32Argument (bool isOptional)
    : base (isOptional)
  {
  }

  public override object ValueObject
  {
    get { return Value; }
  }

  public int? Value
  {
    get { return _value; }
  }

  protected internal override void SetStringValue (string value)
  {
    if (value == null) throw new ArgumentNullException ("value");
    string strValue = value.Trim();
    if (strValue.Length == 0)
    {
      _value = null;
    }
    else
    {
      double result;
      if (! double.TryParse (value, NumberStyles.Integer, null, out result))
        throw new InvalidCommandLineArgumentValueException (this, "Specify a valid integer number.");
      _value = (int) result;
    }

    base.SetStringValue (value);
  }

}

}
