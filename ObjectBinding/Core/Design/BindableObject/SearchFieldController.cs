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
using System.Windows.Forms;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public class SearchFieldController : ControllerBase
  {
    public enum SearchIcons
    {
      [EnumDescription ("VS_Search.bmp")]
      Search = 0
    }

    private readonly TextBox _searchField;
    private readonly Button _searchButton;

    public SearchFieldController (TextBox searchField, Button searchButton)
    {
      ArgumentUtility.CheckNotNull ("searchField", searchField);
      ArgumentUtility.CheckNotNull ("searchButton", searchButton);

      _searchField = searchField;
      _searchButton = searchButton;
      _searchButton.ImageList = CreateImageList (SearchIcons.Search);
      _searchButton.ImageKey = SearchIcons.Search.ToString();

      _searchField.Enabled = false;
      _searchButton.Enabled = false;
    }

    public TextBox SearchField
    {
      get { return _searchField; }
    }

    public Button SearchButton
    {
      get { return _searchButton; }
    }
  }
}
