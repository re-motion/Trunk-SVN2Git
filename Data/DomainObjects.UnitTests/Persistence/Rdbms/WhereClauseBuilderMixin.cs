using System;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  public class WhereClauseBuilderMixin : Mixin<WhereClauseBuilderMixin.IRequirements>
  {
    public interface IRequirements
    {
      StringBuilder Builder { get; }
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();
      This.Builder.Append ("Mixed!");
    }
  }
}