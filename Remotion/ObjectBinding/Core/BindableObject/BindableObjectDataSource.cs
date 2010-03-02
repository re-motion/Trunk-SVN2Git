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
using System.ComponentModel;
using System.Drawing.Design;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: Doc
  public class BindableObjectDataSource : BusinessObjectDataSource
  {
    private IBusinessObject _businessObject;
    private string _typeName;
    private Type _type;

    public BindableObjectDataSource ()
    {
    }

    public override IBusinessObject BusinessObject
    {
      get { return _businessObject; }
      set { _businessObject = value; }
    }

    public override IBusinessObjectClass BusinessObjectClass
    {
      get { return GetBusinessObjectClass(); }
    }

    [Category ("Data")]
    [DefaultValue (null)]
    [Editor (typeof (BindableObjectTypePickerEditor), typeof (UITypeEditor))]
    [TypeConverter (typeof (TypeNameConverter))]
    public Type Type
    {
      get
      {
        if (_type != null)
          return _type;

        if (_typeName == null)
          return null;

        if (IsDesignMode)
          return TypeUtility.GetDesignModeType (_typeName, false);

        _type = TypeUtility.GetType (_typeName, true);
        return _type;
      }
      set
      {
        _type = null;
        if (value == null)
          _typeName = null;
        else
          _typeName = TypeUtility.GetPartialAssemblyQualifiedName (value);
      }
    }

    private IBusinessObjectClass GetBusinessObjectClass ()
    {
      if (Type == null)
        return null;

      if (!BindableObjectProvider.IsBindableObjectImplementation (Type))
      {
        var message = string.Format ("The type '{0}' is not a bindable object implementation. It must either have an BindableObject mixin or be " 
            + "derived from a BindableObject base class to be used with this data source.", Type.FullName);
        throw new InvalidOperationException (message);
      }

      var provider = BindableObjectProvider.GetProviderForBindableObjectType (Type);
      return provider.GetBindableObjectClass (Type);
    }

    private bool IsDesignMode
    {
      get { return Site != null && Site.DesignMode; }
    }
  }
}
