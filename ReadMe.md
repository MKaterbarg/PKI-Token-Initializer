# PKI Hardware Token Initializer 

The PKI Hardware Token Initalizer is a command line tool to automate the initialization of PKI tokens.

It has been designed for the Safenet / Gemalto eToken 5110 CC PKI token. However under "Confirmed Hardware" there's a list available of all tested and confirmed tokens.  
Feel free to reach out if you want this tested and updated for different token types.

## Requirements

A PKCS11 library is required to use this program. I cannot redistibute the file used on the Safenet tokens, so you will need to add the PKCS11 library DLL for your token type, to the installation directory, as "pkcs11.dll"

e.g. If your have installed the application into C:\Program Files\PKIHardwareTokenInitializer, than you must add that file as C:\Program Files\PKIHardwareTokenInitializer\pkcs11.dll

##### Note: 
If this file cannot be found, the application will also search for the eToken 5110 PKCS11 library DLL in the following location: C:\Windows\System32\eTPKCS11.dll

Due to this, if you have the Safenet Authentication Client installed, you shouldn't have to place any DLL file in the installation directory

## Usage
PKIInit.exe Admin:NewAdminPassword User:NewUserPassword TokenLabel:NewTokenLabel

Optionally, the "--Force" parameter can be added after the NewUserPUK, to skip the confirmation question.
Additionally, if the token has already been initialized, or the default password used is not correct for your token, you may add the "CurrentAdminPassword:" parameter as such: 

CurrentAdminPassword:00000000

In this example, the application will use "00000000" as the current administrator password for the token.

If it is not specified, a string of 48 zero's will be used.

## Inner workings
For those wondering how it actually works, a short description of what the application does:

1. Look for thie library file in the 2 locations mentioned above. Priority is given to the local pkcs11.dll file
2. The PKCS11 library is loaded along with the Pkcs11Interop library. Library Details are printed on screen
3. The program scans every slot that has a token present
4. The details of the token (Manufacturer, type, serial number) are printed on screen
5. Confirmation is asked before the token is initalized (Unless the --Force parameter is supplied)
6. The token is initialized
7. The admin and user passwords set

## Confirmed Hardware

| Token      | Status | Comment |
| ----------- | ----------- |
| Aladdin eToken PRO | Working | Works out of the box if the Safenet Authentication Client is installed. If not, the "eTPKCS11.dll" from Safenet should be added to the PKIInit directory
| Safenet eToken 5110 CC | Working | Works out of the box if the Safenet Authentication Client is installed. If not, the "eTPKCS11.dll" from Safenet should be added to the PKIInit directory
| Safenet eToken 511x FIPS | Working | Works out of the box if the Safenet Authentication Client is installed. If not, the "eTPKCS11.dll" from Safenet should be added to the PKIInit directory
