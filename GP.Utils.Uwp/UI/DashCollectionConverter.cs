﻿// ==========================================================================
// DashCollectionConverter.cs
// Jupiter Presenter App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace GP.Utils.UI
{
    /// <summary>
    /// Converts a double array to a double collection for stroke dashing.
    /// </summary>
    public sealed class DashCollectionConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DoubleCollection result = new DoubleCollection();

            float[] dashArray = value as float[];

            if (dashArray != null)
            {
                foreach (float n in dashArray)
                {
                    result.Add(n);
                }
            }

            Dashing dashing = value as Dashing;

            if (dashing != null)
            {
                foreach (float n in dashing.Values)
                {
                    result.Add(n);
                }
            }

            return result;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  
        /// This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
