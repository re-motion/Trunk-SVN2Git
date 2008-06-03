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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{
public class EditableRowDataSourceFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public EditableRowDataSourceFactory ()
  {
  }

  // methods and properties

  public virtual IBusinessObjectReferenceDataSource Create (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull ("businessObject", businessObject);

    BusinessObjectReferenceDataSource dataSource = new BusinessObjectReferenceDataSource();
    dataSource.BusinessObject = businessObject;
    
    return dataSource;
  }
}

}
