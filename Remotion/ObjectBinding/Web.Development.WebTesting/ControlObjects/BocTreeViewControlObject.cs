﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocTreeViewControlObject : BocControlObject, IControlObjectWithNodes<BocTreeViewNodeControlObject>
  {
    private readonly BocTreeViewNodeControlObject _metaRootNode;

    public BocTreeViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _metaRootNode = new BocTreeViewNodeControlObject (context);
    }

    public BocTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode().WithIndex (1);
    }

    public IControlObjectWithNodes<BocTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    public BocTreeViewNodeControlObject GetNode (string itemID)
    {
      return GetNode().WithItemID (itemID);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _metaRootNode.GetNode (itemID);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithIndex (int index)
    {
      return _metaRootNode.GetNode().WithIndex (index);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return _metaRootNode.GetNode().WithText (text);
    }
  }
}