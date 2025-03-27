using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class LabeledTextFieldControl : UserControl
{
    public LabeledTextFieldControl()
    {
        InitializeComponent();
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string TextValue
    {
        get => (string)GetValue(TextValueProperty);
        set => SetValue(TextValueProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public bool ErrorMessageVisible => !string.IsNullOrWhiteSpace(ErrorMessage);

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(LabeledTextFieldControl),
            new PropertyMetadata(null, OnChildViewModelChanged));

    public static readonly DependencyProperty TextValueProperty =
        DependencyProperty.Register(nameof(TextValue), typeof(string), typeof(LabeledTextFieldControl),
            new PropertyMetadata(null, OnChildViewModelChanged));

    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(LabeledTextFieldControl),
            new PropertyMetadata(null, OnChildViewModelChanged));

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(LabeledTextFieldControl),
            new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (LabeledTextFieldControl)d;
        control.DataContext = e.NewValue;
    }
}