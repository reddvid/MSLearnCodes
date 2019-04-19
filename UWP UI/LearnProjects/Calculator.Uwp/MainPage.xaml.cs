using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Calculator.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private double? FirstNumber { get; set; }
        private double? SecondNumber { get; set; }
        private Func<double, double, double> SelectedMathFunction { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private double Add(Double a, double b)
        {
            return a + b;
        }

        private double Subtract(double a, double b)
        {
            return a - b;
        }

        private double Multiply(double a, double b)
        {
            return a * b;
        }

        private double Divide(double a, double b)
        {
            return a / b;
        }

        private void FirstNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if the text in fist box is empty
            if (string.IsNullOrEmpty(FirstNumberBox?.Text))
            {
                FirstNumberBox = null;
                return;
            }

            // If text was entered, check if it can be parsed to a number
            if (Double.TryParse(FirstNumberBox.Text, out double parsedNumber))
            {
                // If text is an int, set the FirstNumber property
                FirstNumber = parsedNumber;
            }
            else
            {
                // If its not a number, remove the last character with Trim()
                FirstNumberBox.Text = FirstNumberBox.Text.TrimEnd(FirstNumberBox.Text.LastOrDefault());
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;

            string radioButtonContent = radioButton?.Content?.ToString();

            switch (radioButtonContent)
            {
                case "Add":
                    SelectedMathFunction = Add;
                    break;

                case "Subtract":
                    SelectedMathFunction = Subtract;
                    break;

                case "Multiply":
                    SelectedMathFunction = Multiply;
                    break;

                case "Divide":
                    SelectedMathFunction = Divide;
                    break;

                default:
                    SelectedMathFunction = null;
                    break;
            }
        }

        private void SecondNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SecondNumberBox?.Text))
            {
                SecondNumber = null;
                return;
            }

            if (double.TryParse(SecondNumberBox.Text, out double parsedNumber))
            {
                SecondNumber = parsedNumber;
            }
            else
            {
                SecondNumberBox.Text = SecondNumberBox.Text.TrimEnd(SecondNumberBox.Text.LastOrDefault());
            }
        }

        private void SecondNumberSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SecondNumberBox.Text = e.NewValue.ToString("N");
        }

        private void IncludeDateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Show DatePickers
            CalculationDatePicker.Visibility = Visibility.Visible;
            CalculationDatePicker.SelectedDate = DateTimeOffset.Now;
        }

        private void IncludeDateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Hide Pickers
            CalculationDatePicker.Visibility = Visibility.Collapsed;
        }

        private async void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            // Before doing any calculations, confirm the user entered both numbers
            if (FirstNumber == null || SecondNumber == null)
            {
                await new MessageDialog("You need to set both numbers to calculate a result.").ShowAsync();
                return;
            }

            // Validations
            if (SecondNumber == 0 && SelectedMathFunction == Divide)
            {
                await new MessageDialog("You cannot divide by zero, please pick a different 2nd number.").ShowAsync();
                return;
            }

            // Do actual Math
            double result = SelectedMathFunction((double)FirstNumber, (double)SecondNumber);

            // Show result
            if (IncludeDateCheckBox.IsChecked == true)
            {
                ResultsTextBlock.Text = $"Result: {result:N2}, Date: {CalculationDatePicker.SelectedDate:d}";
            }
            else
            {
                ResultsTextBlock.Text = $"Result: {result:N2}";
            }
        }
    }
}
