﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Menoo.PrinterService.Client.Properties {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AppMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppMessages() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Menoo.PrinterService.Client.Properties.AppMessages", typeof(AppMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
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
        ///   Busca una cadena traducida similar a Obtener listado de eventos de impresión..
        /// </summary>
        internal static string ButtonReconnectPrintEvents {
            get {
                return ResourceManager.GetString("ButtonReconnectPrintEvents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Obtener listado de restaurantes..
        /// </summary>
        internal static string ButtonReconnectStores {
            get {
                return ResourceManager.GetString("ButtonReconnectStores", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No se han obtenido el listado de restaurantes. Intente usando el botón de reconectar..
        /// </summary>
        internal static string ErrorListStores {
            get {
                return ResourceManager.GetString("ErrorListStores", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Datos básicos.
        /// </summary>
        internal static string GroupBasicInfo {
            get {
                return ResourceManager.GetString("GroupBasicInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Datos de la impresora.
        /// </summary>
        internal static string GroupDataPrinter {
            get {
                return ResourceManager.GetString("GroupDataPrinter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca un recurso adaptado de tipo System.Drawing.Icon similar a (Icono).
        /// </summary>
        internal static System.Drawing.Icon menoo {
            get {
                object obj = ResourceManager.GetObject("menoo", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Busca un recurso adaptado de tipo System.Drawing.Icon similar a (Icono).
        /// </summary>
        internal static System.Drawing.Icon printer {
            get {
                object obj = ResourceManager.GetObject("printer", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Busca un recurso adaptado de tipo System.Drawing.Icon similar a (Icono).
        /// </summary>
        internal static System.Drawing.Icon reconnect {
            get {
                object obj = ResourceManager.GetObject("reconnect", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}
