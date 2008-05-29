using System;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class ManualBusinessObject : IBusinessObject
  {
    private string _stringProperty = "Initial value";

    public string StringProperty
    {
      get { return _stringProperty; }
      set { _stringProperty = value; }
    }

    public object GetProperty (IBusinessObjectProperty property)
    {
      return GetProperty (property.Identifier);
    }

    public object GetProperty (string propertyIdentifier)
    {
      if (propertyIdentifier == "StringProperty")
        return StringProperty;
      else
        throw new NotSupportedException ();
    }

    public void SetProperty (IBusinessObjectProperty property, object value)
    {
      SetProperty (property.Identifier, value);
    }

    public void SetProperty (string propertyIdentifier, object value)
    {
      if (propertyIdentifier == "StringProperty")
        StringProperty = (string) value;
      else
        throw new NotSupportedException ();
    }

    public string GetPropertyString (IBusinessObjectProperty property, string format)
    {
      return GetPropertyString (property.Identifier);
    }

    public string GetPropertyString (string propertyIdentifier)
    {
      return GetProperty (propertyIdentifier).ToString ();
    }

    public string DisplayName
    {
      get { return "Manual business object"; }
    }

    public string DisplayNameSafe
    {
      get { return "Manual business object (safe)"; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return BindableObjectProvider.GetBindableObjectClassFromProvider(typeof (ManualBusinessObject)); }
    }
  }
}
