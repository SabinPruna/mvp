using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mvp.Properties;

namespace mvp {
    public partial class MainWindow {
        private const int MAX_MEMORY_LABEL_LENGTH = 6;
        private const int DEFAULT_FONT_SIZE = 48;

        private const string OVERFLOW = "Overflow";
        private const string INVALID_INPUT = "Invalid input";
        private const string NOT_A_NUMBER = "NaN";
        private readonly string[] _errors = {OVERFLOW, INVALID_INPUT, NOT_A_NUMBER};

        private bool _clearNext;
        private Operations _currentOperation = Operations.Null;
        private bool _functionCheck;
        private bool _isOldText;
        private bool _isResult;
        private double _memory;
        private bool _operationCheck;
        private string _previousText;

        public MainWindow() {
            InitializeComponent();
            MinimizeToTray.Enable(this);

            DigitGrouping.IsChecked = Settings.Default.DG;

            Application.Current.MainWindow.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.DG = DigitGrouping.IsChecked;
            Settings.Default.Save();
        }

        /// <summary>
        ///     Displays the given text to the result box and sets the value of clearNext to true by default (false if specified).
        /// </summary>
        private void ShowText(string text, bool clear = true) {
            try {
                if (Math.Abs(double.Parse(text)) < 1) {
                    text = "0";
                }
            }
            catch (Exception) {
                ShowError(INVALID_INPUT);
                return;
            }

            if (text.Length > 30) {
                return;
            }

            if (text.Length > 12) {
                ResultBox.FontSize = 25;
            }

            if (text.Length > 24) {
                ResultBox.FontSize = 20;
            }

            _clearNext = clear;
            ResultBox.Text = text;
        }

        /// <summary>
        ///     Displays the given text in the result box.
        /// </summary>
        private void ShowError(string text) {
            ResultBox.Text = text;
            _previousText = null;
            _operationCheck = false;
            _clearNext = true;
            UpdateEquationBox("");
            _currentOperation = Operations.Null;
            ResetFontSize();
        }

        /// <summary>
        ///     Updates the equation box with the given equation string.
        ///     If append is true then the given text is appended to the existing text in the equation box.
        /// </summary>
        private void UpdateEquationBox(string equation, bool append = false) {
            equation = Regex.Replace(equation, @"(\d+)\.\s", "$1 ");

            if (equation.Length > 10) {
                EquationBox.FontSize = 18;
            }

            if (!append) {
                EquationBox.Text = equation;
            }
            else {
                EquationBox.Text += equation;
            }
        }

        /// <summary>
        ///     Updates the memory label text with the value in memory variable.
        /// </summary>
        private void UpdateMemoryLabel() {
            MemoryLabel.Content = _memory.ToString(CultureInfo.InvariantCulture);
            if (MemoryLabel.Content.ToString().Length > MAX_MEMORY_LABEL_LENGTH) {
                MemoryLabel.Content = MemoryLabel.Content.ToString().Substring(0, 5) + "...";
            }
        }

        /// <summary>
        ///     Parses the text in the text box into a double datatype and returns it.
        /// </summary>
        private double GetNumber() {
            double number = double.Parse(ResultBox.Text);
            return number;
        }

        /// <summary>
        ///     Resets the result box font size to defaultSize
        /// </summary>
        private void ResetFontSize() {
            ResultBox.FontSize = DEFAULT_FONT_SIZE;
        }

        /// <summary>
        ///     Calculates the result by solving the previousText and current text in the result
        ///     box with the operand in currentOperation.
        /// </summary>
        private void CalculateResult() {
            if (_currentOperation == Operations.Null) {
                return;
            }

            double firstOperand = double.Parse(_previousText);
            double secondOperand = double.Parse(ResultBox.Text);
            double result;

            switch (_currentOperation) {
                case Operations.Division:
                    result = firstOperand / secondOperand;
                    break;
                case Operations.Multiplication:
                    result = firstOperand * secondOperand;
                    break;
                case Operations.Addition:
                    result = firstOperand + secondOperand;
                    break;
                case Operations.Subtraction:
                    result = firstOperand - secondOperand;
                    break;
                case Operations.Power:
                    result = Math.Pow(firstOperand, secondOperand);
                    break;
                default:
                    return;
            }

            if (_errors.Contains(ResultBox.Text)) {
                return;
            }

            _operationCheck = false;
            _previousText = null;
            string equation;
            if (!_functionCheck) {
                equation = EquationBox.Text + secondOperand;
            }
            else {
                equation = EquationBox.Text;
                _functionCheck = false;
            }

            UpdateEquationBox(equation);
            ShowText(result.ToString(CultureInfo.InvariantCulture));
            _currentOperation = Operations.Null;
            _isResult = true;
        }

