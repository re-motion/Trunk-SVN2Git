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
using Remotion.Diagnostics.ToText;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionTreeNode<TParent, TChildren> : IToTextConvertible
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