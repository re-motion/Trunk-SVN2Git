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

      if (!BindableObjectTypeFinder.IsBindableObjectImplementation (Type))
      {
        var message = string.Format ("The type '{0}' is not a bindable object implementation. It must either have an BindableObject mixin or be " 
            + "derived from a BindableObject base class to be used with this data source.", Type.FullName);
        throw new InvalidOperationException (message);
      }

      return BindableObjectProvider.GetBindableObjectClass (Type);
    }

    private bool IsDesignMode
    {
      get { return Site != null && Site.DesignMode; }
    }
  }
}
