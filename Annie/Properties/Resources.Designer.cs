﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AnnieMayDiscordBot.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AnnieMayDiscordBot.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to PLACEHOLDER.
        /// </summary>
        internal static string ANILIST_API_TOKEN {
            get {
                return ResourceManager.GetString("ANILIST_API_TOKEN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PLACEHOLDER.
        /// </summary>
        internal static string DISCORD_BOT_TOKEN {
            get {
                return ResourceManager.GetString("DISCORD_BOT_TOKEN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PLACEHOLDER.
        /// </summary>
        internal static string MAL_API_TOKEN {
            get {
                return ResourceManager.GetString("MAL_API_TOKEN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to mongodb://localhost:27017/.
        /// </summary>
        internal static string MONGO_DB_URI {
            get {
                return ResourceManager.GetString("MONGO_DB_URI", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ~.
        /// </summary>
        internal static string PREFIX {
            get {
                return ResourceManager.GetString("PREFIX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0.
        /// </summary>
        internal static string VERSION_MAJOR {
            get {
                return ResourceManager.GetString("VERSION_MAJOR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 2.
        /// </summary>
        internal static string VERSION_MINOR {
            get {
                return ResourceManager.GetString("VERSION_MINOR", resourceCulture);
            }
        }
    }
}
