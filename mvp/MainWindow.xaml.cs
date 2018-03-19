// General purpose calculator, written by Areeb Beigh
// github.io/areeb-beigh

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace mvp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // Max digits to be shown in the memory label, does not affect the actual number stored in memory
        private const int MAX_MEMORY_LABEL_LENGTH = 6;

        // Default font result box font size
        private const int DEFAULT_FONT_SIZE = 48;

        // Errors that may occur
        private const string OVERFLOW = "Overflow";
        private const string INVALID_INPUT = "Invalid input";
        private const string NOT_A_NUMBER = "NaN";

        private readonly string[] _errors = { OVERFLOW, INVALID_INPUT, NOT_A_NUMBER };


        // True if the result box is to be cleared when a number is entered
        private bool _clearNext;

        // Holds the current operation
        private Operations _currentOperation = Operations.Null;


        // True if a function (sin, tan, ln, log etc) was called on the number during another mathematical operation
        private bool _functionCheck;

        // True if the text in the result box has not been changed after clicking an operator
        private bool _isOldText;

        // True if the text in the result box is the result of some computation
        private bool _isResult;

        // Stores the number in memory accessed via MR
        private double _memory;

        // True if there is an on going math operation
        private bool _operationCheck;

        // Stores the text in the text box after a new math operation is selected
        private string _previousText;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Displays the given text to the result box and sets the value of clearNext to true by default (false if specified).
        /// </summary>
        private void ShowText(string text, bool clear = true)
        {
            try
            {
                if (Math.Abs(double.Parse(text)) < 1)
                {
                    text = "0";
                }
            }
            catch (Exception)
            {
                ShowError(INVALID_INPUT);
                return;
            }

            if (text.Length > 30)
            {
                return;
            }

            if (text.Length > 12)
            {
                ResultBox.FontSize = 25;
            }

            if (text.Length > 24)
            {
                ResultBox.FontSize = 20;
            }

            _clearNext = clear;
            ResultBox.Text = text;
        }

        /// <summary>
        ///     Displays the given text in the result box.
        /// </summary>
        private void ShowError(string text)
        {
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
        private void UpdateEquationBox(string equation, bool append = false)
        {
            // Removes pointless decimals from the numbers in the equation
            equation = Regex.Replace(equation, @"(\d+)\.\s", "$1 ");

            if (equation.Length > 10)
            {
                EquationBox.FontSize = 18;
            }

            if (!append)
            {
                EquationBox.Text = equation;
            }
            else
            {
                EquationBox.Text += equation;
            }
        }

        /// <summary>
        ///     Updates the memory label text with the value in memory variable.
        /// </summary>
        private void UpdateMemoryLabel()
        {
            MemoryLabel.Content = _memory.ToString(CultureInfo.InvariantCulture);
            if (MemoryLabel.Content.ToString().Length > MAX_MEMORY_LABEL_LENGTH)
            {
                MemoryLabel.Content = MemoryLabel.Content.ToString().Substring(0, 5) + "...";
            }
        }

        /// <summary>
        ///     Parses the text in the text box into a double datatype and returns it.
        /// </summary>
        private double GetNumber()
        {
            double number = double.Parse(ResultBox.Text);
            return number;
        }

        /// <summary>
        ///     Resets the result box font size to defaultSize
        /// </summary>
        private void ResetFontSize()
        {
            ResultBox.FontSize = DEFAULT_FONT_SIZE;
        }

        /// <summary>
        ///     Calculates the result by solving the previousText and current text in the result
        ///     box with the operand in currentOperation.
        /// </summary>
        private void CalculateResult()
        {
            if (_currentOperation == Operations.Null)
            {
                return;
            }

            double a = double.Parse(_previousText); // first operand
            double b = double.Parse(ResultBox.Text); // second operand
            double result;

            switch (_currentOperation)
            {
                case Operations.Division:
                    result = a / b;
                    break;
                case Operations.Multiplication:
                    result = a * b;
                    break;
                case Operations.Addition:
                    result = a + b;
                    break;
                case Operations.Subtraction:
                    result = a - b;
                    break;
                case Operations.Power:
                    result = Math.Pow(a, b);
                    break;
                default:
                    return;
            }

            if (_errors.Contains(ResultBox.Text))
            {
                return;
            }

            _operationCheck = false;
            _previousText = null;
            string equation;
            // If a function button was not clicked during a mathematical operation then the equation box will have the text with the
            // format <operand a> <operation> <operand b as a number> else <operand a> <operation> <func>(<operand b>)
            if (!_functionCheck)
            {
                equation = EquationBox.Text + b;
            }
            else
            {
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
        private void NumberClick(object sender, RoutedEventArgs e)
        {
            _isResult = false;
            Button button = (Button)sender;

            if (ResultBox.Text == "0" || _errors.Contains(ResultBox.Text))
            {
                ResultBox.Clear();
            }

            string text;

            if (_clearNext)
            {
                ResetFontSize();
                text = button.Content.ToString();
                _isOldText = false;
            }
            else
            {
                text = ResultBox.Text + button.Content;
            }

            if (!_operationCheck && EquationBox.Text != "")
            {
                UpdateEquationBox("");
            }

            ShowText(text, false);
        }


        /// <summary>
        ///     Deals with function button clicks.
        /// </summary>
        private void Function(object sender, RoutedEventArgs e)
        {
            if (_errors.Contains(ResultBox.Text))
            {
                return;
            }

            Button button = (Button)sender;
            string buttonText = button.Content.ToString();
            double number = GetNumber();
            string equation = "";
            string result = "";

            switch (buttonText)
            {
                // C# doesn't have a Math.factorial()? Who the fuck does that?!
                case "!":
                    if (number < 0 || number.ToString().Contains("."))
                    {
                        ShowError(INVALID_INPUT);
                        return;
                    }

                    if (number > 3248
                    ) // chose this number because the default windows calculator doesn't go beyond this number
                    {
                        ShowError(OVERFLOW);
                        return;
                    }

                    double res = 1;
                    if (Math.Abs(number - 1) < 1 || Math.Abs(number) < 1)
                    {
                        result = res.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        for (int i = 2; i <= number; i++)
                        {
                            res *= i;
                        }
                    }

                    equation = "fact(" + number + ")";
                    result = res.ToString();
                    break;

                case "ln":
                    equation = "ln(" + number + ")";
                    result = Math.Log(number).ToString();
                    break;

                case "log":
                    equation = "log(" + number + ")";
                    result = Math.Log10(number).ToString();
                    break;

                case "√":
                    equation = "√(" + number + ")";
                    result = Math.Sqrt(number).ToString();
                    break;

                case "-n":
                    equation = "negate(" + number + ")";
                    result = decimal.Negate((decimal)number).ToString();
                    break;
            }

            if (_operationCheck)
            {
                equation = EquationBox.Text + equation;
                _functionCheck = true;
            }

            UpdateEquationBox(equation);
            ShowText(result);
        }


        /// <summary>
        ///     Deals with double operand function clicks.
        /// </summary>
        private void DoubleOperandFunction(object sender, RoutedEventArgs e)
        {
            if (_errors.Contains(ResultBox.Text))
            {
                return;
            }

            if (_operationCheck && !_isOldText)
            {
                CalculateResult();
            }

            Button button = (Button)sender;

            _operationCheck = true;
            _previousText = ResultBox.Text;
            string buttonText = button.Content.ToString();
            string equation = _previousText + " " + buttonText + " ";
            switch (buttonText)
            {
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
        private void decimal_button_Click(object sender, RoutedEventArgs e)
        {
            if (!ResultBox.Text.Contains("."))
            {
                string text = ResultBox.Text += ".";
                ShowText(text, false);
            }
        }

        private void pi_button_Click(object sender, RoutedEventArgs e)
        {
            if (!_operationCheck)
            {
                UpdateEquationBox("");
            }

            ShowText(Math.PI.ToString());
            _isResult = true; // Constants cannot be changed
        }

        private void e_button_Click(object sender, RoutedEventArgs e)
        {
            if (!_operationCheck)
            {
                UpdateEquationBox("");
            }

            ShowText(Math.E.ToString());
            _isResult = true; // Constants cannot be changed
        }

        private void Madd_button_Click(object sender, RoutedEventArgs e)
        {
            if (_errors.Contains(ResultBox.Text))
            {
                return;
            }

            _memory += GetNumber();
            UpdateMemoryLabel();
        }

        private void msub_button_Click(object sender, RoutedEventArgs e)
        {
            if (_errors.Contains(ResultBox.Text))
            {
                return;
            }

            _memory -= GetNumber();
            UpdateMemoryLabel();
        }

        private void mc_button_Click(object sender, RoutedEventArgs e)
        {
            _memory = 0;
            UpdateMemoryLabel();
        }

        private void mr_button_Click(object sender, RoutedEventArgs e)
        {
            ShowText(_memory.ToString());
            if (!_operationCheck)
            {
                UpdateEquationBox("");
            }
        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = "0";
            _operationCheck = false;
            _previousText = null;
            UpdateEquationBox("");
            ResetFontSize();
        }

        private void clr_entry_button_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = "0";
            ResetFontSize();
        }

        private void equals_button_Click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
        }

        private void about_button_Click(object sender, RoutedEventArgs e)
        {
            //AboutBox aboutForm = new AboutBox();
            //aboutForm.ShowDialog();
        }

        // Copy
        private void copy_button_Click(object sender, RoutedEventArgs e)
        {
            if (_errors.Contains(ResultBox.Text))
            {
                return;
            }

            Clipboard.SetData(DataFormats.UnicodeText, ResultBox.Text);
        }

        // Paste
        private void paste_button_Click(object sender, RoutedEventArgs e)
        {
            object clipboardData = Clipboard.GetData(DataFormats.UnicodeText);
            if (clipboardData != null)
            {
                string data = clipboardData.ToString();
                ShowText(data);
            }
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            if (_isResult)
            {
                return;
            }

            string text = ResultBox.Text.Length == 1 ? "0" : ResultBox.Text.Substring(0, ResultBox.Text.Length - 1);

            ShowText(text, false);
        }


        // Math operations that take two operands
        private enum Operations
        {
            Addition,
            Subtraction,
            Division,
            Multiplication,
            Power,
            Null // Represents no operation (used to reset the status)
        }

        // private void keyboardInput(object sender, System.Windows.Input.KeyEventArgs e)
        // {
        //     string keyString = e.Key.ToString();
        //     //MessageBox.Show(keyString);
        //     Dictionary<string, Button> buttonShortcuts = new Dictionary<string, Button>()
        //     {
        //         { "D0", zero_button },
        //         { "D1", one_button },
        //         { "D2", two_button },
        //         { "D3", three_button },
        //         { "D4", four_button },
        //         { "D5", five_button },
        //         { "D6", six_button },
        //         { "D7", seven_button },
        //         { "D8", eight_button },
        //         { "D9", nine_button },
        //         { "P", pi_button },
        //         { "E", e_button },
        //         { "S", sin_button },
        //         { "C", cos_button },
        //         { "T", tan_button },
        //         { "Return", equals_button },
        //         { "Back", back_button }
        //     };
        //     string[] numberButtons =
        //     {
        //         "D0",
        //         "D1",
        //         "D2",
        //         "D3",
        //         "D4",
        //         "D5",
        //         "D6",
        //         "D7",
        //         "D8",
        //         "D9",
        //     };

        //     if (numberButtons.Contains(keyString))
        //     numberClick(buttonShortcuts[keyString], null);
        //}
    }

    public class AboutBox { }
}