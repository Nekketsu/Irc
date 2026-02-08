# IRC Client Scripting System

Sistema de scripting dinámico para **todos los clientes IRC** de la solución utilizando Roslyn. Permite automatizar tareas mediante scripts en C# que responden a eventos del cliente.

## 🎯 Para Quién es Este Sistema

Este sistema de scripting está diseñado para ser usado por **cualquier cliente IRC** en la solución:
- ✅ **Irc.Client.Console** - Cliente de consola
- ✅ **Irc.Client.Wpf** - Cliente de escritorio Windows (WPF)
- ✅ **Irc.Client.Blazor** - Cliente web (Blazor WebAssembly)
- ✅ **Irc.Client.Maui** / **Irc.Client.Maui.Blazor** - Apps móviles/multiplataforma
- ✅ **Irc.Bot.Console** / **Irc.Bot.Scripting** - Bots automatizados

**No es solo para bots** - cualquier aplicación que use `IrcClient` puede aprovechar el scripting.

## Características

- ✅ Ejecución dinámica de scripts C# con Roslyn
- ✅ Respuesta a eventos: mensajes privados, mensajes en canales, joins, parts
- ✅ API simple para interactuar con el cliente IRC
- ✅ Carga de scripts desde archivos (.csx)
- ✅ Activación/desactivación de scripts en tiempo de ejecución
- ✅ Timeout automático de 30 segundos para prevenir scripts colgados
- ✅ Manejo de errores de compilación y ejecución

## Eventos Disponibles

### ScriptEventType

- **PrivateMessageReceived**: Cuando el cliente recibe un mensaje privado
- **ChannelMessageReceived**: Cuando alguien habla en un canal
- **UserJoinedChannel**: Cuando un usuario entra a un canal
- **UserLeftChannel**: Cuando un usuario sale de un canal

## API de Scripting (ScriptHost)

### Propiedades

```csharp
IrcClient Client { get; }  // Acceso al cliente IRC completo
```

### Métodos

```csharp
// Enviar mensaje privado a un usuario
await Host.SendPrivateMessageAsync(string target, string message);

// Enviar mensaje a un canal
await Host.SendChannelMessageAsync(string channel, string message);

// Entrar a un canal
await Host.JoinChannelAsync(string channel);

// Salir de un canal
await Host.PartChannelAsync(string channel, string? reason = null);

// Escribir en el log
Host.Log(string message);
```

## Contexto del Script (ScriptContext)

Cada script tiene acceso a un objeto `Context` con información del evento:

```csharp
public class ScriptContext
{
    public ScriptEventType EventType { get; set; }
    public User? Sender { get; set; }        // Usuario que envió el mensaje
    public Channel? Channel { get; set; }    // Canal del evento
    public string? Message { get; set; }     // Contenido del mensaje
    public User? User { get; set; }          // Usuario del evento (join/part)
}
```

## Uso Programático

### Crear ScriptManager

```csharp
var client = new IrcClient("MiBot", "irc.server.com");
var scriptManager = new ScriptManager(client);
```

### Cargar Script desde Archivo

```csharp
// Carga script que responde a cualquier evento
await scriptManager.LoadScriptAsync("AutoResponder.csx");

// Carga script que solo responde a mensajes privados
await scriptManager.LoadScriptAsync(
    "AutoResponder.csx", 
    ScriptEventType.PrivateMessageReceived
);
```

### Crear Script en Código

```csharp
var script = await scriptManager.CreateScriptAsync(@"
    if (EventType == ScriptEventType.ChannelMessageReceived)
    {
        if (Context.Message.Contains('!ping'))
        {
            await Host.SendChannelMessageAsync(Context.Channel.Name, 'Pong!');
        }
    }
", "PingResponder");
```

### Vincular ScriptManager a Canales

Para que los scripts reciban eventos de canales, debes vincular el ScriptManager a cada canal:

```csharp
// Cuando el cliente se une a un canal
client.Channels.CollectionChanged += (sender, e) =>
{
    if (e.Action == NotifyCollectionChangedAction.Add)
    {
        foreach (Channel channel in e.NewItems)
        {
            scriptManager.AttachToChannel(channel);
        }
    }
};
```

### Administrar Scripts

```csharp
// Desactivar un script
scriptManager.DisableScript("AutoResponder");

// Reactivar un script
scriptManager.EnableScript("AutoResponder");

// Eliminar un script
scriptManager.RemoveScript("AutoResponder");

// Ver todos los scripts cargados
foreach (var script in scriptManager.Scripts)
{
    Console.WriteLine($"{script.Name} - Enabled: {script.IsEnabled}");
}
```

## Ejemplos de Scripts

### Auto-responder a Mensajes Privados

```csharp
// AutoResponder.csx
if (EventType == ScriptEventType.PrivateMessageReceived)
{
    var message = Context.Message?.ToLower() ?? "";
    var sender = Context.Sender?.Nickname.ToString() ?? "Unknown";
    
    if (message.Contains("ayuda"))
    {
        await Host.SendPrivateMessageAsync(sender, 
            "Comandos disponibles: !help, !info, !ping");
    }
    else if (message.Contains("!ping"))
    {
        await Host.SendPrivateMessageAsync(sender, "Pong!");
    }
}
```

### Bot de Bienvenida en Canal

