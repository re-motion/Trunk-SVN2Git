using System;
using System.Web.UI;

namespace Remotion.Web.UnitTests.AspNetFramework
{

  public class ControlMock : Control
  {
    // types

    // static members and constants

    // member fields

    private string _valueInViewState;
    private string _valueInControlState;

    // construction and disposing

    public ControlMock ()
    {
    }

    // methods and properties

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      Page.RegisterRequiresControlState (this);
    }

    public string ValueInViewState
    {
      get { return _valueInViewState; }
      set { _valueInViewState = value; }
    }

    public string ValueInControlState
    {
      get { return _valueInControlState; }
      set { _valueInControlState = value; }
    }
    
    protected override void LoadViewState (object savedState)
    {
      _valueInViewState = (string) savedState;
    }

    protected override object SaveViewState()
    {
      return _valueInViewState;
    }

    protected override void LoadControlState (object savedState)
    {
      _valueInControlState = (string) savedState;
    }
  
    protected override object SaveControlState ()
    {
      return _valueInControlState;
    }
  }

}
