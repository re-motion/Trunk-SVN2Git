using System;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///  The <b>IBusinessObjectDataSourceControl</b> interface defines the methods and 
  ///  properties required to implement a control that provides an <see cref="IBusinessObjectDataSource"/>
  ///  to controls of type <see cref="IBusinessObjectBoundWebControl"/> inside an <b>ASPX Web Form</b> 
  ///  or <b>ASCX User Control</b>.
  /// </summary>
  /// <include file='doc\include\UI\Controls\IBusinessObjectDataSourceControl.xml' path='IBusinessObjectDataSourceControl/Class/*' />
  public interface IBusinessObjectDataSourceControl : IBusinessObjectDataSource, IControl
  {
    /// <summary> Prepares all bound controls implementing <see cref="IValidatableControl"/> for validation. </summary>
    void PrepareValidation ();

    /// <summary> Validates all bound controls implementing <see cref="IValidatableControl"/>. </summary>
    /// <returns> <see langword="true"/> if no validation errors where found. </returns>
    bool Validate ();
  }
}