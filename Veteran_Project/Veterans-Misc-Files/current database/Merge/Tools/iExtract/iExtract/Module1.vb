Imports Microsoft.Office.Interop

'Extracts all pictures from all 6 ORIGINAL(mostly) ACCESS db files (.accdb) ignoring duplicates from marker, misc, and military.
Module Module1

    Sub Main()
        Dim dp1 As String = "C:\Users\JM\Desktop\vet db files\new\db1.accdb"
        Dim dp2 As String = "C:\Users\JM\Desktop\vet db files\new\db2.accdb"
        Dim dp3 As String = "C:\Users\JM\Desktop\vet db files\new\db3.accdb"
        Dim dp4 As String = "C:\Users\JM\Desktop\vet db files\new\db4.accdb"
        Dim dp5 As String = "C:\Users\JM\Desktop\vet db files\new\db5.accdb"
        Dim dp6 As String = "C:\Users\JM\Desktop\vet db files\new\db6.accdb"

        Dim AccessEngine As New Microsoft.Office.Interop.Access.Dao.DBEngine
        Dim SourceRS As Microsoft.Office.Interop.Access.Dao.Recordset
        Dim MarkerRS As Microsoft.Office.Interop.Access.Dao.Recordset
        Dim MiscRS As Microsoft.Office.Interop.Access.Dao.Recordset
        Dim MilitaryRS As Microsoft.Office.Interop.Access.Dao.Recordset



        Dim db1 As Microsoft.Office.Interop.Access.Dao.Database = AccessEngine.OpenDatabase(dp1)

        SourceRS = db1.OpenRecordset("SELECT * FROM `Veterans Data`")

        While Not SourceRS.EOF
            MarkerRS = SourceRS.Fields("MARKER PHOTO").Value
            MiscRS = SourceRS.Fields("MISC PHOTO").Value
            MilitaryRS = SourceRS.Fields("MILITARY PHOTO").Value
            Try
                If Not MarkerRS.EOF Then
                    Dim AttachmentFileName As String = MarkerRS.Fields("FileName").Value.ToString
                    Dim markerPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Marker\" & AttachmentFileName
                    MarkerRS.Fields("FileData").SaveToFile(markerPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MiscRS.EOF Then
                    Dim AttachmentFileName As String = MiscRS.Fields("FileName").Value.ToString
                    Dim miscPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Misc\" & AttachmentFileName
                    MiscRS.Fields("FileData").SaveToFile(miscPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MilitaryRS.EOF Then
                    Dim AttachmentFileName As String = MilitaryRS.Fields("FileName").Value.ToString
                    Dim militaryPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Military\" & AttachmentFileName
                    MilitaryRS.Fields("FileData").SaveToFile(militaryPath)
                End If
            Catch ex As Exception
            End Try
            SourceRS.MoveNext()
        End While

        SourceRS.Close()
        SourceRS = Nothing
        MarkerRS = Nothing
        MiscRS = Nothing
        MilitaryRS = Nothing
        db1.Close()
        db1 = Nothing

        Dim db2 As Microsoft.Office.Interop.Access.Dao.Database = AccessEngine.OpenDatabase(dp2)

        SourceRS = db2.OpenRecordset("SELECT * FROM `Veterans Data`")

        While Not SourceRS.EOF
            MarkerRS = SourceRS.Fields("MARKER PHOTO").Value
            MiscRS = SourceRS.Fields("MISC PHOTO").Value
            MilitaryRS = SourceRS.Fields("MILITARY PHOTO").Value
            Try
                If Not MarkerRS.EOF Then
                    Dim AttachmentFileName As String = MarkerRS.Fields("FileName").Value.ToString
                    Dim markerPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Marker\" & AttachmentFileName
                    MarkerRS.Fields("FileData").SaveToFile(markerPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MiscRS.EOF Then
                    Dim AttachmentFileName As String = MiscRS.Fields("FileName").Value.ToString
                    Dim miscPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Misc\" & AttachmentFileName
                    MiscRS.Fields("FileData").SaveToFile(miscPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MilitaryRS.EOF Then
                    Dim AttachmentFileName As String = MilitaryRS.Fields("FileName").Value.ToString
                    Dim militaryPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Military\" & AttachmentFileName
                    MilitaryRS.Fields("FileData").SaveToFile(militaryPath)
                End If
            Catch ex As Exception
            End Try
            SourceRS.MoveNext()
        End While

        SourceRS.Close()
        SourceRS = Nothing
        MarkerRS = Nothing
        MiscRS = Nothing
        MilitaryRS = Nothing
        db2.Close()
        db2 = Nothing

        Dim db3 As Microsoft.Office.Interop.Access.Dao.Database = AccessEngine.OpenDatabase(dp3)

        SourceRS = db3.OpenRecordset("SELECT * FROM `Veterans Data`")

        While Not SourceRS.EOF

            MarkerRS = SourceRS.Fields("MARKER PHOTO").Value
            MiscRS = SourceRS.Fields("MISC PHOTO").Value
            MilitaryRS = SourceRS.Fields("MILITARY PHOTO").Value
            Try
                If Not MarkerRS.EOF Then
                    Dim AttachmentFileName As String = MarkerRS.Fields("FileName").Value.ToString
                    Dim markerPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Marker\" & AttachmentFileName
                    MarkerRS.Fields("FileData").SaveToFile(markerPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MiscRS.EOF Then
                    Dim AttachmentFileName As String = MiscRS.Fields("FileName").Value.ToString
                    Dim miscPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Misc\" & AttachmentFileName
                    MiscRS.Fields("FileData").SaveToFile(miscPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MilitaryRS.EOF Then
                    Dim AttachmentFileName As String = MilitaryRS.Fields("FileName").Value.ToString
                    Dim militaryPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Military\" & AttachmentFileName
                    MilitaryRS.Fields("FileData").SaveToFile(militaryPath)
                End If
            Catch ex As Exception
            End Try
            SourceRS.MoveNext()
        End While

        SourceRS.Close()
        SourceRS = Nothing
        MarkerRS = Nothing
        MiscRS = Nothing
        MilitaryRS = Nothing
        db3.Close()
        db3 = Nothing

        Dim db4 As Microsoft.Office.Interop.Access.Dao.Database = AccessEngine.OpenDatabase(dp4)

        SourceRS = db4.OpenRecordset("SELECT * FROM `Veterans Data`")

        While Not SourceRS.EOF
            MarkerRS = SourceRS.Fields("MARKER PHOTO").Value
            MiscRS = SourceRS.Fields("MISC PHOTO").Value
            MilitaryRS = SourceRS.Fields("MILITARY PHOTO").Value
            Try
                If Not MarkerRS.EOF Then
                    Dim AttachmentFileName As String = MarkerRS.Fields("FileName").Value.ToString
                    Dim markerPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Marker\" & AttachmentFileName
                    MarkerRS.Fields("FileData").SaveToFile(markerPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MiscRS.EOF Then
                    Dim AttachmentFileName As String = MiscRS.Fields("FileName").Value.ToString
                    Dim miscPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Misc\" & AttachmentFileName
                    MiscRS.Fields("FileData").SaveToFile(miscPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MilitaryRS.EOF Then
                    Dim AttachmentFileName As String = MilitaryRS.Fields("FileName").Value.ToString
                    Dim militaryPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Military\" & AttachmentFileName
                    MilitaryRS.Fields("FileData").SaveToFile(militaryPath)
                End If
            Catch ex As Exception
            End Try
            SourceRS.MoveNext()
        End While

        SourceRS.Close()
        SourceRS = Nothing
        MarkerRS = Nothing
        MiscRS = Nothing
        MilitaryRS = Nothing
        db4.Close()
        db4 = Nothing

        Dim db5 As Microsoft.Office.Interop.Access.Dao.Database = AccessEngine.OpenDatabase(dp5)

        SourceRS = db5.OpenRecordset("SELECT * FROM `Veterans Data`")

        While Not SourceRS.EOF
            MarkerRS = SourceRS.Fields("MARKER PHOTO").Value
            MiscRS = SourceRS.Fields("MISC PHOTO").Value
            MilitaryRS = SourceRS.Fields("MILITARY PHOTO").Value
            Try
                If Not MarkerRS.EOF Then
                    Dim AttachmentFileName As String = MarkerRS.Fields("FileName").Value.ToString
                    Dim markerPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Marker\" & AttachmentFileName
                    MarkerRS.Fields("FileData").SaveToFile(markerPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MiscRS.EOF Then
                    Dim AttachmentFileName As String = MiscRS.Fields("FileName").Value.ToString
                    Dim miscPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Misc\" & AttachmentFileName
                    MiscRS.Fields("FileData").SaveToFile(miscPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MilitaryRS.EOF Then
                    Dim AttachmentFileName As String = MilitaryRS.Fields("FileName").Value.ToString
                    Dim militaryPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Military\" & AttachmentFileName
                    MilitaryRS.Fields("FileData").SaveToFile(militaryPath)
                End If
            Catch ex As Exception
            End Try
            SourceRS.MoveNext()
        End While

        SourceRS.Close()
        SourceRS = Nothing
        MarkerRS = Nothing
        MiscRS = Nothing
        MilitaryRS = Nothing
        db5.Close()
        db5 = Nothing

        Dim db6 As Microsoft.Office.Interop.Access.Dao.Database = AccessEngine.OpenDatabase(dp6)

        SourceRS = db6.OpenRecordset("SELECT * FROM `Veterans Data`")

        While Not SourceRS.EOF
            MarkerRS = SourceRS.Fields("MARKER PHOTO").Value
            MiscRS = SourceRS.Fields("MISC PHOTO").Value
            MilitaryRS = SourceRS.Fields("MILITARY PHOTO").Value
            Try
                If Not MarkerRS.EOF Then
                    Dim AttachmentFileName As String = MarkerRS.Fields("FileName").Value.ToString
                    Dim markerPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Marker\" & AttachmentFileName
                    MarkerRS.Fields("FileData").SaveToFile(markerPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MiscRS.EOF Then
                    Dim AttachmentFileName As String = MiscRS.Fields("FileName").Value.ToString
                    Dim miscPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Misc\" & AttachmentFileName
                    MiscRS.Fields("FileData").SaveToFile(miscPath)
                End If
            Catch ex As Exception
            End Try
            Try
                If Not MilitaryRS.EOF Then
                    Dim AttachmentFileName As String = MilitaryRS.Fields("FileName").Value.ToString
                    Dim militaryPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\vet db files\new\Military\" & AttachmentFileName
                    MilitaryRS.Fields("FileData").SaveToFile(militaryPath)
                End If
            Catch ex As Exception
            End Try
            SourceRS.MoveNext()
        End While

        SourceRS.Close()
        SourceRS = Nothing
        MarkerRS = Nothing
        MiscRS = Nothing
        MilitaryRS = Nothing
        db6.Close()
        db6 = Nothing

    End Sub

End Module
