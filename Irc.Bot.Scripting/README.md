# IRC Bot with Script Management

Bot de IRC con sistema de scripting basado en Roslyn C#.

## Características

- 🔧 **Gestión de scripts**: Agregar, eliminar, habilitar y deshabilitar scripts desde consola
- 📁 **Persistencia automática**: Los scripts se almacenan en la carpeta `Scripts/` junto al ejecutable
- 🔄 **Sincronización**: Detecta automáticamente nuevos scripts en la carpeta
- 💻 **Comandos de consola**: Control total mediante comandos `/` en la consola del bot
- 📋 **Manifest JSON**: Configuración persistente en `Scripts/scripts.json`

## Estructura de Carpetas

```
Irc.Bot.Scripting/
├── Scripts/                    # Carpeta de scripts (auto-creada en runtime)
│   ├── scripts.json           # Manifest con configuración de scripts
│   └── [tus scripts .csx]     # Scripts personalizados del bot
└── bin/
```

**Nota**: Los scripts de ejemplo están en `Irc.Client.Scripting\Examples\`. Puedes copiar cualquiera de ellos a la carpeta `Scripts/` del bot cuando se ejecute.

## Comandos Disponibles

### Gestión de Scripts

Todos los comandos se escriben directamente en la consola del bot:

- **`/script list`** - Lista todos los scripts cargados
  ```
  > /script list
  Loaded scripts (5):
  ✓ Enabled     AutoResponder.csx              [PrivateMessage]
  ✗ Disabled    Welcome.csx                    [UserJoined]
  ✓ Enabled     PingPong.csx                   [ChannelMessage]
  ```

- **`/script add <url-or-path>`** - Agrega un script desde URL o ruta local
  ```
  > /script add https://example.com/myscript.csx
  > /script add C:\path\to\script.csx
  ```

- **`/script remove <name>`** - Elimina un script
  ```
  > /script remove Welcome.csx
  ✓ Script 'Welcome.csx' removed.
  ```

- **`/script enable <name>`** - Habilita un script
  ```
  > /script enable Welcome.csx
  ✓ Script 'Welcome.csx' enabled.
  ```

- **`/script disable <name>`** - Deshabilita un script
  ```
  > /script disable PingPong.csx
  ✓ Script 'PingPong.csx' disabled.
  ```

- **`/script reload`** - Recarga todos los scripts desde la carpeta
  ```
  > /script reload
  Reloading scripts from folder...
  ✓ Scripts reloaded successfully.
  ```

### Comandos IRC

- **`/join <channel>`** - Únete a un canal
  ```
  > /join #test
  ✓ Joining #test...
  ```

- **`/part [channel]`** - Sal de un canal
  ```
  > /part #test
  ✓ Leaving #test...
  ```

- **`/msg <target> <message>`** - Envía un mensaje
  ```
  > /msg #test Hello everyone!
  → [#test] Hello everyone!
  ```

### Control del Bot

- **`/help`** - Muestra ayuda de comandos
- **`/quit`** - Cierra el bot

## Manifest (scripts.json)

El archivo `Scripts/scripts.json` contiene la configuración de todos los scripts:

```json
{
  "scripts": [
    {
      "fileName": "AutoResponder.csx",
      "isEnabled": true,
      "description": null,
      "createdAt": "2026-02-07T08:00:00Z",
      "lastModified": "2026-02-07T08:30:00Z"
    },
    {
      "fileName": "Welcome.csx",
      "isEnabled": false,
      "description": null,
      "createdAt": "2026-02-07T08:00:00Z",
      "lastModified": "2026-02-07T08:00:00Z"
    }
  ]
}
```

### Sincronización Automática

Al iniciar el bot:
1. **Lee scripts.json**: Carga la configuración existente
2. **Escanea carpeta Scripts/**: Detecta archivos `.csx`
3. **Sincroniza**:
   - Elimina entradas del manifest para scripts borrados del disco
   - Añade nuevos scripts encontrados como **deshabilitados** por defecto
4. **Carga scripts**: Solo ejecuta los marcados como `isEnabled: true`

Si agregas manualmente un script `.csx` a la carpeta, usa `/script reload` para detectarlo.

## Configuración Inicial

Al ejecutar el bot, te preguntará:

```
╔════════════════════════════════════════════════════════════╗
║           IRC Bot with Script Management                  ║
╚════════════════════════════════════════════════════════════╝

Enter IRC server (default: localhost): irc.example.com
Enter port (default: 6667): 6667
Enter bot nickname (default: ScriptBot): MyBot
Enter channel to join (e.g., #test): #mychannel
```

## Ejemplo de Sesión

```
> /script list
Loaded scripts (3):
✓ Enabled     AutoResponder.csx              [PrivateMessage]
✗ Disabled    Welcome.csx                    [UserJoined]
✓ Enabled     PingPong.csx                   [ChannelMessage]

> /script enable Welcome.csx
✓ Script 'Welcome.csx' enabled.

> /join #test
✓ Joining #test...

> /msg #test Hello everyone!
→ [#test] Hello everyone!

[#test] <Alice> Hi MyBot!

> /script disable AutoResponder.csx
✓ Script 'AutoResponder.csx' disabled.

> /quit
Shutting down...
Bot stopped.
```

## Agregar Scripts

### Método 1: Copiar a la carpeta

1. Crea un archivo `.csx` en la carpeta `Scripts/`
2. Ejecuta `/script reload` en el bot
3. El script se añadirá como **deshabilitado**
4. Usa `/script enable <nombre>` para activarlo

### Método 2: Comando /script add

```
> /script add https://gist.github.com/user/script.csx
Downloading script from https://gist.github.com/user/script.csx...
✓ Script 'script.csx' added and enabled successfully.
```

## Ejemplo de Script

```csharp
// PingPong.csx - Responde a !ping en el canal
Host.OnChannelMessage(async e => 
{
    if (e.Message.StartsWith("!ping"))
    {
        await Host.SendChannelMessageAsync(e.Channel.Name, "Pong!");
    }
});
```

## Logs al Iniciar

```
╔════════════════════════════════════════════════════════════╗
║           IRC Bot with Script Management                  ║
╚════════════════════════════════════════════════════════════╝

Connecting to irc.example.com:6667 as MyBot...
Loading scripts from Scripts folder...
✓ Loaded 5 script(s)
  - 3 enabled
  - 2 disabled

[INFO] Connected to IRC server!
Joining #test...

╔════════════════════════════════════════════════════════════╗
║                    Bot is Running!                         ║
╚════════════════════════════════════════════════════════════╝

Active Scripts:
  ✓ AutoResponder.csx              [PrivateMessage]
  ✓ PingPong.csx                   [ChannelMessage]
  ✓ ChannelLogger.csx              [ChannelMessage]

Type /help for available commands
Type /quit to exit

─────────────────────────────────────────────────────────────

> 
```

## Ver También

- [Irc.Client.Scripting/README.md](../Irc.Client.Scripting/README.md) - Documentación de la API de scripting
- [Irc.Client.Scripting/Examples/](../Irc.Client.Scripting/Examples/) - Scripts de ejemplo
