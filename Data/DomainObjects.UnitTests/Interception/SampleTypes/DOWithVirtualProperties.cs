using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  [Serializable]
  public class DOWithVirtualProperties : DomainObject
  {
    public virtual int PropertyWithGetterAndSetter
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }

    public virtual string PropertyWithGetterOnly
    {
      get { return CurrentProperty.GetValue<string> (); }
    }

    public virtual DateTime PropertyWithSetterOnly
    {
      set { CurrentProperty.SetValue (value); }
    }

    protected virtual int ProtectedProperty
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }

    public virtual DateTime PropertyThrowing
    {
      get { throw new Exception (); }
      set { throw new Exception (); }
    }

    [StorageClassNone]
    public virtual DateTime PropertyNotInMapping
    {
      get { return CurrentProperty.GetValue<DateTime>(); }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    public new string GetAndCheckCurrentPropertyName()
    {
      return base.GetAndCheckCurrentPropertyName();
    }
  }
}