        /// <summary>
        ///     Appends the digit clicked to the text in the text box.
        ///     If an ongoing operation has been selected then the text box value is first assigned to previousText variable and
        ///     then new text
        ///     is appended to the text box after truncating the previous text.
        /// </summary>
        private void NumberClick(object sender, RoutedEventArgs e) {
            _isResult = false;
            Button button = (Button) sender;

            if (ResultBox.Text == "0" || _errors.Contains(ResultBox.Text)) {
                ResultBox.Clear();
            }

            string text;

            if (_clearNext) {
                ResetFontSize();
                text = button.Content.ToString();
                _isOldText = false;
            }
            else {
                text = ResultBox.Text + button.Content;
            }

            if (!_operationCheck && EquationBox.Text != "") {
                UpdateEquationBox("");
            }

            ShowText(text, false);
        }

        /// <summary>
        ///     Deals with function button clicks.
        /// </summary>
        private void Function(object sender, RoutedEventArgs e) {
            if (_errors.Contains(ResultBox.Text)) {
                return;
            }

            Button button = (Button) sender;
            string buttonText = button.Content.ToString();
            double number = GetNumber();
            string equation = "";
            string result = "";

            switch (buttonText) {
                case "!":
                    if (number < 0 || number.ToString().Contains(".")) {
                        ShowError(INVALID_INPUT);
                        return;
                    }

                    double res = 1;
                    if (Math.Abs(number - 1) < 1 || Math.Abs(number) < 1) {
                        result = res.ToString(CultureInfo.InvariantCulture);
                    }
                    else {
                        for (int i = 2; i <= number; i++) {
                            res *= i;
                        }
                    }

                    equation = "fact(" + number + ")";
                    result = res.ToString();
                    break;

                case "√":
                    equation = "√(" + number + ")";
                    result = Math.Sqrt(number).ToString();
                    break;

                case "-n":
                    equation = "negate(" + number + ")";
                    result = decimal.Negate((decimal) number).ToString();
                    break;
            }

            if (_operationCheck) {
                equation = EquationBox.Text + equation;
                _functionCheck = true;
            }

            UpdateEquationBox(equation);
            ShowText(result);
        }

        /// <summary>
        ///     Deals with double operand function clicks.
        /// </summary>
        private void DoubleOperandFunction(object sender, RoutedEventArgs e) {
            if (_errors.Contains(ResultBox.Text)) {
                return;
            }

            if (_operationCheck && !_isOldText) {
                CalculateResult();
            }

            Button button = (Button) sender;

            _operationCheck = true;
            _previousText = ResultBox.Text;
            string buttonText = button.Content.ToString();
            string equation = _previousText + " " + buttonText + " ";
            switch (buttonText) {
                case "/":
                    _currentOperation = Operations.Division;
                    break;
                case "x":
                    _currentOperation = Operations.Multiplication;
                    break;
                case "-":
                    _currentOperation = Operations.Subtraction;
                    break;
                case "+":
                    _currentOperation = Operations.Addition;
                    break;
                case "^":
                    _currentOperation = Operations.Power;
                    break;
            }

            UpdateEquationBox(equation);
            ResetFontSize();
            ShowText(ResultBox.Text);
            _isOldText = true;
        }

        /// <summary>
        ///     Appends a decimal point to the number in the result box on click,
        ///     if the number already has a decimal point then no action is taken
        /// </summary>
        public void Decimal_button_Click(object sender, RoutedEventArgs e) {
            if (!ResultBox.Text.Contains(".")) {
                string text = ResultBox.Text += ".";
                ShowText(text, false);
            }
        }

