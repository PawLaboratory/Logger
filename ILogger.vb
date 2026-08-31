''' <summary>
''' 日志接口
''' </summary>
Public Interface ILogger
    Sub Log(level As LogLevel, message As String, Optional ex As Exception = Nothing)
    Property MinLevel As LogLevel
End Interface
''' <summary>
''' 日志工厂
''' </summary>
Public Interface ILoggerFactory
    Function CreateLogger(name As String) As ILogger
End Interface