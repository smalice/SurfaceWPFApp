using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Surface.Presentation;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Threading;
   
   namespace CapgeminiSurface
   {
       public sealed class GuiHelpers
       {
           public static T GetParentObject<T>(DependencyObject obj, bool exactTypeMatch) where T : DependencyObject
           {
               try
               {
                   while (obj != null &&
                       (exactTypeMatch ? (obj.GetType() != typeof(T)) : !(obj is T)))
                   {
                       if (obj is Visual || obj is Visual3D)
                       {
                           obj = VisualTreeHelper.GetParent(obj) as DependencyObject;
                       }
                       else
                       {
                           obj = LogicalTreeHelper.GetParent(obj) as DependencyObject;
                       }
                   }
                   return obj as T;
               }
               catch (Exception ex)
               {
                   Console.WriteLine(ex);
                   return null;
               }
           }
 
		public static T GetParentObject<T>(DependencyObject obj) where T : DependencyObject
           {
               return GetParentObject<T>(obj, false);
           }
   
           public static T GetChildObject<T>(DependencyObject obj) where T : class
           {
               for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
               {
                   DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                   if (child is T)
                       return child as T;
                   else
                   {
                       child = GetChildObject<T>(child) as DependencyObject;
                       if (child is T)
                           return child as T;
                   }
              }
               return null;
           }
       }
   }
   