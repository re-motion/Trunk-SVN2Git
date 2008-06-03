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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace Remotion.Development.CodeDom
{

public class ExtendedCSharpCodeProvider: ExtendedCodeProvider
{
  // constants

  private static readonly MemberAttributeKeywordMapping[] c_memberAttributeKeywordMappings = new MemberAttributeKeywordMapping[] {
      new MemberAttributeKeywordMapping (MemberAttributes.Abstract, MemberAttributes.ScopeMask, "abstract"),
      new MemberAttributeKeywordMapping (MemberAttributes.Const, MemberAttributes.ScopeMask, "const"),
      new MemberAttributeKeywordMapping (MemberAttributes.Assembly, MemberAttributes.AccessMask, "internal"),
      new MemberAttributeKeywordMapping (MemberAttributes.Family, MemberAttributes.AccessMask, "protected"),
      new MemberAttributeKeywordMapping (MemberAttributes.FamilyOrAssembly, MemberAttributes.AccessMask, "internal protected"),
      new MemberAttributeKeywordMapping (MemberAttributes.Final, MemberAttributes.ScopeMask, "sealed"),
      new MemberAttributeKeywordMapping (MemberAttributes.New, MemberAttributes.VTableMask, "new"),
      new MemberAttributeKeywordMapping (MemberAttributes.Override, (MemberAttributes) 0, "override"),
      new MemberAttributeKeywordMapping (MemberAttributes.Private, MemberAttributes.AccessMask, "private"),
      new MemberAttributeKeywordMapping (MemberAttributes.Public, MemberAttributes.AccessMask, "public"),
      new MemberAttributeKeywordMapping (MemberAttributes.Static, MemberAttributes.ScopeMask, "static") };

  // construction and disposal

  public ExtendedCSharpCodeProvider ()
    : base (new Microsoft.CSharp.CSharpCodeProvider())
  {
  }

  // properties and methods

  public override bool SupportsCastingOperators
  {
    get { return true; }
  }

  public override CodeTypeMember CreateCastingOperator ( 
      string fromType, string toType, string argumentName, CodeStatementCollection statements, 
      MemberAttributes attributes, CodeCastOperatorKind castOperatorKind)
  {
    StringBuilder sb = new StringBuilder ();

    AppendMemberAttributeString (sb, c_memberAttributeKeywordMappings, attributes);
    if (castOperatorKind == CodeCastOperatorKind.Implicit)
      sb.Append (" implicit operator ");
    else
      sb.Append (" explicit operator ");

    sb.Append (toType);
    sb.Append (" (");
    sb.Append (fromType);
    sb.Append (" ");
    sb.Append (argumentName);
    sb.Append (") {");

    StringWriter writer = new StringWriter (sb);
    // CodeGeneratorOptions options  = new CodeGeneratorOptions ();

    foreach (CodeStatement statement in statements)
      Generator.GenerateCodeFromStatement (statement, writer, null);

    sb.Append ("        }");

    return new CodeSnippetTypeMember (sb.ToString());
  }

  public override bool SupportsOperatorOverriding
  {
    get { return true; }
  }

  public override CodeTypeMember CreateBinaryOperator (
      string argumentTypeName, string firstArgumentName, string secondArgumentName, 
      CodeOverridableOperatorType operatorType, string returnTypeName,
      CodeStatementCollection statements, MemberAttributes attributes)
  {
    StringBuilder sb = new StringBuilder ();

    AppendMemberAttributeString (sb, c_memberAttributeKeywordMappings, attributes);
    sb.Append (" ");
    sb.Append (returnTypeName);
    sb.Append (" operator ");
    sb.Append (GetOverridableOperatorString (operatorType));
    sb.Append (" (");
    sb.Append (argumentTypeName);
    sb.Append (" ");
    sb.Append (firstArgumentName);
    sb.Append (", ");
    sb.Append (argumentTypeName);
    sb.Append (" ");
    sb.Append (secondArgumentName);
    sb.Append (") {");

    StringWriter writer = new StringWriter (sb);
    // CodeGeneratorOptions options  = new CodeGeneratorOptions ();

    foreach (CodeStatement statement in statements)
      Generator.GenerateCodeFromStatement (statement, writer, null);

    sb.Append ("        }");

    return new CodeSnippetTypeMember (sb.ToString());
  }



  public override bool SupportsDocumentationComments
  {
    get { return true; }
  }

  public override void AddOptionCreateXmlDocumentation(CompilerParameters parameters, string xmlFilename, bool missingXmlWarnings)
  {
    StringBuilder sb = new StringBuilder (parameters.CompilerOptions);
    sb.Append (" /doc:" + xmlFilename);
    if (! missingXmlWarnings)
      sb.Append (" /nowarn:1591");
    parameters.CompilerOptions = sb.ToString();
  }

  public override string GetValidName (string name)
  {
    if (name == "params")
      return "@" + name;
    return name;
  }

  public override bool IsCaseSensitive
  {
    get { return true; }
  }

  public override CodeExpression CreateUnaryOperatorExpression (CodeUnaryOperatorType operatorType, CodeExpression expression)
  {
    StringBuilder sb = new StringBuilder();
    switch (operatorType)
    {
      case CodeUnaryOperatorType.BooleanNot:
        sb.Append ("(! (");
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

  private string GetOverridableOperatorString (CodeOverridableOperatorType operatorType)
  {
    switch (operatorType)
    {
      case CodeOverridableOperatorType.Equality:
        return "==";
      case CodeOverridableOperatorType.Inequality:
        return "!=";
      case CodeOverridableOperatorType.BitwiseAnd:
        return "&";
      case CodeOverridableOperatorType.BooleanOr:
        return "|";
      case CodeOverridableOperatorType.GreaterThan:
        return ">";
      case CodeOverridableOperatorType.GreaterThanOrEqual:
        return ">=";
      case CodeOverridableOperatorType.LessThan:
        return "<";
      case CodeOverridableOperatorType.LessThanOrEqual:
        return "<=";
      case CodeOverridableOperatorType.Add:
        return "+";
      case CodeOverridableOperatorType.Multiply:
        return "*";
      case CodeOverridableOperatorType.Subtract:
        return "-";
      case CodeOverridableOperatorType.Divide:
        return "/";
      case CodeOverridableOperatorType.Modulus:
        return "%";
      default:
        throw new ArgumentException ("Invalid CodeOverridableOperatorType value: " + operatorType.ToString(), "operatorType");
    }
  }
}

}
