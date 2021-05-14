Imports MySql.Data.MySqlClient
Imports System.Management
Imports System
Imports Microsoft.Win32
Public Class Form1
    Dim id As Integer
    Dim cpuInfo As String
    Dim cmd As New MySqlCommand()

    Function Bancheck()
        Dim dr As MySqlDataReader
        Dim baglanti As New MySqlConnection("server=localhost; userid=root; password=; database=samp; charset=utf8")

        Dim q As New SelectQuery("Win32_bios")
        Dim search As New ManagementObjectSearcher(q)
        Dim info As New ManagementObject

        For Each info In search.Get
        Next

        Label2.Text = info("serialnumber").ToString

        baglanti.Open()
        cmd.Connection = baglanti
        cmd.CommandText = "SELECT serial, banned FROM serials WHERE serial= '" & Label2.Text & "' AND banned='1'"
        dr = cmd.ExecuteReader
        If dr.HasRows Then
            MsgBox("VatanRPG'den banlandın !")
            Me.Close()

        End If

        Return baglanti

    End Function

    Function GetHWID()
        Dim mc As New ManagementClass("win32_processor")
        Dim moc As ManagementObjectCollection = mc.GetInstances
        For Each mo As ManagementObject In moc
            If cpuInfo = "" Then
                cpuInfo = mo.Properties("processorID").Value.ToString
                Exit For
            End If
        Next
        Return cpuInfo
    End Function
    Function GetSerial()
         Dim q As New SelectQuery("Win32_bios")
        Dim search As New ManagementObjectSearcher(q)
        Dim info As New ManagementObject

        For Each info In search.Get
        Next
        Return GetSerial()
    End Function
    Function CheckCpuBan()
        Dim dr As MySqlDataReader
        Dim baglanti As New MySqlConnection("server=localhost; userid=root; password=; database=samp; charset=utf8")

        baglanti.Open()
        cmd.Connection = baglanti
        cmd.CommandText = "SELECT cpuid, banned FROM serials WHERE cpuid= '" & GetHWID() & "' AND banned='1'"
        dr = cmd.ExecuteReader
        If dr.HasRows Then
            MsgBox("Banlısınız !")
            Me.Close()
        End If

        Return baglanti

    End Function
    Function CheckBaglanti()
        Dim baglanti As New MySqlConnection("server=localhost; userid=root; password=; database=samp; charset=utf8")

        Try
            baglanti.Open()
            baglanti.Close()
        Catch ex As Exception
            MsgBox("Bir şeyler ters gitti !")
            baglanti.Close()
            Me.Close()
        End Try
        Return baglanti
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckBaglanti()

        CheckCpuBan()
        Bancheck()
        CheckVirtualMachine()
    End Sub
    Function PlayerGirildi()
        Dim baglanti As New MySqlConnection("server=localhost; userid=root; password=; database=samp; charset=utf8")

        Try
            baglanti.Open()
            cmd.Connection = baglanti
            cmd.CommandText = "UPDATE players SET Girildi='1' WHERE Username='" & TextBox1.Text & "'"
            cmd.ExecuteNonQuery()
            baglanti.Close()
            MsgBox("Başarı ile onaylandınız, lütfen oyuna dönün !")
        Catch ex As Exception
            MsgBox("Karakter bulunamadı!")
        End Try
        Return baglanti
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim baglanti As New MySqlConnection("server=localhost; userid=root; password=; database=samp; charset=utf8")
        Dim dr As MySqlDataReader

        Dim q As New SelectQuery("Win32_bios")
        Dim search As New ManagementObjectSearcher(q)
        Dim info As New ManagementObject

        For Each info In search.Get
        Next

        If TextBox1.Text = "" Then
            MsgBox("Karakter Adı boş kalamaz !")
            Return
        End If
        Try
            baglanti.Open()
            cmd.Connection = baglanti
            cmd.CommandText = "SELECT serial FROM serials WHERE serial='" & info("serialnumber").ToString & "'"
            dr = cmd.ExecuteReader
            If dr.HasRows Then
                baglanti.Close()
                baglanti.Open()
                cmd.Connection = baglanti
                cmd.CommandText = "UPDATE serials SET karakter='" & TextBox1.Text & "',serial='" & info("serialnumber").ToString & "',cpuid='" & GetHWID() & "',banned='0' WHERE serial='" & info("serialnumber").ToString & "'"
                cmd.ExecuteNonQuery()
                PlayerGirildi()
                Return
                baglanti.Close()
            End If
        Catch ex As Exception
        End Try
        baglanti.Close()
        baglanti.Open()
        cmd.CommandText = "INSERT INTO serials(serial,cpuid,karakter,banned) VALUES ('" & info("serialnumber").ToString & "', '" & GetHWID() & "', '" & TextBox1.Text & "','0') "
        cmd.ExecuteNonQuery()
        PlayerGirildi()
        baglanti.Close()
        Return

    End Sub



    Public Function CheckVirtualMachine() As Boolean
        Dim result As Boolean = False
        Const MICROSOFTCORPORATION As String = "microsoft corporation"
        Try
            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_BaseBoard")

            For Each queryObj As ManagementObject In searcher.[Get]()
                result = queryObj("Manufacturer").ToString().ToLower() = MICROSOFTCORPORATION.ToLower()
                If result = True Then
                    MsgBox("Bu programı sanal sunucularda kullanamazsınız!")
                    Me.Close()
                End If
            Next
            Return result
        Catch ex As ManagementException
            Return result
        End Try
    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles UpdateCheck.Tick
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("")
        Dim response As System.Net.HttpWebResponse = request.GetResponse()

        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())

        Dim newestversion As String = sr.ReadToEnd()
        Dim currentversion As String = Application.ProductVersion
        If newestversion.Contains(currentversion) Then
            UpdateCheck.Stop()
        Else
            MsgBox("Sürümünüz güncel değil lütfen sürümünüzü vt-gaming.com/onay adresinden güncelleyin !")
            Me.Close()

        End If
        Return
    End Sub
End Class
