using System;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessControlEntryValidationResult
  {
    // types

    // static members

    // member fields

    private bool _isValid = true;
    private bool _isSpecificTenantMissing = false;

    // construction and disposing

    public AccessControlEntryValidationResult ()
    {
    }

    // methods and properties

    public bool IsValid
    {
      get { return _isValid; }
    }

    public bool IsSpecificTenantMissing
    {
      get { return _isSpecificTenantMissing; }
    }

    public void SetSpecificTenantMissing ()
    {
      _isValid = false;
      _isSpecificTenantMissing = true;
    }
  }
}