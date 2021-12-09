Imports System
Imports Net.Pkcs11Interop.Common
Imports Net.Pkcs11Interop.HighLevelAPI

Module Program
    Dim pkcs11LibraryPath
    Sub Main(args As String())

        Dim AdminPassword = ""
        Dim UserPassword = ""
        Dim UserPIN = ""
        Dim UserPUK = ""
        Dim TokenLabel = ""

        Dim CurrentAdminPassword = ""
        Dim Force As Boolean = False

        For Each arg In args
            If arg = "--Force" Then

                Force = True

            End If

            If arg.StartsWith("Admin:") Then

                AdminPassword = arg.Substring(6)

            End If

            If arg.StartsWith("User:") Then

                UserPassword = arg.Substring(5)

            End If

            If arg.StartsWith("CurrentAdminPassword:") Then

                CurrentAdminPassword = arg.Substring(21)

            End If

            If arg.StartsWith("PIN:") Then

                UserPIN = arg.Substring(4)

            End If

            If arg.StartsWith("PUK:") Then

                UserPUK = arg.Substring(4)

            End If

            If arg.StartsWith("TokenLabel:") Then

                TokenLabel = arg.Substring(11)

            End If

        Next

        If (AdminPassword = "") Then
            Console.WriteLine("Please provide an Admin Password")
            Exit Sub
        End If
        If (UserPassword = "") Then
            Console.WriteLine("Please provide a User Password")
            Exit Sub
        End If
        'If (UserPIN = "") Then
        ' Console.WriteLine("Please provide a User PIN")
        'Exit Sub
        'End If
        'If (UserPUK = "") Then
        ' Console.WriteLine("Please provide a User PUK")
        ' Exit Sub
        ' End If
        If (TokenLabel = "") Then
            Console.WriteLine("Please provide a Token Label")
            Exit Sub
        End If

        Dim pkcs11LibraryPath = SetPKCS11Dll()

        Dim factories As New Pkcs11InteropFactories()

        Try

            Using pkcs11Library As IPkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, pkcs11LibraryPath, AppType.MultiThreaded)

                Dim libraryInfo As ILibraryInfo = pkcs11Library.GetInfo()
                Console.WriteLine("PKCS11 Library Found: " + libraryInfo.ManufacturerId + " - " + libraryInfo.LibraryDescription + " - " + libraryInfo.LibraryVersion)
                For Each slot As ISlot In pkcs11Library.GetSlotList(SlotsType.WithTokenPresent)

                    Dim slotInfo As ISlotInfo = slot.GetSlotInfo()
                    If (slotInfo.SlotFlags.TokenPresent) Then

                        Dim tokenInfo = slot.GetTokenInfo()
                        Console.WriteLine("Token found in slot " + slotInfo.SlotId.ToString + ":")
                        Console.WriteLine(tokenInfo.ManufacturerId + " - " + tokenInfo.Model + " - " + tokenInfo.SerialNumber)

                        If (Force = False) Then

                            Console.Write("Are you sure you want to initialize this token? (Y/N): ")
                            Dim ConfirmResult = Console.ReadKey().KeyChar
                            Console.WriteLine("")
                            If (ConfirmResult.ToString.ToUpper = "Y") Then

                                Initialize(AdminPassword, UserPassword, UserPIN, UserPUK, TokenLabel, CurrentAdminPassword, slot)

                            Else

                                Console.WriteLine("Cancelling Operation")
                                Exit Sub

                            End If

                        Else


                            Initialize(AdminPassword, UserPassword, UserPIN, UserPUK, TokenLabel, CurrentAdminPassword, slot)

                        End If

                    End If

                Next


            End Using

        Catch exUnmanagedException As UnmanagedException

            Console.WriteLine(exUnmanagedException.Message)
            Exit Sub

        End Try





    End Sub

    Private Function SetPKCS11Dll()

        If IO.File.Exists(IO.Path.GetDirectoryName(Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\pkcs11.dll") Then
            Return IO.Path.GetDirectoryName(Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\pkcs11.dll"
        Else
            Return "C:\Windows\System32\eTPKCS11.dll"
        End If

    End Function


    Private Function Initialize(AdminPassword As String, UserPassword As String, UserPIN As String, UserPUK As String, TokenLabel As String, CurrentAdminPassword As String, slot As ISlot)

        If (CurrentAdminPassword = "") Then
            CurrentAdminPassword = "000000000000000000000000000000000000000000000000"
        End If
        slot.InitToken(CurrentAdminPassword, TokenLabel)

        Using session As ISession = slot.OpenSession(SessionType.ReadWrite)

            session.Login(CKU.CKU_SO, CurrentAdminPassword)
            session.SetPin(CurrentAdminPassword, AdminPassword)
            session.InitPin(UserPassword)
            session.Logout()


        End Using

    End Function

End Module
