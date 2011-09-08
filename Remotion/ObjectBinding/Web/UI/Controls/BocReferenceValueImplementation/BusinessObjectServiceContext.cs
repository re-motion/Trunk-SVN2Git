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
using Remotion.FunctionalProgramming;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  /// <summary>
  /// Contains all context information required by a business object service.
  /// </summary>
  [Serializable]
  public class BusinessObjectServiceContext
  {
    private readonly string _businessObjectClass;
    private readonly string _businessObjectProperty;
    private readonly string _businessObjectIdentifier;

    public static BusinessObjectServiceContext Create (IBusinessObjectDataSource dataSource, IBusinessObjectProperty property)
    {
      var dataSourceOrNull = Maybe.ForValue (dataSource);

      var businessObjectClass =
          dataSourceOrNull.Select (ds => ds.BusinessObject).Select (bo => bo.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault()
          ?? dataSourceOrNull.Select (ds => ds.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault();

      var businessObjectProperty = Maybe.ForValue (property).Select (p => p.Identifier).ValueOrDefault();

      var businessObjectIdentifier =
          dataSourceOrNull.Select (ds => ds.BusinessObject as IBusinessObjectWithIdentity).Select (o => o.UniqueIdentifier).ValueOrDefault();

      return new BusinessObjectServiceContext (businessObjectClass, businessObjectProperty, businessObjectIdentifier);
    }

    private BusinessObjectServiceContext (string businessObjectClass, string businessObjectProperty, string businessObjectIdentifier)
    {
      _businessObjectClass = businessObjectClass;
      _businessObjectProperty = businessObjectProperty;
      _businessObjectIdentifier = businessObjectIdentifier;
    }

    /// <summary>
    /// The <see cref="IBusinessObjectClass.Identifier"/> of the <see cref="IBusinessObjectClass"/> the reference value is bound to.
    /// </summary>
    public string BusinessObjectClass
    {
      get { return _businessObjectClass; }
    }

    /// <summary>
    /// The <see cref="IBusinessObjectProperty.Identifier"/> of the <see cref="IBusinessObjectReferenceProperty"/> the reference value is bound to.
    /// </summary>
    public string BusinessObjectProperty
    {
      get { return _businessObjectProperty; }
    }

    /// <summary>
    /// The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the <see cref="IBusinessObjectWithIdentity"/> the reference value is bound to.
    /// </summary>
    public string BusinessObjectIdentifier
    {
      get { return _businessObjectIdentifier; }
    }
  }
}