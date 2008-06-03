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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions
{
  [Serializable]
  public abstract class FormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public FormFunction ()
    {
    }

    protected FormFunction (params object[] args)
      : base (args)
    {
    }

    public FormFunction (ObjectID CurrentObjectID)
      : base (CurrentObjectID)
    {
    }

    // methods and properties
    [WxeParameter (1, false, WxeParameterDirection.In)]
    public ObjectID CurrentObjectID
    {
      get { return (ObjectID) Variables["CurrentObjectID"]; }
      set { Variables["CurrentObjectID"] = value; }
    }

    public BaseSecurityManagerObject CurrentObject
    {
      get
      {
        if (CurrentObjectID != null)
          return BaseSecurityManagerObject.GetObject (CurrentObjectID);

        return null;
      }
      set
      {
        CurrentObjectID = (value != null) ? value.ID : null;
      }
    }
  }
}
