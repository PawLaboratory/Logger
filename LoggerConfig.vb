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
''' 日志配置类
''' </summary>
Public Class LoggerConfig
    ''' <summary>
    ''' 日志目录路径
    ''' </summary>
    Public Property LogPath As String = Path.Combine(AppContext.BaseDirectory, "Logs")
    ''' <summary>
    ''' 日志文件名
    ''' </summary>
    Public Property LogFile As String = "Latest.log"
    ''' <summary>
    ''' 最低日志级别
    ''' </summary>
    Public Property MinLogLevel As LogLevel = LogLevel.INFO
    ''' <summary>
    ''' 是否自动刷新缓冲区
    ''' </summary>
    Public Property AutoFlush As Boolean = False
    ''' <summary>
    ''' 文件编码
    ''' </summary>
    Public Property Encoding As Encoding = Encoding.UTF8
    ''' <summary>
    ''' 日期时间格式
    ''' </summary>
    Public Property DateFormat As String = "HH:mm:ss"
    ''' <summary>
    ''' 日志格式(支持颜色代码)
    ''' </summary>
    Public Property LogFormat As String = "{timestamp} {level} {message}"
    ''' <summary>
    ''' 日志级别长度
    ''' </summary>
    Public Property LevelLength As LogLevelLength = LogLevelLength.Standard
End Class

''' <summary>
''' 日志级别
''' </summary>
Public Enum LogLevel
    ''' <summary>
    ''' 日志级别: 调试
    ''' </summary>
    DEBUG
    ''' <summary>
    ''' 日志级别: 信息
    ''' </summary>
    INFO
    ''' <summary>
    ''' 日志级别: 警告
    ''' </summary>
    WARN
    ''' <summary>
    ''' 日志级别: 错误
    ''' </summary>
    [ERROR]
End Enum

''' <summary>
''' 日志级别长度
''' </summary>
Public Enum LogLevelLength
    ''' <summary>
    ''' 日志级别长度: 1个字母
    ''' </summary>
    [Short]
    ''' <summary>
    ''' 日志级别长度: 3个字母
    ''' </summary>
    Medium
    ''' <summary>
    ''' 日志级别长度: 标准长度
    ''' </summary>
    Standard
End Enum