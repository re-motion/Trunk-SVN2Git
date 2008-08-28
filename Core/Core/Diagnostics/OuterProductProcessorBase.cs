namespace Remotion.Diagnostics
{
  /// <summary>
  /// Convenience class to derive OuterProductIndexGenerator-processors from. Already supplies ProcessingState-functionality,
  /// just requiring override implementation of <see cref="DoBeforeLoop"/> and <see cref="DoAfterLoop"/>.
  /// </summary>
  public class OuterProductProcessorBase : IOuterProductProcessor 
  { 
    private OuterProductProcessingState _processingState;
    /// <summary>
    /// The current <see cref="ProcessingState"/> to be used during callbacks.
    /// </summary>
    public OuterProductProcessingState ProcessingState
    {
      get { return _processingState; }
    }

    /// <summary>
    /// Default implementation for the callback before a new for loop starts. Simply keeps on looping.
    /// Override to implement your own functionality.
    /// </summary>
    /// <returns><see cref="IOuterProductProcessor.DoBeforeLoop"/></returns>
    public virtual bool DoBeforeLoop ()
    {
      return true;
    }

    /// <summary>
    /// Default implementation for the callback after a for loop finishes. Simply keeps on looping.
    /// Override to implement your own functionality.
    /// </summary>
    /// <returns><see cref="IOuterProductProcessor.DoAfterLoop"/></returns>
    public virtual bool DoAfterLoop ()
    {
      return true;
    }
      
    /// <summary>
    /// Internal use only: Used by OuterProductIndexGenerator class to set the current <see cref="ProcessingState"/> before invoking a callback.
    /// </summary>
    /// <param name="processingState"></param>
    public void SetProcessingState (OuterProductProcessingState processingState)
    {
      _processingState = processingState;
    } 
  }
}