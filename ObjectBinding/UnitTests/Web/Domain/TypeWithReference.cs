using System;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObjectWithIdentity]
  public class TypeWithReference
  {
    public static TypeWithReference Create ()
    {
      return ObjectFactory.Create<TypeWithReference> (true).With ();
    }

    public static TypeWithReference Create (TypeWithReference firstValue, TypeWithReference secondValue)
    {
      return ObjectFactory.Create<TypeWithReference> (true).With (firstValue, secondValue);
    }

    public static TypeWithReference Create (string displayName)
    {
      return ObjectFactory.Create<TypeWithReference> (true).With (displayName);
    }

    private TypeWithReference _referenceValue;
    private TypeWithReference[] _referenceList;
    private TypeWithReference _firstValue;
    private TypeWithReference _secondValue;
    private string _displayName;
    private Guid _id = Guid.NewGuid();

    protected TypeWithReference ()
    {
    }

    protected TypeWithReference (TypeWithReference firstValue, TypeWithReference secondValue)
    {
      _firstValue = firstValue;
      _secondValue = secondValue;
    }

    protected TypeWithReference (string displayName)
    {
      _displayName = displayName;
    }

    public TypeWithReference ReferenceValue
    {
      get { return _referenceValue; }
      set { _referenceValue = value; }
    }

    public TypeWithReference[] ReferenceList
    {
      get { return _referenceList; }
      set { _referenceList = value; }
    }

    public TypeWithReference FirstValue
    {
      get { return _firstValue; }
      set { _firstValue = value; }
    }

    public TypeWithReference SecondValue
    {
      get { return _secondValue; }
      set { _secondValue = value; }
    }

    [OverrideMixin]
    public string DisplayName
    {
      get { return _displayName ?? UniqueIdentifier; }
    }

    [OverrideMixin]
    public string UniqueIdentifier
    {
      get { return _id.ToString(); }
    }
  }
}