using System;
using System.IO;
using System.Text;
using System.CodeDom;

namespace Remotion.Development.CodeDom
{

public class ExtendedJSharpCodeProvider: ExtendedCodeProvider
{
	public ExtendedJSharpCodeProvider()
    : base (new Microsoft.VJSharp.VJSharpCodeProvider ())
	{
	}

  public override bool IsCaseSensitive
  {
    get { return true; }
  }

  public override CodeTypeDeclaration CreateEnumDeclaration (string name)
  {
    CodeTypeDeclaration enumDeclaration = new CodeTypeDeclaration (GetValidName(name));
    CodeTypeReference enumDeclarationReference = new CodeTypeReference (name);

    // add member: public int NumericValue;
    CodeMemberField valueField = new CodeMemberField (typeof (int), "NumericValue");
    valueField.Attributes = MemberAttributes.Public;
    enumDeclaration.Members.Add (valueField);

    // add member: public <type> (int value);
    CodeConstructor ctorValue = new CodeConstructor ();
    ctorValue.Attributes = MemberAttributes.Public;
    ctorValue.Parameters.Add (new CodeParameterDeclarationExpression (typeof (int), "value"));
    ctorValue.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "NumericValue"),
        new CodeVariableReferenceExpression ("value")));
    enumDeclaration.Members.Add (ctorValue);

    // add member: public <type> ();
    CodeConstructor ctorVoid = new CodeConstructor ();
    ctorVoid.Attributes = MemberAttributes.Public;
    ctorVoid.Statements.Add (new CodeAssignStatement (
        new CodeFieldReferenceExpression  (new CodeThisReferenceExpression(), "NumericValue"),
        new CodePrimitiveExpression (0)));
    enumDeclaration.Members.Add (ctorVoid);

    // add member: public boolean Equals (Object obj);
    CodeMemberMethod methodEqualsObject = new CodeMemberMethod ();
    methodEqualsObject.Name = "Equals";
    methodEqualsObject.Attributes = MemberAttributes.Public;
    methodEqualsObject.ReturnType = new CodeTypeReference (typeof (bool));
    methodEqualsObject.Parameters.Add (new CodeParameterDeclarationExpression (typeof (object), "obj"));
    // return (obj != null && obj.GetType() == typeof(<type>) && NumericValue == ((<type>)obj).NumericValue;
    methodEqualsObject.Statements.Add (new CodeMethodReturnStatement (
        new CodeBinaryOperatorExpression (
            new CodeBinaryOperatorExpression (
                new CodeBinaryOperatorExpression (
                    new CodeVariableReferenceExpression ("obj"), 
                    CodeBinaryOperatorType.IdentityInequality, 
                    new CodePrimitiveExpression (null)),
                CodeBinaryOperatorType.BooleanAnd,
                new CodeBinaryOperatorExpression (
                    new CodeMethodInvokeExpression (new CodeVariableReferenceExpression("obj"), "GetType"),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodeTypeOfExpression (enumDeclarationReference))),
            CodeBinaryOperatorType.BooleanAnd,
            new CodeBinaryOperatorExpression (
                new CodeFieldReferenceExpression (
                    new CodeThisReferenceExpression(), 
                    "NumericValue"),
                CodeBinaryOperatorType.ValueEquality,
                new CodeFieldReferenceExpression (
                    new CodeCastExpression (enumDeclarationReference, new CodeVariableReferenceExpression ("obj")),
                    "NumericValue")))));
    enumDeclaration.Members.Add (methodEqualsObject);

    // add member: public boolean Equals (<type> val);
    CodeMemberMethod methodEqualsEnum = new CodeMemberMethod ();
    methodEqualsEnum.Name = "Equals";
    methodEqualsEnum.Attributes = MemberAttributes.Public;
    methodEqualsEnum.ReturnType = new CodeTypeReference (typeof (bool));
    methodEqualsEnum.Parameters.Add (new CodeParameterDeclarationExpression (enumDeclarationReference, "val"));
    // return NumericValue == val.NumericValue;
    methodEqualsEnum.Statements.Add (new CodeMethodReturnStatement (
      new CodeBinaryOperatorExpression (
          new CodeFieldReferenceExpression (
              new CodeThisReferenceExpression(), 
              "NumericValue"),
          CodeBinaryOperatorType.ValueEquality,
          new CodeFieldReferenceExpression (
              new CodeVariableReferenceExpression ("val"),
              "NumericValue"))));
    enumDeclaration.Members.Add (methodEqualsEnum);

    // add member: public int GetHashCode ();
    CodeMemberMethod methodGetHashCode = new CodeMemberMethod ();
    methodGetHashCode.Name = "GetHashCode";
    methodGetHashCode.Attributes = MemberAttributes.Public;
    methodGetHashCode.ReturnType = new CodeTypeReference (typeof (int));
    methodGetHashCode.Statements.Add (new CodeMethodReturnStatement (
        new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "NumericValue")));
    enumDeclaration.Members.Add (methodGetHashCode);

    // add member: public string ToString ();
    CodeMemberMethod methodToString = new CodeMemberMethod ();
    methodToString.Name = "ToString";
    methodToString.Attributes = MemberAttributes.Public;
    methodToString.ReturnType = new CodeTypeReference (typeof (string));
    methodToString.Statements.Add (new CodeMethodReturnStatement (
        new CodeMethodInvokeExpression (
            new CodeTypeReferenceExpression ("java.lang.Integer"),
            "toString",
            new CodeFieldReferenceExpression (new CodeThisReferenceExpression(), "NumericValue"))));
    enumDeclaration.Members.Add (methodToString);

    return enumDeclaration;
  }

  public override CodeTypeMember CreateEnumValue (CodeTypeReference enumDeclaration, int numericValue, string name)
  {
    CodeMemberField enumValueField = new CodeMemberField (enumDeclaration, name);
    enumValueField.InitExpression = new CodeObjectCreateExpression (enumDeclaration, new CodePrimitiveExpression (numericValue));
    enumValueField.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Const;
    return enumValueField;
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
}

}
