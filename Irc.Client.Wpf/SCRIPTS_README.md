# IRC Client - Sistema de Scripts

## 🎯 Descripción

El cliente IRC WPF incluye un sistema completo de scripting basado en **Roslyn** que permite automatizar tareas y responder a eventos de IRC usando C#.

## 📍 Acceso al Script Manager

1. **Método 1 (UI)**: Menú **Tools > Script Manager**
2. **Método 2 (Comando)**: Escribe `/script list` en cualquier chat

**Nota**: No es necesario estar conectado para gestionar scripts, pero deben estar habilitados y el cliente conectado para que se ejecuten.

## 💻 Editor de Scripts

El Script Manager incluye un editor profesional basado en **RoslynPad** con:

- ✅ **IntelliSense**: Autocompletado en tiempo real
- ✅ **Syntax Highlighting**: Coloreado de sintaxis C#
- ✅ **Diagnósticos**: Errores y advertencias en tiempo real
- ✅ **Signature Help**: Ayuda de parámetros al escribir métodos
- ✅ **Line Numbers**: Números de línea
- ✅ **Code Formatting**: Formateo automático

### Botón "Insert Template"
Inserta una plantilla completa con:
- Todos los eventos disponibles
- Documentación de la API `Host`
- Ejemplos de uso con async/await

### Botón "Edit in Visual Studio"
Abre el script en Visual Studio/VS Code para:
- IntelliSense completo con todas las referencias
- Debugging avanzado
- Sincronización automática al guardar

## 📜 Comandos Disponibles

### `/script list` o `/script ls`
Muestra todos los scripts ordenados alfabéticamente con su estado (enabled/disabled) e índice.

### `/script show <nombre-o-índice>`
Muestra el contenido de un script con números de línea.
- `/script show Test`
- `/script show Test.csx`
- `/script show 1`

Aliases: `/script view`, `/script cat`

### `/script enable <nombre-o-índice>`
Habilita un script para que se ejecute.
- `/script enable Test`
- `/script enable 1`

### `/script disable <nombre-o-índice>`
Deshabilita un script (no se ejecutará pero permanece cargado).

### `/script delete <nombre-o-índice>` o `/script remove`
Elimina un script permanentemente.

Alias: `/script rm`

### `/script reload`
Recarga todos los scripts desde la carpeta.

## 🔧 API de Scripting

### Objeto Global: `Host`

Todos los scripts tienen acceso al objeto global `Host` que proporciona la API completa.

#### Información del Usuario Local
```csharp
// Acceder a información del usuario conectado
Host.Me.Nickname          // Tu nickname
Host.Me.Channels          // Lista de canales en los que estás
```

#### Suscripción a Eventos

**Mensajes Privados**
```csharp
Host.OnPrivateMessage(async args =>
{
    // args.Sender.Nickname - Quien envió el mensaje
    // args.Message - El contenido del mensaje
    Host.Log($"Mensaje privado de {args.Sender.Nickname}: {args.Message}");
});
```

**Mensajes de Canal**
```csharp
Host.OnChannelMessage(async args =>
{
    // args.Channel.Name - Nombre del canal
    // args.Sender.Nickname - Quien envió el mensaje
    // args.Message - El contenido del mensaje
    
    if (args.Message == "!help")
    {
        await Host.SendChannelMessageAsync(args.Channel.Name, "¡Hola! Soy un bot.");
    }
});
```

**Usuario Entra a Canal**
```csharp
Host.OnUserJoined(async args =>
{
    // args.Channel.Name - Canal al que entró
    // args.User.Nickname - Nickname del usuario
    Host.Log($"{args.User.Nickname} entró a {args.Channel.Name}");
});
```

**Usuario Sale de Canal (Part)**
```csharp
Host.OnUserParted(async args =>
{
    // args.Channel.Name - Canal del que salió (part)
    // args.User.Nickname - Nickname del usuario
    Host.Log($"{args.User.Nickname} salió (part) de {args.Channel.Name}");
});
```

#### Acciones Disponibles

