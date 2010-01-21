using System;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides a common interface for classes performing actions on the re-store data structures on the data management level.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each command has five steps:
  /// <list type="bullet">
  ///   <item>
  ///     <description>
  ///       <see cref="NotifyClientTransactionOfBegin"/> raises all begin event notifications on the associated <see cref="ClientTransaction"/>.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       <see cref="Begin"/> raises all begin event notifications on the objects involved in the operation.
  ///     </description>
  ///   </item>  
  ///   <item>
  ///     <description>
  ///       <see cref="Perform"/> actually performs the operation (without raising any events).
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       <see cref="End"/> raises all end event notifications on the objects involved in the operation.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       <see cref="NotifyClientTransactionOfEnd"/> raises all end event notifications on the associated <see cref="ClientTransaction"/>.
  ///     </description>
  ///   </item>
  /// </list>
  /// Not all commands have to implement all the steps, unrequired steps can be left empty.
  /// </para>
  /// <para>
  /// Some commands can be broadened to include other objects also affected by the operation via <see cref="ExtendToAllRelatedObjects"/>. For example,
  /// a relation end point modification command can be extended to include changes to all affected opposite objects via 
  /// <see cref="ExtendToAllRelatedObjects"/>.
  /// </para>
  /// </remarks>
  public interface IDataManagementCommand
  {
    /// <summary>
    /// Notifies the client transaction that the operation is about to begin. The operation can be canceled at this point of time if an event 
    /// handler throws an exception.
    /// </summary>
    void NotifyClientTransactionOfBegin ();

    /// <summary>
    /// Raises all begin event notifications on the objects involved in the operation. The operation can be canceled at this point of time if an event 
    /// handler throws an exception.
    /// </summary>
    void Begin ();

    /// <summary>
    /// Actually performs the operation without raising any events.
    /// </summary>
    void Perform ();

    /// <summary>
    /// Raises all end event notifications on the objects involved in the operation. Event handlers should not throw any exceptions at this point of 
    /// time, the operation has already been performed.
    /// </summary>
    void End ();

    /// <summary>
    /// Raises all end event notifications on the associated <see cref="ClientTransaction"/>. Event handlers should not throw any exceptions at this 
    /// point of time, the operation has already been performed.
    /// </summary>
    void NotifyClientTransactionOfEnd ();

    /// <summary>
    /// Returns a new <see cref="IDataManagementCommand"/> instance that involves changes to all objects affected by this 
    /// <see cref="IDataManagementCommand"/>. If no other objects are involved by the change, this method returns just this 
    /// <see cref="IDataManagementCommand"/>.
    /// </summary>
    CompositeDataManagementCommand ExtendToAllRelatedObjects ();
  }
}