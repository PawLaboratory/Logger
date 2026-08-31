' Logger - A logging system with colorful text
' Copyright 2026 xionglongztz/PawLaboratory
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
Imports System.IO
Imports System.Text

''' <summary>
''' 全局日志记录器实例, 此类无法被继承
''' </summary>
Public NotInheritable Class Logger

#Region "初始化"
    '单例实例
    Private Shared _instance As Logger
    Private Shared ReadOnly _initLockObj As New Object() '初始化对象锁
    Private Shared ReadOnly _logLockObj As New Object() '日志行锁
    '基本属性
    Private _logPath As String
    Private _logFile As String
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
        _logFile = config.LogFile
        _minLogLevel = config.MinLogLevel
        _autoFlush = config.AutoFlush
        _encoding = config.Encoding
        _dateFormat = config.DateFormat
        _logFormat = config.LogFormat
        _logLevelLength = config.LevelLength
        Directory.CreateDirectory(_logPath) '如果目录不存在则新建
    End Sub
    ''' <summary>
    ''' 初始化 Logger 实例
    ''' </summary>
    Public Shared Sub Initialize(config As LoggerConfig)
        SyncLock _initLockObj '保证原子性
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
                SyncLock _initLockObj
                    If Not _isInitialized Then
                        Throw New InvalidOperationException("Please initialize PawLab.Logger first")
                    End If
                End SyncLock
            End If
            Return _instance
        End Get
    End Property
    Private Sub Log(message As String, level As LogLevel, Optional ex As Exception = Nothing)
        SyncLock _logLockObj '确保日志不会乱掉
            '过滤掉低于特定等级的消息
            If level < _minLogLevel Then Return
            Dim logEntry As New StringBuilder(_logFormat)
            '替换占位符
            logEntry.Replace("{timestamp}", $"{ChrW(&HA7)}8{Now.ToString(_dateFormat)}{ChrW(&HA7)}r")
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
                Using writer As New StreamWriter(Path.Combine(_logPath, _logFile), True, _encoding) '将过滤后的字符写入文件
                    writer.WriteLine(logMessage)
                    If _autoFlush Then writer.Flush() '刷新缓冲区
                End Using
            Catch exIO As IOException
                '如果文件写入失败, 尝试输出到控制台
                Log($"Cannot write log file: {exIO.Message}", LogLevel.ERROR)
            End Try
        End SyncLock
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
        Return $"{ChrW(&HA7)}r[{ChrW(&HA7)}{colorArray(levelIndex)}{textArray(levelIndex, lengthIndex)}{ChrW(&HA7)}r]" '{ChrW(&HA7)} 是分节符
        '该符号无法出现在代码中, 若需要复制请前往 https://zh.wikipedia.org/wiki/%E5%88%86%E8%8A%82%E7%AC%A6%E5%8F%B7
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
            If Input(i) = ChrW(&HA7) AndAlso i + 1 < Input.Length Then 'ChrW(&HA7) 是 "x"c(分节符) 的转义, 避免编译问题
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
            If message(i) = ChrW(&HA7) AndAlso i + 1 < message.Length Then '同理...
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
            Return Path.Combine(_logPath, _logFile)
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