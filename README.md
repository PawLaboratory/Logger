# Logger

A lightweight logging system for .NET applications with colorful console output and file logging support.

**NuGet Package:** [`PawLab.Logger`](https://www.nuget.org/packages/PawLab.Logger)  

---

## Features

- Color-coded console output using Minecraft-style color codes (`∫` + color character)
- Configurable log levels (DEBUG, INFO, WARN, ERROR)
- Customizable log format with placeholders
- File logging with configurable encoding and auto-flush
- Singleton pattern for global logging instance
- Exception stack trace logging
- Support for both VB.NET and C#

## License

This project is licensed under the Apache License, Version 2.0. See the [LICENSE](LICENSE) file for details.

---

## Installation

### Via NuGet Package Manager

```bash
dotnet add package PawLab.Logger
```

Or using the Package Manager Console in Visual Studio:

```powershell
Install-Package PawLab.Logger
```

### Manual Copy

Alternatively, you can copy the `Logger.vb` and `LoggerConfig.vb` files into your project. The code is written in VB.NET but can be used from any .NET language that supports the Common Language Runtime (CLR).

### Requirements

- .NET Framework 4.5+ or .NET Core / .NET 5+
- No external dependencies

---

## Configuration

Before using the logger, you must initialize it with a `LoggerConfig` object. The configuration properties are:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LogPath` | `String` | `AppContext.BaseDirectory + "Logs"` | Directory where log files are stored |
| `LogFile` | `String` | `"Latest.log"` | Name of the log file |
| `MinLogLevel` | `LogLevel` | `INFO` | Minimum level to log (lower levels are ignored) |
| `AutoFlush` | `Boolean` | `False` | Whether to flush the file writer after each write |
| `Encoding` | `Encoding` | `UTF8` | Encoding used for the log file |
| `DateFormat` | `String` | `"HH:mm:ss"` | Format string for timestamps |
| `LogFormat` | `String` | `"{timestamp} {level} {message}"` | Format template with placeholders |
| `LevelLength` | `LogLevelLength` | `Standard` | Length of the level display (Short, Medium, Standard) |

### LogLevel Enum

- `DEBUG`
- `INFO`
- `WARN`
- `ERROR`

### LogLevelLength Enum

- `Short` 每 single letter (D, I, W, E)
- `Medium` 每 three letters (DBG, INF, WRN, ERR)
- `Standard` 每 full names (DEBUG, INFO, WARN, ERROR)

---

## Initialization

You must initialize the logger **once** before using any logging methods. Attempting to initialize twice will throw an `InvalidOperationException`.

### VB.NET

```vbnet
Imports PawLab.Logger

' Create a custom configuration
Dim config As New LoggerConfig()
config.LogPath = "C:\MyApp\Logs"
config.LogFile = "app.log"
config.MinLogLevel = LogLevel.DEBUG
config.AutoFlush = True
config.DateFormat = "yyyy-MM-dd HH:mm:ss"
config.LevelLength = LogLevelLength.Medium

' Initialize the logger
Logger.Initialize(config)
```

### C#

```csharp
using PawLab.Logger;

// Create a custom configuration
var config = new LoggerConfig
{
    LogPath = @"C:\MyApp\Logs",
    LogFile = "app.log",
    MinLogLevel = LogLevel.DEBUG,
    AutoFlush = true,
    DateFormat = "yyyy-MM-dd HH:mm:ss",
    LevelLength = LogLevelLength.Medium
};

// Initialize the logger
Logger.Initialize(config);
```

If you do not provide a configuration, the default settings will be used (but you still need to call `Initialize` with `Nothing` or `null`).

---

## Usage

After initialization, you can log messages using the static methods on the `Logger` class.

### VB.NET

```vbnet
' Logging at different levels
Logger.Debug("This is a debug message")
Logger.Info("Application started")
Logger.Warning("Disk space is low")
Logger.Error("An error occurred", ex)   ' ex is an optional Exception

' The console output will be colored based on level:
' DEBUG -> cyan, INFO -> green, WARN -> yellow, ERROR -> red
```

### C#

```csharp
// Logging at different levels
Logger.Debug("This is a debug message");
Logger.Info("Application started");
Logger.Warning("Disk space is low");
Logger.Error("An error occurred", ex);   // ex is an optional Exception

// Console colors: DEBUG=cyan, INFO=green, WARN=yellow, ERROR=red
```

### Color Codes

The logger supports inline color codes using the `∫` symbol followed by a color character. The following colors are available:

| Code | Color          | Code | Color          |
|------|----------------|------|----------------|
| `0`  | Black          | `8`  | Dark Gray      |
| `1`  | Dark Blue      | `9`  | Blue           |
| `2`  | Dark Green     | `a`  | Green          |
| `3`  | Dark Cyan      | `b`  | Cyan           |
| `4`  | Dark Red       | `c`  | Red            |
| `5`  | Dark Magenta   | `d`  | Magenta        |
| `6`  | Dark Yellow    | `e`  | Yellow         |
| `7`  | Gray           | `f`  | White          |
| `r`  | Reset (Gray)   |      |                |

Example:

```vbnet
Logger.Info("∫aThis is green text∫r and this is default gray")
```

Color codes are automatically stripped from the file output to keep log files clean.

### Custom Log Format

You can customize the log format by setting the `LogFormat` property in `LoggerConfig`. Available placeholders:

- `{timestamp}` 每 formatted timestamp (using `DateFormat`)
- `{level}` 每 colored level text (brackets and color included)
- `{message}` 每 the log message

Example format: `"[{timestamp}] {level}: {message}"`

---

## Advanced: Using the Instance

The logger is a singleton. You can also access the instance directly to read runtime properties:

### VB.NET

```vbnet
Dim instance = Logger.Instance
Console.WriteLine($"Current log level: {instance.MinLogLevel}")
Console.WriteLine($"Log file path: {instance.LogFilePath}")
```

### C#

```csharp
var instance = Logger.Instance;
Console.WriteLine($"Current log level: {instance.MinLogLevel}");
Console.WriteLine($"Log file path: {instance.LogFilePath}");
```

---

## ILogger Interface (Future Extension)

The project includes an `ILogger` interface and an `ILoggerFactory` interface for future extensibility (e.g., dependency injection, named loggers). These are not yet implemented but are provided as a design guideline.

```vbnet
Public Interface ILogger
    Sub Log(level As LogLevel, message As String, Optional ex As Exception = Nothing)
    Property MinLevel As LogLevel
End Interface

Public Interface ILoggerFactory
    Function CreateLogger(name As String) As ILogger
End Interface
```

---

## Exception Handling

When an exception is passed to the `Error` method, the stack trace and exception type are automatically appended to the log entry. This information is both displayed in the console and written to the file.

If file writing fails (e.g., due to permission issues), the logger will attempt to write an error message to the console using the logger itself (fallback). This prevents infinite recursion.

---

## Notes

- The color code character is the section sign (`∫`, Unicode U+00A7). In source code, you can use `ChrW(&HA7)` in VB.NET or `'\u00A7'` in C# if needed.
- The logger is thread-safe for initialization and instance access.
- Log files are appended; they are not rotated automatically. You may implement your own rotation logic externally.

---

## Contributing

Feel free to submit issues or pull requests to the repository. Please ensure your code follows the existing style and includes appropriate documentation.

---

## Acknowledgments

- Developed by xionglongztz / PawLaboratory
- Licensed under Apache 2.0