// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System.Collections.Generic;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionTreeNode<TParent, TChildren> : IToTextConvertible
  {
    public AclExpansionTreeNode (TParent parent, int numberLeafNodes, IList<TChildren> children)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("numberLeafNodes", numberLeafNodes);
      ArgumentUtility.CheckNotNull ("children", children);
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
