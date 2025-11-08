using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_HomeTool.Helpers
{
    //参考教程 https://stackoverflow.com/questions/5916154/how-to-handle-drag-drop-without-violating-mvvm-principals
    //该Helper不仅支持文件拖拽，也支持文件夹拖拽
    public class FileDragDropHelper
    {
        public static bool GetIsFileDragDropEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFileDragDropEnabledProperty);
        }

        public static void SetIsFileDragDropEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFileDragDropEnabledProperty, value);
        }

        public static bool GetFileDragDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(FileDragDropTargetProperty);
        }

        public static void SetFileDragDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(FileDragDropTargetProperty, value);
        }

        public static readonly DependencyProperty IsFileDragDropEnabledProperty =
                DependencyProperty.RegisterAttached("IsFileDragDropEnabled", typeof(bool), typeof(FileDragDropHelper), new PropertyMetadata(OnFileDragDropEnabled));

        public static readonly DependencyProperty FileDragDropTargetProperty =
                DependencyProperty.RegisterAttached("FileDragDropTarget", typeof(object), typeof(FileDragDropHelper), null);

        private static void OnFileDragDropEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            
            //转换为UIElement类型，则Border和Grid等控件都能响应
            var uIElement = d as UIElement;
            if (uIElement != null) uIElement.Drop += OnDrop;

            //如果转换成Control类型，则Border和Grid等控件无法响应
            //var control = d as Control;
            //if (control != null) control.Drop += OnDrop;
        }

        private static void OnDrop(object _sender, DragEventArgs _dragEventArgs)
        {
            DependencyObject d = _sender as DependencyObject;
            if (d == null) return;
            Object target = d.GetValue(FileDragDropTargetProperty);
            IFileDragDropTarget fileTarget = target as IFileDragDropTarget;
            if (fileTarget != null)
            {
                if (_dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    fileTarget.OnFileDrop((string[])_dragEventArgs.Data.GetData(DataFormats.FileDrop));
                }
            }
            else
            {
                throw new Exception("FileDragDropTarget object must be of type IFileDragDropTarget");
            }
        }
    }
}
