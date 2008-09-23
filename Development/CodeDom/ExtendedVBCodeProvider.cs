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
using System.Text;
using System.CodeDom;
using System.IO;

namespace Remotion.Development.CodeDom
{

public class ExtendedVBCodeProvider: ExtendedCodeProvider
{
	public ExtendedVBCodeProvider()
    : base (new Microsoft.VisualBasic.VBCodeProvider())
	{
	}

  public override string GetValidName(string name)
  {
    if (name == "ObjectClass")
      return "[" + name + "]";
    return name;
  }

  public override bool IsCaseSensitive
  {
    get { return false; }
  }

  public override CodeExpression CreateUnaryOperatorExpression (CodeUnaryOperatorType operatorType, CodeExpression expression)
  {
    StringBuilder sb = new StringBuilder();
    switch (operatorType)
    {
      case CodeUnaryOperatorType.BooleanNot:
        sb.Append ("(NOT (");
        break;
      case CodeUnaryOperatorType.Negate:
        sb.Append ("(- (");
        break;
      case CodeUnaryOperatorType.Plus:
        sb.Append ("(+ (");
        break;
    }
    StringWriter writer = new StringWriter (sb);
    Generator.GenerateCodeFromExpression (expression, writer, null);
    sb.Append ("))");
    return new CodeSnippetExpression (sb.ToString());
   }

  /// <summary>
  /// Adds a dummy constructor that can be referenced by other constructors.
  /// </summary>
  /// <remarks>
  /// This is a workaround for the VB CodeDOM bug that code for calling the base constructor
  /// is always generated although this is not valid for value types.
  /// Use <see cref="CreateStructConstructor"/> to define custom constructors.
  /// </remarks>
  public override CodeTypeDeclaration CreateStructWithConstructors(string name)
  {
    CodeTypeDeclaration type = base.CreateStructWithConstructors (name);
    CodeTypeMember dummyCtor = new CodeSnippetTypeMember (
        "' WORKAROUND: this dummy constructor can be called to avoid that CodeDOM generates an illegal call to the base class constructor"
        + "\n        Public Sub New (ByVal dummy1 as System.Int32, ByVal dummy2 as System.Double)"
        + "\n        End Sub"
        + "\n");
    type.Members.Add (dummyCtor);
    return type;
  }

  /// <summary>
  /// Calls the dummy constructor to avoid generation of a call to the base class constructor.
  /// </summary>
  /// <remarks>
  /// Use <see cref="CreateStructWithConstructors"/> to generate the dummy constructor.
  /// </remarks>
  public override CodeConstructor CreateStructConstructor()
  {
    CodeConstructor ctor = base.CreateStructConstructor ();
    ctor.ChainedConstructorArgs.Add (new CodeCastExpression (typeof (System.Int32), new CodePrimitiveExpression (0)));
    ctor.ChainedConstructorArgs.Add (new CodeCastExpression (typeof (System.Double), new CodePrimitiveExpression (0)));
    return ctor;
  }


}

}
