﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Vision;

namespace NeuralAction.WPF.Apps.EyesExercise
{
    public class EyesExercise : IDisposable
    {
        public static EyesExercise Current = new EyesExercise();

        public bool IsShowed { get; set; } = false;

        EyesExerciseWindow Window;

        DispatcherTimer moveUpdater;

        Stopwatch stopwatch;

        double screenwidth = SystemParameters.PrimaryScreenWidth;
        double screenheight = SystemParameters.PrimaryScreenHeight;

        public EyesExercise()
        {
            Window = new EyesExerciseWindow();
        }

        public void Show()
        {
            var scr = InputService.Current.TargetScreen.Bounds;
            Window.MainTitle.Text = "wait a second...";
            Window.TimerProgress.Height = screenheight * 0.35;
            Window.TimerProgress.VerticalAlignment = VerticalAlignment.Top;
            Window.TimerProgress.HorizontalAlignment = HorizontalAlignment.Stretch;
            Window.TimerProgress.Margin = new Thickness(0, 0, 0, 0);
            Window.EyesExerciseArrow.Visibility = Visibility.Visible;
            Window.TimerProgress.Visibility = Visibility.Visible;
            Window.Show();

            step = 0;

            stopwatch = new Stopwatch();
            if (moveUpdater == null)
            {
                moveUpdater = new DispatcherTimer();
                moveUpdater.Interval = TimeSpan.FromMilliseconds(10);
                moveUpdater.Tick += MoveUpdater_Tick;
            }
            moveUpdater.Start();
            var cursor = InputService.Current.Cursor;
            cursor.Window.Visibility = Visibility.Visible;
            GlobalKeyHook.Hook.KeyboardPressed += Hook_KeyboardPressed;
            IsShowed = true;
        }

        public void Close()
        {
            GlobalKeyHook.Hook.KeyboardPressed -= Hook_KeyboardPressed;
            Window.Hide();
            moveUpdater.Stop();
            IsShowed = false;
        }

        void Hook_KeyboardPressed(object sender, GlobalKeyHookEventArgs e)
        {
            if (e.VKeyCode == VKeyCode.F4)
            {
                if (GlobalKeyHook.IsKeyPressed(VKeyCode.LeftControl) && GlobalKeyHook.IsKeyPressed(VKeyCode.LeftAlt))
                {
                    Close();
                }
            }
        }

