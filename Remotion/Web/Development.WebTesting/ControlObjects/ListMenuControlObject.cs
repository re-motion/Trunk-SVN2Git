using JetBrains.Annotations;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="ListMenu"/>.
  /// </summary>
  public class ListMenuControlObject : RemotionControlObject
  {
     public ListMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }
  }
}