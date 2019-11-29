Imports ThoughtWorks.QRCode
Imports ThoughtWorks.QRCode.Codec

Public Class cbb

    Public Sub generarCBB(Emisor As String, receptor As String, total As String, uuid As String, rutaCBB As String)
        Dim generarCodigoQR As QRCodeEncoder = New QRCodeEncoder
        generarCodigoQR.QRCodeEncodeMode = Codec.QRCodeEncoder.ENCODE_MODE.BYTE
        generarCodigoQR.QRCodeScale = Int32.Parse(104)


        generarCodigoQR.QRCodeErrorCorrect = Codec.QRCodeEncoder.ERROR_CORRECTION.Q


        'La versión "8" calcula automáticamente el tamaño
        generarCodigoQR.QRCodeVersion = 8
        Try

            'Con UTF-8 podremos añadir caracteres como ñ, tildes, etc.
            Dim txtTextoQR = "?re=" & Emisor & "&rr=" & receptor & "&tt=" & total & "&id=" & uuid
            generarCodigoQR.Encode(txtTextoQR, System.Text.Encoding.UTF8).Save(rutaCBB + ".jpg")

            Dim btmp As New Bitmap(rutaCBB + ".jpg")
            '' image1.Save(rutaCBB + ".png", System.Drawing.Imaging.ImageFormat.Png)
            btmp.Save(rutaCBB + ".jpg", System.Drawing.Imaging.ImageFormat.Png)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly)
        End Try

    End Sub



    Private Function AdjustQRVersion(ByVal TextEncode As String,
             ByVal qrCodeErrCorrect As _
             Codec.QRCodeEncoder.ERROR_CORRECTION) As Integer
        Dim textb() As Byte =
            System.Text.UTF8Encoding.UTF8.GetBytes(TextEncode)
        Dim ibits As Integer = ((textb.Length + 1) * 8) + 8
        Return -1
        Select Case qrCodeErrCorrect
                'Corrección de errores del 25%
            Case QRCodeEncoder.ERROR_CORRECTION.Q
                Select Case ibits
                    Case Is <= 104
                        Return 1
                    Case Is <= 176
                        Return 2
                    Case Is <= 272
                        Return 3
                    Case Is <= 384
                        Return 4
                    Case Is <= 496
                        Return 5
                    Case Is <= 608
                        Return 6
                    Case Is <= 704
                        Return 7
                    Case Is <= 880
                        Return 8
                    Case Is <= 1056
                        Return 9
                    Case Is <= 1232
                        Return 10
                    Case Is <= 1440
                        Return 11
                    Case Is <= 1648
                        Return 12
                    Case Is <= 1952
                        Return 13
                    Case Is <= 2088
                        Return 14
                    Case Is <= 2360
                        Return 15
                    Case Is <= 2600
                        Return 16
                    Case Is <= 2936
                        Return 17
                    Case Is <= 3176
                        Return 18
                    Case Is <= 3560
                        Return 19
                    Case Is <= 3880
                        Return 20
                    Case Is <= 4096
                        Return 21
                    Case Is <= 4544
                        Return 22
                    Case Is <= 4912
                        Return 23
                    Case Is <= 5312
                        Return 24
                    Case Is <= 5744
                        Return 25
                    Case Is <= 6032
                        Return 26
                    Case Is <= 6464
                        Return 27
                    Case Is <= 6968
                        Return 28
                    Case Is <= 7288
                        Return 29
                    Case Is <= 7880
                        Return 30
                    Case Is <= 8264
                        Return 31
                    Case Is <= 8920
                        Return 32
                    Case Is <= 9368
                        Return 33
                    Case Is <= 9848
                        Return 34
                    Case Is <= 10288
                        Return 35
                    Case Is <= 10832
                        Return 36
                    Case Is <= 11408
                        Return 37
                    Case Is <= 12016
                        Return 38
                    Case Is <= 12656
                        Return 39
                    Case Is <= 13328
                        Return 40
                    Case Else
                        Return -1
                End Select
        End Select
    End Function
End Class