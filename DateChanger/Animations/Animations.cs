﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DataChanger.Animations
{
    internal static class Animations
    {
        /// <summary>
        /// Zamyka okno z animacją zmniejszenia skali i zanikania.
        /// </summary>
        /// <param name="window">Okno do animacji</param>
        public static void CloseWithAnimation(Window window)
        {
            var win = window;
            if (win == null) return;

            var scale = new ScaleTransform(1.0, 1.0);
            win.RenderTransformOrigin = new Point(0.5, 0.5);
            win.RenderTransform = scale;

            var scaleAnim = new DoubleAnimation(1.0, 0.8, TimeSpan.FromMilliseconds(300));
            var fadeAnim = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(300));

            var sb = new Storyboard();
            sb.Children.Add(scaleAnim);
            sb.Children.Add(fadeAnim);

            Storyboard.SetTarget(scaleAnim, win);
            Storyboard.SetTargetProperty(scaleAnim, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(fadeAnim, win);
            Storyboard.SetTargetProperty(fadeAnim, new PropertyPath("Opacity"));

            sb.Completed += (_, _) => win.Close();// Te sugestie są ambitne, no ale działa
            sb.Begin();
        }
    }
}
