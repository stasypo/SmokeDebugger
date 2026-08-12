using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
// WpfApp1 - the Desktop technology ("basics only") and the attach/detach host.
// The window stays idle until a button is clicked, which makes it the easiest
// process to attach to and detach from.
public partial class MainWindow : Window
{
    public MainWindow()
    {
        // SCENARIO "jit-debugger-wpf" - README -> [Before release only] JIT-debugger.
        // ACTION: uncomment one of the lines below and start WpfApp1 WITHOUT the debugger.
        // EXPECTED (Launch): the configured JIT debugger starts and attaches here.
        // EXPECTED (Break): the attached debugger suspends on this line and shows this
        //   source location and the WPF startup stack.
        // Debugger.Launch();
        // Debugger.Break();
        InitializeComponent();
    }

    // SCENARIO "attach-to-local-process" - README -> More -> Attach/Detach 1.
    // SETUP: start WpfApp1 WITHOUT the debugger (Run, not Debug), then use
    //   Run | Attach to Process and pick WpfApp1.
    // BREAKPOINT: on the first 'bool firstOk = ...' line below, set AFTER attaching.
    // ACTION: type two numbers in the window and press Add.
    // EXPECTED: the debugger suspends here exactly as in a normal debug session -
    //   Variables, Frames, stepping and evaluation all work. Detach afterwards and
    //   verify the app keeps running and no other process is affected.
    // WATCH: 'first' and 'second' are out parameters - verify they show their assigned
    //   values after the TryParse calls, not stale garbage
    //   (README -> Basics -> Threads & Variables 3).
    private bool TryGetNumbers(out double first, out double second)
    {
        bool firstOk = double.TryParse(
            FirstNumberTextBox.Text,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out first);

        bool secondOk = double.TryParse(
            SecondNumberTextBox.Text,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out second);

        if (!firstOk || !secondOk)
        {
            MessageBox.Show(
                "Please enter valid numbers using digits and optional decimal points.",
                "Invalid Input",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return false;
        }

        return true;
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetNumbers(out double first, out double second))
            return;

        ResultTextBlock.Text = (first + second).ToString(CultureInfo.InvariantCulture);
    }

    private void Subtract_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetNumbers(out double first, out double second))
            return;

        ResultTextBlock.Text = (first - second).ToString(CultureInfo.InvariantCulture);
    }

    private void Multiply_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetNumbers(out double first, out double second))
            return;

        ResultTextBlock.Text = (first * second).ToString(CultureInfo.InvariantCulture);
    }

    // SCENARIO "set-variable-value-desktop" - README -> Basics -> Threads & Variables 4,
    //   README -> More -> Evaluation 7 (predictive boolean).
    // BREAKPOINT: on 'if (second == 0)' below.
    // ACTION: enter 10 and 0 in the window and press Div. While suspended, the predictive
    //   debugger must show that the condition is true. Now set 'second' to 4 in Variables.
    // EXPECTED: the prediction flips to false, the "Cannot divide by zero" dialog is not
    //   shown, and the window displays 2.5 - the running app used the edited value.
    private void Divide_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetNumbers(out double first, out double second))
            return;

        if (second == 0)
        {
            MessageBox.Show(
                "Cannot divide by zero.",
                "Math Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        ResultTextBlock.Text = (first / second).ToString(CultureInfo.InvariantCulture);
    }
}