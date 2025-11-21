Imports System.IO
Imports System.Media

Namespace WBKPlayerLib
    Public Class Player

        Private Shared currentWBKBytes As Byte() = Nothing
        Private Shared currentWBKPath As String = ""

        ' Load the WBK file into memory
        Public Shared Sub LoadWBK(path As String)
            If Not File.Exists(path) Then
                Throw New FileNotFoundException("WBK file not found", path)
            End If
            currentWBKPath = path
            currentWBKBytes = File.ReadAllBytes(path)
        End Sub

        ' Play a specific wave by its 3-digit ID
        Public Shared Sub PlayWave(id As String)
            If currentWBKBytes Is Nothing Then
                Throw New InvalidOperationException("WBK file not loaded.")
            End If

            ' Extract WAV data
            Dim wavPath As String = ExtractWavByID(id)
            If String.IsNullOrEmpty(wavPath) Then
                Throw New FileNotFoundException("Wave ID not found: " & id)
            End If

            ' Play the WAV
            My.Computer.Audio.Play(wavPath, AudioPlayMode.Background)
        End Sub

        Private Shared Function ExtractWavByID(id As String) As String
            Dim bank = currentWBKBytes
            Dim a As Integer = AscW(id(0))
            Dim b As Integer = AscW(id(1))
            Dim c As Integer = AscW(id(2))

            Dim i As Integer = 0
            While i < bank.Length - 12
                If bank(i) = &H5 AndAlso bank(i + 1) = &H40 AndAlso bank(i + 3) = &H1 Then
                    Dim sa = bank(i + 4), sb = bank(i + 5), sc = bank(i + 6)
                    If sa = a AndAlso sb = b AndAlso sc = c Then
                        ' find matching end tag
                        Dim j As Integer = i + 10
                        While j < bank.Length - 12
                            If bank(j) = &H5 AndAlso bank(j + 1) = &H40 AndAlso bank(j + 2) = &H95 AndAlso bank(j + 3) = &H1 Then
                                Dim ea = bank(j + 4), eb = bank(j + 5), ec = bank(j + 6)
                                If ea = sa AndAlso eb = sb AndAlso ec = sc Then
                                    Dim wavStart As Integer = i + 10
                                    Dim wavLen As Integer = j - wavStart
                                    Dim tmpPath As String = Path.Combine(Path.GetTempPath(), "wbk_" & id & ".wav")
                                    Dim wavBytes(wavLen - 1) As Byte
                                    Array.Copy(bank, wavStart, wavBytes, 0, wavLen)
                                    File.WriteAllBytes(tmpPath, wavBytes)
                                    Return tmpPath
                                End If
                            End If
                            j += 1
                        End While
                    End If
                End If
                i += 1
            End While

            Return String.Empty
        End Function

    End Class
End Namespace
