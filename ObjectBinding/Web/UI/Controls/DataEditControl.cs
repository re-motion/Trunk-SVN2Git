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
using System.ComponentModel;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <remarks>
  ///   This class must be inherited from, overwriting <see cref="DataSource"/>.
  /// </remarks>
  public class DataEditUserControl : UserControl, IDataEditControl
      //public abstract class DataEditUserControl: UserControl, IDataEditControl
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
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public virtual IBusinessObjectDataSourceControl DataSource
    {
      get { throw new NotImplementedException ("Property DataSource must be overridden by derived classes to return a non-null value."); }
    }

    //  /// <summary> Gets the control's data source. </summary>
    //  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    //  public abstract IBusinessObjectDataSourceControl DataSource { get; }
  }
}
