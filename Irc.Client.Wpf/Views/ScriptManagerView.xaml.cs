using Irc.Client.Wpf.ViewModels;
using RoslynPad.Roslyn;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Irc.Client.Wpf.Views
{
    public partial class ScriptManagerView : UserControl
    {
        private RoslynHost roslynHost;
        private Microsoft.CodeAnalysis.DocumentId documentId;

        public ScriptManagerView()
        {
            InitializeComponent();

            // Initialize RoslynHost for IntelliSense
            InitializeRoslynHost();

            Loaded += ScriptManagerView_Loaded;
            Unloaded += ScriptManagerView_Unloaded;
        }

        private void InitializeRoslynHost()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing RoslynHost with custom configuration...");

                // Get IRC assemblies for IntelliSense
                var assemblies = GetScriptAssemblies();

                // Create RoslynHost with configuration
                roslynHost = new RoslynHost(
                    additionalAssemblies: new[]
                    {
                        Assembly.Load("RoslynPad.Roslyn.Windows"),
                        Assembly.Load("RoslynPad.Editor.Windows")
                    },
                    references: RoslynHostReferences.NamespaceDefault.With(
                        assemblyReferences: assemblies,
                        imports: GetDefaultImports()
                    )
                );

                System.Diagnostics.Debug.WriteLine("RoslynHost created successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating RoslynHost: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException is not null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }

                // Try fallback
                try
                {
                    System.Diagnostics.Debug.WriteLine("Trying basic RoslynHost...");
                    roslynHost = new RoslynHost(
                        additionalAssemblies: new[]
                        {
                            Assembly.Load("RoslynPad.Roslyn.Windows"),
                            Assembly.Load("RoslynPad.Editor.Windows")
                        },
                        references: RoslynHostReferences.NamespaceDefault
                    );
                    System.Diagnostics.Debug.WriteLine("Basic RoslynHost created");
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Fallback failed: {fallbackEx.Message}");
                    MessageBox.Show($"Warning: Code editor IntelliSense may not work correctly.\n\nError: {ex.Message}",
                        "Editor Initialization", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private Assembly[] GetScriptAssemblies()
        {
            var assemblies = new List<Assembly>();

            try
            {
                // Core .NET assemblies
                assemblies.Add(typeof(object).Assembly);              // System.Private.CoreLib
                assemblies.Add(typeof(Enumerable).Assembly);          // System.Linq
                assemblies.Add(typeof(List<>).Assembly);              // System.Collections
                assemblies.Add(typeof(System.Text.RegularExpressions.Regex).Assembly); // System.Text.RegularExpressions

                // Explicitly load System.Runtime for Func<,> and other delegates
                try
                {
                    assemblies.Add(Assembly.Load("System.Runtime"));
                    System.Diagnostics.Debug.WriteLine("Added System.Runtime");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Could not load System.Runtime: {ex.Message}");
                }

                // IRC-specific assemblies
                try
                {
                    var ircAssembly = Assembly.Load("Irc");
                    assemblies.Add(ircAssembly);
                    System.Diagnostics.Debug.WriteLine($"Added Irc.dll");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Could not load Irc assembly: {ex.Message}");
                }

                try
                {
                    var ircClientAssembly = Assembly.Load("Irc.Client");
                    assemblies.Add(ircClientAssembly);
                    System.Diagnostics.Debug.WriteLine($"Added Irc.Client.dll");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Could not load Irc.Client assembly: {ex.Message}");
                }

                try
                {
                    var ircScriptingAssembly = Assembly.Load("Irc.Client.Scripting");
                    assemblies.Add(ircScriptingAssembly);
                    System.Diagnostics.Debug.WriteLine($"Added Irc.Client.Scripting.dll");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Could not load Irc.Client.Scripting assembly: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error building assembly list: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine($"Total assemblies: {assemblies.Count}");
            return assemblies.ToArray();
        }

        private string[] GetDefaultImports()
        {
            return new[]
            {
                "System",
                "System.Linq",
                "System.Collections.Generic",
                "System.Threading.Tasks",
                "Irc",
                "Irc.Client",
                "Irc.Client.Scripting.Api",
                "Irc.Client.Scripting.Api.Events"
            };
        }

        private void ScriptManagerView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScriptManagerViewModel viewModel)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            // Subscribe to text changes
            CodeEditor.TextChanged += CodeEditor_TextChanged;

            // Initialize RoslynPad editor with IntelliSense first
            InitializeRoslynPadEditor();
        }

        private void InitializeRoslynPadEditor()
        {
            try
            {
                if (roslynHost is not null && CodeEditor is not null)
                {
                    System.Diagnostics.Debug.WriteLine("Initializing RoslynPad editor async...");

                    // Use async initialization like the sample
                    _ = InitializeEditorAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: Cannot initialize editor - RoslynHost: {roslynHost is not null}, CodeEditor: {CodeEditor is not null}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing RoslynPad: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException is not null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private async System.Threading.Tasks.Task InitializeEditorAsync()
        {
            try
            {
                var workingDirectory = Directory.GetCurrentDirectory();

                // Initialize editor without preamble
                documentId = await CodeEditor.InitializeAsync(
                    roslynHost,
                    new RoslynPad.Editor.ClassificationHighlightColors(),
                    workingDirectory,
                    string.Empty,
                    Microsoft.CodeAnalysis.SourceCodeKind.Script
                ).ConfigureAwait(true);

                System.Diagnostics.Debug.WriteLine($"RoslynPad editor initialized with DocumentId: {documentId}");

                // Load user's script code if exists
                if (DataContext is ScriptManagerViewModel viewModel && !string.IsNullOrEmpty(viewModel.NewScriptCode))
                {
                    CodeEditor.Text = viewModel.NewScriptCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeEditorAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException is not null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private void ScriptManagerView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScriptManagerViewModel viewModel)
            {
                viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            CodeEditor.TextChanged -= CodeEditor_TextChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScriptManagerViewModel.NewScriptCode))
            {
                if (DataContext is ScriptManagerViewModel viewModel)
                {
                    if (CodeEditor.Text != viewModel.NewScriptCode)
                    {
                        CodeEditor.Text = viewModel.NewScriptCode ?? string.Empty;
                    }
                }
            }
        }

        private void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            if (DataContext is ScriptManagerViewModel viewModel)
            {
                viewModel.NewScriptCode = CodeEditor.Text;
            }
        }
    }
}
