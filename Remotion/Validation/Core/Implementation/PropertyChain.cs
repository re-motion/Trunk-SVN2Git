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
    private readonly List<string> memberNames = new List<string>();

    public int Count
    {
      get { return this.memberNames.Count; }
    }

    /// <summary>Creates a new PropertyChain.</summary>
    public PropertyChain()
    {
    }

    /// <summary>Creates a new PropertyChain based on another.</summary>
    public PropertyChain(PropertyChain parent)
    {
      if (parent == null)
        return;
      this.memberNames.AddRange((IEnumerable<string>)parent.memberNames);
    }

    public PropertyChain(IEnumerable<string> memberNames)
    {
      this.memberNames.AddRange(memberNames);
    }

    /// <summary>Adds a property name to the chain</summary>
    /// <param name="propertyName">Name of the property to add</param>
    public void Add(string propertyName)
    {
      this.memberNames.Add(propertyName);
    }

    /// <summary>
    /// Adds an indexer to the property chain. For example, if the following chain has been constructed:
    /// Parent.Child
    /// then calling AddIndexer(0) would convert this to:
    /// Parent.Child[0]
    /// </summary>
    /// <param name="indexer"></param>
    public void AddIndexer (object indexer)
    {
      if (this.memberNames.Count == 0)
        throw new InvalidOperationException ("Could not apply an Indexer because the property chain is empty.");
      this.memberNames[this.memberNames.Count - 1] = this.memberNames[this.memberNames.Count - 1] + "[" + indexer + "]";
    }

    /// <summary>Creates a string representation of a property chain.</summary>
    public override string ToString()
    {
      return string.Join(".", this.memberNames.ToArray());
    }

    /// <summary>Builds a property path.</summary>
    public string BuildPropertyName(string propertyName)
    {
      PropertyChain propertyChain = new PropertyChain(this);
      propertyChain.Add(propertyName);
      return propertyChain.ToString();
    }

    public static PropertyChain FromExpression (LambdaExpression expression)
    {
      Stack<string> stringStack = new Stack<string>();
      Func<Expression, MemberExpression> func = (Func<Expression, MemberExpression>) (toUnwrap =>
      {
        if (toUnwrap is UnaryExpression)
          return ((UnaryExpression) toUnwrap).Operand as MemberExpression;
        return toUnwrap as MemberExpression;
      });
      for (MemberExpression memberExpression = func (expression.Body);
          memberExpression != null;
          memberExpression = func (memberExpression.Expression))
        stringStack.Push (memberExpression.Member.Name);
      return new PropertyChain ((IEnumerable<string>) stringStack);
    }
  }
}