        public void Pi_button_Click(object sender, RoutedEventArgs e) {
            if (!_operationCheck) {
                UpdateEquationBox("");
            }

            ShowText(Math.PI.ToString());
            _isResult = true;
        }

        private void E_button_Click(object sender, RoutedEventArgs e) {
            if (!_operationCheck) {
                UpdateEquationBox("");
            }

            ShowText(Math.E.ToString());
            _isResult = true; // Constants cannot be changed
        }

        private void Madd_button_Click(object sender, RoutedEventArgs e) {
            if (_errors.Contains(ResultBox.Text)) {
                return;
            }

            _memory += GetNumber();
            UpdateMemoryLabel();
        }

        private void Msub_button_Click(object sender, RoutedEventArgs e) {
            if (_errors.Contains(ResultBox.Text)) {
                return;
            }

            _memory -= GetNumber();
            UpdateMemoryLabel();
        }

        private void Mr_button_Click(object sender, RoutedEventArgs e) {
            ShowText(_memory.ToString());
            if (!_operationCheck) {
                UpdateEquationBox("");
            }
        }

        private void Clear_button_Click(object sender, RoutedEventArgs e) {
            ResultBox.Text = "0";
            _operationCheck = false;
            _previousText = null;
            UpdateEquationBox("");
            ResetFontSize();
        }

        private void Clr_entry_button_Click(object sender, RoutedEventArgs e) {
            ResultBox.Text = "0";
            ResetFontSize();
        }
        private void Equals_button_Click(object sender, RoutedEventArgs e) {
            CalculateResult();
        }

        private void About_button_Click(object sender, RoutedEventArgs e) {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void Copy_button_Click(object sender, RoutedEventArgs e) {
            if (_errors.Contains(ResultBox.Text)) {
                return;
            }

            Clipboard.SetData(DataFormats.UnicodeText, ResultBox.Text);
        }

        private void Paste_button_Click(object sender, RoutedEventArgs e) {
            object clipboardData = Clipboard.GetData(DataFormats.UnicodeText);
            if (clipboardData != null) {
                string data = clipboardData.ToString();
                ShowText(data);
            }
        }

        private void Back_button_Click(object sender, RoutedEventArgs e) {
            if (_isResult) {
                return;
            }

            string text = ResultBox.Text.Length == 1 ? "0" : ResultBox.Text.Substring(0, ResultBox.Text.Length - 1);

            ShowText(text, false);
        }

        private void Cut_button_Click(object sender, RoutedEventArgs e) {
            if (_errors.Contains(ResultBox.Text)) {
                return;
            }

            Clipboard.SetData(DataFormats.UnicodeText, ResultBox.Text);
            ResultBox.Text = "0";
            ResetFontSize();
        }

        private void DigitGrouping_Checked_button_Click(object sender, RoutedEventArgs e) {
            CultureInfo culture = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentUICulture.Name);
            double number = double.Parse(GetNumber().ToString(), culture);


            ShowText($"{number:n0}");
        }

        private void KeyboardFunctionality(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                Clr_entry_button_Click(sender, e);
            }

            if (e.Key >= Key.A && e.Key <= Key.Z || e.Key >= Key.D0 && e.Key <= Key.D9 ||
                e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Enter) {
                if (e.Key.ToString().StartsWith("D") && e.Key.ToString().Length == 2) {
                    AddNewCharacter(e.Key.ToString()[1].ToString());
                }
                else {
                    if (e.Key.ToString().StartsWith("NumPad") && e.Key.ToString().Length == 7) {
                        AddNewCharacter(e.Key.ToString()[6].ToString());
                    }
                    else {
                        if (e.Key == Key.Enter) {
                            Equals_button_Click(sender, e);
                        }
                    }
                }
            }
        }

        private void AddNewCharacter(string c) {
            if (ResultBox.Text == "0") {
                ResultBox.Text = c;
            }
            else {
                ResultBox.Text = ResultBox.Text + c;
            }
        }

        private void Mc_button_Click(object sender, RoutedEventArgs e) {
            _memory = 0;
            UpdateMemoryLabel();
        }
    }
}