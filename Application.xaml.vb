Imports System.Globalization

Class Application

    ' 应用程序级事件(例如 Startup、Exit 和 DispatcherUnhandledException)
    ' 可以在此文件中进行处理。
    Private Sub Application_Startup()
        Dim dict As New ResourceDictionary
        Select Case Threading.Thread.CurrentThread.CurrentCulture.Name.Substring(0, 2)
            Case "zh"
                dict.Source = New Uri("Localization/StringResources.zh-CN.xaml", UriKind.Relative)
            Case Else
                dict.Source = New Uri("Localization/StringResources.xaml", UriKind.Relative)
        End Select
        Me.Resources.MergedDictionaries.Add(dict)

        Dim args = Environment.GetCommandLineArgs()
        Dim argsToProcess = If(args Is Nothing, New String() {}, args.Skip(1).ToArray())

        If argsToProcess.Length > 0 Then
            Dim firstArg = argsToProcess(0).Trim().ToLowerInvariant()

            If firstArg = "/s" OrElse firstArg = "/start" Then
                ShowMainWindow()
                Return
            End If

            If firstArg = "/p" OrElse firstArg = "/preview" Then
                Dim previewHandle As IntPtr = IntPtr.Zero
                If argsToProcess.Length > 1 Then
                    previewHandle = ParseHwnd(argsToProcess(1))
                End If

                Dim win As New MainWindow(previewHandle)
                Application.Current.MainWindow = win
                win.Show()
                Return
            End If

            If firstArg = "/c" OrElse firstArg = "/config" OrElse firstArg = "/settings" OrElse firstArg.StartsWith("/c") Then
                Dim settingsWindow As New OptWindow()
                settingsWindow.ShowDialog()
                Application.Current.Shutdown()
                Return
            End If
        End If

        ShowMainWindow()
    End Sub

    Private Sub ShowMainWindow()
        Dim win As New MainWindow
        Application.Current.MainWindow = win
        win.Show()
    End Sub

    Private Shared Function ParseHwnd(value As String) As IntPtr
        If String.IsNullOrWhiteSpace(value) Then Return IntPtr.Zero

        Dim normalized = value.Trim()
        If normalized.StartsWith("0x", StringComparison.OrdinalIgnoreCase) Then normalized = normalized.Substring(2)

        Dim parsed As UInt64
        If UInt64.TryParse(normalized, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, parsed) Then
            Return New IntPtr(CInt(parsed))
        End If

        Dim decimalValue As UInt64
        If UInt64.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, decimalValue) Then
            Return New IntPtr(CInt(decimalValue))
        End If

        Return IntPtr.Zero
    End Function
End Class
