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
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public class TypeTreeViewController : ControllerBase
  {
    public enum TreeViewIcons
    {
      [EnumDescription ("VSObject_Assembly.bmp")]
      Assembly,
      [EnumDescription ("VSObject_Namespace.bmp")]
      Namespace,
      [EnumDescription ("VSObject_Class.bmp")]
      Class = 2
    }

    private readonly TreeView _treeView;

    public TypeTreeViewController (TreeView treeView)
    {
      _treeView = treeView;
      _treeView.ImageList = CreateImageList (TreeViewIcons.Assembly, TreeViewIcons.Namespace, TreeViewIcons.Class);
    }

    public TreeView TreeView
    {
      get { return _treeView; }
    }

    public void PopulateTreeNodes (List<Type> types, Type selectedType)
    {
      _treeView.BeginUpdate();
      _treeView.Nodes.Clear();

      foreach (Type type in types)
      {
        TreeNode assemblyNode = GetAssemblyNode (type, _treeView.Nodes);
        TreeNode namespaceNode = GetNamespaceNode (type, assemblyNode.Nodes);
        TreeNode typeNode = GetTypeNode (type, namespaceNode.Nodes);
        TrySelect (typeNode, selectedType);
      }

      ExpandTypeTreeView();
      _treeView.EndUpdate();
    }

    public Type GetSelectedType ()
    {
      if (_treeView.SelectedNode == null)
        return null;
      return _treeView.SelectedNode.Tag as Type;
    }

    private TreeNode GetAssemblyNode (Type type, TreeNodeCollection assemblyNodes)
    {
      AssemblyName assemblyName = type.Assembly.GetName();
      TreeNode assemblyNode = assemblyNodes[assemblyName.FullName];
      if (assemblyNode == null)
      {
        assemblyNode = new TreeNode();
        assemblyNode.Name = assemblyName.FullName;
        assemblyNode.Text = assemblyName.Name;
        assemblyNode.ToolTipText = assemblyName.FullName;
        assemblyNode.ImageKey = TreeViewIcons.Assembly.ToString();
        assemblyNode.SelectedImageKey = TreeViewIcons.Assembly.ToString();

        assemblyNodes.Add (assemblyNode);
      }

      return assemblyNode;
    }

    private TreeNode GetNamespaceNode (Type type, TreeNodeCollection namespaceNodes)
    {
      TreeNode namespaceNode = namespaceNodes[type.Namespace];
      if (namespaceNode == null)
      {
        namespaceNode = new TreeNode();
        namespaceNode.Name = type.Namespace;
        namespaceNode.Text = type.Namespace;
        namespaceNode.ImageKey = TreeViewIcons.Namespace.ToString();
        namespaceNode.SelectedImageKey = TreeViewIcons.Namespace.ToString();


        namespaceNodes.Add (namespaceNode);
      }

      return namespaceNode;
    }

    private TreeNode GetTypeNode (Type type, TreeNodeCollection typeNodes)
    {
      TreeNode typeNode = typeNodes[type.FullName];
      if (typeNode == null)
      {
        typeNode = new TreeNode();
        typeNode.Name = TypeUtility.GetPartialAssemblyQualifiedName (type);
        typeNode.Text = type.Name;
        typeNode.Tag = type;
        typeNode.ImageKey = TreeViewIcons.Class.ToString();
        typeNode.SelectedImageKey = TreeViewIcons.Class.ToString();

        typeNodes.Add (typeNode);
      }

      return typeNode;
    }

    private void TrySelect (TreeNode node, Type selectedType)
    {
      if (node.Tag is Type
          && selectedType != null
          && ((Type) node.Tag).FullName.Equals (selectedType.FullName, StringComparison.CurrentCultureIgnoreCase))
      {
        _treeView.SelectedNode = node;
        node.EnsureVisible();
      }
    }

    private void ExpandTypeTreeView ()
    {
      if (_treeView.Nodes.Count < 4)
      {
        bool expandAll = _treeView.GetNodeCount (true) < 21;
        foreach (TreeNode assemblyNode in _treeView.Nodes)
        {
          assemblyNode.Expand();
          if (expandAll || assemblyNode.Nodes.Count == 1)
            assemblyNode.ExpandAll();
        }
      }
    }
  }
}
