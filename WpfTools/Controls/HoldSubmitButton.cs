using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace aemarcoCommons.WpfTools.Controls
{
    //https://www.youtube.com/watch?v=rWruXQkzVuU&t=148s

    public class HoldSubmitButton : Button
    {
        private CancellationTokenSource _cancellationTokenSource;

        public static readonly DependencyProperty HoldDurationProperty =
            DependencyProperty.Register(nameof(HoldDuration), typeof(Duration), typeof(HoldSubmitButton),
                new PropertyMetadata(Duration.Automatic, null, CoerceHoldDuration));
        private static object CoerceHoldDuration(DependencyObject d, object baseValue)
        {
            if (baseValue is Duration duration && duration.TimeSpan.TotalSeconds >= 0.5)
            {
                return baseValue;
            }
            return new Duration(TimeSpan.FromSeconds(0.5));
        }
        public Duration HoldDuration
        {
            get => (Duration)GetValue(HoldDurationProperty);
            set => SetValue(HoldDurationProperty, value);
        }


        public static readonly DependencyProperty PrimaryColorProperty =
            DependencyProperty.Register(nameof(PrimaryColor), typeof(Brush), typeof(HoldSubmitButton),
                new PropertyMetadata((SolidColorBrush)(new BrushConverter().ConvertFrom("#451400"))));
        public Brush PrimaryColor
        {
            get => (Brush)GetValue(PrimaryColorProperty);
            set => SetValue(PrimaryColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(HoldSubmitButton),
                new PropertyMetadata((SolidColorBrush)(new BrushConverter().ConvertFrom("#b3831e"))));
        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(nameof(ForegroundColor), typeof(Brush), typeof(HoldSubmitButton),
                new PropertyMetadata(Brushes.White));
        public Brush ForegroundColor
        {
            get => (Brush)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }





        public static readonly RoutedEvent HoldCompletedEvent =
            EventManager.RegisterRoutedEvent(nameof(HoldCompleted), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HoldSubmitButton));
        public event RoutedEventHandler HoldCompleted
        {
            add => AddHandler(HoldCompletedEvent, value);
            remove => RemoveHandler(HoldCompletedEvent, value);
        }

        public static readonly RoutedEvent HoldCancelledEvent =
            EventManager.RegisterRoutedEvent(nameof(HoldCancelled), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HoldSubmitButton));
        public event RoutedEventHandler HoldCancelled
        {
            add => AddHandler(HoldCancelledEvent, value);
            remove => RemoveHandler(HoldCancelledEvent, value);
        }



        static HoldSubmitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HoldSubmitButton), new FrameworkPropertyMetadata(typeof(HoldSubmitButton)));
        }


        protected override async void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Delay(HoldDuration.TimeSpan, _cancellationTokenSource.Token);
                base.OnClick();

                RaiseEvent(new RoutedEventArgs(HoldCompletedEvent));
            }
            catch (TaskCanceledException)
            {
                RaiseEvent(new RoutedEventArgs(HoldCancelledEvent));
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnClick() { }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            CancelSubmit();
            base.OnPreviewMouseLeftButtonUp(e);
        }


        protected override void OnMouseLeave(MouseEventArgs e)
        {
            CancelSubmit();
            base.OnMouseLeave(e);
        }


        private void CancelSubmit()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }

    public class DurationSecondsSubtractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = new NumberFormatInfo { NumberDecimalSeparator = "." };
            if (value is Duration duration && double.TryParse(parameter?.ToString(), NumberStyles.Any, format, out var seconds))
            {
                return duration.Subtract(TimeSpan.FromSeconds(seconds));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
