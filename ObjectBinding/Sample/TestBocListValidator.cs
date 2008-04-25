using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Sample
{
  public class TestBocListValidator : CustomValidator
  {
    protected override bool EvaluateIsValid()
    {
      Control control = this.NamingContainer.FindControl(ControlToValidate);
      BocList bocList = (BocList) control;
      if (! bocList.IsRequired)
        return true;
      return bocList.Value != null && bocList.Value.Count > 0;
    }

    /// <summary>
    ///   Helper function that determines whether the control specified by the 
    ///   <see cref="ControlToValidate"/> property is a valid control.
    /// </summary>
    /// <returns> 
    ///   <see langword="true"/> if the control specified by the <see cref="ControlToValidate"/>
    ///   property is a valid control; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="HttpException"> 
    ///   Thrown if the <see cref="ControlToValidate"/> is not of type <see cref="TestBocList"/>.
    /// </exception>
    protected override bool ControlPropertiesValid()
    {
      Control control = this.NamingContainer.FindControl(ControlToValidate);

      if (! (control is TestBocList))
      {
        throw new HttpException("Control '" + ControlToValidate + "' is not of type '" + typeof (TestBocList) + "'");
      }

      return true;
    } 

    /// <summary> Gets or sets the input control to validate. </summary>
    [TypeConverter (typeof (TestBocListControlToStringConverter))]
    public new string ControlToValidate
    {
      get { return base.ControlToValidate; }
      set { base.ControlToValidate = value; }
    }

  }

  /// <summary>
  ///   Creates a VS.NET designer pick list for a property that references a 
  ///   <see cref="TestBocList"/> control.
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TypeConverter"/> attribute to assign this converter to a property.
  /// </remarks>
  public class TestBocListControlToStringConverter: ControlToStringConverter
  {
    public TestBocListControlToStringConverter ()
        : base (typeof (TestBocList))
    {
    }
  }
}