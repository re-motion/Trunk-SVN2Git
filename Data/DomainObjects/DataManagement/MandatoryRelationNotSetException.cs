using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement
{
  [Serializable]
  public class MandatoryRelationNotSetException : DataManagementException
  {
    // types

    // static members and constants

    // member fields

    private DomainObject _domainObject;
    private string _propertyName;

    // construction and disposing

    public MandatoryRelationNotSetException (DomainObject domainObject, string propertyName, string message) 
        : this (domainObject, propertyName, message, null) 
    { 
    }

    public MandatoryRelationNotSetException (DomainObject domainObject, string propertyName, string message, Exception inner) 
        : base (message, inner) 
    {
      _domainObject = domainObject;
      _propertyName = propertyName;
    }
    
    protected MandatoryRelationNotSetException (SerializationInfo info, StreamingContext context) : base (info, context) 
    { 
    }

    // methods and properties

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }
  }
}
