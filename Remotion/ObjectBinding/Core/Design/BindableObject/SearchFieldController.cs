// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
