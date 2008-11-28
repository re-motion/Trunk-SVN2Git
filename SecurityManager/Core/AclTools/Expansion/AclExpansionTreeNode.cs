/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Linq;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  // TODO AE: Remove commented code.
  public class AclExpansionTreeNode<TParent, TChildren> : IToText
  {
    public AclExpansionTreeNode (TParent parent, int numberLeafNodes, IList<TChildren> children)
    {
      // TODO AE: Argument checks
      Key = parent;
      Children = children;
      NumberLeafNodes = numberLeafNodes;
    }

    public TParent Key { get; private set; }
    public IList<TChildren> Children { get; private set; }
    public int NumberLeafNodes { get; private set; }
    
    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.nl ().sb ().e (Key).e (NumberLeafNodes).nl ().indent ().e (Children).unindent ().se ();
    }
  }
}

// TODO AE: This definitely belongs to another file and project, or namespace.
namespace Remotion.Development.UnitTesting.ObjectMother
{
  /// <summary>
  /// Supplies factories to create <see cref="AclExpansionTreeNode{T0,T1}"/> instances.
  /// </summary>
  public class AclExpansionTreeNode 
  {
    public static AclExpansionTreeNode<TParent, TChildren> New<TParent, TChildren> (TParent parent, int numberLeafNodes, IList<TChildren> children)
    {
      return new AclExpansionTreeNode<TParent, TChildren> (parent, numberLeafNodes, children);
    }


  }
}


//namespace Remotion.SecurityManager.AclTools.Expansion
//{
//  public class AclExpansionTreeNode<TParent, TChildren>
//  {
//    public AclExpansionTreeNode (IGrouping<TParent, AclExpansionEntry> grouping, IList<TChildren> children)
//    {
//      Parent = grouping;
//      Children = children;
//    }

//    public IGrouping<TParent, AclExpansionEntry> Parent { get; private set; }
//    public IList<TChildren> Children { get; private set; }
//  }
//}

//namespace Remotion.Development.UnitTesting.ObjectMother
//{
//  /// <summary>
//  /// Supplies factories to create <see cref="AclExpansionTreeNode{T0,T1}"/> instances.
//  /// </summary>
//  public class AclExpansionTreeNode
//  {
//    public static AclExpansionTreeNode<TParent, TChildren> New<TParent, TChildren> (IGrouping<TParent, AclExpansionEntry> grouping,
//      IList<TChildren> children)
//    {
//      return new AclExpansionTreeNode<TParent, TChildren> (grouping, children);
//    }
//  }
//}