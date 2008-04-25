using System;
using System.ComponentModel;

namespace Remotion.Web.UnitTests.UI
{

  public class SiteMock : ISite
  {
    // types

    // static members and constants

    // member fields

    private string _name = "SiteMock";
    private bool _designMode = false;

    // construction and disposing

    public SiteMock ()
    {
    }

    // methods and properties

    public IComponent Component
    {
      get
      {
        return null;
      }
    }

    public IContainer Container
    {
      get
      {
        return null;
      }
    }

    public bool DesignMode
    {
      get
      {
        return _designMode;
      }
      set
      {
        _designMode = value;
      }
    }

    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        _name = value;
      }
    }

    public object GetService(Type serviceType)
    {
      return null;
    }
  }

}
