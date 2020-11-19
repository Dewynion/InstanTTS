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

namespace InstanTTS.Interface.Controls
{
    /// <summary>
    /// Interaction logic for EditableSlider.xaml
    /// </summary>
    public partial class EditableSlider : UserControl
    {
        public string FieldName
        {
            get
            {
                return (string)GetValue(FieldNameProperty);
            }
            set
            {
                SetValue(FieldNameProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register("FieldName", typeof(string), typeof(EditableSlider));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableSlider));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(EditableSlider));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(EditableSlider));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(EditableSlider));

        public EditableSlider()
        {
            InitializeComponent();
            DataContext = this;
            textBox.Text = slider.Value.ToString();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            double sliderVal = slider.Value;
            try
            {
                double value = double.Parse(textBox.Text);
                value = Math.Max(slider.Minimum, Math.Min(value, slider.Maximum));
                slider.Value = value;
            }
            catch (FormatException)
            {
                textBox.Text = sliderVal.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBox.Text = slider.Value.ToString();
        }
    }
}

