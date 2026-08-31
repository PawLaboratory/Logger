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