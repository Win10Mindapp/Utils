﻿// ==========================================================================
// AdvancedTextBox.cs
// Universal App Utils
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
// ReSharper disable UnusedParameter.Global

namespace GP.Utils.UI.Controls
{
    /// <summary>
    /// An advanced textbox with improved accept handling.
    /// </summary>
    public class AdvancedTextBox : TextBox
    {
        private string originalText;

        /// <summary>
        /// Identifies the <see cref="HandleAllKeys"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HandleAllKeysProperty =
            DependencyPropertyManager.Register<AdvancedTextBox, bool>(nameof(HandleAllKeys), false);
        /// <summary>
        /// Gets or sets the text box will handle all keys.
        /// </summary>
        public bool HandleAllKeys
        {
            get { return (bool)GetValue(HandleAllKeysProperty); }
            set { SetValue(HandleAllKeysProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AcceptsEscape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsEscapeProperty =
            DependencyPropertyManager.Register<AdvancedTextBox, bool>(nameof(AcceptsEscape), false);
        /// <summary>
        /// Gets or sets the value that determines whether the text box allows the escape key to reset the text.
        /// </summary>
        public bool AcceptsEscape
        {
            get { return (bool)GetValue(AcceptsEscapeProperty); }
            set { SetValue(AcceptsEscapeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AcceptsReturnModifierKeysDesktop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnModifierKeysDesktopProperty =
            DependencyPropertyManager.Register<AdvancedTextBox, VirtualKeyModifiers>(nameof(AcceptsReturnModifierKeysDesktop), VirtualKeyModifiers.None);
        /// <summary>
        /// Gets or sets the modifier keys that must be pressed when the return key will be handled.
        /// </summary>
        public VirtualKeyModifiers AcceptsReturnModifierKeysDesktop
        {
            get { return (VirtualKeyModifiers)GetValue(AcceptsReturnModifierKeysDesktopProperty); }
            set { SetValue(AcceptsReturnModifierKeysDesktopProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AcceptsReturnModifierKeysTouch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnModifierKeysTouchProperty =
            DependencyPropertyManager.Register<AdvancedTextBox, VirtualKeyModifiers>(nameof(AcceptsReturnModifierKeysTouch), VirtualKeyModifiers.None);
        /// <summary>
        /// Gets or sets the modifier keys that must be pressed when the return key will be handled.
        /// </summary>
        public VirtualKeyModifiers AcceptsReturnModifierKeysTouch
        {
            get { return (VirtualKeyModifiers)GetValue(AcceptsReturnModifierKeysTouchProperty); }
            set { SetValue(AcceptsReturnModifierKeysTouchProperty, value); }
        }

        /// <summary>
        /// Shows the text box.
        /// </summary>
        protected void Show()
        {
            Visibility = Visibility.Visible;

            Focus(FocusState.Pointer);
        }

        /// <summary>
        /// Hides the text box.
        /// </summary>
        protected void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Gets or sets a value indicating if the text box is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return Visibility == Visibility.Visible && RenderSize.Width > 0 && RenderSize.Height > 0 && Opacity > 0; }
        }

        /// <summary>
        /// Called when the reset key is pressed without any modifiers.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected virtual void OnReset(KeyRoutedEventArgs e)
        {
        }

        /// <summary>
        /// Called when the enter key is pressed without any modifiers.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected virtual void OnEnter(KeyRoutedEventArgs e)
        {
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            SelectAll();

            originalText = Text;

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (IsVisible && ((e.Key == VirtualKey.Enter) || (e.Key == VirtualKey.Escape)))
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// Called before the OnKeyUp event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (IsVisible)
            {
                if (e.Key == VirtualKey.Enter)
                {
                    if (IsCorrectModifier() && AcceptsReturn)
                    {
                        var selection = SelectionStart;
                        var counter = selection;

                        var text = Text;

                        var caretPosition = 0;

                        foreach (var c in text)
                        {
                            caretPosition++;

                            if (c != '\r')
                            {
                                counter--;
                            }

                            if (counter <= 0)
                            {
                                break;
                            }
                        }

                        if (caretPosition < 0 || caretPosition >= text.Length)
                        {
                            Text += Environment.NewLine;
                        }
                        else
                        {
                            Text = text.Insert(caretPosition, Environment.NewLine);
                        }

                        SelectionStart = selection + 1;

                        e.Handled = true;
                    }
                    else if (RequiredModifiers() != VirtualKeyModifiers.None)
                    {
                        OnEnter(e);
                    }
                }
                else if (e.Key == VirtualKey.Escape && AcceptsEscape)
                {
                    Text = originalText;

                    OnReset(e);

                    e.Handled = true;
                }

                base.OnKeyUp(e);

                if (HandleAllKeys)
                {
                    e.Handled = true;
                }
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        private bool IsCorrectModifier()
        {
            var modifiers = RequiredModifiers();

            return
                CheckModifier(modifiers, VirtualKeyModifiers.Control, VirtualKey.Control) &&
                CheckModifier(modifiers, VirtualKeyModifiers.Menu, VirtualKey.Menu) &&
                CheckModifier(modifiers, VirtualKeyModifiers.Shift, VirtualKey.Shift) &&
               (CheckModifier(modifiers, VirtualKeyModifiers.Windows, VirtualKey.LeftWindows) ||
                CheckModifier(modifiers, VirtualKeyModifiers.Windows, VirtualKey.RightWindows));
        }

        private VirtualKeyModifiers RequiredModifiers()
        {
            return IsTouch() ? AcceptsReturnModifierKeysTouch : AcceptsReturnModifierKeysDesktop;
        }

        private static bool IsTouch()
        {
            return InputPane.GetForCurrentView()?.Visible == true;
        }

        private static bool CheckModifier(VirtualKeyModifiers current, VirtualKeyModifiers requested, VirtualKey key)
        {
            if ((current & requested) == requested)
            {
                return (Window.Current.CoreWindow.GetKeyState(key) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            }

            return true;
        }
    }
}
