'This example just plays a wave from a wave bank, fetching it by its ID
Imports System
Imports System.Reflection

Module Program
    Sub Main()
        Try
            Dim asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory & "WAVEBANK.dll")
            Dim type = asm.GetType("WAVEBANK.WBKPlayerLib.Player")
            Dim player = Activator.CreateInstance(type)
            type.GetMethod("LoadWBK").Invoke(player, New Object() {"BANK.wbk"}) 'Replace with the path to the bank
            type.GetMethod("PlayWave").Invoke(player, New Object() {"001"})
        Catch
        End Try

        Console.ReadKey()
    End Sub
End Module
