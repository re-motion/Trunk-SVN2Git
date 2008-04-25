using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   Extends an <see cref="IBusinessObjectBoundWebControl"/> with functionality for validating the control's 
  ///   <see cref="IBusinessObjectBoundControl.Value"/> and writing it back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     See <see cref="IBusinessObjectBoundEditableControl.SaveValue"/> for a description of the data binding 
  ///     process.
  ///   </para><para>
  ///     See <see cref="BusinessObjectBoundEditableWebControl"/> for the <see langword="abstract"/> default 
  ///     implementation.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundWebControl"/>
  /// <seealso cref="IBusinessObjectBoundEditableControl"/>
  /// <seealso cref="IValidatableControl"/>
  /// <seealso cref="IBusinessObjectDataSourceControl"/>
  public interface IBusinessObjectBoundEditableWebControl
      :
          IBusinessObjectBoundWebControl,
          IBusinessObjectBoundEditableControl,
          IValidatableControl,
          IEditableControl
  {
  }
}