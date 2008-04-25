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
