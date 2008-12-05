// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.ComponentModel;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <remarks>
  ///   This class must be inherited from, overwriting <see cref="DataSource"/>.
  /// </remarks>
  //public class DataEditUserControl : UserControl, IDataEditControl
  public abstract class DataEditUserControl: UserControl, IDataEditControl
  {
    public IBusinessObject BusinessObject
    {
      get { return DataSource.BusinessObject; }
      set { DataSource.BusinessObject = value; }
    }

    public virtual void LoadValues (bool interim)
    {
      DataSource.LoadValues (interim);
    }

    public virtual void SaveValues (bool interim)
    {
      DataSource.SaveValues (interim);
    }

    public virtual void CancelEdit ()
    {
    }

    public virtual DataSourceMode Mode
    {
      get { return DataSource.Mode; }
      set { DataSource.Mode = value; }
    }

    public virtual void PrepareValidation ()
    {
      DataSource.PrepareValidation();
    }

    public virtual bool Validate ()
    {
      return DataSource.Validate();
    }

    /// <summary>
    ///   Gets the control's data source. This method must be overridden in derived classes.
    /// </summary>
    /// <remarks>
    ///   This method should be <see langword="abstract"/>, but <see langword="abstract"/> base classes are not 
    ///   supported by VS.NET designer.
    /// </remarks>
    //[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    //public virtual IBusinessObjectDataSourceControl DataSource
    //{
    //  get { throw new NotImplementedException ("Property DataSource must be overridden by derived classes to return a non-null value."); }
    //}

    ///// <summary> Gets the control's data source. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public abstract IBusinessObjectDataSourceControl DataSource { get; }
  }
}
