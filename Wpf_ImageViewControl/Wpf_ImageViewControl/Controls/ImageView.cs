using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_ImageViewControl
{
    [TemplatePart(Name = Part_MainCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = Part_Image, Type = typeof(Image))]
    [TemplatePart(Name = Part_Label, Type = typeof(TipLabel))]
    public class ImageView : Control
    {
        private const string Part_Image = "Part_Image";
        private const string Part_MainCanvas = "Part_MainCanvas";
        private const string Part_Label = "Part_Label";

        internal Image ImageControl;
        internal Canvas MainCanvas;
        internal TipLabel ScaleLabel;

        static ImageView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageView), new FrameworkPropertyMetadata(typeof(ImageView)));
        }

        public ImageView()
        {
            this.SizeChanged += ImageView_SizeChanged;
        }

        private void ImageView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InitImageControl();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MainCanvas = GetTemplateChild(Part_MainCanvas) as Canvas;
            this.ImageControl = GetTemplateChild(Part_Image) as Image;
            this.ScaleLabel = GetTemplateChild(Part_Label) as TipLabel;
            this.ImageControl.MouseUp += ImageControl_MouseUp;
            this.ImageControl.MouseMove += ImageControl_MouseMove;
            this.ImageControl.MouseDown += ImageControl_MouseDown;
            this.MainCanvas.MouseWheel += MainCanvas_MouseWheel;
        }

        private Point? initMousePosition = null;
        private void ImageControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.Cursor = System.Windows.Input.Cursors.SizeAll;
            initMousePosition = e.GetPosition(element);
            element.CaptureMouse();
        }

        private void ImageControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (initMousePosition != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var point = e.GetPosition(MainCanvas);
                Canvas.SetLeft(element, point.X - initMousePosition.Value.X);
                Canvas.SetTop(element, point.Y - initMousePosition.Value.Y);
            }
        }

        private void ImageControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.Cursor = System.Windows.Input.Cursors.Hand;
            initMousePosition = null;
            element.ReleaseMouseCapture();
        }

        //缩放的比例
        private double scaleIncrement = 0.1;
        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //鼠标相对于Image控件的位置
            var position = e.GetPosition(this.ImageControl);
            //鼠标位置在Image控件之外
            if (position.X < 0 || position.X > this.ImageControl.Width
                || position.Y < 0 || position.Y > this.ImageControl.Height)
                return;
            var isMagnify = e.Delta > 0;
            var height = this.ImageControl.Height;
            var width = this.ImageControl.Width;
            if (isMagnify)
            {
                height = height * (1 + scaleIncrement);
                width = width * (1 + scaleIncrement);
            }
            else
            {
                height = height * (1 - scaleIncrement);
                width = width * (1 - scaleIncrement);
            }
            this.ImageControl.Height = height;
            this.ImageControl.Width = width;
            this.CalculateImagePosition(isMagnify, position);
            this.CalculateImageScale();
        }

        /// <summary>
        /// 为Image控件计算一个适宜的位置
        /// </summary>
        /// <param name="isMagnify">是否为放大</param>
        /// <param name="currentPosition">当前鼠标相对于Image控件的位置</param>
        private void CalculateImagePosition(bool isMagnify, Point currentPosition)
        {
            double topOffset = currentPosition.Y * scaleIncrement;
            double leftOffset = currentPosition.X * scaleIncrement;
            double top = Canvas.GetTop(this.ImageControl);
            double left = Canvas.GetLeft(this.ImageControl);
            if (isMagnify)
            {
                top = top - topOffset;
                left = left - leftOffset;
            }
            else
            {
                top = top + topOffset;
                left = left + leftOffset;
            }
            this.SetImagePosition(top, left);
        }

        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapImage), typeof(ImageView), new PropertyMetadata(null, OnImageSourcePropertyChanged));

        private static void OnImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = d as ImageView;
            if (ctl == null)
                return;
            ctl.InitImageControl();
        }

        private void InitImageControl()
        {
            if (this.ActualHeight <= 0 || this.ActualWidth <= 0 || this.ImageControl == null || this.ImageSource == null)
                return;
            double imageControlHeight = 0.0;
            double imageControlWidth = 0.0;
            if (this.ImageSource.PixelHeight <= this.ActualHeight && this.ImageSource.PixelWidth <= this.ActualWidth)
            {
                imageControlHeight = this.ImageSource.PixelHeight;
                imageControlWidth = this.ImageSource.PixelWidth;
            }
            else if (this.ImageSource.PixelHeight > this.ActualHeight && this.ImageSource.PixelWidth <= this.ActualWidth)
            {
                var rate = (double)this.ActualHeight / this.ImageSource.PixelHeight;
                imageControlHeight = this.ImageSource.PixelHeight * rate;
                imageControlWidth = this.ImageSource.PixelWidth * rate;
            }
            else if (this.ImageSource.PixelHeight <= this.ActualHeight && this.ImageSource.PixelWidth > this.ActualWidth)
            {
                var rate = (double)this.ActualWidth / this.ImageSource.PixelWidth;
                imageControlHeight = this.ImageSource.PixelHeight * rate;
                imageControlWidth = this.ImageSource.PixelWidth * rate;
            }
            else
            {
                var widthRate = (double)this.ActualWidth / this.ImageSource.PixelWidth;
                var heightRate = (double)this.ActualHeight / this.ImageSource.PixelHeight;
                var minRate = Math.Min(widthRate, heightRate);
                imageControlHeight = this.ImageSource.PixelHeight * minRate;
                imageControlWidth = this.ImageSource.PixelWidth * minRate;
            }
            this.ImageControl.Height = imageControlHeight;
            this.ImageControl.Width = imageControlWidth;
            double top = this.ActualHeight / 2 - this.ImageControl.Height / 2;
            double left = this.ActualWidth / 2 - this.ImageControl.Width / 2;
            this.SetImagePosition(top, left);
            this.CalculateImageScale();
        }

        private void SetImagePosition(double top, double left)
        {
            Canvas.SetTop(this.ImageControl, top);
            Canvas.SetLeft(this.ImageControl, left);
        }

        private void CalculateImageScale()
        {
            if (this.ImageSource == null || this.ImageSource.PixelHeight <= 0 || this.ImageSource.PixelWidth <= 0
                || this.ImageControl == null || this.ImageControl.Height <= 0 || this.ImageControl.Width <= 0 || this.ScaleLabel == null)
                return;
            var rate = (this.ImageControl.Height * this.ImageControl.Width) / (this.ImageSource.PixelHeight * this.ImageSource.PixelWidth);
            this.ScaleLabel.Content = rate.ToString("0%");
        }

    }
}
