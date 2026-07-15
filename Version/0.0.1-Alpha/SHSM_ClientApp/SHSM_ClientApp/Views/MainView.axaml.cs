using ASodium;
using Avalonia.Controls;
using Avalonia.Media;
using BCASodium;
using Newtonsoft.Json;
using SHSM_ClientApp.APIMethodHelper;
using SHSM_ClientApp.Helper;
using SHSM_ClientApp.PostDataModel;
using SHSM_ClientApp.SHSMDataModel;
using SimplifiedArweaveSDK.ArweaveHelper;
using SimplifiedArweaveSDK.ArweaveSubHelper;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SHSM_ClientApp.Views;

public partial class MainView : UserControl
{
    private static int IPOpsAppUIChooser = 0;
    private static int RegistrationOpsAppUIChooser = 0;
    private static int ETLSOpsAppUIChooser = 0;
    private static int PublicKeyCryptographyOpsAppUIChooser = 0;
    private static int SecretKeyCryptographyOpsAppUIChooser = 0;
    private static int ArweaveOpsAppUIChooser = 0;
    private static int SHSMOpsAppUIChooser = 0;

    //IPOpsApp
    private static TextBlock[] FirstIPOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstIPOpsAppTextBoxArray = new TextBox[] { };
    private static Button[] FirstIPOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SecondIPOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] SecondIPOpsAppTextBoxArray = new TextBox[] { };
    private static Button[] SecondIPOpsAppButtonArray = new Button[] { };

    //RegistrationApp
    private static TextBlock[] FirstRegistrationOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstRegistrationOpsAppTextBoxArray = new TextBox[] { };
    private static RadioButton[] FirstRegistrationOpsAppRadioButtonArray = new RadioButton[] { };
    private static Button[] FirstRegistrationOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FirstRegistrationOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstRegistrationOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] SecondRegistrationOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] SecondRegistrationOpsAppTextBoxArray = new TextBox[] { };
    private static ComboBox[] SecondRegistrationOpsAppComboBoxArray = new ComboBox[] { };
    private static RadioButton[] SecondRegistrationOpsAppRadioButtonArray = new RadioButton[] { };
    private static Button[] SecondRegistrationOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SecondRegistrationOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] SecondRegistrationOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---

    //ETLSApp
    private static TextBlock[] FirstETLSOpsAppTextBlockArray = new TextBlock[] { };
    private static RadioButton[] FirstETLSOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] FirstETLSOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FirstETLSOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FirstETLSOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstETLSOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] SecondETLSOpsAppTextBlockArray = new TextBlock[] { };
    private static ComboBox[] SecondETLSOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] SecondETLSOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SecondETLSOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] SecondETLSOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---

    //PublicKeyApp
    private static TextBlock[] FirstPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static RadioButton[] FirstPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] FirstPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FirstPublicKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FirstPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] SecondPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static RadioButton[] SecondPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] SecondPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] SecondPublicKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SecondPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] SecondPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] ThirdPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] ThirdPublicKeyOpsAppTextBoxArray = new TextBox[] { };
    private static RadioButton[] ThirdPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] ThirdPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] ThirdPublicKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] ThirdPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] ThirdPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] FourthPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] FourthPublicKeyOpsAppTextBoxArray = new TextBox[] { };
    private static RadioButton[] FourthPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] FourthPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FourthPublicKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FourthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FourthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] FifthPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] FifthPublicKeyOpsAppTextBoxArray = new TextBox[] { };
    private static RadioButton[] FifthPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] FifthPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FifthPublicKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FifthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FifthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] SixthPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static ComboBox[] SixthPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] SixthPublicKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SixthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] SixthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] SeventhPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static RadioButton[] SeventhPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] SeventhPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] SeventhPublicKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SeventhPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] SeventhPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };

    //SecretKeyApp
    private static TextBlock[] FirstSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static ComboBox[] FirstSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FirstSecretKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FirstSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] SecondSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static RadioButton[] SecondSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] SecondSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] SecondSecretKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SecondSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] SecondSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] ThirdSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] ThirdSecretKeyOpsAppTextBoxArray = new TextBox[] { };
    private static RadioButton[] ThirdSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] ThirdSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] ThirdSecretKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] ThirdSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] ThirdSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] FourthSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] FourthSecretKeyOpsAppTextBoxArray = new TextBox[] { };
    private static RadioButton[] FourthSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] FourthSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FourthSecretKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FourthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FourthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] FifthSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static RadioButton[] FifthSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
    private static ComboBox[] FifthSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FifthSecretKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FifthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FifthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
    //---
    private static TextBlock[] SixthSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
    private static ComboBox[] SixthSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] SixthSecretKeyOpsAppButtonArray = new Button[] { };
    private static TextBlock[] SixthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] SixthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };

    //ArweaveApp
    private static TextBlock[] FirstArweaveOpsAppTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstArweaveOpsAppTextBoxArray = new TextBox[] { };
    private static ComboBox[] FirstArweaveOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FirstArweaveOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FirstArweaveOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstArweaveOpsAppDeveloperTextBoxArray = new TextBox[] { };

    //SHSMApp
    private static TextBlock[] FirstSHSMOpsAppTextBlockArray = new TextBlock[] { };
    private static ComboBox[] FirstSHSMOpsAppComboBoxArray = new ComboBox[] { };
    private static Button[] FirstSHSMOpsAppButtonArray = new Button[] { };
    private static TextBlock[] FirstSHSMOpsAppDeveloperTextBlockArray = new TextBlock[] { };
    private static TextBox[] FirstSHSMOpsAppDeveloperTextBoxArray = new TextBox[] { };

    private static String ServerRootFolder = "";
    private static String UsersRootFolder = "";
    private static String ETLSRootFolder = "";
    private static String PKCRootFolder = "";
    private static String SecretKeyRootFolder = "";

    private static Boolean HasIPOpsAppUIRendered = false;
    private static Boolean HasRegistrationOpsAppUIRendered = false;
    private static Boolean HasETLSOpsAppUIRendered = false;
    private static Boolean HasPublicKeyCryptographyOpsAppUIRendered = false;
    private static Boolean HasSecretKeyCryptographyOpsAppUIRendered = false;
    private static Boolean HasArweaveOpsAppUIRendered = false;
    private static Boolean HasSHSMOpsAppUIRendered = false;

    public MainView()
    {
        InitializeComponent();
        IPOpsAppInitialization();
        RegistrationOpsAppInitialization();
        ETLSOpsAppInitialization();
        PublicKeyCryptographyOpsAppInitialization();
        SecretKeyCryptographyOpsAppInitialization();
        ArweaveOpsAppInitialization();
        SHSMOpsAppInitialization();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ServerRootFolder = AppContext.BaseDirectory + "\\ServerIP\\";
            UsersRootFolder = AppContext.BaseDirectory + "\\Users\\";
            ETLSRootFolder = AppContext.BaseDirectory + "\\ETLS\\";
            PKCRootFolder = AppContext.BaseDirectory + "\\PublicKeyCryptography\\";
            SecretKeyRootFolder = AppContext.BaseDirectory + "\\SecretKeyCryptography\\";
        }
        else
        {
            ServerRootFolder = AppContext.BaseDirectory + "/ServerIP/";
            UsersRootFolder = AppContext.BaseDirectory + "/Users/";
            ETLSRootFolder = AppContext.BaseDirectory + "/ETLS/";
            PKCRootFolder = AppContext.BaseDirectory + "/PublicKeyCryptography/";
            SecretKeyRootFolder = AppContext.BaseDirectory + "/SecretKeyCryptography/";
        }
        if (Directory.Exists(ServerRootFolder) == false)
        {
            Directory.CreateDirectory(ServerRootFolder);
        }
        if (Directory.Exists(UsersRootFolder) == false)
        {
            Directory.CreateDirectory(UsersRootFolder);
        }
        if (Directory.Exists(ETLSRootFolder) == false) 
        {
            Directory.CreateDirectory(ETLSRootFolder);
        }
        if (Directory.Exists(PKCRootFolder) == false) 
        {
            Directory.CreateDirectory(PKCRootFolder);
        }
        if (Directory.Exists(SecretKeyRootFolder) == false) 
        {
            Directory.CreateDirectory(SecretKeyRootFolder);
        }
        StartupFunction();
    }

    private void StartupFunction()
    {
        if (Directory.Exists(ServerRootFolder) == true)
        {
            if (File.Exists(ServerRootFolder + "IP.txt") == true)
            {
                EstablishConnectionHelper.CreateLinkWithServer();
                File.WriteAllText(ServerRootFolder + "ServerStatus.txt", EstablishConnectionHelper.ConnectionStatus);
            }
        }
    }

    private void IPOpsAppInitialization() 
    {
        IPOpsAppToggleBTN1.IsCheckedChanged += IPOpsAppToggleBTNSFunction;
        IPOpsAppToggleBTN2.IsCheckedChanged += IPOpsAppToggleBTNSFunction;
    }

    private void RegistrationOpsAppInitialization() 
    {
        RegistrationOpsAppToggleBTN1.IsCheckedChanged += RegistrationOpsAppToggleBTNSFunction;
        RegistrationOpsAppToggleBTN2.IsCheckedChanged += RegistrationOpsAppToggleBTNSFunction;
    }

    private void ETLSOpsAppInitialization() 
    {
        ETLSOpsAppToggleBTN1.IsCheckedChanged += ETLSOpsAppToggleBTNSFunction;
        ETLSOpsAppToggleBTN2.IsCheckedChanged += ETLSOpsAppToggleBTNSFunction;
    }

    private void PublicKeyCryptographyOpsAppInitialization() 
    {
        PublicKeyCryptographyOpsAppToggleBTN1.IsCheckedChanged += PublicKeyCryptographyOpsAppToggleBTNSFunction;
        PublicKeyCryptographyOpsAppToggleBTN2.IsCheckedChanged += PublicKeyCryptographyOpsAppToggleBTNSFunction;
        PublicKeyCryptographyOpsAppToggleBTN3.IsCheckedChanged += PublicKeyCryptographyOpsAppToggleBTNSFunction;
        PublicKeyCryptographyOpsAppToggleBTN4.IsCheckedChanged += PublicKeyCryptographyOpsAppToggleBTNSFunction;
        PublicKeyCryptographyOpsAppToggleBTN5.IsCheckedChanged += PublicKeyCryptographyOpsAppToggleBTNSFunction;
        PublicKeyCryptographyOpsAppToggleBTN6.IsCheckedChanged += PublicKeyCryptographyOpsAppToggleBTNSFunction;
        PublicKeyCryptographyOpsAppToggleBTN7.IsCheckedChanged += PublicKeyCryptographyOpsAppToggleBTNSFunction;
    }

    private void SecretKeyCryptographyOpsAppInitialization()
    {
        SecretKeyCryptographyOpsAppToggleBTN1.IsCheckedChanged += SecretKeyCryptographyOpsAppToggleBTNSFunction;
        SecretKeyCryptographyOpsAppToggleBTN2.IsCheckedChanged += SecretKeyCryptographyOpsAppToggleBTNSFunction;
        SecretKeyCryptographyOpsAppToggleBTN3.IsCheckedChanged += SecretKeyCryptographyOpsAppToggleBTNSFunction;
        SecretKeyCryptographyOpsAppToggleBTN4.IsCheckedChanged += SecretKeyCryptographyOpsAppToggleBTNSFunction;
        SecretKeyCryptographyOpsAppToggleBTN5.IsCheckedChanged += SecretKeyCryptographyOpsAppToggleBTNSFunction;
        SecretKeyCryptographyOpsAppToggleBTN6.IsCheckedChanged += SecretKeyCryptographyOpsAppToggleBTNSFunction;
    }

    private void ArweaveOpsAppInitialization()
    {
        ArweaveOpsAppToggleBTN1.IsCheckedChanged += ArweaveOpsAppToggleBTNSFunction;
    }

    private void SHSMOpsAppInitialization() 
    {
        SHSMOpsAppToggleBTN1.IsCheckedChanged += SHSMOpsAppToggleBTNSFunction;
    }

    private void IPOpsAppToggleBTNSFunction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (IPOpsAppToggleBTN1.IsChecked == true)
        {
            IPOpsAppToggleBTN2.IsChecked = false;
            IPOpsAppUIChooser = 1;
        }
        else if (IPOpsAppToggleBTN2.IsChecked == true)
        {
            IPOpsAppToggleBTN1.IsChecked = false;
            IPOpsAppUIChooser = 2;
        }
        else
        {
            ResetIPOpsAppUI();
        }
        IPOpsAppDrawUI();
    }

    private void RegistrationOpsAppToggleBTNSFunction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (RegistrationOpsAppToggleBTN1.IsChecked == true)
        {
            RegistrationOpsAppToggleBTN2.IsChecked = false;
            RegistrationOpsAppUIChooser = 1;
        }
        else if (RegistrationOpsAppToggleBTN2.IsChecked == true)
        {
            RegistrationOpsAppToggleBTN1.IsChecked = false;
            RegistrationOpsAppUIChooser = 2;
        }
        else
        {
            ResetRegistrationOpsAppUI();
        }
        RegistrationOpsAppDrawUI();
    }

    private void ETLSOpsAppToggleBTNSFunction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (ETLSOpsAppToggleBTN1.IsChecked == true)
        {
            ETLSOpsAppToggleBTN2.IsChecked = false;
            ETLSOpsAppUIChooser = 1;
        }
        else if (ETLSOpsAppToggleBTN2.IsChecked == true)
        {
            ETLSOpsAppToggleBTN1.IsChecked = false;
            ETLSOpsAppUIChooser = 2;
        }
        else
        {
            ResetETLSOpsAppUI();
        }
        ETLSOpsAppDrawUI();
    }

    private void PublicKeyCryptographyOpsAppToggleBTNSFunction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if(PublicKeyCryptographyOpsAppToggleBTN1.IsChecked == true) 
        {
            PublicKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN7.IsChecked = false;
            PublicKeyCryptographyOpsAppUIChooser = 1;
        }
        else if (PublicKeyCryptographyOpsAppToggleBTN2.IsChecked == true)
        {
            PublicKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN7.IsChecked = false;
            PublicKeyCryptographyOpsAppUIChooser = 2;
        }
        else if (PublicKeyCryptographyOpsAppToggleBTN3.IsChecked == true)
        {
            PublicKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN7.IsChecked = false;
            PublicKeyCryptographyOpsAppUIChooser = 3;
        }
        else if (PublicKeyCryptographyOpsAppToggleBTN4.IsChecked == true)
        {
            PublicKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN7.IsChecked = false;
            PublicKeyCryptographyOpsAppUIChooser = 4;
        }
        else if (PublicKeyCryptographyOpsAppToggleBTN5.IsChecked == true)
        {
            PublicKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN7.IsChecked = false;
            PublicKeyCryptographyOpsAppUIChooser = 5;
        }
        else if (PublicKeyCryptographyOpsAppToggleBTN6.IsChecked == true)
        {
            PublicKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN7.IsChecked = false;
            PublicKeyCryptographyOpsAppUIChooser = 6;
        }
        else if (PublicKeyCryptographyOpsAppToggleBTN7.IsChecked == true)
        {
            PublicKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            PublicKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            PublicKeyCryptographyOpsAppUIChooser = 7;
        }
        else 
        {
            ResetPublicKeyCryptographyOpsAppUI();
        }
        PublicKeyCryptographyOpsAppDrawUI();
    }

    private void SecretKeyCryptographyOpsAppToggleBTNSFunction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (SecretKeyCryptographyOpsAppToggleBTN1.IsChecked == true)
        {
            SecretKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            SecretKeyCryptographyOpsAppUIChooser = 1;
        }
        else if (SecretKeyCryptographyOpsAppToggleBTN2.IsChecked == true)
        {
            SecretKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            SecretKeyCryptographyOpsAppUIChooser = 2;
        }
        else if (SecretKeyCryptographyOpsAppToggleBTN3.IsChecked == true)
        {
            SecretKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            SecretKeyCryptographyOpsAppUIChooser = 3;
        }
        else if (SecretKeyCryptographyOpsAppToggleBTN4.IsChecked == true)
        {
            SecretKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            SecretKeyCryptographyOpsAppUIChooser = 4;
        }
        else if (SecretKeyCryptographyOpsAppToggleBTN5.IsChecked == true)
        {
            SecretKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN6.IsChecked = false;
            SecretKeyCryptographyOpsAppUIChooser = 5;
        }
        else if (SecretKeyCryptographyOpsAppToggleBTN6.IsChecked == true)
        {
            SecretKeyCryptographyOpsAppToggleBTN2.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN3.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN4.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN5.IsChecked = false;
            SecretKeyCryptographyOpsAppToggleBTN1.IsChecked = false;
            SecretKeyCryptographyOpsAppUIChooser = 6;
        }
        else
        {
            ResetSecretKeyCryptographyOpsAppUI();
        }
        SecretKeyCryptographyOpsAppDrawUI();
    }

    private void ArweaveOpsAppToggleBTNSFunction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (ArweaveOpsAppToggleBTN1.IsChecked == true)
        {
            ArweaveOpsAppUIChooser = 1;
        }
        else
        {
            ResetArweaveOpsAppUI();
        }
        ArweaveOpsAppDrawUI();
    }

    private void SHSMOpsAppToggleBTNSFunction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (SHSMOpsAppToggleBTN1.IsChecked == true)
        {
            SHSMOpsAppUIChooser = 1;
        }
        else
        {
            ResetSHSMOpsAppUI();
        }
        SHSMOpsAppDrawUI();
    }

    private void IPOpsAppDrawUI() 
    {
        if (IPOpsAppUIChooser == 1)
        {
            if (HasIPOpsAppUIRendered == false)
            {
                FirstIPOpsAppTextBlockArray = new TextBlock[2];
                FirstIPOpsAppTextBlockArray[0] = new TextBlock();
                FirstIPOpsAppTextBlockArray[1] = new TextBlock();
                FirstIPOpsAppTextBlockArray[0].Text = "Server API IP address of SHSM";
                FirstIPOpsAppTextBlockArray[1].Text = "Server IP address status (Read Only/RO)";
                FirstIPOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstIPOpsAppTextBoxArray = new TextBox[2];
                FirstIPOpsAppTextBoxArray[0] = new TextBox();
                FirstIPOpsAppTextBoxArray[1] = new TextBox();
                FirstIPOpsAppTextBoxArray[0].Height = 125;
                FirstIPOpsAppTextBoxArray[0].MaxHeight = 125;
                FirstIPOpsAppTextBoxArray[0].Width = 250;
                FirstIPOpsAppTextBoxArray[0].MaxWidth = 250;
                FirstIPOpsAppTextBoxArray[1].Height = 125;
                FirstIPOpsAppTextBoxArray[1].MaxHeight = 125;
                FirstIPOpsAppTextBoxArray[1].Width = 250;
                FirstIPOpsAppTextBoxArray[1].MaxWidth = 250;
                FirstIPOpsAppTextBoxArray[0].TextWrapping = TextWrapping.Wrap;
                FirstIPOpsAppTextBoxArray[1].TextWrapping = TextWrapping.Wrap;
                FirstIPOpsAppTextBoxArray[1].IsReadOnly = true;
                FirstIPOpsAppTextBoxArray[0].HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                FirstIPOpsAppTextBoxArray[1].HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                FirstIPOpsAppButtonArray = new Button[1];
                FirstIPOpsAppButtonArray[0] = new Button();
                FirstIPOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstIPOpsAppButtonArray[0].Content = "Add and establish a connection";
                FirstIPOpsAppButtonArray[0].Click += IPOpsAppBTN_Click;
                IPOpsAppLowerRightSP.Children.Add(FirstIPOpsAppTextBlockArray[0]);
                IPOpsAppLowerRightSP.Children.Add(FirstIPOpsAppTextBoxArray[0]);
                IPOpsAppLowerRightSP.Children.Add(FirstIPOpsAppTextBlockArray[1]);
                IPOpsAppLowerRightSP.Children.Add(FirstIPOpsAppTextBoxArray[1]);
                IPOpsAppLowerRightSP.Children.Add(FirstIPOpsAppButtonArray[0]);
                HasIPOpsAppUIRendered = true;
            }
        }
        else if (IPOpsAppUIChooser == 2)
        {
            if (HasIPOpsAppUIRendered == false)
            {
                SecondIPOpsAppTextBlockArray = new TextBlock[1];
                SecondIPOpsAppTextBlockArray[0] = new TextBlock();
                SecondIPOpsAppTextBlockArray[0].Text = "Supported Algorithms (Read Only)";
                SecondIPOpsAppTextBoxArray = new TextBox[1];
                SecondIPOpsAppTextBoxArray[0] = new TextBox();
                SecondIPOpsAppTextBoxArray[0].Height = 100;
                SecondIPOpsAppTextBoxArray[0].MaxHeight = 100;
                SecondIPOpsAppTextBoxArray[0].Width = 400;
                SecondIPOpsAppTextBoxArray[0].MaxWidth = 400;
                SecondIPOpsAppTextBoxArray[0].IsReadOnly = true;
                SecondIPOpsAppButtonArray = new Button[1];
                SecondIPOpsAppButtonArray[0] = new Button();
                SecondIPOpsAppButtonArray[0].Content = "Get supported algorithms";
                SecondIPOpsAppButtonArray[0].Click += IPOpsAppBTN_Click;
                SecondIPOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                IPOpsAppLowerRightSP.Children.Add(SecondIPOpsAppTextBlockArray[0]);
                IPOpsAppLowerRightSP.Children.Add(SecondIPOpsAppTextBoxArray[0]);
                IPOpsAppLowerRightSP.Children.Add(SecondIPOpsAppButtonArray[0]);
                HasIPOpsAppUIRendered = true;
            }
        }
        else
        {
            ResetIPOpsAppUI();
        }
    }

    private void RegistrationOpsAppDrawUI() 
    {
        if (RegistrationOpsAppUIChooser == 1)
        {
            if (HasRegistrationOpsAppUIRendered == false)
            {
                //Todo..
                //User_ID?
                //AU Info's Arweave ID?
                //AU Signed Sub DSA Public Key Arweave ID?
                //Create new export key pair? (RB) - Default to no
                //Signed Export Public Key B64 (Read Only)
                //Is KEM or SealedBox (Read Only)
                //Algorithm Type (Read Only) (String)
                //The AU Signed Sub DSA Public Key 
                //please make sure you pre-anchor
                //so you can get arweave ID in advance
                //--------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Decoded AU Info's From Arweave ID
                //Decoded AU Signed Sub DSA Public Key
                //Root/Sudo?
                //Status..
                FirstRegistrationOpsAppTextBlockArray = new TextBlock[10];
                FirstRegistrationOpsAppTextBlockArray[0] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[1] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[2] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[3] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[4] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[5] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[6] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[7] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[8] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[9] = new TextBlock();
                FirstRegistrationOpsAppTextBlockArray[0].Text = "User_ID?";
                FirstRegistrationOpsAppTextBlockArray[1].Text = "AU Info's Arweave ID?";
                FirstRegistrationOpsAppTextBlockArray[2].Text = "AU Signed Sub DSA Public Key Arweave ID?";
                FirstRegistrationOpsAppTextBlockArray[3].Text = "Create new export key pair?";
                FirstRegistrationOpsAppTextBlockArray[4].Text = "Signed Export Public Key B64 (Read Only)";
                FirstRegistrationOpsAppTextBlockArray[5].Text = "Is KEM or SealedBox (Read Only)";
                FirstRegistrationOpsAppTextBlockArray[6].Text = "Algorithm Type (Read Only)";
                FirstRegistrationOpsAppTextBlockArray[7].Text = "The AU Signed Sub DSA Public Key";
                FirstRegistrationOpsAppTextBlockArray[8].Text = "please make sure you pre-anchor";
                FirstRegistrationOpsAppTextBlockArray[9].Text = "so you can get arweave ID in advance";
                FirstRegistrationOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppTextBlockArray[6].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppTextBlockArray[7].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppTextBoxArray = new TextBox[6];
                FirstRegistrationOpsAppTextBoxArray[0] = new TextBox();
                FirstRegistrationOpsAppTextBoxArray[1] = new TextBox();
                FirstRegistrationOpsAppTextBoxArray[2] = new TextBox();
                FirstRegistrationOpsAppTextBoxArray[3] = new TextBox();
                FirstRegistrationOpsAppTextBoxArray[4] = new TextBox();
                FirstRegistrationOpsAppTextBoxArray[5] = new TextBox();
                FirstRegistrationOpsAppTextBoxArray[3].IsReadOnly = true;
                FirstRegistrationOpsAppTextBoxArray[4].IsReadOnly = true;
                FirstRegistrationOpsAppTextBoxArray[5].IsReadOnly = true;
                FirstRegistrationOpsAppTextBoxArray[0].MaxWidth = 350;
                FirstRegistrationOpsAppTextBoxArray[0].MaxHeight = 50;
                FirstRegistrationOpsAppTextBoxArray[1].MaxWidth = 350;
                FirstRegistrationOpsAppTextBoxArray[1].MaxHeight = 50;
                FirstRegistrationOpsAppTextBoxArray[2].MaxWidth = 350;
                FirstRegistrationOpsAppTextBoxArray[2].MaxHeight = 50;
                FirstRegistrationOpsAppTextBoxArray[3].MaxWidth = 350;
                FirstRegistrationOpsAppTextBoxArray[3].MaxHeight = 50;
                FirstRegistrationOpsAppTextBoxArray[4].MaxWidth = 350;
                FirstRegistrationOpsAppTextBoxArray[4].MaxHeight = 50;
                FirstRegistrationOpsAppTextBoxArray[5].MaxWidth = 350;
                FirstRegistrationOpsAppTextBoxArray[5].MaxHeight = 50;
                FirstRegistrationOpsAppTextBoxArray[0].Width = 350;
                FirstRegistrationOpsAppTextBoxArray[0].Height = 50;
                FirstRegistrationOpsAppTextBoxArray[1].Width = 350;
                FirstRegistrationOpsAppTextBoxArray[1].Height = 50;
                FirstRegistrationOpsAppTextBoxArray[2].Width = 350;
                FirstRegistrationOpsAppTextBoxArray[2].Height = 50;
                FirstRegistrationOpsAppTextBoxArray[3].Width = 350;
                FirstRegistrationOpsAppTextBoxArray[3].Height = 50;
                FirstRegistrationOpsAppTextBoxArray[4].Width = 350;
                FirstRegistrationOpsAppTextBoxArray[4].Height = 50;
                FirstRegistrationOpsAppTextBoxArray[5].Width = 350;
                FirstRegistrationOpsAppTextBoxArray[5].Height = 50;
                FirstRegistrationOpsAppRadioButtonArray = new RadioButton[3];
                FirstRegistrationOpsAppRadioButtonArray[0] = new RadioButton();
                FirstRegistrationOpsAppRadioButtonArray[1] = new RadioButton();
                FirstRegistrationOpsAppRadioButtonArray[2] = new RadioButton();
                FirstRegistrationOpsAppRadioButtonArray[0].Content = "No";
                FirstRegistrationOpsAppRadioButtonArray[1].Content = "Yes - KEM";
                FirstRegistrationOpsAppRadioButtonArray[2].Content = "Yes - SealedBox";
                FirstRegistrationOpsAppRadioButtonArray[0].GroupName = "Choices";
                FirstRegistrationOpsAppRadioButtonArray[1].GroupName = "Choices";
                FirstRegistrationOpsAppRadioButtonArray[2].GroupName = "Choices";
                FirstRegistrationOpsAppRadioButtonArray[0].IsChecked = true;
                FirstRegistrationOpsAppButtonArray = new Button[1];
                FirstRegistrationOpsAppButtonArray[0] = new Button();
                FirstRegistrationOpsAppButtonArray[0].Content = "Register SHSM as user";
                FirstRegistrationOpsAppButtonArray[0].Click += RegistrationOpsAppBTN_Click;
                FirstRegistrationOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBlockArray = new TextBlock[8];
                FirstRegistrationOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[6] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[7] = new TextBlock();
                FirstRegistrationOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FirstRegistrationOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FirstRegistrationOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FirstRegistrationOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                FirstRegistrationOpsAppDeveloperTextBlockArray[4].Text = "Decoded AU Info's From Arweave ID";
                FirstRegistrationOpsAppDeveloperTextBlockArray[5].Text = "Decoded AU Signed Sub DSA Public Key";
                FirstRegistrationOpsAppDeveloperTextBlockArray[6].Text = "Root/Sudo?";
                FirstRegistrationOpsAppDeveloperTextBlockArray[7].Text = "Status";
                FirstRegistrationOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBlockArray[6].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBlockArray[7].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstRegistrationOpsAppDeveloperTextBoxArray = new TextBox[8];
                FirstRegistrationOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[6] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[7] = new TextBox();
                FirstRegistrationOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[6].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[7].IsReadOnly = true;
                FirstRegistrationOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[6].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[6].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[7].MaxWidth = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[7].MaxHeight = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[0].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[0].Height = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[1].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[1].Height = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[2].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[2].Height = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[3].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[3].Height = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[4].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[4].Height = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[5].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[5].Height = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[6].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[6].Height = 50;
                FirstRegistrationOpsAppDeveloperTextBoxArray[7].Width = 350;
                FirstRegistrationOpsAppDeveloperTextBoxArray[7].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBoxArray[1]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBoxArray[2]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppRadioButtonArray[2]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[4]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBoxArray[3]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[5]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBoxArray[4]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[6]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBoxArray[5]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppButtonArray[0]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[7]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[8]);
                MidStackPanel.Children.Add(FirstRegistrationOpsAppTextBlockArray[9]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                RegistrationOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[5]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[6]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[6]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBlockArray[7]);
                RightStackPanel.Children.Add(FirstRegistrationOpsAppDeveloperTextBoxArray[7]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                RegistrationOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasRegistrationOpsAppUIRendered = true;
            }
        }
        else if (RegistrationOpsAppUIChooser == 2)
        {
            if (HasRegistrationOpsAppUIRendered == false)
            {
                //Todo..
                //User_ID? (Combobox)
                //Keys Export Algorithm? (RB)
                //KEM/SealedBox Public Key (Read Only)
                //Signed version of it (Read Only)
                //Is KEM or SealedBox (Read Only)
                //Algorithm Type (Read Only)
                //--------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                SecondRegistrationOpsAppTextBlockArray = new TextBlock[6];
                SecondRegistrationOpsAppTextBlockArray[0] = new TextBlock();
                SecondRegistrationOpsAppTextBlockArray[1] = new TextBlock();
                SecondRegistrationOpsAppTextBlockArray[2] = new TextBlock();
                SecondRegistrationOpsAppTextBlockArray[3] = new TextBlock();
                SecondRegistrationOpsAppTextBlockArray[4] = new TextBlock();
                SecondRegistrationOpsAppTextBlockArray[5] = new TextBlock();
                SecondRegistrationOpsAppTextBlockArray[0].Text = "User_ID?";
                SecondRegistrationOpsAppTextBlockArray[1].Text = "Keys Export Algorithm?";
                SecondRegistrationOpsAppTextBlockArray[2].Text = "KEM/SealedBox Public Key";
                SecondRegistrationOpsAppTextBlockArray[3].Text = "Signed version of it";
                SecondRegistrationOpsAppTextBlockArray[4].Text = "Is KEM or SealedBox";
                SecondRegistrationOpsAppTextBlockArray[5].Text = "Algorithm Type";
                SecondRegistrationOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppTextBoxArray = new TextBox[4];
                SecondRegistrationOpsAppTextBoxArray[0] = new TextBox();
                SecondRegistrationOpsAppTextBoxArray[1] = new TextBox();
                SecondRegistrationOpsAppTextBoxArray[2] = new TextBox();
                SecondRegistrationOpsAppTextBoxArray[3] = new TextBox();
                SecondRegistrationOpsAppTextBoxArray[0].IsReadOnly = true;
                SecondRegistrationOpsAppTextBoxArray[1].IsReadOnly = true;
                SecondRegistrationOpsAppTextBoxArray[2].IsReadOnly = true;
                SecondRegistrationOpsAppTextBoxArray[3].IsReadOnly = true;
                SecondRegistrationOpsAppTextBoxArray[0].MaxWidth = 350;
                SecondRegistrationOpsAppTextBoxArray[0].MaxHeight = 50;
                SecondRegistrationOpsAppTextBoxArray[1].MaxWidth = 350;
                SecondRegistrationOpsAppTextBoxArray[1].MaxHeight = 50;
                SecondRegistrationOpsAppTextBoxArray[2].MaxWidth = 350;
                SecondRegistrationOpsAppTextBoxArray[2].MaxHeight = 50;
                SecondRegistrationOpsAppTextBoxArray[3].MaxWidth = 350;
                SecondRegistrationOpsAppTextBoxArray[3].MaxHeight = 50;
                SecondRegistrationOpsAppRadioButtonArray = new RadioButton[2];
                SecondRegistrationOpsAppRadioButtonArray[0] = new RadioButton();
                SecondRegistrationOpsAppRadioButtonArray[1] = new RadioButton();
                SecondRegistrationOpsAppRadioButtonArray[0].Content = "KEM";
                SecondRegistrationOpsAppRadioButtonArray[1].Content = "Sealedbox";
                SecondRegistrationOpsAppRadioButtonArray[0].GroupName = "Algorithms";
                SecondRegistrationOpsAppRadioButtonArray[1].GroupName = "Algorithms";
                SecondRegistrationOpsAppRadioButtonArray[0].IsChecked = true;
                SecondRegistrationOpsAppComboBoxArray = new ComboBox[1];
                SecondRegistrationOpsAppComboBoxArray[0] = new ComboBox();
                RegistrationOpsAppLoadUserIDs();
                SecondRegistrationOpsAppComboBoxArray[0].Width = 350;
                SecondRegistrationOpsAppComboBoxArray[0].MaxWidth = 350;
                SecondRegistrationOpsAppButtonArray = new Button[1];
                SecondRegistrationOpsAppButtonArray[0] = new Button();
                SecondRegistrationOpsAppButtonArray[0].Content = "Update SHSM export key";
                SecondRegistrationOpsAppButtonArray[0].Click += RegistrationOpsAppBTN_Click;
                SecondRegistrationOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppDeveloperTextBlockArray = new TextBlock[6];
                SecondRegistrationOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                SecondRegistrationOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                SecondRegistrationOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                SecondRegistrationOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                SecondRegistrationOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                SecondRegistrationOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                SecondRegistrationOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                SecondRegistrationOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                SecondRegistrationOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                SecondRegistrationOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                SecondRegistrationOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                SecondRegistrationOpsAppDeveloperTextBlockArray[5].Text = "Status";
                SecondRegistrationOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondRegistrationOpsAppDeveloperTextBoxArray = new TextBox[6];
                SecondRegistrationOpsAppDeveloperTextBoxArray[0] = new TextBox();
                SecondRegistrationOpsAppDeveloperTextBoxArray[1] = new TextBox();
                SecondRegistrationOpsAppDeveloperTextBoxArray[2] = new TextBox();
                SecondRegistrationOpsAppDeveloperTextBoxArray[3] = new TextBox();
                SecondRegistrationOpsAppDeveloperTextBoxArray[4] = new TextBox();
                SecondRegistrationOpsAppDeveloperTextBoxArray[5] = new TextBox();
                SecondRegistrationOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                SecondRegistrationOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                SecondRegistrationOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                SecondRegistrationOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                SecondRegistrationOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                SecondRegistrationOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                SecondRegistrationOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[0].Width = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[0].Height = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[1].Width = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[1].Height = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[2].Width = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[2].Height = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[3].Width = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[3].Height = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[4].Width = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[4].Height = 50;
                SecondRegistrationOpsAppDeveloperTextBoxArray[5].Width = 350;
                SecondRegistrationOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBoxArray[1]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBlockArray[4]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBoxArray[2]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBlockArray[5]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppTextBoxArray[3]);
                MidStackPanel.Children.Add(SecondRegistrationOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                RegistrationOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(SecondRegistrationOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                RegistrationOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasRegistrationOpsAppUIRendered = true;
            }
        }
        else
        {
            ResetRegistrationOpsAppUI();
        }
    }

    private void ETLSOpsAppDrawUI() 
    {
        if (ETLSOpsAppUIChooser == 1)
        {
            if (HasETLSOpsAppUIRendered == false)
            {
                //Todo..
                //User_ID? (ComboBox)
                //Client import's key algorithm (RB)
                //If there's a mistake in 
                //choosing an import algorithm, kindly
                //delete it and choose it again
                //--------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //URL query params
                //Root/Sudo?
                //Status..
                FirstETLSOpsAppTextBlockArray = new TextBlock[5];
                FirstETLSOpsAppTextBlockArray[0] = new TextBlock();
                FirstETLSOpsAppTextBlockArray[1] = new TextBlock();
                FirstETLSOpsAppTextBlockArray[2] = new TextBlock();
                FirstETLSOpsAppTextBlockArray[3] = new TextBlock();
                FirstETLSOpsAppTextBlockArray[4] = new TextBlock();
                FirstETLSOpsAppTextBlockArray[0].Text = "User_ID?";
                FirstETLSOpsAppTextBlockArray[1].Text = "Server import's key algorithm";
                FirstETLSOpsAppTextBlockArray[2].Text = "If there's a mistake in";
                FirstETLSOpsAppTextBlockArray[3].Text = "choosing an import algorithm, kindly";
                FirstETLSOpsAppTextBlockArray[4].Text = "delete it and choose it again";
                FirstETLSOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppComboBoxArray = new ComboBox[1];
                FirstETLSOpsAppComboBoxArray[0] = new ComboBox();
                ETLSOpsAppLoadUserIDs();
                FirstETLSOpsAppComboBoxArray[0].MaxWidth = 350;
                FirstETLSOpsAppComboBoxArray[0].Width = 350;
                FirstETLSOpsAppRadioButtonArray = new RadioButton[2];
                FirstETLSOpsAppRadioButtonArray[0] = new RadioButton();
                FirstETLSOpsAppRadioButtonArray[1] = new RadioButton();
                FirstETLSOpsAppRadioButtonArray[0].Content = "KEM (X-Wing)";
                FirstETLSOpsAppRadioButtonArray[1].Content = "Sealedbox (X25519)";
                FirstETLSOpsAppRadioButtonArray[0].GroupName = "Choices";
                FirstETLSOpsAppRadioButtonArray[1].GroupName = "Choices";
                FirstETLSOpsAppRadioButtonArray[0].IsChecked = true;
                FirstETLSOpsAppButtonArray = new Button[1];
                FirstETLSOpsAppButtonArray[0] = new Button();
                FirstETLSOpsAppButtonArray[0].Content = "Initialize ETLS for Import";
                FirstETLSOpsAppButtonArray[0].Click += ETLSOpsAppBTN_Click;
                FirstETLSOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FirstETLSOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FirstETLSOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FirstETLSOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FirstETLSOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FirstETLSOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FirstETLSOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FirstETLSOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FirstETLSOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FirstETLSOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FirstETLSOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                FirstETLSOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FirstETLSOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FirstETLSOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstETLSOpsAppDeveloperTextBoxArray = new TextBox[6];
                FirstETLSOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FirstETLSOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FirstETLSOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FirstETLSOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FirstETLSOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FirstETLSOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FirstETLSOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FirstETLSOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FirstETLSOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FirstETLSOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FirstETLSOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FirstETLSOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FirstETLSOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[0].Width = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[0].Height = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[1].Width = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[1].Height = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[2].Width = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[2].Height = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[3].Width = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[3].Height = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[4].Width = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[4].Height = 50;
                FirstETLSOpsAppDeveloperTextBoxArray[5].Width = 350;
                FirstETLSOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FirstETLSOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FirstETLSOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FirstETLSOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FirstETLSOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(FirstETLSOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(FirstETLSOpsAppButtonArray[0]);
                MidStackPanel.Children.Add(FirstETLSOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(FirstETLSOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(FirstETLSOpsAppTextBlockArray[4]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                ETLSOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FirstETLSOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                ETLSOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasETLSOpsAppUIRendered = true;
            }
        }
        else if (ETLSOpsAppUIChooser == 2)
        {
            if (HasETLSOpsAppUIRendered == false)
            {
                //Todo..
                //User_ID? (ComboBox)
                //--------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //URL query params
                //Root/Sudo?
                //Status..
                SecondETLSOpsAppTextBlockArray = new TextBlock[1];
                SecondETLSOpsAppTextBlockArray[0] = new TextBlock();
                SecondETLSOpsAppTextBlockArray[0].Text = "User_ID?";
                SecondETLSOpsAppComboBoxArray = new ComboBox[1];
                SecondETLSOpsAppComboBoxArray[0] = new ComboBox();
                ETLSOpsAppLoadUserIDs();
                SecondETLSOpsAppComboBoxArray[0].Width = 350;
                SecondETLSOpsAppComboBoxArray[0].MaxWidth = 350;
                SecondETLSOpsAppButtonArray = new Button[1];
                SecondETLSOpsAppButtonArray[0] = new Button();
                SecondETLSOpsAppButtonArray[0].Content = "Delete ETLS";
                SecondETLSOpsAppButtonArray[0].Click += ETLSOpsAppBTN_Click;
                SecondETLSOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondETLSOpsAppDeveloperTextBlockArray = new TextBlock[6];
                SecondETLSOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                SecondETLSOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                SecondETLSOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                SecondETLSOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                SecondETLSOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                SecondETLSOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                SecondETLSOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                SecondETLSOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                SecondETLSOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                SecondETLSOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                SecondETLSOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                SecondETLSOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                SecondETLSOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondETLSOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondETLSOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondETLSOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondETLSOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondETLSOpsAppDeveloperTextBoxArray = new TextBox[6];
                SecondETLSOpsAppDeveloperTextBoxArray[0] = new TextBox();
                SecondETLSOpsAppDeveloperTextBoxArray[1] = new TextBox();
                SecondETLSOpsAppDeveloperTextBoxArray[2] = new TextBox();
                SecondETLSOpsAppDeveloperTextBoxArray[3] = new TextBox();
                SecondETLSOpsAppDeveloperTextBoxArray[4] = new TextBox();
                SecondETLSOpsAppDeveloperTextBoxArray[5] = new TextBox();
                SecondETLSOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                SecondETLSOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                SecondETLSOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                SecondETLSOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                SecondETLSOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                SecondETLSOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                SecondETLSOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[0].Width = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[0].Height = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[1].Width = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[1].Height = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[2].Width = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[2].Height = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[3].Width = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[3].Height = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[4].Width = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[4].Height = 50;
                SecondETLSOpsAppDeveloperTextBoxArray[5].Width = 350;
                SecondETLSOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(SecondETLSOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(SecondETLSOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(SecondETLSOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                ETLSOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(SecondETLSOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                ETLSOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasETLSOpsAppUIRendered = true;
            }
        }
        else
        {
            ResetETLSOpsAppUI();
        }
    }

    private void PublicKeyCryptographyOpsAppDrawUI() 
    {
        if(PublicKeyCryptographyOpsAppUIChooser == 1) 
        {
            if (HasPublicKeyCryptographyOpsAppUIRendered == false) 
            {
                //Todo..
                //User_ID? (ComboBox)
                //DSA Algorithm? (RB)
                //--------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //URL query params
                //Root/Sudo?
                //Status..
                FirstPublicKeyOpsAppTextBlockArray = new TextBlock[2];
                FirstPublicKeyOpsAppTextBlockArray[0] = new TextBlock();
                FirstPublicKeyOpsAppTextBlockArray[1] = new TextBlock();
                FirstPublicKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                FirstPublicKeyOpsAppTextBlockArray[1].Text = "DSA Algorithm?";
                FirstPublicKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstPublicKeyOpsAppComboBoxArray = new ComboBox[1];
                FirstPublicKeyOpsAppComboBoxArray[0] = new ComboBox();
                PublicKeyCryptographyOpsAppLoadUserIDs();
                FirstPublicKeyOpsAppComboBoxArray[0].Width = 350;
                FirstPublicKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                FirstPublicKeyOpsAppRadioButtonArray = new RadioButton[3];
                FirstPublicKeyOpsAppRadioButtonArray[0] = new RadioButton();
                FirstPublicKeyOpsAppRadioButtonArray[1] = new RadioButton();
                FirstPublicKeyOpsAppRadioButtonArray[2] = new RadioButton();
                FirstPublicKeyOpsAppRadioButtonArray[0].Content = "ED25519";
                FirstPublicKeyOpsAppRadioButtonArray[1].Content = "ED448";
                FirstPublicKeyOpsAppRadioButtonArray[2].Content = "RSA";
                FirstPublicKeyOpsAppRadioButtonArray[0].GroupName = "DS Algorithms";
                FirstPublicKeyOpsAppRadioButtonArray[1].GroupName = "DS Algorithms";
                FirstPublicKeyOpsAppRadioButtonArray[2].GroupName = "DS Algorithms";
                FirstPublicKeyOpsAppRadioButtonArray[0].IsChecked = true;
                FirstPublicKeyOpsAppButtonArray = new Button[1];
                FirstPublicKeyOpsAppButtonArray[0] = new Button();
                FirstPublicKeyOpsAppButtonArray[0].Content = "Initialize DSA";
                FirstPublicKeyOpsAppButtonArray[0].Click += PublicKeyCryptographyOpsAppBTN_Click;
                FirstPublicKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FirstPublicKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FirstPublicKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FirstPublicKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FirstPublicKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FirstPublicKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FirstPublicKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FirstPublicKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FirstPublicKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FirstPublicKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FirstPublicKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                FirstPublicKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FirstPublicKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FirstPublicKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstPublicKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstPublicKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstPublicKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstPublicKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                FirstPublicKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FirstPublicKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FirstPublicKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FirstPublicKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FirstPublicKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FirstPublicKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FirstPublicKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FirstPublicKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FirstPublicKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FirstPublicKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(FirstPublicKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(FirstPublicKeyOpsAppRadioButtonArray[2]);
                MidStackPanel.Children.Add(FirstPublicKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FirstPublicKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasPublicKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if(PublicKeyCryptographyOpsAppUIChooser == 2) 
        {
            if (HasPublicKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //Imported Key Type? (RB) - 3 (DSA, Sealedbox, KEM)
                //DS Algorithm? (RB) - 3 (ED25519,ED448,RSA)
                //Symmetric encryption algorithm? (RB) - 2 (XSalsa20Poly1305, XChaCha20Poly1305)
                //Generate required keys? (RB) (Yes,No)
                //-------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                SecondPublicKeyOpsAppTextBlockArray = new TextBlock[5];
                SecondPublicKeyOpsAppTextBlockArray[0] = new TextBlock();
                SecondPublicKeyOpsAppTextBlockArray[1] = new TextBlock();
                SecondPublicKeyOpsAppTextBlockArray[2] = new TextBlock();
                SecondPublicKeyOpsAppTextBlockArray[3] = new TextBlock();
                SecondPublicKeyOpsAppTextBlockArray[4] = new TextBlock();
                SecondPublicKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                SecondPublicKeyOpsAppTextBlockArray[1].Text = "Imported Key Type?";
                SecondPublicKeyOpsAppTextBlockArray[2].Text = "DS Algorithm?";
                SecondPublicKeyOpsAppTextBlockArray[3].Text = "Symmetric encryption algorithm?";
                SecondPublicKeyOpsAppTextBlockArray[4].Text = "Generate required keys?";
                SecondPublicKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppComboBoxArray = new ComboBox[1];
                SecondPublicKeyOpsAppComboBoxArray[0] = new ComboBox();
                PublicKeyCryptographyOpsAppLoadUserIDs();
                SecondPublicKeyOpsAppComboBoxArray[0].Width = 350;
                SecondPublicKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                SecondPublicKeyOpsAppRadioButtonArray = new RadioButton[10];
                SecondPublicKeyOpsAppRadioButtonArray[0] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[1] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[2] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[3] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[4] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[5] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[6] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[7] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[8] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[9] = new RadioButton();
                SecondPublicKeyOpsAppRadioButtonArray[0].Content = "DSA";
                SecondPublicKeyOpsAppRadioButtonArray[1].Content = "SealedBox";
                SecondPublicKeyOpsAppRadioButtonArray[2].Content = "KEM";
                SecondPublicKeyOpsAppRadioButtonArray[3].Content = "ED25519";
                SecondPublicKeyOpsAppRadioButtonArray[4].Content = "ED448";
                SecondPublicKeyOpsAppRadioButtonArray[5].Content = "RSA";
                SecondPublicKeyOpsAppRadioButtonArray[6].Content = "XSalsa20Poly1305";
                SecondPublicKeyOpsAppRadioButtonArray[7].Content = "XChaCha20Poly1305";
                SecondPublicKeyOpsAppRadioButtonArray[8].Content = "Yes";
                SecondPublicKeyOpsAppRadioButtonArray[9].Content = "No";
                SecondPublicKeyOpsAppRadioButtonArray[0].GroupName = "ImportedKeyType";
                SecondPublicKeyOpsAppRadioButtonArray[1].GroupName = "ImportedKeyType";
                SecondPublicKeyOpsAppRadioButtonArray[2].GroupName = "ImportedKeyType";
                SecondPublicKeyOpsAppRadioButtonArray[3].GroupName = "DigitalSignatureAlgorithm";
                SecondPublicKeyOpsAppRadioButtonArray[4].GroupName = "DigitalSignatureAlgorithm";
                SecondPublicKeyOpsAppRadioButtonArray[5].GroupName = "DigitalSignatureAlgorithm";
                SecondPublicKeyOpsAppRadioButtonArray[6].GroupName = "SymmetricEncryptionAlgorithm";
                SecondPublicKeyOpsAppRadioButtonArray[7].GroupName = "SymmetricEncryptionAlgorithm";
                SecondPublicKeyOpsAppRadioButtonArray[8].GroupName = "GenerationOpt";
                SecondPublicKeyOpsAppRadioButtonArray[9].GroupName = "GenerationOpt";
                SecondPublicKeyOpsAppRadioButtonArray[0].IsChecked = true;
                SecondPublicKeyOpsAppRadioButtonArray[3].IsChecked = true;
                SecondPublicKeyOpsAppRadioButtonArray[6].IsChecked = true;
                SecondPublicKeyOpsAppRadioButtonArray[8].IsChecked = true;
                SecondPublicKeyOpsAppRadioButtonArray[0].Tag = 0;
                SecondPublicKeyOpsAppRadioButtonArray[1].Tag = 1;
                SecondPublicKeyOpsAppRadioButtonArray[2].Tag = 2;
                SecondPublicKeyOpsAppRadioButtonArray[3].Tag = 0;
                SecondPublicKeyOpsAppRadioButtonArray[4].Tag = 1;
                SecondPublicKeyOpsAppRadioButtonArray[5].Tag = 2;
                SecondPublicKeyOpsAppRadioButtonArray[6].Tag = 0;
                SecondPublicKeyOpsAppRadioButtonArray[7].Tag = 1;
                SecondPublicKeyOpsAppRadioButtonArray[2].IsEnabled = false;
                SecondPublicKeyOpsAppButtonArray = new Button[1];
                SecondPublicKeyOpsAppButtonArray[0] = new Button();
                SecondPublicKeyOpsAppButtonArray[0].Content = "Import Key";
                SecondPublicKeyOpsAppButtonArray[0].Click += PublicKeyCryptographyOpsAppBTN_Click;
                SecondPublicKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                SecondPublicKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                SecondPublicKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                SecondPublicKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                SecondPublicKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                SecondPublicKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                SecondPublicKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                SecondPublicKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                SecondPublicKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                SecondPublicKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                SecondPublicKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                SecondPublicKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                SecondPublicKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                SecondPublicKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                SecondPublicKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                SecondPublicKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                SecondPublicKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                SecondPublicKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                SecondPublicKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                SecondPublicKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[2]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[3]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[4]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[5]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[6]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[7]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppTextBlockArray[4]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[8]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppRadioButtonArray[9]);
                MidStackPanel.Children.Add(SecondPublicKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(SecondPublicKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasPublicKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 3)
        {
            if (HasPublicKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //DS Algorithm? (RB) - 3 (ED25519,ED448,RSA)
                //Data to be signed? (Textbox)
                //Data Type? -2 (RB) (Unicode - UTF8, Base64)
                //-------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                ThirdPublicKeyOpsAppTextBlockArray = new TextBlock[4];
                ThirdPublicKeyOpsAppTextBlockArray[0] = new TextBlock();
                ThirdPublicKeyOpsAppTextBlockArray[1] = new TextBlock();
                ThirdPublicKeyOpsAppTextBlockArray[2] = new TextBlock();
                ThirdPublicKeyOpsAppTextBlockArray[3] = new TextBlock();
                ThirdPublicKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                ThirdPublicKeyOpsAppTextBlockArray[1].Text = "DS Algorithm?";
                ThirdPublicKeyOpsAppTextBlockArray[2].Text = "Data to be signed?";
                ThirdPublicKeyOpsAppTextBlockArray[3].Text = "Data Type?";
                ThirdPublicKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppComboBoxArray = new ComboBox[1];
                ThirdPublicKeyOpsAppComboBoxArray[0] = new ComboBox();
                PublicKeyCryptographyOpsAppLoadUserIDs();
                ThirdPublicKeyOpsAppComboBoxArray[0].Width = 350;
                ThirdPublicKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                ThirdPublicKeyOpsAppTextBoxArray = new TextBox[1];
                ThirdPublicKeyOpsAppTextBoxArray[0] = new TextBox();
                ThirdPublicKeyOpsAppTextBoxArray[0].MaxWidth = 350;
                ThirdPublicKeyOpsAppTextBoxArray[0].MaxHeight = 50;
                ThirdPublicKeyOpsAppTextBoxArray[0].Width = 350;
                ThirdPublicKeyOpsAppTextBoxArray[0].Height = 50;
                ThirdPublicKeyOpsAppRadioButtonArray = new RadioButton[5];
                ThirdPublicKeyOpsAppRadioButtonArray[0] = new RadioButton();
                ThirdPublicKeyOpsAppRadioButtonArray[1] = new RadioButton();
                ThirdPublicKeyOpsAppRadioButtonArray[2] = new RadioButton();
                ThirdPublicKeyOpsAppRadioButtonArray[3] = new RadioButton();
                ThirdPublicKeyOpsAppRadioButtonArray[4] = new RadioButton();
                ThirdPublicKeyOpsAppRadioButtonArray[0].Content = "ED25519";
                ThirdPublicKeyOpsAppRadioButtonArray[1].Content = "ED448";
                ThirdPublicKeyOpsAppRadioButtonArray[2].Content = "RSA";
                ThirdPublicKeyOpsAppRadioButtonArray[3].Content = "Unicode - UTF8";
                ThirdPublicKeyOpsAppRadioButtonArray[4].Content = "Base64";
                ThirdPublicKeyOpsAppRadioButtonArray[0].GroupName = "DigitalSignatureAlgorithm";
                ThirdPublicKeyOpsAppRadioButtonArray[1].GroupName = "DigitalSignatureAlgorithm";
                ThirdPublicKeyOpsAppRadioButtonArray[2].GroupName = "DigitalSignatureAlgorithm";
                ThirdPublicKeyOpsAppRadioButtonArray[3].GroupName = "DataType";
                ThirdPublicKeyOpsAppRadioButtonArray[4].GroupName = "DataType";
                ThirdPublicKeyOpsAppRadioButtonArray[0].IsChecked = true;
                ThirdPublicKeyOpsAppRadioButtonArray[3].IsChecked = true;
                ThirdPublicKeyOpsAppRadioButtonArray[2].IsEnabled = false;
                ThirdPublicKeyOpsAppButtonArray = new Button[1];
                ThirdPublicKeyOpsAppButtonArray[0] = new Button();
                ThirdPublicKeyOpsAppButtonArray[0].Content = "SHSM Sign Data";
                ThirdPublicKeyOpsAppButtonArray[0].Click += PublicKeyCryptographyOpsAppBTN_Click;
                ThirdPublicKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppRadioButtonArray[2]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppRadioButtonArray[3]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppRadioButtonArray[4]);
                MidStackPanel.Children.Add(ThirdPublicKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(ThirdPublicKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasPublicKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 4)
        {
            if (HasPublicKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (Combobox)
                //Action? (Combobox) - (SHSM Sealedbox decrypt, local sealedbox encrypt)
                //Data to be encrypted?
                //Encrypted Data (Base64)?
                //Symmetric encryption algorithm? (RB) - (XSalsa20Poly1305, XChaCha20Poly1305)
                //----
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                //.. Similar UI for the next condition..
                FourthPublicKeyOpsAppTextBlockArray = new TextBlock[5];
                FourthPublicKeyOpsAppTextBlockArray[0] = new TextBlock();
                FourthPublicKeyOpsAppTextBlockArray[1] = new TextBlock();
                FourthPublicKeyOpsAppTextBlockArray[2] = new TextBlock();
                FourthPublicKeyOpsAppTextBlockArray[3] = new TextBlock();
                FourthPublicKeyOpsAppTextBlockArray[4] = new TextBlock();
                FourthPublicKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                FourthPublicKeyOpsAppTextBlockArray[1].Text = "Action?";
                FourthPublicKeyOpsAppTextBlockArray[2].Text = "Data to be encrypted?";
                FourthPublicKeyOpsAppTextBlockArray[3].Text = "Encrypted Data (Base64)?";
                FourthPublicKeyOpsAppTextBlockArray[4].Text = "Symmetric encryption algorithm?";
                FourthPublicKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppComboBoxArray = new ComboBox[2];
                FourthPublicKeyOpsAppComboBoxArray[0] = new ComboBox();
                FourthPublicKeyOpsAppComboBoxArray[1] = new ComboBox();
                PublicKeyCryptographyOpsAppLoadUserIDs();
                FourthPublicKeyOpsAppComboBoxArray[0].Width = 350;
                FourthPublicKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                FourthPublicKeyOpsAppComboBoxArray[1].Width = 350;
                FourthPublicKeyOpsAppComboBoxArray[1].MaxWidth = 350;
                FourthPublicKeyOpsAppComboBoxArray[1].Items.Add("SHSM Sealedbox decrypt");
                FourthPublicKeyOpsAppComboBoxArray[1].Items.Add("Local sealedbox encrypt");
                FourthPublicKeyOpsAppTextBoxArray = new TextBox[2];
                FourthPublicKeyOpsAppTextBoxArray[0] = new TextBox();
                FourthPublicKeyOpsAppTextBoxArray[1] = new TextBox();
                FourthPublicKeyOpsAppTextBoxArray[0].MaxWidth = 350;
                FourthPublicKeyOpsAppTextBoxArray[0].MaxHeight = 50;
                FourthPublicKeyOpsAppTextBoxArray[1].MaxWidth = 350;
                FourthPublicKeyOpsAppTextBoxArray[1].MaxHeight = 50;
                FourthPublicKeyOpsAppTextBoxArray[0].Width = 350;
                FourthPublicKeyOpsAppTextBoxArray[0].Height = 50;
                FourthPublicKeyOpsAppTextBoxArray[1].Width = 350;
                FourthPublicKeyOpsAppTextBoxArray[1].Height = 50;
                FourthPublicKeyOpsAppRadioButtonArray = new RadioButton[2];
                FourthPublicKeyOpsAppRadioButtonArray[0] = new RadioButton();
                FourthPublicKeyOpsAppRadioButtonArray[1] = new RadioButton();
                FourthPublicKeyOpsAppRadioButtonArray[0].Content = "XSalsa20Poly1305";
                FourthPublicKeyOpsAppRadioButtonArray[1].Content = "XChaCha20Poly1305";
                FourthPublicKeyOpsAppRadioButtonArray[0].GroupName = "SymmetricEncryptionAlgorithms";
                FourthPublicKeyOpsAppRadioButtonArray[1].GroupName = "SymmetricEncryptionAlgorithms";
                FourthPublicKeyOpsAppRadioButtonArray[0].IsChecked = true;
                FourthPublicKeyOpsAppButtonArray = new Button[1];
                FourthPublicKeyOpsAppButtonArray[0] = new Button();
                FourthPublicKeyOpsAppButtonArray[0].Content = "SHSM SealedBox Decrypt";
                FourthPublicKeyOpsAppButtonArray[0].Click += PublicKeyCryptographyOpsAppBTN_Click;
                FourthPublicKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FourthPublicKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FourthPublicKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FourthPublicKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FourthPublicKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FourthPublicKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FourthPublicKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FourthPublicKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FourthPublicKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FourthPublicKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FourthPublicKeyOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                FourthPublicKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FourthPublicKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FourthPublicKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                FourthPublicKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FourthPublicKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FourthPublicKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FourthPublicKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FourthPublicKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FourthPublicKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FourthPublicKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppComboBoxArray[1]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppTextBoxArray[1]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppTextBlockArray[4]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(FourthPublicKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FourthPublicKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasPublicKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 5)
        {
            if (HasPublicKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (Combobox)
                //Action? (Combobox) - (SHSM KEM decrypt, local KEM encrypt)
                //Data to be encrypted?
                //Encrypted Data (Base64)?
                //Symmetric encryption algorithm? (RB) - (XSalsa20Poly1305, XChaCha20Poly1305)
                //----
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                FifthPublicKeyOpsAppTextBlockArray = new TextBlock[5];
                FifthPublicKeyOpsAppTextBlockArray[0] = new TextBlock();
                FifthPublicKeyOpsAppTextBlockArray[1] = new TextBlock();
                FifthPublicKeyOpsAppTextBlockArray[2] = new TextBlock();
                FifthPublicKeyOpsAppTextBlockArray[3] = new TextBlock();
                FifthPublicKeyOpsAppTextBlockArray[4] = new TextBlock();
                FifthPublicKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                FifthPublicKeyOpsAppTextBlockArray[1].Text = "Action?";
                FifthPublicKeyOpsAppTextBlockArray[2].Text = "Data to be encrypted?";
                FifthPublicKeyOpsAppTextBlockArray[3].Text = "Encrypted Data (Base64)?";
                FifthPublicKeyOpsAppTextBlockArray[4].Text = "Symmetric encryption algorithm?";
                FifthPublicKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppComboBoxArray = new ComboBox[2];
                FifthPublicKeyOpsAppComboBoxArray[0] = new ComboBox();
                FifthPublicKeyOpsAppComboBoxArray[1] = new ComboBox();
                PublicKeyCryptographyOpsAppLoadUserIDs();
                FifthPublicKeyOpsAppComboBoxArray[0].Width = 350;
                FifthPublicKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                FifthPublicKeyOpsAppComboBoxArray[1].Width = 350;
                FifthPublicKeyOpsAppComboBoxArray[1].MaxWidth = 350;
                FifthPublicKeyOpsAppComboBoxArray[1].Items.Add("SHSM KEM decrypt");
                FifthPublicKeyOpsAppComboBoxArray[1].Items.Add("Local KEM encrypt");
                FifthPublicKeyOpsAppTextBoxArray = new TextBox[2];
                FifthPublicKeyOpsAppTextBoxArray[0] = new TextBox();
                FifthPublicKeyOpsAppTextBoxArray[1] = new TextBox();
                FifthPublicKeyOpsAppTextBoxArray[0].MaxWidth = 350;
                FifthPublicKeyOpsAppTextBoxArray[0].MaxHeight = 50;
                FifthPublicKeyOpsAppTextBoxArray[1].MaxWidth = 350;
                FifthPublicKeyOpsAppTextBoxArray[1].MaxHeight = 50;
                FifthPublicKeyOpsAppTextBoxArray[0].Width = 350;
                FifthPublicKeyOpsAppTextBoxArray[0].Height = 50;
                FifthPublicKeyOpsAppTextBoxArray[1].Width = 350;
                FifthPublicKeyOpsAppTextBoxArray[1].Height = 50;
                FifthPublicKeyOpsAppRadioButtonArray = new RadioButton[2];
                FifthPublicKeyOpsAppRadioButtonArray[0] = new RadioButton();
                FifthPublicKeyOpsAppRadioButtonArray[1] = new RadioButton();
                FifthPublicKeyOpsAppRadioButtonArray[0].Content = "XSalsa20Poly1305";
                FifthPublicKeyOpsAppRadioButtonArray[1].Content = "XChaCha20Poly1305";
                FifthPublicKeyOpsAppRadioButtonArray[0].GroupName = "SymmetricEncryptionAlgorithms";
                FifthPublicKeyOpsAppRadioButtonArray[1].GroupName = "SymmetricEncryptionAlgorithms";
                FifthPublicKeyOpsAppRadioButtonArray[0].IsChecked = true;
                FifthPublicKeyOpsAppButtonArray = new Button[1];
                FifthPublicKeyOpsAppButtonArray[0] = new Button();
                FifthPublicKeyOpsAppButtonArray[0].Content = "SHSM KEM Decrypt";
                FifthPublicKeyOpsAppButtonArray[0].Click += PublicKeyCryptographyOpsAppBTN_Click;
                FifthPublicKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FifthPublicKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FifthPublicKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FifthPublicKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FifthPublicKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FifthPublicKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FifthPublicKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FifthPublicKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FifthPublicKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FifthPublicKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FifthPublicKeyOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                FifthPublicKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FifthPublicKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FifthPublicKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                FifthPublicKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FifthPublicKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FifthPublicKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FifthPublicKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FifthPublicKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FifthPublicKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FifthPublicKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppComboBoxArray[1]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppTextBoxArray[1]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppTextBlockArray[4]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(FifthPublicKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FifthPublicKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasPublicKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 6)
        {
            if (HasPublicKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Query Params
                //Root/Sudo?
                //Status..
                SixthPublicKeyOpsAppTextBlockArray = new TextBlock[1];
                SixthPublicKeyOpsAppTextBlockArray[0] = new TextBlock();
                SixthPublicKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                SixthPublicKeyOpsAppComboBoxArray = new ComboBox[1];
                SixthPublicKeyOpsAppComboBoxArray[0] = new ComboBox();
                PublicKeyCryptographyOpsAppLoadUserIDs();
                SixthPublicKeyOpsAppComboBoxArray[0].Width = 350;
                SixthPublicKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                SixthPublicKeyOpsAppButtonArray = new Button[1];
                SixthPublicKeyOpsAppButtonArray[0] = new Button();
                SixthPublicKeyOpsAppButtonArray[0].Content = "SHSM Extend Duration";
                SixthPublicKeyOpsAppButtonArray[0].Click += PublicKeyCryptographyOpsAppBTN_Click;
                SixthPublicKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                SixthPublicKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                SixthPublicKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                SixthPublicKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                SixthPublicKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                SixthPublicKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                SixthPublicKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                SixthPublicKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                SixthPublicKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                SixthPublicKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                SixthPublicKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                SixthPublicKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                SixthPublicKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                SixthPublicKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthPublicKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthPublicKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthPublicKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthPublicKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                SixthPublicKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                SixthPublicKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                SixthPublicKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                SixthPublicKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                SixthPublicKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                SixthPublicKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(SixthPublicKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(SixthPublicKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(SixthPublicKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(SixthPublicKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasPublicKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 7)
        {
            if (HasPublicKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //DS Algorithm? (RB) - (ED25519, ED448, RSA)
                //Symmetric Encryption Algorithm? (RB) - (XSalsa20Poly1305,XChaCha20Poly1305)
                //------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                SeventhPublicKeyOpsAppTextBlockArray = new TextBlock[3];
                SeventhPublicKeyOpsAppTextBlockArray[0] = new TextBlock();
                SeventhPublicKeyOpsAppTextBlockArray[1] = new TextBlock();
                SeventhPublicKeyOpsAppTextBlockArray[2] = new TextBlock();
                SeventhPublicKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                SeventhPublicKeyOpsAppTextBlockArray[1].Text = "DS Algorithm?";
                SeventhPublicKeyOpsAppTextBlockArray[2].Text = "Symmetric Encryption Algorithm?";
                SeventhPublicKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppComboBoxArray = new ComboBox[1];
                SeventhPublicKeyOpsAppComboBoxArray[0] = new ComboBox();
                PublicKeyCryptographyOpsAppLoadUserIDs();
                SeventhPublicKeyOpsAppComboBoxArray[0].Width = 350;
                SeventhPublicKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                SeventhPublicKeyOpsAppRadioButtonArray = new RadioButton[5];
                SeventhPublicKeyOpsAppRadioButtonArray[0] = new RadioButton();
                SeventhPublicKeyOpsAppRadioButtonArray[1] = new RadioButton();
                SeventhPublicKeyOpsAppRadioButtonArray[2] = new RadioButton();
                SeventhPublicKeyOpsAppRadioButtonArray[3] = new RadioButton();
                SeventhPublicKeyOpsAppRadioButtonArray[4] = new RadioButton();
                SeventhPublicKeyOpsAppRadioButtonArray[0].Content = "ED25519";
                SeventhPublicKeyOpsAppRadioButtonArray[1].Content = "ED448";
                SeventhPublicKeyOpsAppRadioButtonArray[2].Content = "RSA";
                SeventhPublicKeyOpsAppRadioButtonArray[3].Content = "XSalsa20Poly1305";
                SeventhPublicKeyOpsAppRadioButtonArray[4].Content = "XChaCha20Poly1305";
                SeventhPublicKeyOpsAppRadioButtonArray[0].GroupName = "DigitalSignatureAlgorithm";
                SeventhPublicKeyOpsAppRadioButtonArray[1].GroupName = "DigitalSignatureAlgorithm";
                SeventhPublicKeyOpsAppRadioButtonArray[2].GroupName = "DigitalSignatureAlgorithm";
                SeventhPublicKeyOpsAppRadioButtonArray[3].GroupName = "SymmetricEncryptionAlgorithm";
                SeventhPublicKeyOpsAppRadioButtonArray[4].GroupName = "SymmetricEncryptionAlgorithm";
                SeventhPublicKeyOpsAppRadioButtonArray[0].IsChecked = true;
                SeventhPublicKeyOpsAppRadioButtonArray[3].IsChecked = true;
                SeventhPublicKeyOpsAppButtonArray = new Button[1];
                SeventhPublicKeyOpsAppButtonArray[0] = new Button();
                SeventhPublicKeyOpsAppButtonArray[0].Content = "SHSM Export DSA SK";
                SeventhPublicKeyOpsAppButtonArray[0].Click += PublicKeyCryptographyOpsAppBTN_Click;
                SeventhPublicKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SeventhPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppRadioButtonArray[2]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppRadioButtonArray[3]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppRadioButtonArray[4]);
                MidStackPanel.Children.Add(SeventhPublicKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(SeventhPublicKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                PublicKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasPublicKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else 
        {
            ResetPublicKeyCryptographyOpsAppUI();
        }
    }

    private void SecretKeyCryptographyOpsAppDrawUI() 
    {
        if (SecretKeyCryptographyOpsAppUIChooser == 1)
        {
            if (HasSecretKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //--------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //URL query params
                //Root/Sudo?
                //Status..
                FirstSecretKeyOpsAppTextBlockArray = new TextBlock[1];
                FirstSecretKeyOpsAppTextBlockArray[0] = new TextBlock();
                FirstSecretKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                FirstSecretKeyOpsAppComboBoxArray = new ComboBox[1];
                FirstSecretKeyOpsAppComboBoxArray[0] = new ComboBox();
                SecretKeyCryptographyOpsAppLoadUserIDs();
                FirstSecretKeyOpsAppComboBoxArray[0].Width = 350;
                FirstSecretKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                FirstSecretKeyOpsAppButtonArray = new Button[1];
                FirstSecretKeyOpsAppButtonArray[0] = new Button();
                FirstSecretKeyOpsAppButtonArray[0].Content = "Initialize Secret Keys";
                FirstSecretKeyOpsAppButtonArray[0].Click += SecretKeyCryptographyOpsAppBTN_Click;
                FirstSecretKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FirstSecretKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FirstSecretKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FirstSecretKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FirstSecretKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FirstSecretKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FirstSecretKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FirstSecretKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FirstSecretKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FirstSecretKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FirstSecretKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                FirstSecretKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FirstSecretKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FirstSecretKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSecretKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSecretKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSecretKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSecretKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                FirstSecretKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FirstSecretKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FirstSecretKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FirstSecretKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FirstSecretKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FirstSecretKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FirstSecretKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FirstSecretKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FirstSecretKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FirstSecretKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasSecretKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 2)
        {
            if (HasSecretKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //Symmetric encryption algorithm? (RB) - 2 (XSalsa20Poly1305, XChaCha20Poly1305)
                //Generate required keys? (RB) (Yes,No)
                //-------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                SecondSecretKeyOpsAppTextBlockArray = new TextBlock[3];
                SecondSecretKeyOpsAppTextBlockArray[0] = new TextBlock();
                SecondSecretKeyOpsAppTextBlockArray[1] = new TextBlock();
                SecondSecretKeyOpsAppTextBlockArray[2] = new TextBlock();
                SecondSecretKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                SecondSecretKeyOpsAppTextBlockArray[1].Text = "Symmetric encryption algorithm?";
                SecondSecretKeyOpsAppTextBlockArray[2].Text = "Generate required keys?";
                SecondSecretKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppComboBoxArray = new ComboBox[1];
                SecondSecretKeyOpsAppComboBoxArray[0] = new ComboBox();
                SecretKeyCryptographyOpsAppLoadUserIDs();
                SecondSecretKeyOpsAppComboBoxArray[0].Width = 350;
                SecondSecretKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                SecondSecretKeyOpsAppRadioButtonArray = new RadioButton[4];
                SecondSecretKeyOpsAppRadioButtonArray[0] = new RadioButton();
                SecondSecretKeyOpsAppRadioButtonArray[1] = new RadioButton();
                SecondSecretKeyOpsAppRadioButtonArray[2] = new RadioButton();
                SecondSecretKeyOpsAppRadioButtonArray[3] = new RadioButton();
                SecondSecretKeyOpsAppRadioButtonArray[0].Content = "XSalsa20Poly1305";
                SecondSecretKeyOpsAppRadioButtonArray[1].Content = "XChaCha20Poly1305";
                SecondSecretKeyOpsAppRadioButtonArray[2].Content = "Yes";
                SecondSecretKeyOpsAppRadioButtonArray[3].Content = "No";
                SecondSecretKeyOpsAppRadioButtonArray[0].GroupName = "SymmetricEncryptionAlgorithm";
                SecondSecretKeyOpsAppRadioButtonArray[1].GroupName = "SymmetricEncryptionAlgorithm";
                SecondSecretKeyOpsAppRadioButtonArray[2].GroupName = "GenerationOpt";
                SecondSecretKeyOpsAppRadioButtonArray[3].GroupName = "GenerationOpt";
                SecondSecretKeyOpsAppRadioButtonArray[0].IsChecked = true;
                SecondSecretKeyOpsAppRadioButtonArray[2].IsChecked = true;
                SecondSecretKeyOpsAppButtonArray = new Button[1];
                SecondSecretKeyOpsAppButtonArray[0] = new Button();
                SecondSecretKeyOpsAppButtonArray[0].Content = "Import Secret Keys";
                SecondSecretKeyOpsAppButtonArray[0].Click += SecretKeyCryptographyOpsAppBTN_Click;
                SecondSecretKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                SecondSecretKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                SecondSecretKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                SecondSecretKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                SecondSecretKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                SecondSecretKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                SecondSecretKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                SecondSecretKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                SecondSecretKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                SecondSecretKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                SecondSecretKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                SecondSecretKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                SecondSecretKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                SecondSecretKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SecondSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                SecondSecretKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                SecondSecretKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                SecondSecretKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                SecondSecretKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                SecondSecretKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                SecondSecretKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppRadioButtonArray[2]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppRadioButtonArray[3]);
                MidStackPanel.Children.Add(SecondSecretKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(SecondSecretKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasSecretKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 3)
        {
            if (HasSecretKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (Combobox)
                //Data to be encrypted?
                //Additional Data?
                //Data Type? (RB) - (Unicode - UTF8, Base64)
                //AES Algorithm? (RB) - (No,AES256GCM,AEGIS256,AEGIS128L) - Use Tag
                //AEAD Algorithm? (RB) - (No,XChaCha20Poly1305IETF,ChaCha20Poly1305IETF,ChaCha20Poly1305) - Use Tag
                //Stream Cipher Algorithm? (RB) - (No,XChaCha20,XSalsa20,ChaCha20,ChaCha20IETF,Salsa20,Salsa12,Salsa8) - Use Tag
                //MAC Algorithm? (RB) - (No,HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305) - Use Tag
                //----
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                //.. Similar UI for the next condition..
                ThirdSecretKeyOpsAppTextBlockArray = new TextBlock[8];
                ThirdSecretKeyOpsAppTextBlockArray[0] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[1] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[2] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[3] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[4] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[5] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[6] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[7] = new TextBlock();
                ThirdSecretKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                ThirdSecretKeyOpsAppTextBlockArray[1].Text = "Data to be encrypted?";
                ThirdSecretKeyOpsAppTextBlockArray[2].Text = "Additional Data?";
                ThirdSecretKeyOpsAppTextBlockArray[3].Text = "Data Type?";
                ThirdSecretKeyOpsAppTextBlockArray[4].Text = "AES Algorithm?";
                ThirdSecretKeyOpsAppTextBlockArray[5].Text = "AEAD Algorithm?";
                ThirdSecretKeyOpsAppTextBlockArray[6].Text = "Stream Cipher Algorithm?";
                ThirdSecretKeyOpsAppTextBlockArray[7].Text = "MAC Algorithm?";
                ThirdSecretKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppTextBlockArray[6].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppTextBlockArray[7].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppComboBoxArray = new ComboBox[1];
                ThirdSecretKeyOpsAppComboBoxArray[0] = new ComboBox();
                SecretKeyCryptographyOpsAppLoadUserIDs();
                ThirdSecretKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                ThirdSecretKeyOpsAppComboBoxArray[0].Width = 350;
                ThirdSecretKeyOpsAppTextBoxArray = new TextBox[2];
                ThirdSecretKeyOpsAppTextBoxArray[0] = new TextBox();
                ThirdSecretKeyOpsAppTextBoxArray[1] = new TextBox();
                ThirdSecretKeyOpsAppTextBoxArray[0].MaxWidth = 350;
                ThirdSecretKeyOpsAppTextBoxArray[0].Width = 350;
                ThirdSecretKeyOpsAppTextBoxArray[0].MaxHeight = 50;
                ThirdSecretKeyOpsAppTextBoxArray[0].Height = 50;
                ThirdSecretKeyOpsAppTextBoxArray[1].MaxWidth = 350;
                ThirdSecretKeyOpsAppTextBoxArray[1].Width = 350;
                ThirdSecretKeyOpsAppTextBoxArray[1].MaxHeight = 50;
                ThirdSecretKeyOpsAppTextBoxArray[1].Height = 50;
                ThirdSecretKeyOpsAppRadioButtonArray = new RadioButton[23];
                int x = 0;
                int y = -1;
                String RadioButtonOriginalContents = "Unicode - UTF8,Base64,No,AES256GCM,AEGIS256,AEGIS128L,No,XChaCha20Poly1305IETF,ChaCha20Poly1305IETF,ChaCha20Poly1305,No,XChaCha20,XSalsa20,ChaCha20,ChaCha20IETF,Salsa20,Salsa12,Salsa8,No,HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305";
                String[] RadioButtonSplitContents = RadioButtonOriginalContents.Split(",");
                String RadioButtonOriginalGroupNames = "DataTypes,DataTypes,AESAlgorithms,AESAlgorithms,AESAlgorithms,AESAlgorithms,AEADAlgorithms,AEADAlgorithms,AEADAlgorithms,AEADAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,MACAlgorithms,MACAlgorithms,MACAlgorithms,MACAlgorithms,MACAlgorithms";
                String[] RadioButtonSplitGroupNames = RadioButtonOriginalGroupNames.Split(",");
                while (x<ThirdSecretKeyOpsAppRadioButtonArray.Length) 
                {
                    ThirdSecretKeyOpsAppRadioButtonArray[x] = new RadioButton();
                    ThirdSecretKeyOpsAppRadioButtonArray[x].Content = RadioButtonSplitContents[x];
                    ThirdSecretKeyOpsAppRadioButtonArray[x].GroupName = RadioButtonSplitGroupNames[x];
                    x += 1;
                }
                ThirdSecretKeyOpsAppRadioButtonArray[0].IsChecked = true;
                ThirdSecretKeyOpsAppRadioButtonArray[2].IsChecked = true;
                ThirdSecretKeyOpsAppRadioButtonArray[6].IsChecked = true;
                ThirdSecretKeyOpsAppRadioButtonArray[10].IsChecked = true;
                ThirdSecretKeyOpsAppRadioButtonArray[18].IsChecked = true;
                x = 2;
                while (x < 10) 
                {
                    ThirdSecretKeyOpsAppRadioButtonArray[x].Tag = y;
                    if (y == 2)
                    {
                        y = -1;
                    }
                    else 
                    {
                        y += 1;
                    }
                    x += 1;
                }
                x = 10;
                y = -1;
                while (x < 18) 
                {
                    ThirdSecretKeyOpsAppRadioButtonArray[x].Tag = y;
                    y += 1;
                    x += 1;
                }
                x = 18;
                y = -1;
                while (x < 23) 
                {
                    ThirdSecretKeyOpsAppRadioButtonArray[x].Tag = y;
                    y += 1;
                    x += 1;
                }
                ThirdSecretKeyOpsAppButtonArray = new Button[1];
                ThirdSecretKeyOpsAppButtonArray[0] = new Button();
                ThirdSecretKeyOpsAppButtonArray[0].Content = "SHSM SecretKeys Encrypt";
                ThirdSecretKeyOpsAppButtonArray[0].Click += SecretKeyCryptographyOpsAppBTN_Click;
                ThirdSecretKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                ThirdSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBoxArray[1]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[4]);
                x = 2;
                while (x < 6) 
                {
                    MidStackPanel.Children.Add(ThirdSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[5]);
                x = 6;
                while (x < 10) 
                {
                    MidStackPanel.Children.Add(ThirdSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[6]);
                x = 10;
                while (x < 18) 
                {
                    MidStackPanel.Children.Add(ThirdSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppTextBlockArray[7]);
                x = 18;
                while (x < 23) 
                {
                    MidStackPanel.Children.Add(ThirdSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(ThirdSecretKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(ThirdSecretKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasSecretKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 4)
        {
            if (HasSecretKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (Combobox)
                //Cipher Text?
                //Additional Data?
                //Data Type? (RB) - (Unicode - UTF8, Base64)
                //AES Algorithm? (RB) - (No,AES256GCM, AEGIS256, AEGIS128L) - Use Tag
                //AEAD Algorithm? (RB) - (No,XChaCha20Poly1305IETF, ChaCha20Poly1305IETF, ChaCha20Poly1305) - Use Tag
                //Stream Cipher Algorithm? (RB) - (No,XChaCha20, XSalsa20, ChaCha20, ChaCha20IETF, Salsa20, Salsa12, Salsa8) - Use Tag
                //MAC Algorithm? (RB) - (No,HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305) - Use Tag
                //----
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                FourthSecretKeyOpsAppTextBlockArray = new TextBlock[8];
                FourthSecretKeyOpsAppTextBlockArray[0] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[1] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[2] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[3] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[4] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[5] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[6] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[7] = new TextBlock();
                FourthSecretKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                FourthSecretKeyOpsAppTextBlockArray[1].Text = "Cipher Text?";
                FourthSecretKeyOpsAppTextBlockArray[2].Text = "Additional Data?";
                FourthSecretKeyOpsAppTextBlockArray[3].Text = "Data Type?";
                FourthSecretKeyOpsAppTextBlockArray[4].Text = "AES Algorithm?";
                FourthSecretKeyOpsAppTextBlockArray[5].Text = "AEAD Algorithm?";
                FourthSecretKeyOpsAppTextBlockArray[6].Text = "Stream Cipher Algorithm?";
                FourthSecretKeyOpsAppTextBlockArray[7].Text = "MAC Algorithm?";
                FourthSecretKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppTextBlockArray[6].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppTextBlockArray[7].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppComboBoxArray = new ComboBox[1];
                FourthSecretKeyOpsAppComboBoxArray[0] = new ComboBox();
                SecretKeyCryptographyOpsAppLoadUserIDs();
                FourthSecretKeyOpsAppComboBoxArray[0].Width = 350;
                FourthSecretKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                FourthSecretKeyOpsAppTextBoxArray = new TextBox[2];
                FourthSecretKeyOpsAppTextBoxArray[0] = new TextBox();
                FourthSecretKeyOpsAppTextBoxArray[1] = new TextBox();
                FourthSecretKeyOpsAppTextBoxArray[0].MaxWidth = 350;
                FourthSecretKeyOpsAppTextBoxArray[0].Width = 350;
                FourthSecretKeyOpsAppTextBoxArray[0].MaxHeight = 50;
                FourthSecretKeyOpsAppTextBoxArray[0].Height = 50;
                FourthSecretKeyOpsAppTextBoxArray[1].MaxWidth = 350;
                FourthSecretKeyOpsAppTextBoxArray[1].Width = 350;
                FourthSecretKeyOpsAppTextBoxArray[1].MaxHeight = 50;
                FourthSecretKeyOpsAppTextBoxArray[1].Height = 50;
                FourthSecretKeyOpsAppRadioButtonArray = new RadioButton[23];
                int x = 0;
                int y = -1;
                String RadioButtonOriginalContents = "Unicode - UTF8,Base64,No,AES256GCM,AEGIS256,AEGIS128L,No,XChaCha20Poly1305IETF,ChaCha20Poly1305IETF,ChaCha20Poly1305,No,XChaCha20,XSalsa20,ChaCha20,ChaCha20IETF,Salsa20,Salsa12,Salsa8,No,HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305";
                String[] RadioButtonSplitContents = RadioButtonOriginalContents.Split(",");
                String RadioButtonOriginalGroupNames = "DataTypes,DataTypes,AESAlgorithms,AESAlgorithms,AESAlgorithms,AESAlgorithms,AEADAlgorithms,AEADAlgorithms,AEADAlgorithms,AEADAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,StreamCipherAlgorithms,MACAlgorithms,MACAlgorithms,MACAlgorithms,MACAlgorithms,MACAlgorithms";
                String[] RadioButtonSplitGroupNames = RadioButtonOriginalGroupNames.Split(",");
                while (x < FourthSecretKeyOpsAppRadioButtonArray.Length)
                {
                    FourthSecretKeyOpsAppRadioButtonArray[x] = new RadioButton();
                    FourthSecretKeyOpsAppRadioButtonArray[x].Content = RadioButtonSplitContents[x];
                    FourthSecretKeyOpsAppRadioButtonArray[x].GroupName = RadioButtonSplitGroupNames[x];
                    x += 1;
                }
                FourthSecretKeyOpsAppRadioButtonArray[0].IsChecked = true;
                FourthSecretKeyOpsAppRadioButtonArray[2].IsChecked = true;
                FourthSecretKeyOpsAppRadioButtonArray[6].IsChecked = true;
                FourthSecretKeyOpsAppRadioButtonArray[10].IsChecked = true;
                FourthSecretKeyOpsAppRadioButtonArray[18].IsChecked = true;
                x = 2;
                while (x < 10)
                {
                    FourthSecretKeyOpsAppRadioButtonArray[x].Tag = y;
                    if (y == 2)
                    {
                        y = -1;
                    }
                    else 
                    {
                        y += 1;
                    }
                    x += 1;
                }
                x = 10;
                y = -1;
                while (x < 18)
                {
                    FourthSecretKeyOpsAppRadioButtonArray[x].Tag = y;
                    y += 1;
                    x += 1;
                }
                x = 18;
                y = -1;
                while (x < 23)
                {
                    FourthSecretKeyOpsAppRadioButtonArray[x].Tag = y;
                    y += 1;
                    x += 1;
                }
                FourthSecretKeyOpsAppButtonArray = new Button[1];
                FourthSecretKeyOpsAppButtonArray[0] = new Button();
                FourthSecretKeyOpsAppButtonArray[0].Content = "SHSM SecretKeys Decrypt";
                FourthSecretKeyOpsAppButtonArray[0].Click += SecretKeyCryptographyOpsAppBTN_Click;
                FourthSecretKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FourthSecretKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FourthSecretKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FourthSecretKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FourthSecretKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FourthSecretKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FourthSecretKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FourthSecretKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FourthSecretKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FourthSecretKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FourthSecretKeyOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                FourthSecretKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FourthSecretKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FourthSecretKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FourthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                FourthSecretKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FourthSecretKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FourthSecretKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FourthSecretKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FourthSecretKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FourthSecretKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FourthSecretKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[2]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBoxArray[1]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[3]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[4]);
                x = 2;
                while (x < 6)
                {
                    MidStackPanel.Children.Add(FourthSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[5]);
                x = 6;
                while (x < 10)
                {
                    MidStackPanel.Children.Add(FourthSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[6]);
                x = 10;
                while (x < 18)
                {
                    MidStackPanel.Children.Add(FourthSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppTextBlockArray[7]);
                x = 18;
                while (x < 23)
                {
                    MidStackPanel.Children.Add(FourthSecretKeyOpsAppRadioButtonArray[x]);
                    x += 1;
                }
                MidStackPanel.Children.Add(FourthSecretKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FourthSecretKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasSecretKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 5)
        {
            if (HasSecretKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //Symmetric Encryption Algorithm? (RB) - (XSalsa20Poly1305,XChaCha20Poly1305)
                //------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Query Params
                //Root/Sudo?
                //Status..
                FifthSecretKeyOpsAppTextBlockArray = new TextBlock[2];
                FifthSecretKeyOpsAppTextBlockArray[0] = new TextBlock();
                FifthSecretKeyOpsAppTextBlockArray[1] = new TextBlock();
                FifthSecretKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                FifthSecretKeyOpsAppTextBlockArray[1].Text = "Symmetric Encryption Algorithm?";
                FifthSecretKeyOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthSecretKeyOpsAppComboBoxArray = new ComboBox[1];
                FifthSecretKeyOpsAppComboBoxArray[0] = new ComboBox();
                SecretKeyCryptographyOpsAppLoadUserIDs();
                FifthSecretKeyOpsAppComboBoxArray[0].Width = 350;
                FifthSecretKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                FifthSecretKeyOpsAppRadioButtonArray = new RadioButton[2];
                FifthSecretKeyOpsAppRadioButtonArray[0] = new RadioButton();
                FifthSecretKeyOpsAppRadioButtonArray[1] = new RadioButton();
                FifthSecretKeyOpsAppRadioButtonArray[0].Content = "XSalsa20Poly1305";
                FifthSecretKeyOpsAppRadioButtonArray[1].Content = "XChaCha20Poly1305";
                FifthSecretKeyOpsAppRadioButtonArray[0].GroupName = "SymmetricEncryptionAlgorithm";
                FifthSecretKeyOpsAppRadioButtonArray[1].GroupName = "SymmetricEncryptionAlgorithm";
                FifthSecretKeyOpsAppRadioButtonArray[0].IsChecked = true;
                FifthSecretKeyOpsAppButtonArray = new Button[1];
                FifthSecretKeyOpsAppButtonArray[0] = new Button();
                FifthSecretKeyOpsAppButtonArray[0].Content = "SHSM Export Secret Keys";
                FifthSecretKeyOpsAppButtonArray[0].Click += SecretKeyCryptographyOpsAppBTN_Click;
                FifthSecretKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FifthSecretKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FifthSecretKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FifthSecretKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FifthSecretKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FifthSecretKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FifthSecretKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FifthSecretKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FifthSecretKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FifthSecretKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FifthSecretKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                FifthSecretKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FifthSecretKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FifthSecretKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthSecretKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthSecretKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthSecretKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthSecretKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FifthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                FifthSecretKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FifthSecretKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FifthSecretKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FifthSecretKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FifthSecretKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FifthSecretKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FifthSecretKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FifthSecretKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FifthSecretKeyOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FifthSecretKeyOpsAppRadioButtonArray[0]);
                MidStackPanel.Children.Add(FifthSecretKeyOpsAppRadioButtonArray[1]);
                MidStackPanel.Children.Add(FifthSecretKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FifthSecretKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasSecretKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 6)
        {
            if (HasSecretKeyCryptographyOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Query Params
                //Root/Sudo?
                //Status..
                SixthSecretKeyOpsAppTextBlockArray = new TextBlock[1];
                SixthSecretKeyOpsAppTextBlockArray[0] = new TextBlock();
                SixthSecretKeyOpsAppTextBlockArray[0].Text = "User_ID?";
                SixthSecretKeyOpsAppComboBoxArray = new ComboBox[1];
                SixthSecretKeyOpsAppComboBoxArray[0] = new ComboBox();
                SecretKeyCryptographyOpsAppLoadUserIDs();
                SixthSecretKeyOpsAppComboBoxArray[0].MaxWidth = 350;
                SixthSecretKeyOpsAppComboBoxArray[0].Width = 350;
                SixthSecretKeyOpsAppButtonArray = new Button[1];
                SixthSecretKeyOpsAppButtonArray[0] = new Button();
                SixthSecretKeyOpsAppButtonArray[0].Content = "SHSM SKeys Extend Duration";
                SixthSecretKeyOpsAppButtonArray[0].Click += SecretKeyCryptographyOpsAppBTN_Click;
                SixthSecretKeyOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[6];
                SixthSecretKeyOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                SixthSecretKeyOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                SixthSecretKeyOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                SixthSecretKeyOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                SixthSecretKeyOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                SixthSecretKeyOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                SixthSecretKeyOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                SixthSecretKeyOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                SixthSecretKeyOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                SixthSecretKeyOpsAppDeveloperTextBlockArray[3].Text = "URL query params";
                SixthSecretKeyOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                SixthSecretKeyOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                SixthSecretKeyOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthSecretKeyOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthSecretKeyOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthSecretKeyOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthSecretKeyOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                SixthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[6];
                SixthSecretKeyOpsAppDeveloperTextBoxArray[0] = new TextBox();
                SixthSecretKeyOpsAppDeveloperTextBoxArray[1] = new TextBox();
                SixthSecretKeyOpsAppDeveloperTextBoxArray[2] = new TextBox();
                SixthSecretKeyOpsAppDeveloperTextBoxArray[3] = new TextBox();
                SixthSecretKeyOpsAppDeveloperTextBoxArray[4] = new TextBox();
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5] = new TextBox();
                SixthSecretKeyOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[0].Width = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[0].Height = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[1].Width = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[1].Height = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[2].Width = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[2].Height = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[3].Width = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[3].Height = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[4].Width = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[4].Height = 50;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5].Width = 350;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(SixthSecretKeyOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(SixthSecretKeyOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(SixthSecretKeyOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(SixthSecretKeyOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SecretKeyCryptographyOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasSecretKeyCryptographyOpsAppUIRendered = true;
            }
        }
        else
        {
            ResetSecretKeyCryptographyOpsAppUI();
        }
    }

    private void ArweaveOpsAppDrawUI() 
    {
        if (ArweaveOpsAppUIChooser == 1)
        {
            if (HasArweaveOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //JSON Data/Data String? 
                //------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Request Body (JSON)
                //Root/Sudo?
                //Status..
                FirstArweaveOpsAppTextBlockArray = new TextBlock[2];
                FirstArweaveOpsAppTextBlockArray[0] = new TextBlock();
                FirstArweaveOpsAppTextBlockArray[1] = new TextBlock();
                FirstArweaveOpsAppTextBlockArray[0].Text = "User_ID?";
                FirstArweaveOpsAppTextBlockArray[1].Text = "JSON Data/Data String?";
                FirstArweaveOpsAppTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstArweaveOpsAppComboBoxArray = new ComboBox[1];
                FirstArweaveOpsAppComboBoxArray[0] = new ComboBox();
                ArweaveOpsAppLoadUserIDs();
                FirstArweaveOpsAppComboBoxArray[0].Width = 350;
                FirstArweaveOpsAppComboBoxArray[0].MaxWidth = 350;
                FirstArweaveOpsAppTextBoxArray = new TextBox[1];
                FirstArweaveOpsAppTextBoxArray[0] = new TextBox();
                FirstArweaveOpsAppTextBoxArray[0].MaxWidth = 350;
                FirstArweaveOpsAppTextBoxArray[0].MaxHeight = 50;
                FirstArweaveOpsAppTextBoxArray[0].Width = 350;
                FirstArweaveOpsAppTextBoxArray[0].Height = 50;
                FirstArweaveOpsAppButtonArray = new Button[1];
                FirstArweaveOpsAppButtonArray[0] = new Button();
                FirstArweaveOpsAppButtonArray[0].Content = "Arweave Upload Data";
                FirstArweaveOpsAppButtonArray[0].Click += ArweaveOpsAppBTN_Click;
                FirstArweaveOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstArweaveOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FirstArweaveOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FirstArweaveOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FirstArweaveOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FirstArweaveOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FirstArweaveOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FirstArweaveOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FirstArweaveOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FirstArweaveOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FirstArweaveOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FirstArweaveOpsAppDeveloperTextBlockArray[3].Text = "Request Body (JSON)";
                FirstArweaveOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FirstArweaveOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FirstArweaveOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstArweaveOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstArweaveOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstArweaveOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstArweaveOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstArweaveOpsAppDeveloperTextBoxArray = new TextBox[6];
                FirstArweaveOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FirstArweaveOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FirstArweaveOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FirstArweaveOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FirstArweaveOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FirstArweaveOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FirstArweaveOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FirstArweaveOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FirstArweaveOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FirstArweaveOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FirstArweaveOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FirstArweaveOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FirstArweaveOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[0].Width = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[0].Height = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[1].Width = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[1].Height = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[2].Width = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[2].Height = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[3].Width = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[3].Height = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[4].Width = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[4].Height = 50;
                FirstArweaveOpsAppDeveloperTextBoxArray[5].Width = 350;
                FirstArweaveOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FirstArweaveOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FirstArweaveOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FirstArweaveOpsAppTextBlockArray[1]);
                MidStackPanel.Children.Add(FirstArweaveOpsAppTextBoxArray[0]);
                MidStackPanel.Children.Add(FirstArweaveOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                ArweaveOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FirstArweaveOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                ArweaveOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasArweaveOpsAppUIRendered = true;
            }
        }
        else 
        {
            ResetArweaveOpsAppUI();
        }
    }

    private void SHSMOpsAppDrawUI() 
    {
        if (SHSMOpsAppUIChooser == 1)
        {
            if (HasSHSMOpsAppUIRendered == false)
            {
                //User_ID? (ComboBox)
                //------
                //Server API IP Address
                //Server full API IP address
                //HTTP Type (Post/Get/..)
                //Query Params
                //Root/Sudo?
                //Status..
                FirstSHSMOpsAppTextBlockArray = new TextBlock[1];
                FirstSHSMOpsAppTextBlockArray[0] = new TextBlock();
                FirstSHSMOpsAppTextBlockArray[0].Text = "User_ID?";
                FirstSHSMOpsAppComboBoxArray = new ComboBox[1];
                FirstSHSMOpsAppComboBoxArray[0] = new ComboBox();
                SHSMOpsAppLoadUserIDs();
                FirstSHSMOpsAppComboBoxArray[0].MaxWidth = 350;
                FirstSHSMOpsAppComboBoxArray[0].Width = 350;
                FirstSHSMOpsAppButtonArray = new Button[1];
                FirstSHSMOpsAppButtonArray[0] = new Button();
                FirstSHSMOpsAppButtonArray[0].Content = "Delete SHSM";
                FirstSHSMOpsAppButtonArray[0].Click += SHSMOpsAppBTN_Click;
                FirstSHSMOpsAppButtonArray[0].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSHSMOpsAppDeveloperTextBlockArray = new TextBlock[6];
                FirstSHSMOpsAppDeveloperTextBlockArray[0] = new TextBlock();
                FirstSHSMOpsAppDeveloperTextBlockArray[1] = new TextBlock();
                FirstSHSMOpsAppDeveloperTextBlockArray[2] = new TextBlock();
                FirstSHSMOpsAppDeveloperTextBlockArray[3] = new TextBlock();
                FirstSHSMOpsAppDeveloperTextBlockArray[4] = new TextBlock();
                FirstSHSMOpsAppDeveloperTextBlockArray[5] = new TextBlock();
                FirstSHSMOpsAppDeveloperTextBlockArray[0].Text = "Server API IP Address";
                FirstSHSMOpsAppDeveloperTextBlockArray[1].Text = "Server full API IP address";
                FirstSHSMOpsAppDeveloperTextBlockArray[2].Text = "HTTP Type (Post/Get/..)";
                FirstSHSMOpsAppDeveloperTextBlockArray[3].Text = "Query Params";
                FirstSHSMOpsAppDeveloperTextBlockArray[4].Text = "Root/Sudo?";
                FirstSHSMOpsAppDeveloperTextBlockArray[5].Text = "Status..";
                FirstSHSMOpsAppDeveloperTextBlockArray[1].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSHSMOpsAppDeveloperTextBlockArray[2].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSHSMOpsAppDeveloperTextBlockArray[3].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSHSMOpsAppDeveloperTextBlockArray[4].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSHSMOpsAppDeveloperTextBlockArray[5].Margin = Avalonia.Thickness.Parse("0, 10, 0, 0");
                FirstSHSMOpsAppDeveloperTextBoxArray = new TextBox[6];
                FirstSHSMOpsAppDeveloperTextBoxArray[0] = new TextBox();
                FirstSHSMOpsAppDeveloperTextBoxArray[1] = new TextBox();
                FirstSHSMOpsAppDeveloperTextBoxArray[2] = new TextBox();
                FirstSHSMOpsAppDeveloperTextBoxArray[3] = new TextBox();
                FirstSHSMOpsAppDeveloperTextBoxArray[4] = new TextBox();
                FirstSHSMOpsAppDeveloperTextBoxArray[5] = new TextBox();
                FirstSHSMOpsAppDeveloperTextBoxArray[0].IsReadOnly = true;
                FirstSHSMOpsAppDeveloperTextBoxArray[1].IsReadOnly = true;
                FirstSHSMOpsAppDeveloperTextBoxArray[2].IsReadOnly = true;
                FirstSHSMOpsAppDeveloperTextBoxArray[3].IsReadOnly = true;
                FirstSHSMOpsAppDeveloperTextBoxArray[4].IsReadOnly = true;
                FirstSHSMOpsAppDeveloperTextBoxArray[5].IsReadOnly = true;
                FirstSHSMOpsAppDeveloperTextBoxArray[0].MaxWidth = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[0].MaxHeight = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[1].MaxWidth = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[1].MaxHeight = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[2].MaxWidth = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[2].MaxHeight = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[3].MaxWidth = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[3].MaxHeight = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[4].MaxWidth = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[4].MaxHeight = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[5].MaxWidth = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[5].MaxHeight = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[0].Width = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[0].Height = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[1].Width = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[1].Height = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[2].Width = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[2].Height = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[3].Width = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[3].Height = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[4].Width = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[4].Height = 50;
                FirstSHSMOpsAppDeveloperTextBoxArray[5].Width = 350;
                FirstSHSMOpsAppDeveloperTextBoxArray[5].Height = 50;
                ScrollViewer MidScrollViewer = new ScrollViewer();
                MidScrollViewer.MaxHeight = 700;
                MidScrollViewer.MaxWidth = 400;
                StackPanel MidStackPanel = new StackPanel();
                MidStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                MidStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                MidStackPanel.Children.Add(FirstSHSMOpsAppTextBlockArray[0]);
                MidStackPanel.Children.Add(FirstSHSMOpsAppComboBoxArray[0]);
                MidStackPanel.Children.Add(FirstSHSMOpsAppButtonArray[0]);
                MidScrollViewer.Content = MidStackPanel;
                MidScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SHSMOpsAppLowerMidSP.Children.Add(MidScrollViewer);
                ScrollViewer RightScrollViewer = new ScrollViewer();
                RightScrollViewer.MaxHeight = 700;
                RightScrollViewer.MaxWidth = 400;
                StackPanel RightStackPanel = new StackPanel();
                RightStackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                RightStackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBlockArray[0]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBoxArray[0]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBlockArray[1]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBoxArray[1]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBlockArray[2]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBoxArray[2]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBlockArray[3]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBoxArray[3]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBlockArray[4]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBoxArray[4]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBlockArray[5]);
                RightStackPanel.Children.Add(FirstSHSMOpsAppDeveloperTextBoxArray[5]);
                RightScrollViewer.Content = RightStackPanel;
                RightScrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
                SHSMOpsAppLowerRightSP.Children.Add(RightScrollViewer);
                HasSHSMOpsAppUIRendered = true;
            }
        }
        else
        {
            ResetSHSMOpsAppUI();
        }
    }

    private void ResetIPOpsAppUI()
    {
        IPOpsAppUIChooser = 0;
        FirstIPOpsAppTextBlockArray = new TextBlock[] { };
        FirstIPOpsAppTextBoxArray = new TextBox[] { };
        FirstIPOpsAppButtonArray = new Button[] { };
        SecondIPOpsAppTextBlockArray = new TextBlock[] { };
        SecondIPOpsAppTextBoxArray = new TextBox[] { };
        SecondIPOpsAppButtonArray = new Button[] { };
        IPOpsAppLowerRightSP.Children.Clear();
        HasIPOpsAppUIRendered = false;
    }

    private void ResetRegistrationOpsAppUI() 
    {
        RegistrationOpsAppUIChooser = 0;
        FirstRegistrationOpsAppTextBlockArray = new TextBlock[] { };
        FirstRegistrationOpsAppTextBoxArray = new TextBox[] { };
        FirstRegistrationOpsAppRadioButtonArray = new RadioButton[] { };
        FirstRegistrationOpsAppButtonArray = new Button[] { };
        FirstRegistrationOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FirstRegistrationOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SecondRegistrationOpsAppTextBlockArray = new TextBlock[] { };
        SecondRegistrationOpsAppTextBoxArray = new TextBox[] { };
        SecondRegistrationOpsAppComboBoxArray = new ComboBox[] { };
        SecondRegistrationOpsAppRadioButtonArray = new RadioButton[] { };
        SecondRegistrationOpsAppButtonArray = new Button[] { };
        SecondRegistrationOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        SecondRegistrationOpsAppDeveloperTextBoxArray = new TextBox[] { };
        RegistrationOpsAppLowerMidSP.Children.Clear();
        RegistrationOpsAppLowerRightSP.Children.Clear();
        HasRegistrationOpsAppUIRendered = false;
    }

    private void ResetETLSOpsAppUI() 
    {
        ETLSOpsAppUIChooser = 0;
        FirstETLSOpsAppTextBlockArray = new TextBlock[] { };
        FirstETLSOpsAppRadioButtonArray = new RadioButton[] { };
        FirstETLSOpsAppComboBoxArray = new ComboBox[] { };
        FirstETLSOpsAppButtonArray = new Button[] { };
        FirstETLSOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FirstETLSOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SecondETLSOpsAppTextBlockArray = new TextBlock[] { };
        SecondETLSOpsAppComboBoxArray = new ComboBox[] { };
        SecondETLSOpsAppButtonArray = new Button[] { };
        SecondETLSOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        SecondETLSOpsAppDeveloperTextBoxArray = new TextBox[] { };
        ETLSOpsAppLowerMidSP.Children.Clear();
        ETLSOpsAppLowerRightSP.Children.Clear();
        HasETLSOpsAppUIRendered = false;
    }

    private void ResetPublicKeyCryptographyOpsAppUI() 
    {
        PublicKeyCryptographyOpsAppUIChooser = 0;
        FirstPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
        FirstPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
        FirstPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
        FirstPublicKeyOpsAppButtonArray = new Button[] { };
        FirstPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FirstPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SecondPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
        SecondPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
        SecondPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
        SecondPublicKeyOpsAppButtonArray = new Button[] { };
        SecondPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        SecondPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        ThirdPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
        ThirdPublicKeyOpsAppTextBoxArray = new TextBox[] { };
        ThirdPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
        ThirdPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
        ThirdPublicKeyOpsAppButtonArray = new Button[] { };
        ThirdPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        ThirdPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        FourthPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
        FourthPublicKeyOpsAppTextBoxArray = new TextBox[] { };
        FourthPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
        FourthPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
        FourthPublicKeyOpsAppButtonArray = new Button[] { };
        FourthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FourthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        FifthPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
        FifthPublicKeyOpsAppTextBoxArray = new TextBox[] { };
        FifthPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
        FifthPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
        FifthPublicKeyOpsAppButtonArray = new Button[] { };
        FifthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FifthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SixthPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
        SixthPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
        SixthPublicKeyOpsAppButtonArray = new Button[] { };
        SixthPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        SixthPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SeventhPublicKeyOpsAppTextBlockArray = new TextBlock[] { };
        SeventhPublicKeyOpsAppRadioButtonArray = new RadioButton[] { };
        SeventhPublicKeyOpsAppComboBoxArray = new ComboBox[] { };
        SeventhPublicKeyOpsAppButtonArray = new Button[] { };
        SeventhPublicKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        SeventhPublicKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        PublicKeyCryptographyOpsAppLowerMidSP.Children.Clear();
        PublicKeyCryptographyOpsAppLowerRightSP.Children.Clear();
        HasPublicKeyCryptographyOpsAppUIRendered = false;
    }

    private void ResetSecretKeyCryptographyOpsAppUI()
    {
        SecretKeyCryptographyOpsAppUIChooser = 0;
        FirstSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
        FirstSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
        FirstSecretKeyOpsAppButtonArray = new Button[] { };
        FirstSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FirstSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SecondSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
        SecondSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
        SecondSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
        SecondSecretKeyOpsAppButtonArray = new Button[] { };
        SecondSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        SecondSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        ThirdSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
        ThirdSecretKeyOpsAppTextBoxArray = new TextBox[] { };
        ThirdSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
        ThirdSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
        ThirdSecretKeyOpsAppButtonArray = new Button[] { };
        ThirdSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        ThirdSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        FourthSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
        FourthSecretKeyOpsAppTextBoxArray = new TextBox[] { };
        FourthSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
        FourthSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
        FourthSecretKeyOpsAppButtonArray = new Button[] { };
        FourthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FourthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        FifthSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
        FifthSecretKeyOpsAppRadioButtonArray = new RadioButton[] { };
        FifthSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
        FifthSecretKeyOpsAppButtonArray = new Button[] { };
        FifthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FifthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SixthSecretKeyOpsAppTextBlockArray = new TextBlock[] { };
        SixthSecretKeyOpsAppComboBoxArray = new ComboBox[] { };
        SixthSecretKeyOpsAppButtonArray = new Button[] { };
        SixthSecretKeyOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        SixthSecretKeyOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SecretKeyCryptographyOpsAppLowerMidSP.Children.Clear();
        SecretKeyCryptographyOpsAppLowerRightSP.Children.Clear();
        HasSecretKeyCryptographyOpsAppUIRendered = false;
    }

    private void ResetArweaveOpsAppUI()
    {
        ArweaveOpsAppUIChooser = 0;
        FirstArweaveOpsAppTextBlockArray = new TextBlock[] { };
        FirstArweaveOpsAppTextBoxArray = new TextBox[] { };
        FirstArweaveOpsAppComboBoxArray = new ComboBox[] { };
        FirstArweaveOpsAppButtonArray = new Button[] { };
        FirstArweaveOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FirstArweaveOpsAppDeveloperTextBoxArray = new TextBox[] { };
        ArweaveOpsAppLowerMidSP.Children.Clear();
        ArweaveOpsAppLowerRightSP.Children.Clear();
        HasArweaveOpsAppUIRendered = false;
    }

    private void ResetSHSMOpsAppUI() 
    {
        SHSMOpsAppUIChooser = 0;
        FirstSHSMOpsAppTextBlockArray = new TextBlock[] { };
        FirstSHSMOpsAppComboBoxArray = new ComboBox[] { };
        FirstSHSMOpsAppButtonArray = new Button[] { };
        FirstSHSMOpsAppDeveloperTextBlockArray = new TextBlock[] { };
        FirstSHSMOpsAppDeveloperTextBoxArray = new TextBox[] { };
        SHSMOpsAppLowerMidSP.Children.Clear();
        SHSMOpsAppLowerRightSP.Children.Clear();
        HasSHSMOpsAppUIRendered = false;
    }

    private void IPOpsAppBTN_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (IPOpsAppUIChooser == 1)
        {
            String IPAddress = FirstIPOpsAppTextBoxArray[0].Text;
            if (IPAddress != null && IPAddress.CompareTo("") != 0)
            {
                File.WriteAllText(ServerRootFolder + "IP.txt", IPAddress);
                EstablishConnectionHelper.CreateLinkWithServer();
                FirstIPOpsAppTextBoxArray[1].Text = EstablishConnectionHelper.ConnectionStatus;
            }
        }
        else if (IPOpsAppUIChooser == 2)
        {
            String IPAddress = File.ReadAllText(ServerRootFolder + "IP.txt");
            if (IPAddress != null && IPAddress.CompareTo("") != 0) 
            {
                SecondIPOpsAppTextBoxArray[0].Text = GetNodeInformationHelper.GetNodeInfo();
            }
            else 
            {
                SecondIPOpsAppTextBoxArray[0].Text = "Error: You haven't configure and establish connection with the SHSM server..";
            }
        }
    }

    private void RegistrationOpsAppBTN_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (RegistrationOpsAppUIChooser == 1) 
        {
            //User_ID?
            //AU Info's Arweave ID?
            //AU Signed Sub DSA Public Key Arweave ID?
            //Create new export key pair? (RB) - Default to no
            //Signed Export Public Key B64 (Read Only)
            //Is KEM or SealedBox (Read Only)
            //Algorithm Type (Read Only) (String)
            //The AU Signed Sub DSA Public Key 
            //please make sure you pre-anchor
            //so you can get arweave ID in advance
            //--------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Decoded AU Info's From Arweave ID
            //Decoded AU Signed Sub DSA Public Key
            //Root/Sudo?
            //Status..
            String User_ID = FirstRegistrationOpsAppTextBoxArray[0].Text;
            if(User_ID!=null && User_ID.CompareTo("") != 0) 
            {
                if(Directory.Exists(UsersRootFolder+User_ID) == true) 
                {
                    String AU_User_Info_Arweave_ID = FirstRegistrationOpsAppTextBoxArray[1].Text;
                    String Sub_PublicKey_Arweave_ID = FirstRegistrationOpsAppTextBoxArray[2].Text;
                    //Line 853 was vibe coded with using AI
                    int selectedIndex = Array.FindIndex(FirstRegistrationOpsAppRadioButtonArray, rb => rb.IsChecked == true);
                    String SignedExportPublicKeyB64 = "";
                    String IsKEMorSealedBoxString = "";
                    //String AlgorithmTypeString = "";
                    //=========
                    String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                    String ServerFullAPIIPAddress = ServerAPIIPAddress+"Registration";
                    String HTTPTypeString = "POST (Web API)";
                    String RequestBodyString = "";
                    String DecodedAU_User_Info = "";
                    String DecodedAU_Signed_Sub_PublicKey = "";
                    String IsRootOrSudoString = "Root";
                    if(AU_User_Info_Arweave_ID!=null && AU_User_Info_Arweave_ID.CompareTo("")!=0 &&
                        Sub_PublicKey_Arweave_ID!=null && Sub_PublicKey_Arweave_ID.CompareTo("") !=0
                        && selectedIndex!=-1) 
                    {
                        Boolean IsValidArweaveIDForAU_User = true;
                        Boolean IsValidArweaveIDForSub_PublicKey = true;
                        String TXData = "";
                        try
                        {
                            TXData = GetTransactionDataHelper.GetTransactionData(AU_User_Info_Arweave_ID);
                        }
                        catch
                        {
                            IsValidArweaveIDForAU_User = false;
                        }
                        if (IsValidArweaveIDForAU_User)
                        {
                            Byte[] DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(TXData);
                            String OriginalArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                            DecodedAU_User_Info = OriginalArweaveTXData;
                        }
                        TXData = "";
                        try
                        {
                            TXData = GetTransactionDataHelper.GetTransactionData(Sub_PublicKey_Arweave_ID);
                        }
                        catch
                        {
                            IsValidArweaveIDForSub_PublicKey = false;
                        }
                        if (IsValidArweaveIDForSub_PublicKey)
                        {
                            Byte[] DecodedArweaveTXData = Base64URLEncodeDecodeHelper.Decode(TXData);
                            String OriginalArweaveTXData = Encoding.UTF8.GetString(DecodedArweaveTXData);
                            DecodedAU_Signed_Sub_PublicKey = OriginalArweaveTXData;
                        }
                        if(IsValidArweaveIDForAU_User==true && IsValidArweaveIDForSub_PublicKey == true) 
                        {
                            if (DecodedAU_User_Info.Contains(User_ID) == true) 
                            {
                                //Getting AU_Info_ArweaveID should be easy when you're coming from SPKI
                                //The latter.. might need developers having to an extent of advanced skill
                                //They need to be able to use libsodium and corresponding binding to create
                                //corresponding keypair if it's ED25519
                                //If it's ED448 then one can refer to how bouncycastle work and output,
                                //having similar cryptography library that might not come with default language
                                //support will do the trick.
                                //After generating corresponding sub signed digital signature key pair,
                                //then the corresponding public key and some info can then be anchored to
                                //Arweave using their SDK or with modern C#'s SimplifiedArweaveSDK. 
                                //The second step might be tedious and I will see if i can come up with
                                //simple command line application but not command line application tool
                                //to smooth the process..
                                //The same goes for creating corresponding C# or other applicable programming
                                //languages' library or code snippets. Only selected applicable programming
                                //languages will be having the tools needed due to SHSM's safety concerns..
                                //There might be more stable version in the future..
                                //but for now, this most likely will remain as alpha version..
                                RegisteredUserModel MyModel = new RegisteredUserModel();
                                MyModel.User_ID = User_ID;
                                MyModel.AUInfoModelArweaveID = AU_User_Info_Arweave_ID;
                                MyModel.SubSignedDSPKArweaveID = Sub_PublicKey_Arweave_ID;
                                MyModel.UserSignedPublicKeys = new SignedPublicKeysModel();
                                if (selectedIndex == 0) 
                                {
                                    Byte[] SubDSAPrivateKeyBytes = new Byte[] { };
                                    Byte[] ExportPublicKeyBytes = new Byte[] { };
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                                    {
                                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                                        ExportPublicKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\ExportPublicKey.txt");
                                    }
                                    else 
                                    {
                                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                                        ExportPublicKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/ExportPublicKey.txt");
                                    }
                                    if (ExportPublicKeyBytes.Length == 32) 
                                    {
                                        IsKEMorSealedBoxString = "Sealedbox (X25519)";
                                    }
                                    else 
                                    {
                                        IsKEMorSealedBoxString = "KEM (X-Wing variant)";
                                    }
                                    Byte[] SignedExportPublicKeyBytes = new Byte[] { };
                                    if (SubDSAPrivateKeyBytes.Length == 64) 
                                    {
                                        SignedExportPublicKeyBytes = SodiumPublicKeyAuth.Sign(ExportPublicKeyBytes, SubDSAPrivateKeyBytes,true);
                                    }
                                    else 
                                    {
                                        SignedExportPublicKeyBytes = SecureED448.GenerateSignatureMessage(SubDSAPrivateKeyBytes, ExportPublicKeyBytes, new Byte[] { }, true);
                                    }
                                    SignedExportPublicKeyB64 = Convert.ToBase64String(SignedExportPublicKeyBytes);
                                    MyModel.UserSignedPublicKeys.SubSignedPublicKeysB64 = new String[1];
                                    MyModel.UserSignedPublicKeys.SubSignedPublicKeysB64[0] = SignedExportPublicKeyB64;
                                    MyModel.UserSignedPublicKeys.IsKEMorSealedBox = new Boolean[1];
                                    MyModel.UserSignedPublicKeys.IsKEMorSealedBox[0] = IsKEMorSealedBoxString.CompareTo("KEM (X-Wing variant)")==0;
                                    MyModel.UserSignedPublicKeys.AlgorithmTypes = new String[1];
                                    MyModel.UserSignedPublicKeys.AlgorithmTypes[0] = IsKEMorSealedBoxString;
                                }
                                else if (selectedIndex == 1) 
                                {
                                    IsKEMorSealedBoxString = "KEM";
                                    Byte[] SubDSAPrivateKeyBytes = new Byte[] { };
                                    Byte[] ExportPublicKeyBytes = new Byte[] { };
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                    {
                                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                                    }
                                    else
                                    {
                                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                                    }
                                    RevampedKeyPair MyKEMKeyPair = SodiumKEM.GenerateRevampedKeyPair();
                                    ExportPublicKeyBytes = MyKEMKeyPair.PublicKey;
                                    if (ExportPublicKeyBytes.Length == 32)
                                    {
                                        IsKEMorSealedBoxString = "Sealedbox (X25519)";
                                    }
                                    else
                                    {
                                        IsKEMorSealedBoxString = "KEM (X-Wing variant)";
                                    }
                                    Byte[] SignedExportPublicKeyBytes = new Byte[] { };
                                    if (SubDSAPrivateKeyBytes.Length == 64)
                                    {
                                        SignedExportPublicKeyBytes = SodiumPublicKeyAuth.Sign(ExportPublicKeyBytes, SubDSAPrivateKeyBytes, true);
                                    }
                                    else
                                    {
                                        SignedExportPublicKeyBytes = SecureED448.GenerateSignatureMessage(SubDSAPrivateKeyBytes, ExportPublicKeyBytes, new Byte[] { }, true);
                                    }
                                    SignedExportPublicKeyB64 = Convert.ToBase64String(SignedExportPublicKeyBytes);
                                    MyModel.UserSignedPublicKeys.SubSignedPublicKeysB64 = new String[1];
                                    MyModel.UserSignedPublicKeys.SubSignedPublicKeysB64[0] = SignedExportPublicKeyB64;
                                    MyModel.UserSignedPublicKeys.IsKEMorSealedBox = new Boolean[1];
                                    MyModel.UserSignedPublicKeys.IsKEMorSealedBox[0] = true;
                                    MyModel.UserSignedPublicKeys.AlgorithmTypes = new String[1];
                                    MyModel.UserSignedPublicKeys.AlgorithmTypes[0] = IsKEMorSealedBoxString;
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                                    {
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPrivateKey.txt", MyKEMKeyPair.PrivateKey);
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPublicKey.txt", ExportPublicKeyBytes);
                                    }
                                    else 
                                    {
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPrivateKey.txt", MyKEMKeyPair.PrivateKey);
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPublicKey.txt", ExportPublicKeyBytes);
                                    }
                                    MyKEMKeyPair.Clear();
                                }
                                else 
                                {
                                    IsKEMorSealedBoxString = "Sealedbox";
                                    Byte[] SubDSAPrivateKeyBytes = new Byte[] { };
                                    Byte[] ExportPublicKeyBytes = new Byte[] { };
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                    {
                                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                                    }
                                    else
                                    {
                                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                                    }
                                    RevampedKeyPair MySealedBoxKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
                                    ExportPublicKeyBytes = MySealedBoxKeyPair.PublicKey;
                                    if (ExportPublicKeyBytes.Length == 32)
                                    {
                                        IsKEMorSealedBoxString = "Sealedbox (X25519)";
                                    }
                                    else
                                    {
                                        IsKEMorSealedBoxString = "KEM (X-Wing variant)";
                                    }
                                    Byte[] SignedExportPublicKeyBytes = new Byte[] { };
                                    if (SubDSAPrivateKeyBytes.Length == 64)
                                    {
                                        SignedExportPublicKeyBytes = SodiumPublicKeyAuth.Sign(ExportPublicKeyBytes, SubDSAPrivateKeyBytes, true);
                                    }
                                    else
                                    {
                                        SignedExportPublicKeyBytes = SecureED448.GenerateSignatureMessage(SubDSAPrivateKeyBytes, ExportPublicKeyBytes, new Byte[] { }, true);
                                    }
                                    SignedExportPublicKeyB64 = Convert.ToBase64String(SignedExportPublicKeyBytes);
                                    MyModel.UserSignedPublicKeys.SubSignedPublicKeysB64 = new String[1];
                                    MyModel.UserSignedPublicKeys.SubSignedPublicKeysB64[0] = SignedExportPublicKeyB64;
                                    MyModel.UserSignedPublicKeys.IsKEMorSealedBox = new Boolean[1];
                                    MyModel.UserSignedPublicKeys.IsKEMorSealedBox[0] = false;
                                    MyModel.UserSignedPublicKeys.AlgorithmTypes = new String[1];
                                    MyModel.UserSignedPublicKeys.AlgorithmTypes[0] = IsKEMorSealedBoxString;
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                    {
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPrivateKey.txt", MySealedBoxKeyPair.PrivateKey);
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPublicKey.txt", ExportPublicKeyBytes);
                                    }
                                    else
                                    {
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPrivateKey.txt", MySealedBoxKeyPair.PrivateKey);
                                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPublicKey.txt", ExportPublicKeyBytes);
                                    }
                                    MySealedBoxKeyPair.Clear();
                                }
                                RequestBodyString = JsonConvert.SerializeObject(MyModel);
                                String OutputString = SignupAsSHSMUserHelper.SignupAsSHSMUser(RequestBodyString);
                                FirstRegistrationOpsAppTextBoxArray[3].Text = SignedExportPublicKeyB64;
                                FirstRegistrationOpsAppTextBoxArray[4].Text = IsKEMorSealedBoxString;
                                FirstRegistrationOpsAppTextBoxArray[5].Text = "Kindly refer to 'Is KEM or SealedBox' textbox";
                                FirstRegistrationOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                                FirstRegistrationOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                                FirstRegistrationOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                                FirstRegistrationOpsAppDeveloperTextBoxArray[3].Text = RequestBodyString;
                                FirstRegistrationOpsAppDeveloperTextBoxArray[4].Text = DecodedAU_User_Info;
                                FirstRegistrationOpsAppDeveloperTextBoxArray[5].Text = DecodedAU_Signed_Sub_PublicKey;
                                FirstRegistrationOpsAppDeveloperTextBoxArray[6].Text = IsRootOrSudoString;
                                FirstRegistrationOpsAppDeveloperTextBoxArray[7].Text = OutputString;
                            }
                            else 
                            {
                                FirstRegistrationOpsAppDeveloperTextBoxArray[7].Text = "Error: This specified arweave id does not match with the user id you specified..";
                            }
                        }
                        else 
                        {
                            FirstRegistrationOpsAppDeveloperTextBoxArray[7].Text = "Error: These specified arweave ids do not exist on arweave..";
                        }
                    }
                    else 
                    {
                        FirstRegistrationOpsAppDeveloperTextBoxArray[7].Text = "Error: AU Info's Arweave ID and AU Signed Sub DSA Public Key Arweave ID must not be null/empty" + Environment.NewLine
                            + "Create new export key pair must indicate a choice..";
                    }
                }
                else 
                {
                    FirstRegistrationOpsAppDeveloperTextBoxArray[7].Text = "Error: This user id does not exist locally..";
                }
            }
            else 
            {
                FirstRegistrationOpsAppDeveloperTextBoxArray[7].Text = "Error: User ID must not be null or empty";
            }
        }
        else if(RegistrationOpsAppUIChooser == 2) 
        {
            //Todo..
            //User_ID? (Combobox)
            //Keys Export Algorithm? (RB)
            //KEM/SealedBox Public Key (Read Only)
            //Signed version of it (Read Only)
            //Is KEM or SealedBox (Read Only)
            //Algorithm Type (Read Only)
            //--------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Index = SecondRegistrationOpsAppComboBoxArray[0].SelectedIndex;
            int selectedIndex = Array.FindIndex(SecondRegistrationOpsAppRadioButtonArray, rb => rb.IsChecked == true);
            if(User_ID_Index!=-1 && selectedIndex != -1) 
            {
                String User_ID = SecondRegistrationOpsAppComboBoxArray[0].SelectedItem.ToString();
                RegistrationUpdateSignedPublicKeysModel MyModel = new RegistrationUpdateSignedPublicKeysModel();
                //========
                String ExportPublicKeyB64 = "";
                String SignedExportPublicKeyB64 = "";
                String IsKEMOrSealedBoxString = "";
                String AlgorithmType = "";
                //-----------
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "Registration/Update";
                String HTTPTypeString = "POST (Web API)";
                String RequestBodyString = "";
                String IsRootOrSudoString = "Sudo";
                //KEM
                if (selectedIndex == 0) 
                {
                    Byte[] SubDSAPrivateKeyBytes = new Byte[] { };
                    Byte[] ExportPublicKeyBytes = new Byte[] { };
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                    }
                    else
                    {
                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                    }
                    RevampedKeyPair MyKEMKeyPair = SodiumKEM.GenerateRevampedKeyPair();
                    ExportPublicKeyBytes = MyKEMKeyPair.PublicKey;
                    Byte[] SignedExportPublicKeyBytes = new Byte[] { };
                    if (SubDSAPrivateKeyBytes.Length == 64)
                    {
                        SignedExportPublicKeyBytes = SodiumPublicKeyAuth.Sign(ExportPublicKeyBytes, SubDSAPrivateKeyBytes);
                    }
                    else
                    {
                        SignedExportPublicKeyBytes = SecureED448.GenerateSignatureMessage(SubDSAPrivateKeyBytes, ExportPublicKeyBytes, new Byte[] { });
                    }
                    SignedExportPublicKeyB64 = Convert.ToBase64String(SignedExportPublicKeyBytes);
                    Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                    Byte[] SignedChallengeBytes = new Byte[] { };
                    if (SubDSAPrivateKeyBytes.Length == 64)
                    {
                        SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, SubDSAPrivateKeyBytes,true);
                    }
                    else
                    {
                        SignedChallengeBytes = SecureED448.GenerateSignatureMessage(SubDSAPrivateKeyBytes, ChallengeBytes, new Byte[] { },true);
                    }
                    MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                    MyModel.DataUpdateModel = new SignedPublicKeysModel();
                    MyModel.DataUpdateModel.AlgorithmTypes = new String[1];
                    MyModel.DataUpdateModel.AlgorithmTypes[0] = "KEM";
                    MyModel.DataUpdateModel.IsKEMorSealedBox = new Boolean[1];
                    MyModel.DataUpdateModel.IsKEMorSealedBox[0] = true;
                    MyModel.DataUpdateModel.SubSignedPublicKeysB64 = new String[1];
                    MyModel.DataUpdateModel.SubSignedPublicKeysB64[0] = SignedExportPublicKeyB64;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPrivateKey.txt", MyKEMKeyPair.PrivateKey);
                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPublicKey.txt", ExportPublicKeyBytes);
                    }
                    else
                    {
                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPrivateKey.txt", MyKEMKeyPair.PrivateKey);
                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPublicKey.txt", ExportPublicKeyBytes);
                    }
                    ExportPublicKeyB64 = Convert.ToBase64String(MyKEMKeyPair.PublicKey);
                    IsKEMOrSealedBoxString = "KEM";
                    AlgorithmType = "KEM (X-Wing)";
                    MyKEMKeyPair.Clear();
                }
                //Sealedbox..
                else 
                {
                    Byte[] SubDSAPrivateKeyBytes = new Byte[] { };
                    Byte[] ExportPublicKeyBytes = new Byte[] { };
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                    }
                    else
                    {
                        SubDSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                    }
                    RevampedKeyPair MySealedBoxKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
                    ExportPublicKeyBytes = MySealedBoxKeyPair.PublicKey;
                    Byte[] SignedExportPublicKeyBytes = new Byte[] { };
                    if (SubDSAPrivateKeyBytes.Length == 64)
                    {
                        SignedExportPublicKeyBytes = SodiumPublicKeyAuth.Sign(ExportPublicKeyBytes, SubDSAPrivateKeyBytes);
                    }
                    else
                    {
                        SignedExportPublicKeyBytes = SecureED448.GenerateSignatureMessage(SubDSAPrivateKeyBytes, ExportPublicKeyBytes, new Byte[] { });
                    }
                    SignedExportPublicKeyB64 = Convert.ToBase64String(SignedExportPublicKeyBytes);
                    Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                    Byte[] SignedChallengeBytes = new Byte[] { };
                    if (SubDSAPrivateKeyBytes.Length == 64)
                    {
                        SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, SubDSAPrivateKeyBytes, true);
                    }
                    else
                    {
                        SignedChallengeBytes = SecureED448.GenerateSignatureMessage(SubDSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                    }
                    MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                    MyModel.DataUpdateModel = new SignedPublicKeysModel();
                    MyModel.DataUpdateModel.AlgorithmTypes = new String[1];
                    MyModel.DataUpdateModel.AlgorithmTypes[0] = "Sealedbox";
                    MyModel.DataUpdateModel.IsKEMorSealedBox = new Boolean[1];
                    MyModel.DataUpdateModel.IsKEMorSealedBox[0] = false;
                    MyModel.DataUpdateModel.SubSignedPublicKeysB64 = new String[1];
                    MyModel.DataUpdateModel.SubSignedPublicKeysB64[0] = SignedExportPublicKeyB64;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPrivateKey.txt", MySealedBoxKeyPair.PrivateKey);
                        File.WriteAllBytes(UsersRootFolder + User_ID + "\\ExportPublicKey.txt", ExportPublicKeyBytes);
                    }
                    else
                    {
                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPrivateKey.txt", MySealedBoxKeyPair.PrivateKey);
                        File.WriteAllBytes(UsersRootFolder + User_ID + "/ExportPublicKey.txt", ExportPublicKeyBytes);
                    }
                    ExportPublicKeyB64 = Convert.ToBase64String(MySealedBoxKeyPair.PublicKey);
                    IsKEMOrSealedBoxString = "SealedBox";
                    AlgorithmType = "SealedBox (X25519)";
                    MySealedBoxKeyPair.Clear();
                }
                MyModel.User_ID = User_ID;
                String JSONDataString = JsonConvert.SerializeObject(MyModel);
                RequestBodyString = JSONDataString;
                String OutputString = UpdateRegisteredSHSMUserInfoHelper.UpdateRegisteredSHSMUserInfo(JSONDataString);
                SecondRegistrationOpsAppTextBoxArray[0].Text = ExportPublicKeyB64;
                SecondRegistrationOpsAppTextBoxArray[1].Text = SignedExportPublicKeyB64;
                SecondRegistrationOpsAppTextBoxArray[2].Text = IsKEMOrSealedBoxString;
                SecondRegistrationOpsAppTextBoxArray[3].Text = AlgorithmType;
                SecondRegistrationOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                SecondRegistrationOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                SecondRegistrationOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                SecondRegistrationOpsAppDeveloperTextBoxArray[3].Text = RequestBodyString;
                SecondRegistrationOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                SecondRegistrationOpsAppDeveloperTextBoxArray[5].Text = OutputString;
            }
            else 
            {
                SecondRegistrationOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not select a User_ID or Keys Export Algorithm..";
            }
        }
    }

    private void ETLSOpsAppBTN_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (ETLSOpsAppUIChooser == 1) 
        {
            //Todo..
            //User_ID? (ComboBox)
            //Client import's key algorithm (RB)
            //If there's a mistake in 
            //choosing an import algorithm, kindly
            //delete it and choose it again
            //--------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //URL query params
            //Root/Sudo?
            //Status..
            int User_ID_Index = FirstETLSOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Index != -1) 
            {
                String User_ID = FirstETLSOpsAppComboBoxArray[0].SelectedItem.ToString();
                Boolean IsKEMOrSealedBox = (Boolean)(FirstETLSOpsAppRadioButtonArray[0].IsChecked);
                String InitializedResult = ETLSInitializationHelper.ETLSInitialization(User_ID, IsKEMOrSealedBox);
                //-----
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "ETLS?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&IsKEM={IsKEMOrSealedBox}";
                String IsRootOrSudoString = "Root";
                FirstETLSOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                FirstETLSOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                FirstETLSOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                FirstETLSOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                FirstETLSOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                FirstETLSOpsAppDeveloperTextBoxArray[5].Text = InitializedResult;
            }
            else 
            {
                FirstETLSOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not select an existing user id";
            }
        }
        else if (ETLSOpsAppUIChooser == 2) 
        {
            //Todo..
            //User_ID? (ComboBox)
            //--------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //URL query params
            //Root/Sudo?
            //Status..
            int User_ID_Index = SecondETLSOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Index != -1) 
            {
                String User_ID = SecondETLSOpsAppComboBoxArray[0].SelectedItem.ToString();
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\"+User_ID+"APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "ETLS/DeleteETLS?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(Convert.ToBase64String(SignedChallengeBytes))}";
                String IsRootOrSudoString = "Root";
                String ResultString = ETLSDeleteHelper.ETLSDelete(User_ID, Convert.ToBase64String(SignedChallengeBytes));
                SecondETLSOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                SecondETLSOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                SecondETLSOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                SecondETLSOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                SecondETLSOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                SecondETLSOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                if (ResultString.Contains("Error") == false) 
                {
                    if (Directory.Exists(ETLSRootFolder + User_ID) == true)
                    {
                        Directory.Delete(ETLSRootFolder + User_ID,true);
                    }
                }
            }
            else 
            {
                SecondETLSOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not select an existing user id";
            }
        }
    }

    private void PublicKeyCryptographyOpsAppBTN_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (PublicKeyCryptographyOpsAppUIChooser == 1) 
        {
            //(Initiate DSA Keys)
            //User_ID? (ComboBox)
            //DSA Algorithm? (RB)
            //--------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //URL query params
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FirstPublicKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1) 
            {
                String User_ID = FirstPublicKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                int selectedIndex = Array.FindIndex(FirstPublicKeyOpsAppRadioButtonArray, rb => rb.IsChecked == true);
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "PublicKeyCryptography?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(Convert.ToBase64String(SignedChallengeBytes))}&DigitalSignatureAlgorithmsIndex={selectedIndex}";
                String IsRootOrSudoString = "Sudo";
                String ResultString = PublicKeyCryptoInitializeHelper.PublicKeyCryptoInitialize(User_ID, Convert.ToBase64String(SignedChallengeBytes),selectedIndex);
                FirstPublicKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else 
            {
                FirstPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
        else if(PublicKeyCryptographyOpsAppUIChooser == 2) 
        {
            //(Import Keys)
            //User_ID? (ComboBox)
            //Imported Key Type? (RB) - 3 (DSA, Sealedbox, KEM)
            //DS Algorithm? (RB) - 3 (ED25519,ED448,RSA)
            //Symmetric encryption algorithm? (RB) - 2 (XSalsa20Poly1305, XChaCha20Poly1305)
            //Generate required keys? (RB) (Yes,No)
            //-------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = SecondPublicKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1) 
            {
                String User_ID = SecondPublicKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                Boolean RequireKeysGenerationLocally = (Boolean)(SecondPublicKeyOpsAppRadioButtonArray[8].IsChecked);
                int ImportedKeyTypeInt = 0;
                int x = 0;
                while (x < 3)
                {
                    if (SecondPublicKeyOpsAppRadioButtonArray[x].IsChecked == true)
                    {
                        ImportedKeyTypeInt = (int)(SecondPublicKeyOpsAppRadioButtonArray[x].Tag);
                        break;
                    }
                    x += 1;
                }
                int DigitalSignatureAlgorithmIndex = 0;
                x = 3;
                while (x < 6)
                {
                    if (SecondPublicKeyOpsAppRadioButtonArray[x].IsChecked == true)
                    {
                        DigitalSignatureAlgorithmIndex = (int)(SecondPublicKeyOpsAppRadioButtonArray[x].Tag);
                        break;
                    }
                    x += 1;
                }
                int SymmetricEncryptionAlgorithmIndex = 0;
                x = 6;
                while (x < 8)
                {
                    if (SecondPublicKeyOpsAppRadioButtonArray[x].IsChecked == true)
                    {
                        SymmetricEncryptionAlgorithmIndex = (int)(SecondPublicKeyOpsAppRadioButtonArray[x].Tag);
                        break;
                    }
                    x += 1;
                }
                Byte[] PrivateKeyBytes = new Byte[] { };
                Byte[] ExponentBytes = new Byte[] { };
                Byte[] ModulusBytes = new Byte[] { };
                Byte[] DBytes = new Byte[] { };
                Byte[] PBytes = new Byte[] { };
                Byte[] QBytes = new Byte[] { };
                Byte[] DPBytes = new Byte[] { };
                Byte[] DQBytes = new Byte[] { };
                Byte[] InverseQBytes = new Byte[] { };
                Byte[] EncryptedPrivateKeyBytes = new Byte[] { };
                Byte[] EncryptedExponentBytes = new Byte[] { };
                Byte[] EncryptedModulusBytes = new Byte[] { };
                Byte[] EncryptedDBytes = new Byte[] { };
                Byte[] EncryptedPBytes = new Byte[] { };
                Byte[] EncryptedQBytes = new Byte[] { };
                Byte[] EncryptedDPBytes = new Byte[] { };
                Byte[] EncryptedDQBytes = new Byte[] { };
                Byte[] EncryptedInverseQBytes = new Byte[] { };
                String ServerSealedBoxAlgorithm = "";
                Byte[] ServerSealedBoxPublicKeyBytes = new Byte[] { };
                Byte[] Nonce = new Byte[] { };
                if (RequireKeysGenerationLocally) 
                {
                    if (Directory.Exists(PKCRootFolder + User_ID) == false)
                    {
                        Directory.CreateDirectory(PKCRootFolder + User_ID);
                    }
                    if (ImportedKeyTypeInt == 0) 
                    {
                        if (DigitalSignatureAlgorithmIndex == 0) 
                        {
                            RevampedKeyPair MyED25519KeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\ED25519PrivateKey.txt", MyED25519KeyPair.PrivateKey);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\ED25519PublicKey.txt", MyED25519KeyPair.PublicKey);
                            }
                            else 
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/ED25519PrivateKey.txt", MyED25519KeyPair.PrivateKey);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/ED25519PublicKey.txt", MyED25519KeyPair.PublicKey);
                            }
                            MyED25519KeyPair.Clear();
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\ED25519PrivateKey.txt");
                            }
                            else
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/ED25519PrivateKey.txt");
                            }
                        }
                        else if (DigitalSignatureAlgorithmIndex == 1) 
                        {
                            ED448RevampedKeyPair MyED448KeyPair = SecureED448.GenerateED448RevampedKeyPair();
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\ED448PrivateKey.txt", MyED448KeyPair.PrivateKey);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\ED448PublicKey.txt", MyED448KeyPair.PublicKey);
                            }
                            else
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/ED448PrivateKey.txt", MyED448KeyPair.PrivateKey);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/ED448PublicKey.txt", MyED448KeyPair.PublicKey);
                            }
                            MyED448KeyPair.Clear();
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\ED448PrivateKey.txt");
                            }
                            else
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/ED448PrivateKey.txt");
                            }
                        }
                        else 
                        {
                            RSA rsa = RSA.Create(4096);
                            RSAParameters MyRSAParams = rsa.ExportParameters(true);
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\P.txt", MyRSAParams.P);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\Q.txt", MyRSAParams.Q);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\Modulus.txt", MyRSAParams.Modulus);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\Exponent.txt", MyRSAParams.Exponent);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\D.txt", MyRSAParams.D);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\DP.txt", MyRSAParams.DP);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\DQ.txt", MyRSAParams.DQ);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "\\InverseQ.txt", MyRSAParams.InverseQ);
                            }
                            else
                            {
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/P.txt", MyRSAParams.P);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/Q.txt", MyRSAParams.Q);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/Modulus.txt", MyRSAParams.Modulus);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/Exponent.txt", MyRSAParams.Exponent);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/D.txt", MyRSAParams.D);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/DP.txt", MyRSAParams.DP);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/DQ.txt", MyRSAParams.DQ);
                                File.WriteAllBytes(PKCRootFolder + User_ID + "/InverseQ.txt", MyRSAParams.InverseQ);
                            }
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.P);
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.Q);
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.Modulus);
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.Exponent);
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.D);
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.DP);
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.DQ);
                            SodiumSecureMemory.SecureClearBytes(MyRSAParams.InverseQ);
                            rsa = RSA.Create();
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                PBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\P.txt");
                                QBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\Q.txt");
                                ModulusBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\Modulus.txt");
                                ExponentBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\Exponent.txt");
                                DBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\D.txt");
                                DPBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\DP.txt");
                                DQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\DQ.txt");
                                InverseQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\InverseQ.txt");
                            }
                            else
                            {
                                PBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/P.txt");
                                QBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/Q.txt");
                                ModulusBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/Modulus.txt");
                                ExponentBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/Exponent.txt");
                                DBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/D.txt");
                                DPBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/DP.txt");
                                DQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/DQ.txt");
                                InverseQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/InverseQ.txt");
                            }
                        }
                    }
                    else if (ImportedKeyTypeInt == 1) 
                    {
                        RevampedKeyPair MySealedBoxKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            File.WriteAllBytes(PKCRootFolder + User_ID + "\\SealedBoxPrivateKey.txt", MySealedBoxKeyPair.PrivateKey);
                            File.WriteAllBytes(PKCRootFolder + User_ID + "\\SealedBoxPublicKey.txt", MySealedBoxKeyPair.PublicKey);
                        }
                        else
                        {
                            File.WriteAllBytes(PKCRootFolder + User_ID + "/SealedBoxPrivateKey.txt", MySealedBoxKeyPair.PrivateKey);
                            File.WriteAllBytes(PKCRootFolder + User_ID + "/SealedBoxPublicKey.txt", MySealedBoxKeyPair.PublicKey);
                        }
                        MySealedBoxKeyPair.Clear();
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\SealedBoxPrivateKey.txt");
                        }
                        else
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/SealedBoxPrivateKey.txt");
                        }
                    }
                    else 
                    {
                        RevampedKeyPair MyKEMKeyPair = SodiumKEM.GenerateRevampedKeyPair();
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            File.WriteAllBytes(PKCRootFolder + User_ID + "\\KEMPrivateKey.txt", MyKEMKeyPair.PrivateKey);
                            File.WriteAllBytes(PKCRootFolder + User_ID + "\\KEMPublicKey.txt", MyKEMKeyPair.PublicKey);
                        }
                        else
                        {
                            File.WriteAllBytes(PKCRootFolder + User_ID + "/KEMPrivateKey.txt", MyKEMKeyPair.PrivateKey);
                            File.WriteAllBytes(PKCRootFolder + User_ID + "/KEMPublicKey.txt", MyKEMKeyPair.PublicKey);
                        }
                        MyKEMKeyPair.Clear();
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\KEMPrivateKey.txt");
                        }
                        else
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/KEMPrivateKey.txt");
                        }
                    }
                }
                else 
                {
                    if (ImportedKeyTypeInt == 0)
                    {
                        if (DigitalSignatureAlgorithmIndex == 0)
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\ED25519PrivateKey.txt");
                            }
                            else
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/ED25519PrivateKey.txt");
                            }
                        }
                        else if (DigitalSignatureAlgorithmIndex == 1)
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\ED448PrivateKey.txt");
                            }
                            else
                            {
                                PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/ED448PrivateKey.txt");
                            }
                        }
                        else
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                PBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\P.txt");
                                QBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\Q.txt");
                                ModulusBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\Modulus.txt");
                                ExponentBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\Exponent.txt");
                                DBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\D.txt");
                                DPBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\DP.txt");
                                DQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\DQ.txt");
                                InverseQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\InverseQ.txt");
                            }
                            else
                            {
                                PBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/P.txt");
                                QBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/Q.txt");
                                ModulusBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/Modulus.txt");
                                ExponentBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/Exponent.txt");
                                DBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/D.txt");
                                DPBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/DP.txt");
                                DQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/DQ.txt");
                                InverseQBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/InverseQ.txt");
                            }
                        }
                    }
                    else if (ImportedKeyTypeInt == 1)
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\SealedBoxPrivateKey.txt");
                        }
                        else
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/SealedBoxPrivateKey.txt");
                        }
                    }
                    else
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\KEMPrivateKey.txt");
                        }
                        else
                        {
                            PrivateKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/KEMPrivateKey.txt");
                        }
                    }
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                {
                    ServerSealedBoxAlgorithm = File.ReadAllText(ETLSRootFolder + User_ID + "\\ETLSAlgorithmType.txt");
                }
                else 
                {
                    ServerSealedBoxAlgorithm = File.ReadAllText(ETLSRootFolder + User_ID + "/ETLSAlgorithmType.txt");
                }
                if (ServerSealedBoxAlgorithm.Equals("KEM")) 
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        ServerSealedBoxPublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "\\ETLSKEMPublicKey.txt");
                    }
                    else
                    {
                        ServerSealedBoxPublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "/ETLSKEMPublicKey.txt");
                    }
                }
                else 
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        ServerSealedBoxPublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "\\ETLSEncryptionX25519PublicKey.txt");
                    }
                    else
                    {
                        ServerSealedBoxPublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "/ETLSEncryptionX25519PublicKey.txt");
                    }
                }
                if (ImportedKeyTypeInt == 0)
                {
                    if (ServerSealedBoxAlgorithm.Equals("KEM"))
                    {
                        EncapsulatedSharedSecretBox MyBox = SodiumKEM.EncapsulateSecretKeyBytes(ServerSealedBoxPublicKeyBytes);
                        if (DigitalSignatureAlgorithmIndex == 0 || DigitalSignatureAlgorithmIndex == 1)
                        {
                            if (SymmetricEncryptionAlgorithmIndex == 0) 
                            {
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedPrivateKeyBytes = SodiumSecretBox.Create(PrivateKeyBytes, Nonce, MyBox.SharedSecretBytes,true);
                                EncryptedPrivateKeyBytes = Nonce.Concat(EncryptedPrivateKeyBytes).ToArray();
                                EncryptedPrivateKeyBytes = MyBox.CipherTextBytes.Concat(EncryptedPrivateKeyBytes).ToArray();
                            }
                            else 
                            {
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedPrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Create(PrivateKeyBytes, Nonce, MyBox.SharedSecretBytes,true);
                                EncryptedPrivateKeyBytes = Nonce.Concat(EncryptedPrivateKeyBytes).ToArray();
                                EncryptedPrivateKeyBytes = MyBox.CipherTextBytes.Concat(EncryptedPrivateKeyBytes).ToArray();
                            }
                            SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                        }
                        else
                        {
                            if (SymmetricEncryptionAlgorithmIndex == 0)
                            {
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedExponentBytes = SodiumSecretBox.Create(ExponentBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedExponentBytes = Nonce.Concat(EncryptedExponentBytes).ToArray();
                                EncryptedExponentBytes = MyBox.CipherTextBytes.Concat(EncryptedExponentBytes).ToArray();
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedModulusBytes = SodiumSecretBox.Create(ModulusBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedModulusBytes = Nonce.Concat(EncryptedModulusBytes).ToArray();
                                EncryptedModulusBytes = MyBox.CipherTextBytes.Concat(EncryptedModulusBytes).ToArray();
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedDBytes = SodiumSecretBox.Create(DBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedDBytes = Nonce.Concat(EncryptedDBytes).ToArray();
                                EncryptedDBytes = MyBox.CipherTextBytes.Concat(EncryptedDBytes).ToArray();
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedPBytes = SodiumSecretBox.Create(PBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedPBytes = Nonce.Concat(EncryptedPBytes).ToArray();
                                EncryptedPBytes = MyBox.CipherTextBytes.Concat(EncryptedPBytes).ToArray();
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedQBytes = SodiumSecretBox.Create(QBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedQBytes = Nonce.Concat(EncryptedQBytes).ToArray();
                                EncryptedQBytes = MyBox.CipherTextBytes.Concat(EncryptedQBytes).ToArray();
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedDPBytes = SodiumSecretBox.Create(DPBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedDPBytes = Nonce.Concat(EncryptedDPBytes).ToArray();
                                EncryptedDPBytes = MyBox.CipherTextBytes.Concat(EncryptedDPBytes).ToArray();
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedDQBytes = SodiumSecretBox.Create(DQBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedDQBytes = Nonce.Concat(EncryptedDQBytes).ToArray();
                                EncryptedDQBytes = MyBox.CipherTextBytes.Concat(EncryptedDQBytes).ToArray();
                                Nonce = SodiumSecretBox.GenerateNonce();
                                EncryptedInverseQBytes = SodiumSecretBox.Create(InverseQBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedInverseQBytes = Nonce.Concat(EncryptedInverseQBytes).ToArray();
                                EncryptedInverseQBytes = MyBox.CipherTextBytes.Concat(EncryptedInverseQBytes).ToArray();
                            }
                            else
                            {
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedExponentBytes = SodiumSecretBoxXChaCha20Poly1305.Create(ExponentBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedExponentBytes = Nonce.Concat(EncryptedExponentBytes).ToArray();
                                EncryptedExponentBytes = MyBox.CipherTextBytes.Concat(EncryptedExponentBytes).ToArray();
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedModulusBytes = SodiumSecretBoxXChaCha20Poly1305.Create(ModulusBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedModulusBytes = Nonce.Concat(EncryptedModulusBytes).ToArray();
                                EncryptedModulusBytes = MyBox.CipherTextBytes.Concat(EncryptedModulusBytes).ToArray();
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedDBytes = SodiumSecretBoxXChaCha20Poly1305.Create(DBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedDBytes = Nonce.Concat(EncryptedDBytes).ToArray();
                                EncryptedDBytes = MyBox.CipherTextBytes.Concat(EncryptedDBytes).ToArray();
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedPBytes = SodiumSecretBoxXChaCha20Poly1305.Create(PBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedPBytes = Nonce.Concat(EncryptedPBytes).ToArray();
                                EncryptedPBytes = MyBox.CipherTextBytes.Concat(EncryptedPBytes).ToArray();
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedQBytes = SodiumSecretBoxXChaCha20Poly1305.Create(QBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedQBytes = Nonce.Concat(EncryptedQBytes).ToArray();
                                EncryptedQBytes = MyBox.CipherTextBytes.Concat(EncryptedQBytes).ToArray();
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedDPBytes = SodiumSecretBoxXChaCha20Poly1305.Create(DPBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedDPBytes = Nonce.Concat(EncryptedDPBytes).ToArray();
                                EncryptedDPBytes = MyBox.CipherTextBytes.Concat(EncryptedDPBytes).ToArray();
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedDQBytes = SodiumSecretBoxXChaCha20Poly1305.Create(DQBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedDQBytes = Nonce.Concat(EncryptedDQBytes).ToArray();
                                EncryptedDQBytes = MyBox.CipherTextBytes.Concat(EncryptedDQBytes).ToArray();
                                Nonce = SodiumSecretBoxXChaCha20Poly1305.GenerateNonce();
                                EncryptedInverseQBytes = SodiumSecretBoxXChaCha20Poly1305.Create(InverseQBytes, Nonce, MyBox.SharedSecretBytes);
                                EncryptedInverseQBytes = Nonce.Concat(EncryptedInverseQBytes).ToArray();
                                EncryptedInverseQBytes = MyBox.CipherTextBytes.Concat(EncryptedInverseQBytes).ToArray();
                            }
                            SodiumSecureMemory.SecureClearBytes(ExponentBytes);
                            SodiumSecureMemory.SecureClearBytes(ModulusBytes);
                            SodiumSecureMemory.SecureClearBytes(DBytes);
                            SodiumSecureMemory.SecureClearBytes(PBytes);
                            SodiumSecureMemory.SecureClearBytes(QBytes);
                            SodiumSecureMemory.SecureClearBytes(DPBytes);
                            SodiumSecureMemory.SecureClearBytes(DQBytes);
                            SodiumSecureMemory.SecureClearBytes(InverseQBytes);
                            SodiumSecureMemory.SecureClearBytes(MyBox.SharedSecretBytes);
                        }
                    }
                    else
                    {
                        if (DigitalSignatureAlgorithmIndex == 0 || DigitalSignatureAlgorithmIndex == 1)
                        {
                            if (SymmetricEncryptionAlgorithmIndex == 0)
                            {
                                EncryptedPrivateKeyBytes = SodiumSealedPublicKeyBox.Create(PrivateKeyBytes, ServerSealedBoxPublicKeyBytes);
                            }
                            else
                            {
                                EncryptedPrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(PrivateKeyBytes, ServerSealedBoxPublicKeyBytes);
                            }
                            SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                        }
                        else
                        {
                            if (SymmetricEncryptionAlgorithmIndex == 0)
                            {
                                EncryptedExponentBytes = SodiumSealedPublicKeyBox.Create(ExponentBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedModulusBytes = SodiumSealedPublicKeyBox.Create(ModulusBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedDBytes = SodiumSealedPublicKeyBox.Create(DBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedPBytes = SodiumSealedPublicKeyBox.Create(PBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedQBytes = SodiumSealedPublicKeyBox.Create(QBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedDPBytes = SodiumSealedPublicKeyBox.Create(DPBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedDQBytes = SodiumSealedPublicKeyBox.Create(DQBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedInverseQBytes = SodiumSealedPublicKeyBox.Create(InverseQBytes, ServerSealedBoxPublicKeyBytes);
                            }
                            else
                            {

                                EncryptedExponentBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(ExponentBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedModulusBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(ModulusBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedDBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(DBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedPBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(PBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedQBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(QBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedDPBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(DPBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedDQBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(DQBytes, ServerSealedBoxPublicKeyBytes);
                                EncryptedInverseQBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(InverseQBytes, ServerSealedBoxPublicKeyBytes);
                            }
                            SodiumSecureMemory.SecureClearBytes(ExponentBytes);
                            SodiumSecureMemory.SecureClearBytes(ModulusBytes);
                            SodiumSecureMemory.SecureClearBytes(DBytes);
                            SodiumSecureMemory.SecureClearBytes(PBytes);
                            SodiumSecureMemory.SecureClearBytes(QBytes);
                            SodiumSecureMemory.SecureClearBytes(DPBytes);
                            SodiumSecureMemory.SecureClearBytes(DQBytes);
                            SodiumSecureMemory.SecureClearBytes(InverseQBytes);
                        }
                    }
                }
                else if (ImportedKeyTypeInt == 1)
                {
                    if (ServerSealedBoxAlgorithm.Equals("KEM")) 
                    {
                        EncapsulatedSharedSecretBox MyBox = SodiumKEM.EncapsulateSecretKeyBytes(ServerSealedBoxPublicKeyBytes);
                        if (SymmetricEncryptionAlgorithmIndex == 0)
                        {
                            Nonce = SodiumSecretBox.GenerateNonce();
                            EncryptedPrivateKeyBytes = SodiumSecretBox.Create(PrivateKeyBytes, Nonce, MyBox.SharedSecretBytes, true);
                            EncryptedPrivateKeyBytes = Nonce.Concat(EncryptedPrivateKeyBytes).ToArray();
                            EncryptedPrivateKeyBytes = MyBox.CipherTextBytes.Concat(EncryptedPrivateKeyBytes).ToArray();
                        }
                        else
                        {
                            Nonce = SodiumSecretBox.GenerateNonce();
                            EncryptedPrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Create(PrivateKeyBytes, Nonce, MyBox.SharedSecretBytes, true);
                            EncryptedPrivateKeyBytes = Nonce.Concat(EncryptedPrivateKeyBytes).ToArray();
                            EncryptedPrivateKeyBytes = MyBox.CipherTextBytes.Concat(EncryptedPrivateKeyBytes).ToArray();
                        }
                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                    }
                    else 
                    {
                        if (SymmetricEncryptionAlgorithmIndex == 0)
                        {
                            EncryptedPrivateKeyBytes = SodiumSealedPublicKeyBox.Create(PrivateKeyBytes, ServerSealedBoxPublicKeyBytes);
                        }
                        else
                        {
                            EncryptedPrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(PrivateKeyBytes, ServerSealedBoxPublicKeyBytes);
                        }
                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                    }
                }
                else
                {
                    if (ServerSealedBoxAlgorithm.Equals("KEM"))
                    {
                        EncapsulatedSharedSecretBox MyBox = SodiumKEM.EncapsulateSecretKeyBytes(ServerSealedBoxPublicKeyBytes);
                        if (SymmetricEncryptionAlgorithmIndex == 0)
                        {
                            Nonce = SodiumSecretBox.GenerateNonce();
                            EncryptedPrivateKeyBytes = SodiumSecretBox.Create(PrivateKeyBytes, Nonce, MyBox.SharedSecretBytes, true);
                            EncryptedPrivateKeyBytes = Nonce.Concat(EncryptedPrivateKeyBytes).ToArray();
                            EncryptedPrivateKeyBytes = MyBox.CipherTextBytes.Concat(EncryptedPrivateKeyBytes).ToArray();
                        }
                        else
                        {
                            Nonce = SodiumSecretBox.GenerateNonce();
                            EncryptedPrivateKeyBytes = SodiumSecretBoxXChaCha20Poly1305.Create(PrivateKeyBytes, Nonce, MyBox.SharedSecretBytes, true);
                            EncryptedPrivateKeyBytes = Nonce.Concat(EncryptedPrivateKeyBytes).ToArray();
                            EncryptedPrivateKeyBytes = MyBox.CipherTextBytes.Concat(EncryptedPrivateKeyBytes).ToArray();
                        }
                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                    }
                    else
                    {
                        if (SymmetricEncryptionAlgorithmIndex == 0)
                        {
                            EncryptedPrivateKeyBytes = SodiumSealedPublicKeyBox.Create(PrivateKeyBytes, ServerSealedBoxPublicKeyBytes);
                        }
                        else
                        {
                            EncryptedPrivateKeyBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(PrivateKeyBytes, ServerSealedBoxPublicKeyBytes);
                        }
                        SodiumSecureMemory.SecureClearBytes(PrivateKeyBytes);
                    }
                }
                PublicKeyCryptographyKeysImportModel MyModel = new PublicKeyCryptographyKeysImportModel();
                MyModel.EncryptedPrivateKeyB64 = "";
                MyModel.IsDSOrSealedBoxOrKEMKeyType = ImportedKeyTypeInt;
                MyModel.IsED25519OrED448OrRSA = DigitalSignatureAlgorithmIndex;
                MyModel.IsKEMOrSealedBox = ServerSealedBoxAlgorithm.Equals("KEM");
                MyModel.IsXSalsa20Poly1305OrXChaCha20Poly1305 = SymmetricEncryptionAlgorithmIndex == 0;
                MyModel.MyRSAKey = new EncryptedRSACredentialsModel();
                MyModel.MyRSAKey.EncryptedDB64 = "";
                MyModel.MyRSAKey.EncryptedDPB64 = "";
                MyModel.MyRSAKey.EncryptedDQB64 = "";
                MyModel.MyRSAKey.EncryptedExponentB64 = "";
                MyModel.MyRSAKey.EncryptedInverseQB64 = "";
                MyModel.MyRSAKey.EncryptedModulusB64 = "";
                MyModel.MyRSAKey.EncryptedPB64 = "";
                MyModel.MyRSAKey.EncryptedQB64 = "";
                MyModel.SignedChallengeB64 = "";
                MyModel.User_ID = User_ID;
                if (ImportedKeyTypeInt == 0) 
                {
                    if(DigitalSignatureAlgorithmIndex == 0 || DigitalSignatureAlgorithmIndex==1) 
                    {
                        MyModel.EncryptedPrivateKeyB64 = Convert.ToBase64String(EncryptedPrivateKeyBytes);
                    }
                    else 
                    {
                        MyModel.MyRSAKey.EncryptedDB64 = Convert.ToBase64String(EncryptedDBytes);
                        MyModel.MyRSAKey.EncryptedDPB64 = Convert.ToBase64String(EncryptedDPBytes);
                        MyModel.MyRSAKey.EncryptedDQB64 = Convert.ToBase64String(EncryptedDQBytes);
                        MyModel.MyRSAKey.EncryptedExponentB64 = Convert.ToBase64String(EncryptedExponentBytes);
                        MyModel.MyRSAKey.EncryptedInverseQB64 = Convert.ToBase64String(EncryptedInverseQBytes);
                        MyModel.MyRSAKey.EncryptedModulusB64 = Convert.ToBase64String(EncryptedModulusBytes);
                        MyModel.MyRSAKey.EncryptedPB64 = Convert.ToBase64String(EncryptedPBytes);
                        MyModel.MyRSAKey.EncryptedQB64 = Convert.ToBase64String(EncryptedQBytes);
                    }
                }
                else 
                {
                    MyModel.EncryptedPrivateKeyB64 = Convert.ToBase64String(EncryptedPrivateKeyBytes);
                }
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\" + User_ID + "APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "PublicKeyCryptography/ImportKeys";
                String HTTPTypeString = "POST";
                String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                String IsRootOrSudoString = "Root";
                String ResultString = PublicKeyCryptoImportKeysHelper.PublicKeyCryptoImportKeys(JSONBodyString);
                SecondPublicKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else 
            {
                SecondPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 3)
        {
            //(Sign Data)
            //User_ID? (ComboBox)
            //DS Algorithm? (RB) - 3 (ED25519,ED448,RSA)
            //Data to be signed? (Textbox)
            //Data Type? -2 (RB) (Unicode - UTF8, Base64)
            //-------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = ThirdPublicKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1) 
            {
                String User_ID = ThirdPublicKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                int DigitalSignatureAlgorithmIndex = 0;
                int x = 0;
                while (x < 3) 
                {
                    if (ThirdPublicKeyOpsAppRadioButtonArray[x].IsChecked == true) 
                    {
                        DigitalSignatureAlgorithmIndex = x;
                        break;
                    }
                    x += 1;
                }
                Boolean IsUnicode = (Boolean)(ThirdPublicKeyOpsAppRadioButtonArray[3].IsChecked);
                String DataString = ThirdPublicKeyOpsAppTextBoxArray[0].Text;
                Byte[] DataStringBytes = new Byte[] { };
                String ActualDataString = "";
                if (IsUnicode) 
                {
                    DataStringBytes = Encoding.UTF8.GetBytes(DataString);
                    ActualDataString = Convert.ToBase64String(DataStringBytes);
                }
                else 
                {
                    ActualDataString = DataString;
                }
                PublicKeyCryptographySignDataModel MyModel = new PublicKeyCryptographySignDataModel();
                MyModel.DataB64 = ActualDataString;
                MyModel.IsED25519OrED448OrRSA = DigitalSignatureAlgorithmIndex;
                MyModel.SignedChallengeB64 = "";
                MyModel.User_ID = User_ID;
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "PublicKeyCryptography/SignData";
                String HTTPTypeString = "POST";
                String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                String IsRootOrSudoString = "Sudo";
                String ResultString = PublicKeyCryptoSignDataHelper.PublicKeyCryptoSignData(JSONBodyString);
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else 
            {
                ThirdPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 4)
        {
            //(Sealedbox decrypt)
            //User_ID? (Combobox)
            //Action? (Combobox) - (SHSM Sealedbox decrypt, local sealedbox encrypt)
            //Data to be encrypted?
            //Encrypted Data (Base64)?
            //Symmetric encryption algorithm? (RB) - (XSalsa20Poly1305, XChaCha20Poly1305)
            //----
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FourthPublicKeyOpsAppComboBoxArray[0].SelectedIndex;
            int Action_Selected_Index = FourthPublicKeyOpsAppComboBoxArray[1].SelectedIndex;
            if (Action_Selected_Index != -1) 
            {
                if (User_ID_Selected_Index != -1) 
                {
                    String User_ID = FourthPublicKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                    if (Action_Selected_Index == 0)
                    {
                        String EncryptedDataString = FourthPublicKeyOpsAppTextBoxArray[1].Text;
                        Byte[] EncryptedDataStringBytes = new Byte[] { };
                        Boolean AbleToBeConvertFromB64String = true;
                        if (EncryptedDataString.CompareTo("") != 0) 
                        {
                            try 
                            {
                                EncryptedDataStringBytes = Convert.FromBase64String(EncryptedDataString);
                            }
                            catch 
                            {
                                AbleToBeConvertFromB64String = false;
                            }
                            Boolean IsXSalsa20Poly1305 = (Boolean)(FourthPublicKeyOpsAppRadioButtonArray[0].IsChecked);
                            if (AbleToBeConvertFromB64String) 
                            {
                                PublicKeyCryptographyDecryptDataModel MyModel = new PublicKeyCryptographyDecryptDataModel();
                                MyModel.EncryptedDataB64 = EncryptedDataString;
                                MyModel.IsSealedBoxOrKEM = true;
                                MyModel.IsXSalsa20Poly1305OrXChaCha20Poly1305 = IsXSalsa20Poly1305;
                                MyModel.KEMEncryptionPKB64 = "";
                                MyModel.SignedChallengeB64 = "";
                                MyModel.User_ID = User_ID;
                                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                                }
                                else
                                {
                                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                                }
                                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                                Byte[] SignedChallengeBytes = new Byte[] { };
                                if (DSAPrivateKeyBytes.Length == 64)
                                {
                                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                                }
                                else
                                {
                                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                                }
                                MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                                String ServerFullAPIIPAddress = ServerAPIIPAddress + "PublicKeyCryptography/SealedBoxDecrypt";
                                String HTTPTypeString = "POST";
                                String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                                String IsRootOrSudoString = "Sudo";
                                String ResultString = PublicKeyCryptoSealedBoxDecryptHelper.PublicKeyCryptoSealedBoxDecrypt(JSONBodyString);
                                FourthPublicKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                                FourthPublicKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                                FourthPublicKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                                FourthPublicKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                                FourthPublicKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                            }
                            else 
                            {
                                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: The data to be decrypted was not in base64 format..";
                            }
                        }
                        else 
                        {
                            FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet input data to be decrypted";
                        }
                    }
                    else
                    {
                        String DataString = FourthPublicKeyOpsAppTextBoxArray[0].Text;
                        if (DataString.CompareTo("") != 0)
                        {
                            Byte[] DataStringBytes = Encoding.UTF8.GetBytes(DataString);
                            Byte[] EncryptedDataStringBytes = new Byte[] { };
                            Byte[] UserSealedBoxPublicKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                UserSealedBoxPublicKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\SealedBoxPublicKey.txt");
                            }
                            else
                            {
                                UserSealedBoxPublicKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/SealedBoxPublicKey.txt");
                            }
                            Boolean IsXSalsa20Poly1305 = (Boolean)(FourthPublicKeyOpsAppRadioButtonArray[0].IsChecked);
                            if (IsXSalsa20Poly1305) 
                            {
                                EncryptedDataStringBytes = SodiumSealedPublicKeyBox.Create(DataStringBytes, UserSealedBoxPublicKeyBytes);
                            }
                            else 
                            {
                                EncryptedDataStringBytes = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(DataStringBytes, UserSealedBoxPublicKeyBytes);
                            }
                            FourthPublicKeyOpsAppTextBoxArray[1].Text = Convert.ToBase64String(EncryptedDataStringBytes);
                        }
                        else
                        {
                            FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet input data to be encrypted";
                        }
                    }
                }
                else 
                {
                    FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual user id";
                }
            }
            else 
            {
                FourthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an action";
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 5)
        {
            //(KEM Decrypt)
            //User_ID? (Combobox)
            //Action? (Combobox) - (SHSM KEM decrypt, local KEM encrypt)
            //Data to be encrypted?
            //Encrypted Data (Base64)?
            //Symmetric encryption algorithm? (RB) - (XSalsa20Poly1305, XChaCha20Poly1305)
            //----
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FifthPublicKeyOpsAppComboBoxArray[0].SelectedIndex;
            int Action_Selected_Index = FifthPublicKeyOpsAppComboBoxArray[1].SelectedIndex;
            if (Action_Selected_Index != -1)
            {
                if (User_ID_Selected_Index != -1)
                {
                    String User_ID = FifthPublicKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                    if (Action_Selected_Index == 0)
                    {
                        String EncryptedDataString = FifthPublicKeyOpsAppTextBoxArray[1].Text;
                        Byte[] EncryptedDataStringBytes = new Byte[] { };
                        Boolean AbleToBeConvertFromB64String = true;
                        if (EncryptedDataString.CompareTo("") != 0)
                        {
                            try
                            {
                                EncryptedDataStringBytes = Convert.FromBase64String(EncryptedDataString);
                            }
                            catch
                            {
                                AbleToBeConvertFromB64String = false;
                            }
                            Boolean IsXSalsa20Poly1305 = (Boolean)(FifthPublicKeyOpsAppRadioButtonArray[0].IsChecked);
                            if (AbleToBeConvertFromB64String)
                            {
                                PublicKeyCryptographyDecryptDataModel MyModel = new PublicKeyCryptographyDecryptDataModel();
                                MyModel.EncryptedDataB64 = EncryptedDataString;
                                MyModel.IsSealedBoxOrKEM = false;
                                MyModel.IsXSalsa20Poly1305OrXChaCha20Poly1305 = IsXSalsa20Poly1305;
                                MyModel.KEMEncryptionPKB64 = "";
                                MyModel.SignedChallengeB64 = "";
                                MyModel.User_ID = User_ID;
                                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                                }
                                else
                                {
                                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                                }
                                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                                Byte[] SignedChallengeBytes = new Byte[] { };
                                if (DSAPrivateKeyBytes.Length == 64)
                                {
                                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                                }
                                else
                                {
                                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                                }
                                MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                                Byte[] KEMPublicKeyBytes = new Byte[] { };
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    KEMPublicKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\KEMPublicKey.txt");
                                }
                                else
                                {
                                    KEMPublicKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/KEMPublicKey.txt");
                                }
                                MyModel.KEMEncryptionPKB64 = Convert.ToBase64String(KEMPublicKeyBytes);
                                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                                String ServerFullAPIIPAddress = ServerAPIIPAddress + "PublicKeyCryptography/KEMDecrypt";
                                String HTTPTypeString = "POST";
                                String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                                String IsRootOrSudoString = "Sudo";
                                String ResultString = PublicKeyCryptoKEMDecryptHelper.PublicKeyCryptoKEMDecrypt(JSONBodyString);
                                FifthPublicKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                                FifthPublicKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                                FifthPublicKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                                FifthPublicKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                                FifthPublicKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                            }
                            else
                            {
                                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: The data to be decrypted was not in base64 format..";
                            }
                        }
                        else
                        {
                            FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet input data to be decrypted";
                        }
                    }
                    else
                    {
                        String DataString = FifthPublicKeyOpsAppTextBoxArray[0].Text;
                        if (DataString.CompareTo("") != 0)
                        {
                            Byte[] DataStringBytes = Encoding.UTF8.GetBytes(DataString);
                            Byte[] EncryptedDataStringBytes = new Byte[] { };
                            Byte[] UserKEMPublicKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                UserKEMPublicKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "\\KEMPublicKey.txt");
                            }
                            else
                            {
                                UserKEMPublicKeyBytes = File.ReadAllBytes(PKCRootFolder + User_ID + "/KEMPublicKey.txt");
                            }
                            Byte[] Nonce = new Byte[] { };
                            Boolean IsXSalsa20Poly1305 = (Boolean)(FifthPublicKeyOpsAppRadioButtonArray[0].IsChecked);
                            EncapsulatedSharedSecretBox MyBox = SodiumKEM.EncapsulateSecretKeyBytes(UserKEMPublicKeyBytes);
                            if (IsXSalsa20Poly1305)
                            {
                                Nonce = SodiumGenericHash.ComputeHash(64, UserKEMPublicKeyBytes);
                                Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                                EncryptedDataStringBytes = SodiumSecretBox.Create(DataStringBytes, Nonce,MyBox.SharedSecretBytes,true);
                                EncryptedDataStringBytes = MyBox.CipherTextBytes.Concat(EncryptedDataStringBytes).ToArray();
                            }
                            else
                            {
                                Nonce = SodiumGenericHash.ComputeHash(64, UserKEMPublicKeyBytes);
                                Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                                EncryptedDataStringBytes = SodiumSecretBoxXChaCha20Poly1305.Create(DataStringBytes, Nonce, MyBox.SharedSecretBytes, true);
                                EncryptedDataStringBytes = MyBox.CipherTextBytes.Concat(EncryptedDataStringBytes).ToArray();
                            }
                            FifthPublicKeyOpsAppTextBoxArray[1].Text = Convert.ToBase64String(EncryptedDataStringBytes);
                        }
                        else
                        {
                            FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet input data to be encrypted";
                        }
                    }
                }
                else
                {
                    FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual user id";
                }
            }
            else
            {
                FifthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an action";
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 6)
        {
            //(Extend Duration)
            //User_ID? (ComboBox)
            //------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Query Params
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = SixthPublicKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1) 
            {
                String User_ID = SixthPublicKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\" + User_ID + "APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "PublicKeyCryptography/ExtendDuration?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(Convert.ToBase64String(SignedChallengeBytes))}";
                String IsRootOrSudoString = "Root";
                String ResultString = PublicKeyCryptoExtendDurationHelper.PublicKeyCryptoExtendDuration(User_ID, Convert.ToBase64String(SignedChallengeBytes));
                SixthPublicKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else 
            {
                SixthPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 7)
        {
            //(Export DSA Keys)
            //User_ID? (ComboBox)
            //DS Algorithm? (RB) - (ED25519, ED448, RSA)
            //Symmetric Encryption Algorithm? (RB) - (XSalsa20Poly1305,XChaCha20Poly1305)
            //------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = SeventhPublicKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = SeventhPublicKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                int selectedIndex = Array.FindIndex(SeventhPublicKeyOpsAppRadioButtonArray, rb => rb.IsChecked == true);
                Boolean IsXSalsa20Poly1305 = (Boolean)(SeventhPublicKeyOpsAppRadioButtonArray[3].IsChecked);
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\" + User_ID + "APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                PublicKeyCryptographyExportDSAPostModel MyModel = new PublicKeyCryptographyExportDSAPostModel();
                MyModel.User_ID = User_ID;
                MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                MyModel.UseXSalsa20Poly1305 = IsXSalsa20Poly1305;
                MyModel.IsED25519OrED448OrRSA = selectedIndex;
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "PublicKeyCryptography/ExportDSAKeys";
                String HTTPTypeString = "POST";
                String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                String IsRootOrSudoString = "Root";
                String ResultString = PublicKeyCryptoExportDSAPrivateKeysHelper.PublicKeyCryptoExportDSAPrivateKeys(JSONBodyString,User_ID, IsXSalsa20Poly1305);
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else
            {
                SeventhPublicKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
    }

    private void SecretKeyCryptographyOpsAppBTN_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (SecretKeyCryptographyOpsAppUIChooser == 1) 
        {
            //(Initiate Secret Keys)
            //User_ID? (ComboBox)
            //--------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //URL query params
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FirstSecretKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = FirstSecretKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(Convert.ToBase64String(SignedChallengeBytes))}";
                String IsRootOrSudoString = "Sudo";
                String ResultString = SecretKeyCryptoInitializeHelper.SecretKeyCryptoInitialize(User_ID, Convert.ToBase64String(SignedChallengeBytes));
                FirstSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else
            {
                FirstSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
        else if(SecretKeyCryptographyOpsAppUIChooser == 2) 
        {
            //User_ID? (ComboBox)
            //Symmetric encryption algorithm? (RB) - 2 (XSalsa20Poly1305, XChaCha20Poly1305)
            //Generate required keys? (RB) (Yes,No)
            //-------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = SecondSecretKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = SecondSecretKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                Boolean RequireKeysGenerationLocally = (Boolean)(SecondSecretKeyOpsAppRadioButtonArray[2].IsChecked);
                Boolean IsXSalsa20Poly1305 = (Boolean)(SecondSecretKeyOpsAppRadioButtonArray[0].IsChecked);
                String ServerSealedBoxAlgorithm = "";
                Byte[] ServerSealedBoxKEMPublicKeyBytes = new Byte[] { };
                Byte[] ServerSealedBoxMACX25519PublicKeyBytes = new Byte[] { };
                Byte[] ServerSealedBoxEncryptionX25519PublicKeyBytes = new Byte[] { };
                Byte[] Nonce = new Byte[] { };
                Byte[] MACSecretKey = new Byte[] { };
                Byte[] EncryptionSecretKey = new Byte[] { };
                Byte[] EncryptedMACSecretKey = new Byte[] { };
                Byte[] EncryptedEncryptionSecretKey = new Byte[] { };
                if (RequireKeysGenerationLocally)
                {
                    if (Directory.Exists(SecretKeyRootFolder + User_ID) == false)
                    {
                        Directory.CreateDirectory(SecretKeyRootFolder + User_ID);
                    }
                    MACSecretKey = SodiumRNG.GetRandomBytes(32);
                    EncryptionSecretKey = SodiumRNG.GetRandomBytes(32);
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\MACSecretKey.txt", MACSecretKey);
                        File.WriteAllBytes(SecretKeyRootFolder + User_ID + "\\EncryptionSecretKey.txt", EncryptionSecretKey);
                    }
                    else
                    {
                        File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/MACSecretKey.txt", MACSecretKey);
                        File.WriteAllBytes(SecretKeyRootFolder + User_ID + "/EncryptionSecretKey.txt", EncryptionSecretKey);
                    }
                }
                else 
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        MACSecretKey = File.ReadAllBytes(SecretKeyRootFolder + User_ID + "\\MACSecretKey.txt");
                        EncryptionSecretKey = File.ReadAllBytes(SecretKeyRootFolder + User_ID + "\\EncryptionSecretKey.txt");
                    }
                    else
                    {
                        MACSecretKey = File.ReadAllBytes(SecretKeyRootFolder + User_ID + "/MACSecretKey.txt");
                        EncryptionSecretKey = File.ReadAllBytes(SecretKeyRootFolder + User_ID + "/EncryptionSecretKey.txt");
                    }
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    ServerSealedBoxAlgorithm = File.ReadAllText(ETLSRootFolder + User_ID + "\\ETLSAlgorithmType.txt");
                }
                else
                {
                    ServerSealedBoxAlgorithm = File.ReadAllText(ETLSRootFolder + User_ID + "/ETLSAlgorithmType.txt");
                }
                if (ServerSealedBoxAlgorithm.Equals("KEM"))
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        ServerSealedBoxKEMPublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "\\ETLSKEMPublicKey.txt");
                    }
                    else
                    {
                        ServerSealedBoxKEMPublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "/ETLSKEMPublicKey.txt");
                    }
                    EncapsulatedSharedSecretBox MyBox = SodiumKEM.EncapsulateSecretKeyBytes(ServerSealedBoxKEMPublicKeyBytes);
                    EncapsulatedSharedSecretBox MyBox2 = SodiumKEM.EncapsulateSecretKeyBytes(ServerSealedBoxKEMPublicKeyBytes);
                    if (IsXSalsa20Poly1305) 
                    {
                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), ServerSealedBoxKEMPublicKeyBytes);
                        EncryptedMACSecretKey = SodiumSecretBox.Create(MACSecretKey, Nonce, MyBox.SharedSecretBytes,true);
                        EncryptedMACSecretKey = MyBox.CipherTextBytes.Concat(EncryptedMACSecretKey).ToArray();
                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXSalsa20.GetXSalsa20NonceBytesLength(), Nonce);
                        EncryptedEncryptionSecretKey = SodiumSecretBox.Create(EncryptionSecretKey, Nonce, MyBox2.SharedSecretBytes, true);
                        EncryptedEncryptionSecretKey = MyBox2.CipherTextBytes.Concat(EncryptedEncryptionSecretKey).ToArray();
                    }
                    else 
                    {
                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), ServerSealedBoxKEMPublicKeyBytes);
                        EncryptedMACSecretKey = SodiumSecretBoxXChaCha20Poly1305.Create(MACSecretKey, Nonce, MyBox.SharedSecretBytes, true);
                        EncryptedMACSecretKey = MyBox.CipherTextBytes.Concat(EncryptedMACSecretKey).ToArray();
                        Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumStreamCipherXChaCha20.GetXChaCha20NonceBytesLength(), Nonce);
                        EncryptedEncryptionSecretKey = SodiumSecretBoxXChaCha20Poly1305.Create(EncryptionSecretKey, Nonce, MyBox2.SharedSecretBytes, true);
                        EncryptedEncryptionSecretKey = MyBox2.CipherTextBytes.Concat(EncryptedEncryptionSecretKey).ToArray();
                    }
                }
                else
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        ServerSealedBoxEncryptionX25519PublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "\\ETLSEncryptionX25519PublicKey.txt");
                        ServerSealedBoxMACX25519PublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "\\ETLSMACX25519PublicKey.txt");
                    }
                    else
                    {
                        ServerSealedBoxEncryptionX25519PublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "\\ETLSEncryptionX25519PublicKey.txt");
                        ServerSealedBoxMACX25519PublicKeyBytes = File.ReadAllBytes(ETLSRootFolder + User_ID + "\\ETLSMACX25519PublicKey.txt");
                    }
                    if (IsXSalsa20Poly1305) 
                    {
                        EncryptedEncryptionSecretKey = SodiumSealedPublicKeyBox.Create(EncryptionSecretKey,ServerSealedBoxEncryptionX25519PublicKeyBytes);
                        EncryptedMACSecretKey = SodiumSealedPublicKeyBox.Create(MACSecretKey, ServerSealedBoxMACX25519PublicKeyBytes);
                    }
                    else 
                    {
                        EncryptedEncryptionSecretKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(EncryptionSecretKey, ServerSealedBoxEncryptionX25519PublicKeyBytes);
                        EncryptedMACSecretKey = SodiumSealedPublicKeyBoxXChaCha20Poly1305.Create(MACSecretKey, ServerSealedBoxMACX25519PublicKeyBytes);
                    }
                }
                SodiumSecureMemory.SecureClearBytes(MACSecretKey);
                SodiumSecureMemory.SecureClearBytes(EncryptionSecretKey);
                SecretKeyCryptographyImportKeysDataModel MyModel = new SecretKeyCryptographyImportKeysDataModel();
                MyModel.EncryptedMACSecretKeyB64 = Convert.ToBase64String(EncryptedMACSecretKey);
                MyModel.EncryptedEncryptionSecretKeyB64 = Convert.ToBase64String(EncryptedEncryptionSecretKey);
                MyModel.IsKEMOrSealedBox = ServerSealedBoxAlgorithm.Equals("KEM");
                MyModel.IsXSalsa20Poly1305OrXChaCha20Poly1305 = IsXSalsa20Poly1305;
                MyModel.SignedChallengeB64 = "";
                MyModel.User_ID = User_ID;
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\" + User_ID + "APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/ImportKeys";
                String HTTPTypeString = "POST";
                String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                String IsRootOrSudoString = "Root";
                String ResultString = SecretKeyCryptoImportKeysHelper.SecretKeyCryptoImportKeys(JSONBodyString);
                SecondSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else
            {
                SecondSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
        else if(SecretKeyCryptographyOpsAppUIChooser == 3)
        {
            //User_ID? (Combobox)
            //Data to be encrypted?
            //Additional Data?
            //Data Type? (RB) - (Unicode - UTF8, Base64)
            //AES Algorithm? (RB) - (No,AES256GCM, AEGIS256, AEGIS128L) - Use Tag
            //AEAD Algorithm? (RB) - (No,XChaCha20Poly1305IETF, ChaCha20Poly1305IETF, ChaCha20Poly1305) - Use Tag
            //Stream Cipher Algorithm? (RB) - (No,XChaCha20, XSalsa20, ChaCha20, ChaCha20IETF, Salsa20, Salsa12, Salsa8) - Use Tag
            //MAC Algorithm? (RB) - (No,HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305) - Use Tag
            //----
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = ThirdSecretKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = ThirdSecretKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                String DataString = ThirdSecretKeyOpsAppTextBoxArray[0].Text;
                Byte[] DataStringBytes = new Byte[] { };
                String ActualDataString = "";
                String AdditionalDataString = ThirdSecretKeyOpsAppTextBoxArray[1].Text;
                Byte[] AdditionalDataStringBytes = new Byte[] { };
                String ActualAdditionalDataString = "";
                Boolean AbleToBeConvertFromB64String = true;
                Boolean IsUnicode = (Boolean)(ThirdSecretKeyOpsAppRadioButtonArray[0].IsChecked);
                if (DataString.CompareTo("") != 0)
                {
                    if (IsUnicode) 
                    {
                        DataStringBytes = Encoding.UTF8.GetBytes(DataString);
                        ActualDataString = Convert.ToBase64String(DataStringBytes);
                        if (AdditionalDataString!=null && AdditionalDataString.CompareTo("") != 0)
                        {
                            if (IsUnicode)
                            {
                                AdditionalDataStringBytes = Encoding.UTF8.GetBytes(AdditionalDataString);
                                ActualAdditionalDataString = Convert.ToBase64String(AdditionalDataStringBytes);
                            }
                            else
                            {
                                try
                                {
                                    AdditionalDataStringBytes = Convert.FromBase64String(AdditionalDataString);
                                    ActualAdditionalDataString = Convert.ToBase64String(AdditionalDataStringBytes);
                                }
                                catch
                                {
                                    AbleToBeConvertFromB64String = false;
                                }
                            }
                        }
                        else
                        {
                            ActualAdditionalDataString = "";
                        }
                        if (AbleToBeConvertFromB64String == false) 
                        {
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: The additional data is supposed to be base64 encoded but it isn't.";
                        }
                    }
                    else
                    {
                        try 
                        {
                            DataStringBytes = Convert.FromBase64String(DataString);
                            ActualDataString = Convert.ToBase64String(DataStringBytes);
                        }
                        catch 
                        {
                            AbleToBeConvertFromB64String = false;
                        }
                        if (AbleToBeConvertFromB64String) 
                        {
                            if (AdditionalDataString !=null && AdditionalDataString.CompareTo("") != 0)
                            {
                                if (IsUnicode)
                                {
                                    AdditionalDataStringBytes = Encoding.UTF8.GetBytes(AdditionalDataString);
                                    ActualAdditionalDataString = Convert.ToBase64String(AdditionalDataStringBytes);
                                }
                                else
                                {
                                    try
                                    {
                                        AdditionalDataStringBytes = Convert.FromBase64String(AdditionalDataString);
                                        ActualAdditionalDataString = Convert.ToBase64String(AdditionalDataStringBytes);
                                    }
                                    catch
                                    {
                                        AbleToBeConvertFromB64String = false;
                                    }
                                }
                            }
                            else
                            {
                                ActualAdditionalDataString = "";
                            }
                            if (AbleToBeConvertFromB64String == false)
                            {
                                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: The additional data is supposed to be base64 encoded but it isn't.";
                            }
                        }
                        else 
                        {
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: The data is supposed to be base64 encoded but it isn't.";
                        }
                    }
                    if (AbleToBeConvertFromB64String) 
                    {
                        int x = 2;
                        int AESAlgorithmIndex = -1;
                        int AEADAlgorithmIndex = -1;
                        int StreamCipherAlgorithmIndex = -1;
                        int MACAlgorithmIndex = -1;
                        while (x < 6)
                        {
                            if ((Boolean)(ThirdSecretKeyOpsAppRadioButtonArray[x].IsChecked)) 
                            {
                                AESAlgorithmIndex = (int)(ThirdSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        x = 6;
                        while (x < 10)
                        {
                            if ((Boolean)(ThirdSecretKeyOpsAppRadioButtonArray[x].IsChecked))
                            {
                                AEADAlgorithmIndex = (int)(ThirdSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        x = 10;
                        while (x < 18)
                        {
                            if ((Boolean)(ThirdSecretKeyOpsAppRadioButtonArray[x].IsChecked))
                            {
                                StreamCipherAlgorithmIndex = (int)(ThirdSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        x = 18;
                        while (x < 23)
                        {
                            if ((Boolean)(ThirdSecretKeyOpsAppRadioButtonArray[x].IsChecked))
                            {
                                MACAlgorithmIndex = (int)(ThirdSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        if (AESAlgorithmIndex != -1) 
                        {
                            SecretKeyCryptographyDataModel MyModel = new SecretKeyCryptographyDataModel();
                            MyModel.AESAlgorithmIndex = AESAlgorithmIndex;
                            MyModel.AdditionalDataB64 = ActualAdditionalDataString;
                            MyModel.Base64DataOrCipherText = ActualDataString;
                            MyModel.SignedChallengeB64 = "";
                            MyModel.User_ID = User_ID;
                            Byte[] DSAPrivateKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                            }
                            else
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                            }
                            Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                            Byte[] SignedChallengeBytes = new Byte[] { };
                            if (DSAPrivateKeyBytes.Length == 64)
                            {
                                SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                            }
                            else
                            {
                                SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                            }
                            MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                            String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                            String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography";
                            String HTTPTypeString = "POST";
                            String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                            String IsRootOrSudoString = "Sudo";
                            String ResultString = SecretKeyCryptoEncryptHelper.SecretKeyCryptoEncrypt(JSONBodyString);
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                        }
                        else if (AEADAlgorithmIndex != -1) 
                        {
                            SecretKeyCryptographyAEADDataModel MyModel = new SecretKeyCryptographyAEADDataModel();
                            MyModel.AEADAlgorithmIndex = AEADAlgorithmIndex;
                            MyModel.AdditionalDataB64 = ActualAdditionalDataString;
                            MyModel.Base64DataOrCipherText = ActualDataString;
                            MyModel.SignedChallengeB64 = "";
                            MyModel.User_ID = User_ID;
                            Byte[] DSAPrivateKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                            }
                            else
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                            }
                            Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                            Byte[] SignedChallengeBytes = new Byte[] { };
                            if (DSAPrivateKeyBytes.Length == 64)
                            {
                                SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                            }
                            else
                            {
                                SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                            }
                            MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                            String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                            String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/AEADEncrypt";
                            String HTTPTypeString = "POST";
                            String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                            String IsRootOrSudoString = "Sudo";
                            String ResultString = SecretKeyCryptoAEADEncryptHelper.SecretKeyCryptoAEADEncrypt(JSONBodyString);
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                        }
                        else 
                        {
                            SecretKeyCryptographyStreamCipherDataModel MyModel = new SecretKeyCryptographyStreamCipherDataModel();
                            MyModel.StreamCipherAlgorithmIndex = StreamCipherAlgorithmIndex;
                            MyModel.MACAlgorithmIndex = MACAlgorithmIndex;
                            MyModel.Base64DataOrCipherText = ActualDataString;
                            MyModel.SignedChallengeB64 = "";
                            MyModel.User_ID = User_ID;
                            Byte[] DSAPrivateKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                            }
                            else
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                            }
                            Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                            Byte[] SignedChallengeBytes = new Byte[] { };
                            if (DSAPrivateKeyBytes.Length == 64)
                            {
                                SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                            }
                            else
                            {
                                SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                            }
                            MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                            String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                            String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/StreamCipherEncrypt";
                            String HTTPTypeString = "POST";
                            String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                            String IsRootOrSudoString = "Sudo";
                            String ResultString = SecretKeyCryptoStreamCipherEncryptHelper.SecretKeyCryptoStreamCipherEncrypt(JSONBodyString);
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                            ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                        }
                    }
                }
                else
                {
                    ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet input data to be encrypted";
                }
            }
            else
            {
                ThirdSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual user id";
            }
        }
        else if(SecretKeyCryptographyOpsAppUIChooser == 4) 
        {
            //User_ID? (Combobox)
            //Cipher Text?
            //Additional Data?
            //Data Type? (RB) - (Unicode - UTF8, Base64)
            //AES Algorithm? (RB) - (No,AES256GCM, AEGIS256, AEGIS128L) - Use Tag
            //AEAD Algorithm? (RB) - (No,XChaCha20Poly1305IETF, ChaCha20Poly1305IETF, ChaCha20Poly1305) - Use Tag
            //Stream Cipher Algorithm? (RB) - (No,XChaCha20, XSalsa20, ChaCha20, ChaCha20IETF, Salsa20, Salsa12, Salsa8) - Use Tag
            //MAC Algorithm? (RB) - (No,HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305) - Use Tag
            //----
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FourthSecretKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = FourthSecretKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                String CipherTextString = FourthSecretKeyOpsAppTextBoxArray[0].Text;
                Byte[] CipherTextStringBytes = new Byte[] { };
                String ActualCipherTextString = "";
                String AdditionalDataString = FourthSecretKeyOpsAppTextBoxArray[1].Text;
                Byte[] AdditionalDataStringBytes = new Byte[] { };
                String ActualAdditionalDataString = "";
                Boolean AbleToBeConvertFromB64String = true;
                Boolean AbleToBeConvertFromB64AdditionalData = true;
                Boolean IsUnicode = (Boolean)(FourthSecretKeyOpsAppRadioButtonArray[0].IsChecked);
                if (CipherTextString.CompareTo("") != 0)
                {
                    try
                    {
                        CipherTextStringBytes = Convert.FromBase64String(CipherTextString);
                        ActualCipherTextString = Convert.ToBase64String(CipherTextStringBytes);
                    }
                    catch
                    {
                        AbleToBeConvertFromB64String = false;
                    }
                    if (AbleToBeConvertFromB64String)
                    {
                        if (AdditionalDataString != null && AdditionalDataString.CompareTo("") != 0)
                        {
                            if (IsUnicode)
                            {
                                AdditionalDataStringBytes = Encoding.UTF8.GetBytes(AdditionalDataString);
                                ActualAdditionalDataString = Convert.ToBase64String(AdditionalDataStringBytes);
                            }
                            else
                            {
                                try
                                {
                                    AdditionalDataStringBytes = Convert.FromBase64String(AdditionalDataString);
                                    ActualAdditionalDataString = Convert.ToBase64String(AdditionalDataStringBytes);
                                }
                                catch
                                {
                                    AbleToBeConvertFromB64AdditionalData = false;
                                }
                            }
                        }
                        else
                        {
                            ActualAdditionalDataString = "";
                        }
                        if (AbleToBeConvertFromB64AdditionalData == false)
                        {
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: The additional data is supposed to be base64 encoded but it isn't.";
                        }
                    }
                    else
                    {
                        FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: The data is supposed to be base64 encoded but it isn't.";
                    }
                    if (AbleToBeConvertFromB64String)
                    {
                        int x = 2;
                        int AESAlgorithmIndex = -1;
                        int AEADAlgorithmIndex = -1;
                        int StreamCipherAlgorithmIndex = -1;
                        int MACAlgorithmIndex = -1;
                        while (x < 6)
                        {
                            if ((Boolean)(FourthSecretKeyOpsAppRadioButtonArray[x].IsChecked))
                            {
                                AESAlgorithmIndex = (int)(FourthSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        x = 6;
                        while (x < 10)
                        {
                            if ((Boolean)(FourthSecretKeyOpsAppRadioButtonArray[x].IsChecked))
                            {
                                AEADAlgorithmIndex = (int)(FourthSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        x = 10;
                        while (x < 18)
                        {
                            if ((Boolean)(FourthSecretKeyOpsAppRadioButtonArray[x].IsChecked))
                            {
                                StreamCipherAlgorithmIndex = (int)(FourthSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        x = 18;
                        while (x < 23)
                        {
                            if ((Boolean)(FourthSecretKeyOpsAppRadioButtonArray[x].IsChecked))
                            {
                                MACAlgorithmIndex = (int)(FourthSecretKeyOpsAppRadioButtonArray[x].Tag);
                                break;
                            }
                            x += 1;
                        }
                        if (AESAlgorithmIndex != -1) 
                        {
                            SecretKeyCryptographyDataModel MyModel = new SecretKeyCryptographyDataModel();
                            MyModel.AdditionalDataB64 = ActualAdditionalDataString;
                            MyModel.AESAlgorithmIndex = AESAlgorithmIndex;
                            MyModel.Base64DataOrCipherText = ActualCipherTextString;
                            MyModel.SignedChallengeB64 = "";
                            MyModel.User_ID = User_ID;
                            Byte[] DSAPrivateKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                            }
                            else
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                            }
                            Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                            Byte[] SignedChallengeBytes = new Byte[] { };
                            if (DSAPrivateKeyBytes.Length == 64)
                            {
                                SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                            }
                            else
                            {
                                SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                            }
                            MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                            String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                            String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/Decrypt";
                            String HTTPTypeString = "POST";
                            String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                            String IsRootOrSudoString = "Sudo";
                            String ResultString = SecretKeyCryptoDecryptHelper.SecretKeyCryptoDecrypt(JSONBodyString);
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                        }
                        else if(AEADAlgorithmIndex != -1) 
                        {
                            SecretKeyCryptographyAEADDataModel MyModel = new SecretKeyCryptographyAEADDataModel();
                            MyModel.AdditionalDataB64 = ActualAdditionalDataString;
                            MyModel.AEADAlgorithmIndex = AEADAlgorithmIndex;
                            MyModel.Base64DataOrCipherText = ActualCipherTextString;
                            MyModel.SignedChallengeB64 = "";
                            MyModel.User_ID = User_ID;
                            Byte[] DSAPrivateKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                            }
                            else
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                            }
                            Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                            Byte[] SignedChallengeBytes = new Byte[] { };
                            if (DSAPrivateKeyBytes.Length == 64)
                            {
                                SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                            }
                            else
                            {
                                SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                            }
                            MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                            String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                            String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/AEADDecrypt";
                            String HTTPTypeString = "POST";
                            String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                            String IsRootOrSudoString = "Sudo";
                            String ResultString = SecretKeyCryptoAEADDecryptHelper.SecretKeyCryptoAEADDecrypt(JSONBodyString);
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                        }
                        else 
                        {
                            SecretKeyCryptographyStreamCipherDataModel MyModel = new SecretKeyCryptographyStreamCipherDataModel();
                            MyModel.StreamCipherAlgorithmIndex = StreamCipherAlgorithmIndex;
                            MyModel.MACAlgorithmIndex = MACAlgorithmIndex;
                            MyModel.Base64DataOrCipherText = ActualCipherTextString;
                            MyModel.SignedChallengeB64 = "";
                            MyModel.User_ID = User_ID;
                            Byte[] DSAPrivateKeyBytes = new Byte[] { };
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                            }
                            else
                            {
                                DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                            }
                            Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                            Byte[] SignedChallengeBytes = new Byte[] { };
                            if (DSAPrivateKeyBytes.Length == 64)
                            {
                                SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                            }
                            else
                            {
                                SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                            }
                            MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                            String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                            String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/StreamCipherDecrypt";
                            String HTTPTypeString = "POST";
                            String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                            String IsRootOrSudoString = "Sudo";
                            String ResultString = SecretKeyCryptoStreamCipherDecryptHelper.SecretKeyCryptoStreamCipherDecrypt(JSONBodyString);
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                            FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
                        }
                    }
                }
                else
                {
                    FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet input data to be encrypted";
                }
            }
            else
            {
                FourthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual user id";
            }
        }
        else if(SecretKeyCryptographyOpsAppUIChooser == 5) 
        {
            //(Export Secret Keys)
            //User_ID? (ComboBox)
            //Symmetric Encryption Algorithm? (RB) - (XSalsa20Poly1305,XChaCha20Poly1305)
            //------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Query Params
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FifthSecretKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = FifthSecretKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                Boolean IsXSalsa20Poly1305 = (Boolean)(FifthSecretKeyOpsAppRadioButtonArray[0].IsChecked);
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\" + User_ID + "APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/ExportSecretKeys?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(Convert.ToBase64String(SignedChallengeBytes))}&UseXSalsa20Poly1305={IsXSalsa20Poly1305}";
                String IsRootOrSudoString = "Root";
                String ResultString = SecretKeyCryptoExportSecretKeysHelper.SecretKeyCryptoExportSecretKeys(User_ID, Convert.ToBase64String(SignedChallengeBytes), IsXSalsa20Poly1305);
                FifthSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else
            {
                FifthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
        else if(SecretKeyCryptographyOpsAppUIChooser == 6) 
        {
            //(Extend Duration)
            //User_ID? (ComboBox)
            //------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Query Params
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = SixthSecretKeyOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = SixthSecretKeyOpsAppComboBoxArray[0].SelectedItem.ToString();
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\" + User_ID + "APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "SecretKeyCryptography/ExtendDuration?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(Convert.ToBase64String(SignedChallengeBytes))}";
                String IsRootOrSudoString = "Root";
                String ResultString = SecretKeyCryptoExtendDurationHelper.SecretKeyCryptoExtendDuration(User_ID, Convert.ToBase64String(SignedChallengeBytes));
                SixthSecretKeyOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else
            {
                SixthSecretKeyOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
    }

    private void ArweaveOpsAppBTN_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (ArweaveOpsAppUIChooser == 1) 
        {
            //User_ID? (ComboBox)
            //JSON Data/Data String? 
            //------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Request Body (JSON)
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FirstArweaveOpsAppComboBoxArray[0].SelectedIndex;
            String DataString = FirstArweaveOpsAppTextBoxArray[0].Text;
            if (User_ID_Selected_Index != -1 && DataString.CompareTo("")!=0)
            {
                String User_ID = FirstArweaveOpsAppComboBoxArray[0].SelectedItem.ToString();
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\SubDSAPrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/SubDSAPrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                ArweaveRSAOpsDataModel MyModel = new ArweaveRSAOpsDataModel();
                MyModel.User_ID = User_ID;
                MyModel.SignedChallengeB64 = Convert.ToBase64String(SignedChallengeBytes);
                MyModel.JSONDataString = DataString;
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "ArweaveRSAOps";
                String HTTPTypeString = "POST";
                String JSONBodyString = JsonConvert.SerializeObject(MyModel);
                String IsRootOrSudoString = "Sudo";
                String ResultString = ArweaveUploadDataHelper.ArweaveUploadData(JSONBodyString);
                FirstArweaveOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                FirstArweaveOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                FirstArweaveOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                FirstArweaveOpsAppDeveloperTextBoxArray[3].Text = JSONBodyString;
                FirstArweaveOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                FirstArweaveOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else
            {
                FirstArweaveOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID or input valid data to be uploaded";
            }
        }
    }

    private void SHSMOpsAppBTN_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        if (SHSMOpsAppUIChooser == 1) 
        {
            //User_ID? (ComboBox)
            //------
            //Server API IP Address
            //Server full API IP address
            //HTTP Type (Post/Get/..)
            //Query Params
            //Root/Sudo?
            //Status..
            int User_ID_Selected_Index = FirstSHSMOpsAppComboBoxArray[0].SelectedIndex;
            if (User_ID_Selected_Index != -1)
            {
                String User_ID = FirstSHSMOpsAppComboBoxArray[0].SelectedItem.ToString();
                Byte[] DSAPrivateKeyBytes = new Byte[] { };
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "\\" + User_ID + "APrivateKey.txt");
                }
                else
                {
                    DSAPrivateKeyBytes = File.ReadAllBytes(UsersRootFolder + User_ID + "/" + User_ID + "APrivateKey.txt");
                }
                Byte[] ChallengeBytes = GetChallengeForSHSMRegisteredUserHelper.GetChallenge(User_ID);
                Byte[] SignedChallengeBytes = new Byte[] { };
                if (DSAPrivateKeyBytes.Length == 64)
                {
                    SignedChallengeBytes = SodiumPublicKeyAuth.Sign(ChallengeBytes, DSAPrivateKeyBytes, true);
                }
                else
                {
                    SignedChallengeBytes = SecureED448.GenerateSignatureMessage(DSAPrivateKeyBytes, ChallengeBytes, new Byte[] { }, true);
                }
                String ServerAPIIPAddress = APIIPAddressHelper.IPAddress;
                String ServerFullAPIIPAddress = ServerAPIIPAddress + "SHSMOps?";
                String HTTPTypeString = "GET";
                String URLQueryParams = $"User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(Convert.ToBase64String(SignedChallengeBytes))}";
                String IsRootOrSudoString = "Root";
                String ResultString = DeleteSHSMHelper.DeleteSHSM(User_ID, Convert.ToBase64String(SignedChallengeBytes));
                FirstSHSMOpsAppDeveloperTextBoxArray[0].Text = ServerAPIIPAddress;
                FirstSHSMOpsAppDeveloperTextBoxArray[1].Text = ServerFullAPIIPAddress;
                FirstSHSMOpsAppDeveloperTextBoxArray[2].Text = HTTPTypeString;
                FirstSHSMOpsAppDeveloperTextBoxArray[3].Text = URLQueryParams;
                FirstSHSMOpsAppDeveloperTextBoxArray[4].Text = IsRootOrSudoString;
                FirstSHSMOpsAppDeveloperTextBoxArray[5].Text = ResultString;
            }
            else
            {
                FirstSHSMOpsAppDeveloperTextBoxArray[5].Text = "Error: You have not yet select an actual User ID";
            }
        }
    }

    private void RegistrationOpsAppLoadUserIDs()
    {
        if (RegistrationOpsAppUIChooser == 2)
        {
            String[] User_IDWithDirectoryPath = Directory.GetDirectories(UsersRootFolder);
            String[] User_ID = new String[User_IDWithDirectoryPath.Length];
            int Loop = 0;
            while (Loop < User_IDWithDirectoryPath.Length)
            {
                User_ID[Loop] = User_IDWithDirectoryPath[Loop].Remove(0, UsersRootFolder.Length);
                Loop += 1;
            }
            SecondRegistrationOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                SecondRegistrationOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
    }

    private void ETLSOpsAppLoadUserIDs()
    {
        String[] User_IDWithDirectoryPath = Directory.GetDirectories(UsersRootFolder);
        String[] User_ID = new String[User_IDWithDirectoryPath.Length];
        int Loop = 0;
        while (Loop < User_IDWithDirectoryPath.Length)
        {
            User_ID[Loop] = User_IDWithDirectoryPath[Loop].Remove(0, UsersRootFolder.Length);
            Loop += 1;
        }
        if (ETLSOpsAppUIChooser == 1)
        {
            FirstETLSOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FirstETLSOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if(ETLSOpsAppUIChooser == 2) 
        {
            SecondETLSOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                SecondETLSOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
    }

    private void PublicKeyCryptographyOpsAppLoadUserIDs()
    {
        String[] User_IDWithDirectoryPath = Directory.GetDirectories(UsersRootFolder);
        String[] User_ID = new String[User_IDWithDirectoryPath.Length];
        int Loop = 0;
        while (Loop < User_IDWithDirectoryPath.Length)
        {
            User_ID[Loop] = User_IDWithDirectoryPath[Loop].Remove(0, UsersRootFolder.Length);
            Loop += 1;
        }
        if (PublicKeyCryptographyOpsAppUIChooser == 1)
        {
            FirstPublicKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FirstPublicKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 2)
        {
            SecondPublicKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                SecondPublicKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 3)
        {
            ThirdPublicKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                ThirdPublicKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 4)
        {
            FourthPublicKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FourthPublicKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 5)
        {
            FifthPublicKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FifthPublicKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 6)
        {
            SixthPublicKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                SixthPublicKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (PublicKeyCryptographyOpsAppUIChooser == 7)
        {
            SeventhPublicKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                SeventhPublicKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
    }

    private void SecretKeyCryptographyOpsAppLoadUserIDs()
    {
        String[] User_IDWithDirectoryPath = Directory.GetDirectories(UsersRootFolder);
        String[] User_ID = new String[User_IDWithDirectoryPath.Length];
        int Loop = 0;
        while (Loop < User_IDWithDirectoryPath.Length)
        {
            User_ID[Loop] = User_IDWithDirectoryPath[Loop].Remove(0, UsersRootFolder.Length);
            Loop += 1;
        }
        if (SecretKeyCryptographyOpsAppUIChooser == 1)
        {
            FirstSecretKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FirstSecretKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 2)
        {
            SecondSecretKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                SecondSecretKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 3)
        {
            ThirdSecretKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                ThirdSecretKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 4)
        {
            FourthSecretKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FourthSecretKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 5)
        {
            FifthSecretKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FifthSecretKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
        else if (SecretKeyCryptographyOpsAppUIChooser == 6)
        {
            SixthSecretKeyOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                SixthSecretKeyOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
    }

    private void ArweaveOpsAppLoadUserIDs()
    {
        String[] User_IDWithDirectoryPath = Directory.GetDirectories(UsersRootFolder);
        String[] User_ID = new String[User_IDWithDirectoryPath.Length];
        int Loop = 0;
        while (Loop < User_IDWithDirectoryPath.Length)
        {
            User_ID[Loop] = User_IDWithDirectoryPath[Loop].Remove(0, UsersRootFolder.Length);
            Loop += 1;
        }
        if (ArweaveOpsAppUIChooser == 1)
        {
            FirstArweaveOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FirstArweaveOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
    }

    private void SHSMOpsAppLoadUserIDs()
    {
        String[] User_IDWithDirectoryPath = Directory.GetDirectories(UsersRootFolder);
        String[] User_ID = new String[User_IDWithDirectoryPath.Length];
        int Loop = 0;
        while (Loop < User_IDWithDirectoryPath.Length)
        {
            User_ID[Loop] = User_IDWithDirectoryPath[Loop].Remove(0, UsersRootFolder.Length);
            Loop += 1;
        }
        if (SHSMOpsAppUIChooser == 1)
        {
            FirstSHSMOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length)
            {
                FirstSHSMOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
    }

    /*
    private void RegistrationOpsAppComboBoxSelectedItemsChanged(object? sender, SelectionChangedEventArgs e) 
    {
        if (AUOpsAppUIChooser == 1) 
        {
            if (SecondAUOpsAppComboBoxArray[0].SelectedIndex != -1) 
            {
                SecondAUOpsAppTextBoxArray[0].Text = SecondAUOpsAppComboBoxArray[0].SelectedItem.ToString();
            }
        }
        else if (AUOpsAppUIChooser == 2) 
        {
            if (ThirdAUOpsAppComboBoxArray[0].SelectedIndex != -1)
            {
                ThirdAUOpsAppTextBoxArray[0].Text = ThirdAUOpsAppComboBoxArray[0].SelectedItem.ToString();
            }
        }
    }

    private void RegistrationOpsAppLoadUserIDs()
    { 
        if (RegistrationOpsAppUIChooser==2) 
        {
            String[] User_IDWithDirectoryPath = Directory.GetDirectories(UsersRootFolder);
            String[] User_ID = new String[User_IDWithDirectoryPath.Length];
            int Loop = 0;
            while (Loop < User_IDWithDirectoryPath.Length) 
            {
                User_ID[Loop] = User_IDWithDirectoryPath[Loop].Remove(0, UsersRootFolder.Length);
                Loop += 1;
            }
            SecondRegistrationOpsAppComboBoxArray[0].Items.Clear();
            Loop = 0;
            while (Loop < User_ID.Length) 
            {
                SecondRegistrationOpsAppComboBoxArray[0].Items.Add(User_ID[Loop]);
                Loop += 1;
            }
        }
    }
    */
}
