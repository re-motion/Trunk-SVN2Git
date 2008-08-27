namespace Remotion.Diagnostic
{
  /// <summary>
  /// Interface to a class which can be passed to <see cref="OuterProductIndexGenerator.ProcessOuterProduct"/>.
  /// <see cref="OuterProductIndexGenerator.ProcessOuterProduct"/> will then call its methods <see cref="DoBeforeLoop"/>
  /// and <see cref="DoAfterLoop"/> before and after it starts iterating over a new outer product dimension.
  /// </summary>
  public interface IOuterProductProcessor {
    /// <summary>
    /// Processor callback invoked before a nested for loop starts.
    /// </summary>
    /// <returns><c>true</c> to continue looping, <c>false</c> to break from the current loop.</returns>
    //TODO: rename ProcessStateBeforeLoop?
    bool DoBeforeLoop ();
    /// <summary>
    /// Processor callback invoked after a nested for loop has finished.
    /// </summary>
    /// <returns><c>true</c> to continue looping, <c>false</c> to break from the current loop.</returns>
    //TODO: rename ProcessStateAfterLoop?
    bool DoAfterLoop ();
    /// <summary>
    /// Before each callback to the processor the OuterProductIndexGenerator class sets the current <see cref="OuterProductProcessingState"/> through a
    /// call to this method. The processor class is expected to store the <see cref="OuterProductProcessingState"/> to be able to access
    /// it during the callbacks.
    /// </summary>
    /// <param name="processingState"></param>
    void SetProcessingState (OuterProductProcessingState processingState);

    /// <summary>
    /// The current <see cref="OuterProductProcessingState"/> to be used during callbacks. Set by the OuterProductIndexGenerator class
    /// in call to <see cref="SetProcessingState"/>.
    /// </summary>
    OuterProductProcessingState ProcessingState { get; }
  }
}