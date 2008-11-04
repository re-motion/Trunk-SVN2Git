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
using System.Collections.Generic;
using System.IO;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class PageFunctionGenerator : TemplateControlFunctionGeneratorBase
  {
    public PageFunctionGenerator (CodeCompileUnit codeCompileUnit, FunctionDeclaration functionDeclaration, List<string> importNamespaces, FileInfo file, int line)
        : base(codeCompileUnit, functionDeclaration, importNamespaces, file, line)
    {
    }

    protected override void GenerateWxeFunctionTemplateControlStep (CodeTypeDeclaration functionClass, FunctionDeclaration templateControlFile)
    {
      ArgumentUtility.CheckNotNull ("functionClass", functionClass);
      ArgumentUtility.CheckNotNull ("templateControlFile", templateControlFile);

      CodeMemberField step1 = new CodeMemberField (typeof (WxePageStep), "Step1");
      functionClass.Members.Add (step1);
      step1.InitExpression = new CodeObjectCreateExpression (
          new CodeTypeReference (typeof (WxePageStep)),
          new CodePrimitiveExpression (templateControlFile.AspxFile));
    }

    /// <summary>
    /// &lt;returnType&gt; Call (IWxePage page, IWxeCallArguments arguments, &lt;type&gt; [ref|out] param1, &lt;type&gt; [ref|out] param2, ...)
    /// </summary>
    protected override CodeMemberMethod GenerateWxeTemplateControlCallMethod (FunctionDeclaration functionDeclaration, CodeTypeDeclaration partialTemplateControlClass, CodeTypeDeclaration functionClass)
    {
      CodeMemberMethod callMethod = new CodeMemberMethod();
      partialTemplateControlClass.Members.Add (callMethod);
      callMethod.Name = "Call";
      callMethod.Attributes = MemberAttributes.Static | MemberAttributes.Public;
      callMethod.Parameters.Add (
          new CodeParameterDeclarationExpression (
              new CodeTypeReference (typeof (IWxePage)), "currentPage"));
      callMethod.Parameters.Add (
          new CodeParameterDeclarationExpression (
              new CodeTypeReference (typeof (IWxeCallArguments)), "arguments"));
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.IsReturnValue)
          callMethod.ReturnType = new CodeTypeReference (parameterDeclaration.TypeName);
        else
        {
          CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression (
              new CodeTypeReference (parameterDeclaration.TypeName),
              parameterDeclaration.Name);
          callMethod.Parameters.Add (parameter);
          if (parameterDeclaration.Direction == WxeParameterDirection.InOut)
            parameter.Direction = FieldDirection.Ref;
          else if (parameterDeclaration.Direction == WxeParameterDirection.Out)
            parameter.Direction = FieldDirection.Out;
        }
      }
      // <class>Function function;
      CodeVariableDeclarationStatement functionVariable = new CodeVariableDeclarationStatement (
          new CodeTypeReference (functionClass.Name), "function");
      callMethod.Statements.Add (functionVariable);
      // common variables
      CodeArgumentReferenceExpression currentPage = new CodeArgumentReferenceExpression ("currentPage");
      CodeVariableReferenceExpression function = new CodeVariableReferenceExpression ("function");
      CodePropertyReferenceExpression exceptionHandler = new CodePropertyReferenceExpression (function, "ExceptionHandler");
      // if (! currentPage.IsReturningPostBack)
      CodeConditionStatement ifNotIsReturningPostBack = new CodeConditionStatement();
      callMethod.Statements.Add (ifNotIsReturningPostBack);
      ifNotIsReturningPostBack.Condition = new CodeBinaryOperatorExpression (
          new CodePropertyReferenceExpression (currentPage, "IsReturningPostBack"),
          CodeBinaryOperatorType.ValueEquality,
          new CodePrimitiveExpression (false));
      // { 
      //   function = new <class>Function();
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeAssignStatement (
              function,
              new CodeObjectCreateExpression (new CodeTypeReference (functionClass.Name))));
      //   function.ParamN = ParamN;
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction != WxeParameterDirection.Out)
        {
          ifNotIsReturningPostBack.TrueStatements.Add (
              new CodeAssignStatement (
                  new CodePropertyReferenceExpression (function, parameterDeclaration.Name),
                  new CodeArgumentReferenceExpression (parameterDeclaration.Name)));
        }
      }
      //   function.ExceptionHandler.SetCatchExceptionTypes (typeof (Exception));
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeMethodInvokeExpression (
              exceptionHandler,
              "SetCatchExceptionTypes",
              new CodeTypeOfExpression (typeof (Exception))));
      //  currentPage.ExecuteFunction (function, arguments);
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeMethodInvokeExpression (
              currentPage,
              "ExecuteFunction",
              function,
              new CodeVariableReferenceExpression ("arguments")));
      // //   currentPage.ExecuteFunction (function);
      // ifNotIsReturningPostBack.TrueStatements.Add (new CodeMethodInvokeExpression (
      //     currentPage, "ExecuteFunction",
      //     new CodeExpression[] { new CodeVariableReferenceExpression ("function") }));
      //    throw new Exception ("(Unreachable code)"); 
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeThrowExceptionStatement (
              new CodeObjectCreateExpression (
                  new CodeTypeReference (typeof (Exception)),
                  new CodeExpression[]
                  {
                      new CodePrimitiveExpression ("(Unreachable code)")
                  })));
      // } else {
      //   function = (<class>Function) currentPage.ReturningFunction;
      ifNotIsReturningPostBack.FalseStatements.Add (
          new CodeAssignStatement (
              function,
              new CodeCastExpression (
                  new CodeTypeReference (functionClass.Name),
                  new CodePropertyReferenceExpression (currentPage, "ReturningFunction"))));
      //    if (function.Exception != null)
      CodeConditionStatement ifException = new CodeConditionStatement();
      ifNotIsReturningPostBack.FalseStatements.Add (ifException);
      ifException.Condition = new CodeBinaryOperatorExpression (
          new CodePropertyReferenceExpression (
              exceptionHandler,
              "Exception"),
          CodeBinaryOperatorType.IdentityInequality,
          new CodePrimitiveExpression (null));
      //      throw function.Exception;
      ifException.TrueStatements.Add (
          new CodeThrowExceptionStatement (
              new CodePropertyReferenceExpression (
                  exceptionHandler,
                  "Exception")
              ));
      //   ParamN = function.ParamN;
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction != WxeParameterDirection.In && !parameterDeclaration.IsReturnValue)
        {
          ifNotIsReturningPostBack.FalseStatements.Add (
              new CodeAssignStatement (
                  new CodeArgumentReferenceExpression (parameterDeclaration.Name),
                  new CodePropertyReferenceExpression (function, parameterDeclaration.Name)));
        }
        else if (parameterDeclaration.IsReturnValue)
        {
          // TODO: Throw Exception if any but last parameter has return value flag!
          ifNotIsReturningPostBack.FalseStatements.Add (
              new CodeMethodReturnStatement (
                  new CodePropertyReferenceExpression (function, parameterDeclaration.Name)));
        }
      }
      return callMethod;
    }

    /// <summary>
    /// &lt;returnType&gt; Call (IWxePage page, &lt;type&gt; [ref|out] param1, &lt;type&gt; [ref|out] param2, ...)
    /// </summary>
    protected override void GenerateWxePageCallMethodOverloadWithoutCallArguments (CodeTypeDeclaration partialTemplateControlClass, CodeMemberMethod callMethod)
    {
      CodeMemberMethod callMethodOverload = new CodeMemberMethod();
      callMethodOverload.Name = callMethod.Name;
      callMethodOverload.ReturnType = callMethod.ReturnType;
      callMethodOverload.Attributes = callMethod.Attributes;
      CodeExpression[] callOverloadParameters = new CodeExpression[callMethod.Parameters.Count];
      for (int i = 0; i < callMethod.Parameters.Count; ++i)
      {
        if (i == 1) // options parameter
          callOverloadParameters[i] = new CodePropertyReferenceExpression (new CodeTypeReferenceExpression (typeof (WxeCallArguments)), "Default");
        else
        {
          callMethodOverload.Parameters.Add (callMethod.Parameters[i]);
          callOverloadParameters[i] = new CodeDirectionExpression (
              callMethod.Parameters[i].Direction,
              new CodeArgumentReferenceExpression (callMethod.Parameters[i].Name));
        }
      }
      partialTemplateControlClass.Members.Add (callMethodOverload);
      // [return] Call (page, WxeCallArguments.Default, param1, param2, ...);
      CodeMethodInvokeExpression callOverloadStatement = new CodeMethodInvokeExpression (
          new CodeTypeReferenceExpression ( /*nameSpace + "." + */partialTemplateControlClass.Name), callMethod.Name, callOverloadParameters);
      if (callMethod.ReturnType.BaseType != typeof (void).FullName)
        callMethodOverload.Statements.Add (new CodeMethodReturnStatement (callOverloadStatement));
      else
        callMethodOverload.Statements.Add (callOverloadStatement);
    }
  }
}