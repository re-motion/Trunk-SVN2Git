using System;
using System.ComponentModel;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface contains all public members of System.Web.UI.Control. It is used to derive interfaces that will be
///   implemented by deriving from System.Web.UI.Control.
/// </summary>
/// <remarks>
///   The reason for providing this interface is that derived interfaces do not need to be casted to System.Web.UI.Control.
/// </remarks>
public interface IControl: IComponent, IDataBindingsAccessor, IParserAccessor
{
  event EventHandler DataBinding;
  event EventHandler Init;
  event EventHandler Load;
  event EventHandler PreRender;
  event EventHandler Unload;

  void DataBind();
  Control FindControl(string id);
  bool HasControls();

  void RenderControl(HtmlTextWriter writer);
  void SetRenderMethodDelegate(RenderMethod renderMethod);

  Control BindingContainer { get; }
  string ClientID { get; }
  ControlCollection Controls { get; }
  bool EnableViewState { get; set; }
  string ID { get; set; }
  Control NamingContainer { get; }
  Page Page { get; set; }
  Control Parent { get; }
  string TemplateSourceDirectory { get; }
  string UniqueID { get; }
  bool Visible { get; set; }
}

}
