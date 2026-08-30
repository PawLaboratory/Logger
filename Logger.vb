Imports System.IO
Imports System.Text

''' <summary>
''' 全局日志记录器实例, 此类无法被继承
''' </summary>
Public NotInheritable Class Logger

#Region "初始化"
    '单例实例
    Private Shared _instance As Logger
    Private Shared ReadOnly _lockObj As New Object()
    '基本属性
    Private _logPath As String
    Private _minLogLevel As LogLevel
    Private _autoFlush As Boolean
    Private _encoding As Encoding
    Private _dateFormat As String
    Private _logFormat As String
    Private _logLevelLength As LogLevelLength
    Private Shared ReadOnly _defaultConfig As New LoggerConfig() '默认配置
    Private Shared _isInitialized As Boolean = False
    '私有构造函数
    Private Sub New(config As LoggerConfig)
        If config Is Nothing Then config = _defaultConfig '如果配置参数不存在, 则使用默认配置
        _logPath = config.LogPath
        _minLogLevel = config.MinLogLevel
        _autoFlush = config.AutoFlush
        _encoding = config.Encoding
        _dateFormat = config.DateFormat
        _logFormat = config.LogFormat
        _logLevelLength = config.LevelLength
    End Sub
    ''' <summary>
    ''' 初始化 Logger 实例
    ''' </summary>
    Public Shared Sub Initialize(config As LoggerConfig)
        ArgumentNullException.ThrowIfNull(config) '配置不存在时抛出异常
        SyncLock _lockObj '保证原子性
            If _isInitialized Then
                Throw New InvalidOperationException("PawLab.Logger has been initialized")
            End If
            _instance = New Logger(config)
            _isInitialized = True
        End SyncLock
    End Sub
#End Region