```csharp
// Welcome.csx
if (EventType == ScriptEventType.UserJoinedChannel)
{
    var user = Context.User?.Nickname.ToString() ?? "Unknown";
    var channel = Context.Channel?.Name ?? "#unknown";
    var localUser = Host.Client.LocalUser.Nickname.ToString();
    
    if (user != localUser)
    {
        await Host.SendChannelMessageAsync(channel, 
            $"¡Bienvenido {user} a {channel}!");
    }
}
```

### Logger de Mensajes de Canal

```csharp
// ChannelLogger.csx
if (EventType == ScriptEventType.ChannelMessageReceived)
{
    var sender = Context.Sender?.Nickname.ToString() ?? "Unknown";
    var channel = Context.Channel?.Name ?? "#unknown";
    var message = Context.Message ?? "";
    
    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    Host.Log($"[{timestamp}] {channel} <{sender}> {message}");
}
```

### Comandos en Canal

```csharp
// PingPong.csx
if (EventType == ScriptEventType.ChannelMessageReceived)
{
    var message = Context.Message?.ToLower() ?? "";
    var channel = Context.Channel?.Name ?? "#unknown";
    
    if (message.Trim() == "!ping")
    {
        await Host.SendChannelMessageAsync(channel, "Pong!");
    }
    else if (message.StartsWith("!echo "))
    {
        var echoMessage = Context.Message?.Substring(6) ?? "";
        await Host.SendChannelMessageAsync(channel, $"Echo: {echoMessage}");
    }
}
```

## Variables Globales Disponibles en Scripts

Todos los scripts tienen acceso a estas variables globales:

```csharp
ScriptEventType EventType;  // Tipo de evento actual
ScriptContext Context;      // Contexto del evento
ScriptHost Host;           // API para interactuar con IRC
```

## Imports Automáticos

Los siguientes namespaces están importados automáticamente:

- `System`
- `System.Linq`
- `System.Collections.Generic`
- `Irc.Client`
- `Irc.Client.Scripting`
- `Irc.Messages.Messages`

## Consideraciones de Seguridad

- Los scripts se ejecutan en el contexto del cliente (confianza total)
- Timeout automático de 30 segundos para evitar scripts colgados
- Los errores de compilación se capturan y reportan
- Los errores de ejecución no detienen otros scripts
- Los scripts se ejecutan de forma asíncrona en tareas separadas

## Integración por Tipo de Cliente

### Cliente de Consola / Bot

```csharp
using Irc.Client;
using Irc.Client.Scripting;

var client = new IrcClient("MiCliente", "irc.example.com");
var scriptManager = new ScriptManager(client);

// Cargar scripts desde archivos
await scriptManager.LoadScriptAsync("Examples/AutoResponder.csx");
await scriptManager.LoadScriptAsync("Examples/Welcome.csx");

// Vincular a canales cuando se una
client.MessageReceived += (sender, msg) =>
{
    if (msg is JoinMessage join && join.From?.StartsWith(client.LocalUser.Nickname) == true)
    {
        scriptManager.AttachToChannel(client.Channels[join.ChannelName]);
    }
};

await client.RunAsync(cancellationToken);
```

### Cliente WPF (MVVM)

```csharp
// En App.xaml.cs o ViewModel
public class MainViewModel
{
    private readonly IrcClient client;
    private readonly ScriptManager scriptManager;

    public MainViewModel(IrcClient client)
    {
        this.client = client;
        this.scriptManager = new ScriptManager(client);
        
        // Cargar scripts predeterminados
        LoadDefaultScripts();
    }

    private async void LoadDefaultScripts()
    {
        try
        {
            await scriptManager.LoadScriptAsync("Scripts/AutoResponder.csx");
            // Más scripts...
        }
        catch (Exception ex)
        {
            // Manejar error
        }
    }

    // Exponer scriptManager para la UI
    public ScriptManager Scripts => scriptManager;
}
```

### Cliente Blazor

```csharp
// En un servicio o componente
@inject IrcClient Client

@code {
    private ScriptManager scriptManager;

    protected override void OnInitialized()
    {
        scriptManager = new ScriptManager(Client);
        
        // Cargar scripts
        _ = LoadScriptsAsync();
    }

    private async Task LoadScriptsAsync()
    {
        await scriptManager.LoadScriptAsync("Scripts/AutoResponder.csx");
    }

    // Métodos para administrar scripts desde la UI
    public async Task EnableScript(string name) 
        => scriptManager.EnableScript(name);
        
    public async Task DisableScript(string name) 
        => scriptManager.DisableScript(name);
}
```

### Cliente MAUI

```csharp
// En MainPage.xaml.cs o ViewModel
public class MainPage : ContentPage
{
    private readonly IrcClient client;
    private readonly ScriptManager scriptManager;

    public MainPage(IrcClient client)
    {
        InitializeComponent();
        
        this.client = client;
        this.scriptManager = new ScriptManager(client);
        
        LoadScripts();
    }

    private async void LoadScripts()
    {
        var scriptsPath = Path.Combine(FileSystem.AppDataDirectory, "Scripts");
        
        // Cargar scripts desde almacenamiento de la app
        foreach (var file in Directory.GetFiles(scriptsPath, "*.csx"))
        {
            await scriptManager.LoadScriptAsync(file);
        }
    }
}
```

## Depuración

Para depurar scripts, usa el método `Host.Log()`:

```csharp
Host.Log($"Processing message: {Context.Message}");
Host.Log($"Channel: {Context.Channel?.Name}");
Host.Log($"Sender: {Context.Sender?.Nickname}");
```

Los mensajes aparecerán en la consola con el prefijo `[SCRIPT]`.
