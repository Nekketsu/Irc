using Irc.Client.Scripting.Api;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Irc.Client.Scripting;

public class ScriptEngine
{
    private readonly ScriptOptions scriptOptions;

    public ScriptEngine()
    {
        // Get all currently loaded assemblies that are safe to reference
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToList();

        scriptOptions = ScriptOptions.Default
            .AddReferences(
                typeof(object).Assembly, // System.Private.CoreLib
                typeof(IrcClient).Assembly,
                typeof(ScriptHost).Assembly,
                typeof(Irc.Messages.Message).Assembly,
                typeof(System.Linq.Enumerable).Assembly, // System.Linq
                typeof(System.Collections.Generic.List<>).Assembly, // System.Collections
                typeof(System.Threading.Tasks.Task).Assembly, // System.Threading.Tasks
                typeof(System.Threading.Tasks.ValueTask).Assembly, // System.Threading.Tasks.Extensions
                typeof(Console).Assembly, // System.Console
                typeof(System.Runtime.CompilerServices.AsyncTaskMethodBuilder).Assembly // Required for async
            )
            .WithReferences(assemblies) // Add all loaded assemblies
            .AddImports(
                "System",
                "System.Linq",
                "System.Collections.Generic",
                "System.Threading.Tasks",
                "Irc.Client",
                "Irc.Client.Events",
                "Irc.Client.Scripting",
                "Irc.Client.Scripting.Events",
                "Irc.Messages",
                "Irc.Messages.Messages"
            );
    }

    /// <summary>
    /// Executes a script, which subscribes to events directly.
    /// </summary>
    public async Task<ScriptResult> ExecuteScriptAsync(
        string code,
        ScriptHost host,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Set the static Host.Current for this thread
            Host.Current = host;

            var globals = new ScriptGlobals { Host = host };

            var script = CSharpScript.Create(code, scriptOptions, typeof(ScriptGlobals));
            var compilation = script.GetCompilation();

            var diagnostics = compilation.GetDiagnostics();
            var errors = diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToList();

            if (errors.Any())
            {
                var errorMessages = errors.Select(e => e.ToString());
                return ScriptResult.Failure(string.Join(Environment.NewLine, errorMessages));
            }

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            // Execute the script - it will subscribe to events
            await script.RunAsync(globals, linkedCts.Token);

            return ScriptResult.Success();
        }
        catch (OperationCanceledException)
        {
            var errorMessage = "Script execution timeout (30 seconds)";
            System.Diagnostics.Debug.WriteLine($"[SCRIPT ERROR] {errorMessage}");
            return ScriptResult.Failure(errorMessage);
        }
        catch (CompilationErrorException ex)
        {
            var errorMessage = $"Compilation error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[SCRIPT ERROR] {errorMessage}");
            System.Diagnostics.Debug.WriteLine($"  Stack Trace: {ex.StackTrace}");
            return ScriptResult.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Runtime error: {ex.GetType().Name}: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[SCRIPT ERROR] {errorMessage}");
            System.Diagnostics.Debug.WriteLine($"  Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"  Inner Exception: {ex.InnerException.Message}");
            }
            return ScriptResult.Failure(errorMessage);
        }
        finally
        {
            // Clear the static reference
            Host.Current = null;
        }
    }
}

public class ScriptGlobals
{
    public ScriptHost Host { get; set; } = null!;
}

public class ScriptResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ScriptResult() { }

    public static ScriptResult Success()
    {
        return new ScriptResult { IsSuccess = true };
    }

    public static ScriptResult Failure(string errorMessage)
    {
        return new ScriptResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