**Enviar Mensajes**
```csharp
// Mensaje privado
await Host.SendPrivateMessageAsync("NickDestino", "Hola!");

// Mensaje a canal
await Host.SendChannelMessageAsync("#general", "Hola a todos!");
```

**Gestión de Canales**
```csharp
// Unirse a un canal
await Host.JoinChannelAsync("#nuevo-canal");

// Salir de un canal
await Host.PartChannelAsync("#canal", "Adiós!");
```

**Logging**
```csharp
// Escribir en el log (aparece en el tab Status con prefijo [SCRIPT])
Host.Log("Mensaje de log");
```

## 📁 Ubicación de Scripts

Los scripts se guardan en: `<AppDirectory>/Scripts/`

Cada script es un archivo `.csx` (C# Script) y tiene su metadata en `scripts.json` que incluye:
- Estado enabled/disabled
- Fecha de creación
- Última modificación
- Descripción (opcional)

## ✨ Características Avanzadas

### Gestión sin Conexión
- Puedes crear, editar y organizar scripts sin estar conectado
- Los scripts se cargan y ejecutan automáticamente al conectarte

### Sincronización Automática
- Los cambios en la carpeta `Scripts/` se detectan automáticamente
- La UI se actualiza al crear/modificar/eliminar archivos externamente

### Prevención de Duplicados
- Al crear un script con nombre existente, se pregunta si deseas sobreescribirlo
- Confirmación requerida para operaciones destructivas

### Ordenación Alfabética
- Todos los listados ignoran la extensión `.csx`
- Ordenación case-insensitive

### Identificación Flexible
- Los scripts se pueden identificar por:
  - Nombre completo: `Test.csx`
  - Nombre sin extensión: `Test`
  - Índice numérico: `1`, `2`, etc.

## 🤖 Comandos del Bot IRC (Obsoleto)

Si usas el bot de consola (obsoleto), estos comandos están disponibles:

- `!scripts` - Lista de scripts
- `!showscript <nombre>` - Muestra contenido de un script
- `!addscript <url>` - (Admin) Añade script desde URL
- `!removescript <nombre>` - (Admin) Elimina script
- `!enable <nombre>` - (Admin) Habilita script
- `!disable <nombre>` - (Admin) Deshabilita script
- `!reloadscripts` - (Admin) Recarga scripts

## 📝 Ejemplo Completo

```csharp
// Bot de bienvenida y respuesta automática
Host.OnUserJoined(async args =>
{
    await Host.SendChannelMessageAsync(
        args.Channel.Name, 
        $"¡Bienvenido {args.User.Nickname}! 👋"
    );
});

Host.OnChannelMessage(async args =>
{
    var msg = args.Message.ToLower();
    
    if (msg == "!hora")
    {
        await Host.SendChannelMessageAsync(
            args.Channel.Name,
            $"Son las {DateTime.Now:HH:mm:ss}"
        );
    }
    
    if (msg.StartsWith("!echo "))
    {
        var texto = args.Message.Substring(6);
        await Host.SendChannelMessageAsync(args.Channel.Name, texto);
    }
});

Host.Log("Bot de bienvenida iniciado!");
```

## 🔍 Resolución de Problemas

### Los scripts no se ejecutan
- Verifica que estés conectado al servidor IRC
- Comprueba que el script esté **habilitado** (checkbox en la UI)
- Revisa el tab **Status** para ver logs de errores

### IntelliSense no funciona
- RoslynPad puede tardar unos segundos en cargar
- Verifica que el script tenga sintaxis válida
- Usa "Edit in Visual Studio" para IntelliSense completo

### El script tiene errores de compilación
- Los errores aparecen subrayados en rojo en el editor
- Coloca el cursor sobre el error para ver el mensaje
- Usa `Host.Log()` para debugging

## 📚 Referencias

- **Roslyn Scripting**: [Microsoft Docs](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/january/essential-net-csharp-scripting)
- **async/await**: [Task-based Asynchronous Pattern](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
- **IRC Protocol**: [RFC 1459](https://tools.ietf.org/html/rfc1459)
