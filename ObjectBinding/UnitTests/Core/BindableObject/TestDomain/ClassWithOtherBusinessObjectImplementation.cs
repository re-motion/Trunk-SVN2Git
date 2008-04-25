using System;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public abstract class ClassWithOtherBusinessObjectImplementation : IBusinessObject
  {
    public abstract object GetProperty (IBusinessObjectProperty property);

    public abstract object GetProperty (string propertyIdentifier);

    public abstract void SetProperty (IBusinessObjectProperty property, object value);

    public abstract void SetProperty (string propertyIdentifier, object value);

    public abstract string GetPropertyString (IBusinessObjectProperty property, string format);

    public abstract string GetPropertyString (string propertyIdentifier);

    public abstract string DisplayName { get; }

    public abstract string DisplayNameSafe { get; }

    public abstract IBusinessObjectClass BusinessObjectClass { get; }
  }
}