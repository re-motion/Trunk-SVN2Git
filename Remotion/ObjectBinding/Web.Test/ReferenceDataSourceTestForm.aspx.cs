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
using Remotion.ObjectBinding;

namespace OBWTest
{
  public partial class ReferenceDataSourceTestForm : TestBasePage<ReferenceDataSourceTestFunction>
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      LevelOneDataSource.BusinessObject =(IBusinessObject) CurrentFunction.RootObject;
      LevelOneDataSource.LoadValues (IsPostBack);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      // Normally only in OnUnload. Call here only to provide diagnostic output
      LevelOneDataSource.SaveValues (true);

      var hasLevelOneInstance = CurrentFunction.RootObject != null;
      var hasLevelTwoInstance = hasLevelOneInstance && CurrentFunction.RootObject.ReferenceValue != null;
      var hasLevelThreeInstance = hasLevelTwoInstance && CurrentFunction.RootObject.ReferenceValue.ReferenceValue != null;

      Stack.Text = "";
      Stack.Text += string.Format ("LevelOne: hasInstance = {0}<br/>", hasLevelOneInstance);
      Stack.Text += string.Format ("LevelTwo (LevelOne.ReferenceValue): hasInstance = {0}<br/>", hasLevelTwoInstance);
      Stack.Text += string.Format ("LevelThree (LevelOne.ReferenceValue.ReferenceValue): hasInstance = {0}<br/>", hasLevelThreeInstance);
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);
      LevelOneDataSource.SaveValues (true);
    }

    protected void ValidateButton_OnClick (object sender, EventArgs e)
    {
      ValidateDataSource ();
    }

    private bool ValidateDataSource ()
    {
      PrepareValidation ();

     return LevelOneDataSource.Validate();
    }

    protected void SaveButton_OnClick (object sender, EventArgs e)
    {
      if (ValidateDataSource ())
        LevelOneDataSource.SaveValues (false);
    }
  }
}