﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class EditGroupControlResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EditGroupControlResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.Edi" +
                            "tGroupControlResources", typeof(EditGroupControlResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Group.
        /// </summary>
        internal static string auto_ChildrenField_FixedColumns_GroupNameItem_ColumnTitle {
            get {
                return ResourceManager.GetString("auto:ChildrenField:FixedColumns:GroupNameItem:ColumnTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One of the child groups form a circular hierarchy with the group being edited..
        /// </summary>
        internal static string auto_ChildrenValidator_Text {
            get {
                return ResourceManager.GetString("auto:ChildrenValidator:Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Edit Group.
        /// </summary>
        internal static string auto_GroupLabel_Text {
            get {
                return ResourceManager.GetString("auto:GroupLabel:Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The selected group cannot be selected as parent because it would result in a circular hierarchy with the group being edited..
        /// </summary>
        internal static string auto_ParentValidator_Text {
            get {
                return ResourceManager.GetString("auto:ParentValidator:Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Position.
        /// </summary>
        internal static string auto_RolesField_FixedColumns_PositionNameItem_ColumnTitle {
            get {
                return ResourceManager.GetString("auto:RolesField:FixedColumns:PositionNameItem:ColumnTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User.
        /// </summary>
        internal static string auto_RolesField_FixedColumns_UserNameItem_ColumnTitle {
            get {
                return ResourceManager.GetString("auto:RolesField:FixedColumns:UserNameItem:ColumnTitle", resourceCulture);
            }
        }
    }
}
