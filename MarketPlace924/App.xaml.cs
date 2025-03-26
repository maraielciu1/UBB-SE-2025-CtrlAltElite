using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;
namespace MarketPlace924
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            UnhandledException += (_, e) =>
            {
                Debug.WriteLine($"Unhandled UI Exception: {e.Exception.StackTrace}");
                e.Handled = true; // Prevents app from crashing
            };
        }
        public static Window? m_window { get; private set; }
    }
}
