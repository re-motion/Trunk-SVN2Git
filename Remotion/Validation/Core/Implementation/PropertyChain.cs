// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.PropertyChain
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Remotion.Validation.Implementation
{
  /// <summary>Represents a chain of properties</summary>
  public class PropertyChain
  {
    private readonly List<string> _memberNames = new List<string>();

    public int Count
    {
      get { return _memberNames.Count; }
    }

    /// <summary>Creates a new PropertyChain.</summary>
    public PropertyChain ()
    {
    }

    /// <summary>Creates a new PropertyChain based on another.</summary>
    public PropertyChain (PropertyChain parent)
    {
      if (parent == null)
        return;
      _memberNames.AddRange (parent._memberNames);
    }

    public PropertyChain (IEnumerable<string> memberNames)
    {
      _memberNames.AddRange (memberNames);
    }

    /// <summary>Adds a property name to the chain</summary>
    /// <param name="propertyName">Name of the property to add</param>
    public void Add (string propertyName)
    {
      _memberNames.Add (propertyName);
    }

    /// <summary>Creates a string representation of a property chain.</summary>
    public override string ToString ()
    {
      return string.Join (".", _memberNames.ToArray());
    }

    /// <summary>Builds a property path.</summary>
    public string BuildPropertyName (string propertyName)
    {
      var propertyChain = new PropertyChain (this);
      propertyChain.Add (propertyName);
      return propertyChain.ToString();
    }

    public static PropertyChain FromExpression (LambdaExpression expression)
    {
      var stringStack = new Stack<string>();

      MemberExpression Func (Expression toUnwrap)
      {
        if (toUnwrap is UnaryExpression unaryExpression)
          return unaryExpression.Operand as MemberExpression;

        return toUnwrap as MemberExpression;
      }

      for (var memberExpression = Func (expression.Body); memberExpression != null; memberExpression = Func (memberExpression.Expression))
        stringStack.Push (memberExpression.Member.Name);

      return new PropertyChain (stringStack);
    }
  }
}