﻿#pragma checksum "..\..\..\..\Controls\Dialogs\AuthenticationDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "900C39756BC55B4E77541DD2F6EF8DA1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace VMClusterManager.Controls.Dialogs {
    
    
    /// <summary>
    /// AuthenticationDialog
    /// </summary>
    public partial class AuthenticationDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\..\Controls\Dialogs\AuthenticationDialog.xaml"
        internal System.Windows.Controls.TextBox tbUserName;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\Controls\Dialogs\AuthenticationDialog.xaml"
        internal System.Windows.Controls.PasswordBox pwdPassword;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\Controls\Dialogs\AuthenticationDialog.xaml"
        internal System.Windows.Controls.Button btnOK;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\Controls\Dialogs\AuthenticationDialog.xaml"
        internal System.Windows.Controls.Button btnCansel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VMClusterManager;component/controls/dialogs/authenticationdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\Dialogs\AuthenticationDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tbUserName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.pwdPassword = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 3:
            this.btnOK = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\..\..\Controls\Dialogs\AuthenticationDialog.xaml"
            this.btnOK.Click += new System.Windows.RoutedEventHandler(this.btnOK_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnCansel = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
