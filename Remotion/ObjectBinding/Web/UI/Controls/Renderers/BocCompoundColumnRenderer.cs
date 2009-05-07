using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocCompoundColumnDefinition"/> columns.
  /// </summary>
  public class BocCompoundColumnRenderer : BocValueColumnRenderer<BocCompoundColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocCompoundColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocCompoundColumnRenderer (BocList list, HtmlTextWriter writer, BocCompoundColumnDefinition column)
        : base(list, writer, column)
    {
    }

    /// <summary>
    /// Renders a string representation of the property of <paramref name="businessObject"/> that is shown in the column.
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> whose property will be rendered.</param>
    /// <param name="showEditModeControl">Prevents rendering if <see langword="true"/>.</param>
    /// <param name="editableRow">Ignored.</param>
    protected override void RenderCellText (IBusinessObject businessObject, bool showEditModeControl, EditableRow editableRow)
    {
      string valueColumnText = null;
      if (!showEditModeControl)
        valueColumnText = Column.GetStringValue (businessObject);

      RenderValueColumnCellText (valueColumnText);
    }
  }
}