        double deltaTime = 1;
        double lastMs = 0;
        int step = 0;
        void MoveUpdater_Tick(object sender, EventArgs e)
        {
            var elapsed = Logger.Stopwatch.ElapsedMilliseconds - lastMs;
            lastMs = Logger.Stopwatch.ElapsedMilliseconds;
            deltaTime = elapsed / 1000;
            if (step == 9)
            {
                stopwatch.Start();
                Window.EyesExerciseArrow.Visibility = Visibility.Hidden;
                Window.TimerProgress.Visibility = Visibility.Hidden;
                Window.MainTitle.Text = "Finish! This will be closed in " + (5 - stopwatch.Elapsed.Seconds);
                if (stopwatch.Elapsed.Seconds == 5)
                {
                    Close();
                }
            }

            if (InputService.Current.Cursor.Position != null && step != 9)
            {
                stopwatch.Start();

                if (stopwatch.Elapsed.Seconds == 5 && step != 4)
                {
                    step++;
                    stopwatch.Reset();
                    Window.TimerProgress.Value = 0;
                }

                if (stopwatch.Elapsed.Seconds == 10)
                {
                    step++;
                    stopwatch.Reset();
                    Window.TimerProgress.Value = 0;
                }

                if (step == 0 || step == 1)
                {
                    Window.EyesExerciseArrow.HorizontalAlignment = HorizontalAlignment.Center;
                }

                if (step == 2 || step == 5 || step == 8)
                {
                    Window.EyesExerciseArrow.HorizontalAlignment = HorizontalAlignment.Left;
                }

                if (step == 3 || step == 6 || step == 7)
                {
                    Window.EyesExerciseArrow.HorizontalAlignment = HorizontalAlignment.Right;
                }

                if (step == 0 || step == 5 || step == 7)
                {
                    Window.EyesExerciseArrow.VerticalAlignment = VerticalAlignment.Top;
                }

                if (step == 1 || step == 6 || step == 8)
                {
                    Window.EyesExerciseArrow.VerticalAlignment = VerticalAlignment.Bottom;
                }

                if (step == 2 || step == 3)
                {
                    Window.EyesExerciseArrow.VerticalAlignment = VerticalAlignment.Center;
                }


                if (step == 0)
                {
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(90, 0, 0);
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Top;
                    Window.TimerProgress.Margin = new Thickness(0, 0, 0, 0);
                    if (InputService.Current.Cursor.Position.Y <= InputService.Current.Cursor.Screen.PixelSize.Height * 0.35)
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }
                    Window.MainTitle.Text = "Looking at top without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }

                if (step == 1)
                {
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(270, 0, 0);
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Bottom;
                    Window.TimerProgress.Margin = new Thickness(0, screenheight - (screenheight * 0.35), 0, 0);
                    if (InputService.Current.Cursor.Position.Y >= InputService.Current.Cursor.Screen.PixelSize.Height - (InputService.Current.Cursor.Screen.PixelSize.Height * 0.35))
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }

                    Window.MainTitle.Text = "Looking at bottom without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }

                if (step == 2)
                {
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(0, 0, 0);
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Top;
                    Window.TimerProgress.HorizontalAlignment = HorizontalAlignment.Left;
                    Window.TimerProgress.Width = screenwidth * 0.35;
                    Window.TimerProgress.Height = screenheight;
                    Window.TimerProgress.Orientation = System.Windows.Controls.Orientation.Vertical;
                    Window.TimerProgress.Margin = new Thickness(0, 0, 0, 0);
                    if (InputService.Current.Cursor.Position.X <= InputService.Current.Cursor.Screen.PixelSize.Width * 0.35)
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }
                    Window.MainTitle.Text = "Looking at left without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }

                if (step == 3)
                {
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(180, 0, 0);
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Top;
                    Window.TimerProgress.HorizontalAlignment = HorizontalAlignment.Right;
                    if (InputService.Current.Cursor.Position.X >= InputService.Current.Cursor.Screen.PixelSize.Width - (InputService.Current.Cursor.Screen.PixelSize.Width * 0.35))
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }
                    Window.MainTitle.Text = "Looking at right without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }

                if (step == 4)
                {
                    Window.TimerProgress.Width = screenwidth;
                    Window.TimerProgress.Height = screenheight;
                    Window.TimerProgress.HorizontalAlignment = HorizontalAlignment.Left;
                    Window.TimerProgress.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    Window.TimerProgress.Value += 10 * deltaTime;
                    Window.EyesExerciseArrow.Opacity = 0;
                    Window.MainTitle.Text = "Close your eyes for " + (10 - stopwatch.Elapsed.Seconds);
                }

                if (step == 5)
                {

                    Window.EyesExerciseArrow.Opacity = 1;
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(45, 0, 0);
                    Window.TimerProgress.Height = screenheight * 0.35;
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Top;
                    Window.TimerProgress.HorizontalAlignment = HorizontalAlignment.Stretch;
                    Window.TimerProgress.Margin = new Thickness(0, 0, 0, 0);
                    if (InputService.Current.Cursor.Position.Y <= InputService.Current.Cursor.Screen.PixelSize.Height * 0.35)
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }
                    Window.MainTitle.Text = "Looking at left-top without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }
                if (step == 6)
                {
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(225, 0, 0);
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Bottom;
                    Window.TimerProgress.Margin = new Thickness(0, screenheight - (screenheight * 0.35), 0, 0);
                    if (InputService.Current.Cursor.Position.Y >= InputService.Current.Cursor.Screen.PixelSize.Height - (InputService.Current.Cursor.Screen.PixelSize.Height * 0.35))
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }
                    Window.MainTitle.Text = "Looking at right-bottom without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }
                if (step == 7)
                {
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(135, 0, 0);
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Top;
                    Window.TimerProgress.Margin = new Thickness(0, 0, 0, 0);
                    if (InputService.Current.Cursor.Position.Y <= InputService.Current.Cursor.Screen.PixelSize.Height * 0.35)
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }
                    Window.MainTitle.Text = "Looking at right-top without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }
                if (step == 8)
                {
                    Window.EyesExerciseArrow.RenderTransform = new RotateTransform(315, 0, 0);
                    Window.TimerProgress.VerticalAlignment = VerticalAlignment.Bottom;
                    Window.TimerProgress.Margin = new Thickness(0, screenheight - (screenheight * 0.35), 0, 0);
                    if (InputService.Current.Cursor.Position.Y >= InputService.Current.Cursor.Screen.PixelSize.Height - (InputService.Current.Cursor.Screen.PixelSize.Height * 0.35))
                    {
                        stopwatch.Start();
                        Window.TimerProgress.Value += 20 * deltaTime;
                    }
                    else
                    {
                        stopwatch.Stop();
                    }
                    Window.MainTitle.Text = "Looking at left-bottom without eyes-focus for " + (5 - stopwatch.Elapsed.Seconds);
                }
            }
            else if (step != 9)
            {
                Window.MainTitle.Text = "Can't detect your eyes!";
                stopwatch.Stop();
            }
        }

        public void Dispose()
        {
            if (IsShowed)
                Close();

            Window?.Close();
            Window = null;
        }
    }
}
