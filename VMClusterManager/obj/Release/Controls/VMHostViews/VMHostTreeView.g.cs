﻿#pragma checksum "..\..\..\..\Controls\VMHostViews\VMHostTreeView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "54D5B85C4D5CB787DCC02F638C366F33"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
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
using VMClusterManager.ViewModels.VMHostModels;


namespace VMClusterManager.Controls.VMHostViews {
    
    
    /// <summary>
    /// VMHostTreeView
    /// </summary>
    public partial class VMHostTreeView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\Controls\VMHostViews\VMHostTreeView.xaml"
        internal System.Windows.Controls.TreeView treeElements;
        
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
            System.Uri resourceLocater = new System.Uri("/VMClusterManager;component/controls/vmhostviews/vmhosttreeview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\VMHostViews\VMHostTreeView.xaml"
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
            this.treeElements = ((System.Windows.Controls.TreeView)(target));
            
            #line 10 "..\..\..\..\Controls\VMHostViews\VMHostTreeView.xaml"
            this.treeElements.LayoutUpdated += new System.EventHandler(this.treeElements_LayoutUpdated);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
