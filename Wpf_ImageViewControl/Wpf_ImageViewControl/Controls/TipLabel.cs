using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

    [TemplatePart(Name = Part_Label, Type = typeof(Label))]
    public class TipLabel : ContentControl
    {
        private Timer Timer = new Timer();
        static TipLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TipLabel), new FrameworkPropertyMetadata(typeof(TipLabel)));
        }

        private const string Part_Label = "Part_Label";
        internal Label PartLabel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PartLabel = this.GetTemplateChild(Part_Label) as Label;
        }

        /// <summary>
        /// 单位是毫秒
        /// </summary>
        public double ClearTime
        {
            get { return (double)GetValue(ClearTimeProperty); }
            set { SetValue(ClearTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearTimeProperty =
            DependencyProperty.Register("ClearTime", typeof(double), typeof(TipLabel), new PropertyMetadata(2000.0));



        public bool AllowAutoClear
        {
            get { return (bool)GetValue(AllowAutoClearProperty); }
            set { SetValue(AllowAutoClearProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowAutoClear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowAutoClearProperty =
            DependencyProperty.Register("AllowAutoClear", typeof(bool), typeof(TipLabel), new PropertyMetadata(true));



        private void LaunchTimer(double milliseconds)
        {
            if (!AllowAutoClear)
                return;
            if (this.Timer == null)
                this.Timer = new Timer();
            this.Timer.Enabled = true;
            this.Timer.AutoReset = false;
            this.Timer.Interval = milliseconds;
            this.Timer.Elapsed -= Timer_Elapsed;
            this.Timer.Elapsed += Timer_Elapsed;
            this.Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Content = null;
            });
        }


        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent == null)
                return;
            this.LaunchTimer(this.ClearTime);
        }
    }
}
