# IRC Script Examples

Esta carpeta contiene scripts de ejemplo para el sistema de scripting IRC.

## Scripts Disponibles

### 📋 TemplateScript.csx
**Plantilla completa con todos los eventos disponibles**
- Incluye ejemplos comentados de todos los eventos
- Documentación de las variables disponibles en cada evento
- Ideal para comenzar un nuevo script

### 🤖 AutoResponder.csx
**Responde automáticamente a mensajes privados**
- Responde a mensajes que contienen palabras clave
- Ejemplo de manejo de mensajes privados

### 👋 Welcome.csx
**Da la bienvenida a usuarios que entran a canales**
- Detecta cuando un usuario entra a un canal
- Envía mensaje de bienvenida personalizado

### 👋 Farewell.csx
**Despide a usuarios que salen de canales**
- Detecta cuando un usuario sale (part) de un canal
- Envía mensaje de despedida

### 🏓 PingPong.csx
**Bot simple de ping-pong**
- Responde "pong" cuando alguien dice "!ping" en un canal
- Ejemplo básico de comandos de canal

### 📊 ChannelLogger.csx
**Registra todos los mensajes de canal**
- Guarda un log de todos los mensajes recibidos
- Ejemplo de logging y manejo de archivos

### ℹ️ InfoBot.csx
**Bot de información**
- Responde a comandos de información
- Ejemplo de bot con múltiples comandos

### 🔧 ExampleWithIntelliSense.csx
**Ejemplo con directivas #r para IntelliSense**
- Incluye referencias para usar en VS Code
- Muestra cómo configurar IntelliSense durante desarrollo

## Cómo Usar

### En el Cliente WPF
1. Abre el Script Manager (Tools > Script Manager)
2. Haz clic en "Load Script..."
3. Navega a esta carpeta y selecciona un script
4. O copia el contenido del script y pégalo en el editor

### En el Bot de Consola
1. Copia los scripts que quieras usar a la carpeta `Scripts/` del bot (se crea automáticamente al ejecutar)
2. Usa los comandos `/script list`, `/script enable`, etc.

### Para Desarrollo
Para obtener IntelliSense completo al editar estos scripts en VS Code o Visual Studio:
1. Abre el script en tu editor
2. Las referencias ya están incluidas en ExampleWithIntelliSense.csx como ejemplo
3. O usa el botón "Edit in Visual Studio" del WPF Script Manager

## Personalización

Puedes modificar cualquiera de estos scripts para adaptarlos a tus necesidades:
- Cambia los mensajes de respuesta
- Modifica las condiciones de activación
- Combina funcionalidades de múltiples scripts
- Usa TemplateScript.csx como base para crear uno nuevo

## API Disponible

Todos los scripts tienen acceso al objeto global `Host` con estos métodos:

**Eventos:**
- `Host.OnPrivateMessage(async args => { ... })`
- `Host.OnChannelMessage(async args => { ... })`
- `Host.OnUserJoined(async args => { ... })`
- `Host.OnUserParted(async args => { ... })`

**Acciones:**
- `await Host.SendPrivateMessageAsync(nickname, message)`
- `await Host.SendChannelMessageAsync(channelName, message)`
- `await Host.JoinChannelAsync(channelName)`
- `await Host.PartChannelAsync(channelName, reason)`
- `Host.Log(message)` - Escribe en el log del cliente

**Información:**
- `Host.Me.Nickname` - Tu nickname
- `Host.Me.Channels` - Canales en los que estás

Para más información, consulta la documentación completa en `SCRIPTS_README.md` del proyecto WPF.
