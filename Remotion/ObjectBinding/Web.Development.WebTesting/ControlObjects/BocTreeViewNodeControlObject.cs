// This file is part of the re-motion Core Framework (www.re-motion.org)
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
  /// Control object representing a node within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  public class BocTreeViewNodeControlObject : BocControlObject, IControlObjectWithNodes<BocTreeViewNodeControlObject>
  {
    private readonly WebTreeViewNodeControlObject _webTreeViewNode;

    [UsedImplicitly]
    public BocTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _webTreeViewNode = new WebTreeViewNodeControlObject (context);
    }

    internal BocTreeViewNodeControlObject ([NotNull] WebTreeViewNodeControlObject webTreeViewNode)
        : base (webTreeViewNode.Context)
    {
      _webTreeViewNode = webTreeViewNode;
    }

    public string GetText ()
    {
      return _webTreeViewNode.GetText();
    }

    public bool IsSelected ()
    {
      return _webTreeViewNode.IsSelected();
    }

    public int GetNumberOfChildren ()
    {
      return _webTreeViewNode.GetNumberOfChildren();
    }

    public IControlObjectWithNodes<BocTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    public BocTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var webTreeViewNode = _webTreeViewNode.GetNode (itemID);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithIndex (int index)
    {
      var webTreeViewNode = _webTreeViewNode.GetNode().WithIndex (index);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    BocTreeViewNodeControlObject IControlObjectWithNodes<BocTreeViewNodeControlObject>.WithText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var webTreeViewNode = _webTreeViewNode.GetNode().WithText (text);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject Expand ()
    {
      var webTreeViewNode = _webTreeViewNode.Expand();
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject Collapse ()
    {
      var webTreeViewNode = _webTreeViewNode.Collapse();
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public BocTreeViewNodeControlObject Select ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var webTreeViewNode = _webTreeViewNode.Select (completionDetection);
      return new BocTreeViewNodeControlObject (webTreeViewNode);
    }

    public UnspecifiedPageObject Click ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      return _webTreeViewNode.Click (completionDetection);
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      return _webTreeViewNode.OpenContextMenu();
    }
  }
}