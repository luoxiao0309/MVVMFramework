﻿#pragma checksum "..\..\..\CustomNode\DisplayNode.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0975FC363CDFEC95AA8CF3800FDD3F637BC1E9C1"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using NodeSample;
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
using System.Windows.Shell;


namespace NodeSample {
    
    
    /// <summary>
    /// DisplayNode
    /// </summary>
    public partial class DisplayNode : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\..\CustomNode\DisplayNode.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel stack_input;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\CustomNode\DisplayNode.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel stack_output;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\CustomNode\DisplayNode.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid_content;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\CustomNode\DisplayNode.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_display;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\CustomNode\DisplayNode.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid_controlbar;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\CustomNode\DisplayNode.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tb_title;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\CustomNode\DisplayNode.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_delete;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/NodeSample;component/customnode/displaynode.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CustomNode\DisplayNode.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 21 "..\..\..\CustomNode\DisplayNode.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.Btn_delete_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.stack_input = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.stack_output = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 4:
            this.grid_content = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.tb_display = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.grid_controlbar = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.tb_title = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.btn_delete = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\..\CustomNode\DisplayNode.xaml"
            this.btn_delete.Click += new System.Windows.RoutedEventHandler(this.Btn_delete_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