#Region "实例相关方法"
    ''' <summary>
    ''' 获取单例实例
    ''' </summary>
    Public Shared ReadOnly Property Instance As Logger
        Get
            If Not _isInitialized Then
                SyncLock _lockObj
                    If Not _isInitialized Then
                        Throw New InvalidOperationException("Please initialize PawLab.Logger first")
                    End If
                End SyncLock
            End If
            Return _instance
        End Get
    End Property
    Private Sub Log(message As String, level As LogLevel, Optional ex As Exception = Nothing)
        '过滤掉低于特定等级的消息
        If level < _minLogLevel Then Return
        Dim logEntry As New StringBuilder(_logFormat)
        '替换占位符
        logEntry.Replace("{timestamp}", $"§8{Now.ToString(_dateFormat)}§r")
        logEntry.Replace("{level}", LoglevelStr(_logLevelLength, level))
        logEntry.Replace("{message}", message)
        '如果有异常, 添加异常信息
        If ex IsNot Nothing Then
            logEntry.AppendLine()
            logEntry.AppendLine($"Exception: {ex.GetType}")
            logEntry.AppendLine($"{ex.StackTrace}")
        End If
        '将日志输出到控制台
        ConsoleWriteLineWithColor(logEntry.ToString())
        '将日志写入到文件
        Try
            Dim logMessage As String = RemoveColorCodes(logEntry.ToString()) '将颜色字符过滤以便写入文件
            Using writer As New StreamWriter(Path.Combine(_logPath, "Latest.log"), True, _encoding) '将过滤后的字符写入文件
                writer.WriteLine(logMessage)
                If _autoFlush Then writer.Flush() '刷新缓冲区
            End Using
        Catch exIO As IOException
            '如果文件写入失败, 尝试输出到控制台
            Log($"Cannot write log file: {exIO.Message}", LogLevel.ERROR)
        End Try
    End Sub
    'DEBUG, INFO, WARN, ERROR
    Private Shared ReadOnly colorArray As String() = {"b", "a", "e", "c"}
    '行对应 Level, 列对应 Length
    Private Shared ReadOnly textArray As String(,) = {
    {"D", "DBG", "DEBUG"},
    {"I", "INF", "INFO"},
    {"W", "WRN", "WARN"},
    {"E", "ERR", "ERROR"}
}
    ''' <summary>
    ''' 根据日志级别与长度自动格式化
    ''' </summary>
    ''' <param name="length">日志级别长度枚举</param>
    ''' <param name="level">日志级别枚举</param>
    ''' <returns></returns>
    Private Function LoglevelStr(length As LogLevelLength, level As LogLevel) As String
        Dim levelIndex = CInt(level)
        Dim lengthIndex = CInt(length)
        Return $"§r[§{colorArray(levelIndex)}{textArray(levelIndex, lengthIndex)}§r]"
    End Function
#End Region

#Region "外部日志方法"
    ''' <summary>
    ''' 输出一条“调试”日志
    ''' </summary>
    Public Shared Sub Debug(message As String)
        Instance.Log(message, LogLevel.DEBUG)
    End Sub
    ''' <summary>
    ''' 输出一条“信息”日志
    ''' </summary>
    Public Shared Sub Info(message As String)
        Instance.Log(message, LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' 输出一条“警告”日志
    ''' </summary>
    Public Shared Sub Warning(message As String)
        Instance.Log(message, LogLevel.WARN)
    End Sub
    ''' <summary>
    ''' 输出一条“错误”日志
    ''' </summary>
    Public Shared Sub [Error](message As String, Optional ex As Exception = Nothing)
        Instance.Log(message, LogLevel.ERROR, ex)
    End Sub
#End Region

#Region "彩色字符方法"
    Private ReadOnly colorCodes As New Dictionary(Of Char, ConsoleColor) From {
    {"0", ConsoleColor.Black},
    {"1", ConsoleColor.DarkBlue},
    {"2", ConsoleColor.DarkGreen},
    {"3", ConsoleColor.DarkCyan},
    {"4", ConsoleColor.DarkRed},
    {"5", ConsoleColor.DarkMagenta},
    {"6", ConsoleColor.DarkYellow},
    {"7", ConsoleColor.Gray},
    {"8", ConsoleColor.DarkGray},
    {"9", ConsoleColor.Blue},
    {"a", ConsoleColor.Green},
    {"b", ConsoleColor.Cyan},
    {"c", ConsoleColor.Red},
    {"d", ConsoleColor.Magenta},
    {"e", ConsoleColor.Yellow},
    {"f", ConsoleColor.White},
    {"r", ConsoleColor.Gray} '重置颜色(默认为灰色)
} '颜色常量表
    ''' <summary>
    ''' 过滤颜色字符
    ''' </summary>
    ''' <param name="Input">文字内容</param>
    Private Function RemoveColorCodes(Input As String) As String
        Dim result As New StringBuilder()
        Dim i As Integer = 0
        While i < Input.Length
            If Input(i) = "§"c AndAlso i + 1 < Input.Length Then
                '跳过颜色代码
                i += 2
            Else
                result.Append(Input(i))
                i += 1
            End If
        End While
        Return result.ToString()
    End Function
    ''' <summary>
    ''' 输出带颜色的日志
    ''' </summary>
    ''' <param name="message">文字内容</param>
    Private Sub ConsoleWriteLineWithColor(message As String)
        Dim buffer As New StringBuilder()
        Dim currentColor As ConsoleColor = Console.ForegroundColor
        For i As Integer = 0 To message.Length - 1
            If message(i) = "§"c AndAlso i + 1 < message.Length Then
                '输出缓冲内容(应用当前颜色)
                If buffer.Length > 0 Then
                    Console.Write(buffer.ToString())
                    buffer.Clear()
                End If
                '处理颜色代码
                Dim code As Char = message(i + 1)
                If colorCodes.ContainsKey(code) Then
                    Console.ForegroundColor = colorCodes(code)
                End If
                i += 1 '跳过颜色代码
            Else
                buffer.Append(message(i))
            End If
        Next
        If buffer.Length > 0 Then '输出剩余内容
            Console.Write(buffer.ToString().TrimEnd())
        End If
        '重置颜色
        Console.ForegroundColor = currentColor
        Console.WriteLine()
    End Sub
#End Region

#Region "日志系统相关属性"
    ''' <summary>
    ''' 获得当前日志系统最低日志级别
    ''' </summary>
    Public ReadOnly Property MinLogLevel As LogLevel
        Get
            Return _minLogLevel
        End Get
    End Property
    ''' <summary>
    ''' 获得当前日志文件路径
    ''' </summary>
    Public ReadOnly Property LogFilePath As String
        Get
            Return Path.Combine(_logPath, "Latest.log")
        End Get
    End Property
    ''' <summary>
    ''' 获得当前日志目录
    ''' </summary>
    Public ReadOnly Property LogPath As String
        Get
            Return _logPath
        End Get
    End Property
#End Region

End Class