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
using System.Reflection;
using System.Text;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  [Serializable]
  public class DelegateValidationRule<TDefinition> : IValidationRule<TDefinition> where TDefinition : IVisitableDefinition
  {
    public struct Args
    {
      public readonly ValidatingVisitor Validator;
      public readonly TDefinition Definition;
      public readonly IValidationLog Log;
      public readonly DelegateValidationRule<TDefinition> Self;

      public Args (ValidatingVisitor validator, TDefinition definition, IValidationLog log, DelegateValidationRule<TDefinition> self)
      {
        ArgumentUtility.CheckNotNull ("validator", validator);
        ArgumentUtility.CheckNotNull ("definition", definition);
        ArgumentUtility.CheckNotNull ("log", log);
        ArgumentUtility.CheckNotNull ("self", self);

        Validator = validator;
        Self = self;
        Log = log;
        Definition = definition;
      }
    }

    private static string GetRuleName (Rule rule)
    {
      MethodInfo method = rule.Method;
      DelegateRuleDescriptionAttribute attribute = AttributeUtility.GetCustomAttribute<DelegateRuleDescriptionAttribute> (method, true);
      if (attribute == null || attribute.RuleName == null)
        return method.DeclaringType.FullName + "." + rule.Method.Name;
      else
        return attribute.RuleName;
    }

    private static string GetMessage (Rule rule)
    {
      MethodInfo method = rule.Method;
      DelegateRuleDescriptionAttribute attribute = AttributeUtility.GetCustomAttribute<DelegateRuleDescriptionAttribute> (method, true);
      if (attribute == null || attribute.Message == null)
        return FormatMessage (rule.Method.Name);
      else
        return attribute.Message;
    }

    private static string FormatMessage (string message)
    {
      StringBuilder sb = new StringBuilder ();

      for (int i = 0; i < message.Length; ++i)
      {
        if (i > 0 && char.IsUpper (message[i]))
          sb.Append (' ').Append (char.ToLower (message[i]));
        else
          sb.Append (message[i]);
      }
      return sb.ToString ();
    }

    public delegate void Rule (Args args);

    private readonly Rule _rule;
    private readonly string _ruleName;
    private readonly string _message;

    public DelegateValidationRule(Rule rule, string ruleName, string message)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      ArgumentUtility.CheckNotNull ("ruleName", ruleName);
      ArgumentUtility.CheckNotNull ("message", message);

      _rule = rule;
      _ruleName = ruleName;
      _message = message;
    }

    public DelegateValidationRule (Rule rule)
        : this (ArgumentUtility.CheckNotNull("rule", rule), GetRuleName(rule), GetMessage(rule))
    {
    }

    public Rule RuleDelegate
    {
      get { return _rule; }
    }

    public string RuleName
    {
      get { return _ruleName; }
    }

    public string Message
    {
      get { return _message; }
    }

    public void Execute (ValidatingVisitor validator, TDefinition definition, IValidationLog log)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("log", log);
      RuleDelegate (new Args(validator, definition, log, this));
    }
  }
}
