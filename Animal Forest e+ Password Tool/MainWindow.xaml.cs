using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PasswordLibrary.Decoder;
using PasswordLibrary.Encoder;

namespace Animal_Forest_e__Password_Tool
{
    public enum CodeType
    {
        Famicom = 0, // NES
        NPC = 1, // Original NPC Code
        Card_E = 2, // NOTE: This is a stubbed method (just returns 4)
        Magazine = 3, // Contest?
        User = 4, // Player-to-Player
        Card_E_Mini = 5, // Only one data strip?
        New_NPC = 6, // Using the new password system?
        Monument = 7 // Town Decorations (from Object Delivery Service, see: https://www.nintendo.co.jp/ngc/gaej/obje/)
    }

    public enum MonumentType
    {
        ParkClock = 0,
        GasLamp = 1,
        Windpump = 2,
        FlowerClock = 3,
        Heliport = 4,
        WindTurbine = 5,
        PipeStack = 6,
        Stonehenge = 7,
        Egg = 8,
        Footprints = 9,
        Geoglyph = 10,
        Mushroom = 11,
        Signpost = 12,
        Well = 13,
        Fountain = 14
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CodeTypeComboBox.SelectionChanged += CodeTypeComboBox_SelectionChanged;
        }

        // Encoder Code \\

        private string MakeMonumentCode(int MonumentType, int AcreY, int AcreX, string TownName, string Recipient, string Price)
        {
            return Encoder.Encode((int)CodeType.Monument, 0, TownName, Recipient, Price, (ushort)(MonumentType % 15), ((AcreY & 7) << 3) | (AcreX & 7));
        }

        private string MakeUserCode(string TownName, string Recipient, string Sender, ushort ItemId)
        {
            Console.WriteLine("Town Name: " + TownName);
            Console.WriteLine("Recipient Name: " + Recipient);
            Console.WriteLine("Sender Name: " + Sender);
            Console.WriteLine("Item ID: " + ItemId.ToString("X4"));
            return Encoder.Encode((int)CodeType.User, 1, TownName, Recipient, Sender, ItemId, 0);
        }

        private string MakeMagazineCode(string SenderTownName, string Sender, string Unknown, ushort ItemId)
        {
            return Encoder.Encode((int)CodeType.Magazine, 0, SenderTownName, Sender, Unknown, ItemId, 0);
        }

        // Decoder Code \\

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DecoderTextBox.Text))
            {
                string SecretCode = DecoderTextBox.Text.Replace("\r", "").Replace("\n", "");
                if (ContainsInvalidCharacters(SecretCode) == false)
                {
                    // TODO: Add some way of displaying the info
                    byte[] DecoderResult = Decoder.Decode(SecretCode);
                    ParseDecodedPassword(DecoderResult);
                }
            }
        }

        private bool ContainsInvalidCharacters(string Text)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                if (Array.IndexOf(PasswordLibrary.Common.AFe_CharList, Text.Substring(i, 1)) < 0)
                {
                    return true;
                }
            }
            return false;
        }

        private readonly string[] MonumentNames = new string[15]
        {
            "Park Clock", "Gas Lamp", "Windpump", "Flower Clock", "Heliport",
            "Wind Turbine", "Pipe Stack", "Stonehenge", "Egg", "Footprints",
            "Geoglyph", "Mushroom", "Signpost", "Well", "Fountain"
        };

        private readonly string[] CodeTypes = new string[8]
        {
            "Famicom", "Villager (Old)", "E-Reader+ Card", "Magazine", "Player-to-Player", "E-Reader+ Card (Mini)", "Villager", "Object Delivery Service"
        };

        private void PrintByteArray(byte[] Input)
        {
            for (int i = 0; i < Input.Length; i++)
            {
                Console.Write(Input[i].ToString("X2") + " ");
            }
            Console.Write("\r\n\r\n");
        }

        private string ByteArrayToString(byte[] Data, int StartIdx = 0, int Length = -1)
        {
            Length = Length == -1 ? Data.Length : Length;

            string Output = "";
            for (int i = StartIdx; i < StartIdx + Length; i++)
            {
                Output += PasswordLibrary.Common.AFe_CharList[Data[i]];
            }

            return Output;
        }

        private void ParseDecodedPassword(byte[] Data)
        {
            var A = Data[0];
            var X = (A >> 5) & 7;
            var Y = (A << 2) & 0x0C;
            var U = Y | ((Data[1] >> 6) & 3);
            var r28 = Data[2];
            var Unknown = (ushort)((Data[15] << 8) | Data[16]);
            var PresentItemId = (ushort)((Data[21] << 8) | Data[22]);

            // TODO: Figure out Acre X & Y coordinates for monument

            var TownName = ByteArrayToString(Data, 3, 6);
            var PlayerName = ByteArrayToString(Data, 9, 6);
            var SenderString = ByteArrayToString(Data, 15, 6);

            Console.WriteLine("Code Type: " + X);
            CodeTypeLabel.Content = "Code Type: " + CodeTypes[X];

            var type = (CodeType)X;

            switch (type)
            {
                // 7 = Monument (Town Decoration)
                case CodeType.Monument when uint.TryParse(SenderString, out var price):
                    int AcreX = Data[1] & 7;
                    int AcreY = (Data[1] >> 3) & 7;
                    Console.WriteLine(string.Format("Town Name: {0}\r\nPlayer Name: {1}\r\nDecoration Price: {2}\r\nTown Decoration: {6} [0x{3}]\r\nPlacement Acre [Y-X]: {4}-{5}",
                        TownName, PlayerName, price.ToString("#,##0"),
                        PresentItemId < 0x5853 ? (PresentItemId + 0x5853).ToString("X4") : PresentItemId.ToString("X4"), AcreY, AcreX, MonumentNames[PresentItemId % 15]));

                    String1Label.Content = "Recipient's Town Name: " + TownName;
                    String2Label.Content = "Recipient's Name: " + PlayerName;
                    String3Label.Content = "Town Decoration: " + MonumentNames[PresentItemId % 15];
                    String4Label.Content = "Price: " + price.ToString("#,##0") + " Bells";
                    String5Label.Content = "Placement Acre: " + AcreY + "-" + AcreX;
                    break;
                case CodeType.User:
                case CodeType.Magazine:
                    Console.WriteLine(string.Format("Town Name: {0}\r\nPlayer Name: {1}\r\nSender Name: {2}\r\nSent Item ID: 0x{3}",
                        TownName, PlayerName, SenderString, PresentItemId.ToString("X4")));

                    String1Label.Content = "Recipient's Town Name: " + TownName;
                    String2Label.Content = "Recipient's Name: " + PlayerName;
                    String3Label.Content = "Sender's Name: " + SenderString;
                    String4Label.Content = "Item ID: 0x" + PresentItemId.ToString("X4");
                    String5Label.Content = "";
                    break;
            }

            int CodeTypeValue = 0;

            if (X == 7)
            {
                CodeTypeValue = (A >> 2) & 7;
                // There's more here under mMpswd_new_password
                int AcreX = Data[1] & 7;
                int AcreY = (Data[1] >> 3) & 7;
            }
            else if (X >= 2 && X < 7)
            {
                CodeTypeValue = (A >> 2) & 3;
            }
            else // 3 is included in this since it's the same as the default route
            {
                CodeTypeValue = (A >> 2) & 7;
            }
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeTypeComboBox.SelectedIndex <= -1 || CodeTypeComboBox.SelectedIndex >= 8) return;

            var passwordCodeType = (CodeType) CodeTypeComboBox.SelectedIndex;
            switch (passwordCodeType)
            {
                case CodeType.Magazine when ushort.TryParse(ItemIdTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out var itemId):
                    EncoderResultTextBox.Text = MakeMagazineCode(PadAFString(TownNameTextBox.Text), PadAFString(RecipientTextBox.Text),
                        PadAFString(SenderTextBox.Text), itemId);
                    break;
                case CodeType.User when ushort.TryParse(ItemIdTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out var itemId):
                    EncoderResultTextBox.Text = MakeUserCode(PadAFString(TownNameTextBox.Text), PadAFString(RecipientTextBox.Text),
                        PadAFString(SenderTextBox.Text), itemId);
                    break;
                case CodeType.Monument when DecorationComboBox.SelectedIndex > -1 && DecorationComboBox.SelectedIndex < 15 &&
                                            int.TryParse(YAcreTextBox.Text, out var acreY) && int.TryParse(XAcreTextBox.Text, out var acreX):
                    EncoderResultTextBox.Text = MakeMonumentCode(DecorationComboBox.SelectedIndex, acreY, acreX,
                        PadAFString(TownNameTextBox.Text), PadAFString(RecipientTextBox.Text), PadAFString(DecorationPriceTextBox.Text));
                    break;
            }
        }

        // ReSharper disable once InconsistentNaming
        private static string PadAFString(string input)
        {
            while (input.Length < 6)
            {
                input += " ";
            }
            return input;
        }

        private void CodeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CodeTypeComboBox.SelectedIndex <= -1) return;

            switch ((CodeType) CodeTypeComboBox.SelectedIndex)
            {
                case CodeType.Magazine:
                    Label2.Content = "Sender's Town Name:";
                    Label3.Content = "Sender's Name:";
                    Label4.Content = "Unknown:";
                    DecorationComboBox.Visibility = DecorationPriceTextBox.Visibility = Visibility.Hidden;
                    SenderTextBox.Visibility = ItemIdTextBox.Visibility = Visibility.Visible;
                    Label6.Visibility = Label7.Visibility = Visibility.Hidden;
                    XAcreTextBox.Visibility = YAcreTextBox.Visibility = Visibility.Hidden;
                    break;
                case CodeType.User:
                    Label2.Content = "Recipient's Town Name:";
                    Label3.Content = "Recipient's Name:";
                    Label4.Content = "Sender's Name:";
                    Label5.Content = "Item ID:";
                    DecorationComboBox.Visibility = DecorationPriceTextBox.Visibility = Visibility.Hidden;
                    SenderTextBox.Visibility = ItemIdTextBox.Visibility = Visibility.Visible;
                    Label6.Visibility =  Label7.Visibility = Visibility.Hidden;
                    XAcreTextBox.Visibility = YAcreTextBox.Visibility = Visibility.Hidden;
                    break;
                case CodeType.Monument:
                    Label2.Content = "Recipient's Town Name:";
                    Label3.Content = "Recipient's Name:";
                    Label4.Content = "Decoration:";
                    Label5.Content = "Price:";
                    DecorationComboBox.Visibility = DecorationPriceTextBox.Visibility = Visibility.Visible;
                    SenderTextBox.Visibility = ItemIdTextBox.Visibility = Visibility.Hidden;
                    Label6.Visibility = Label7.Visibility = Visibility.Visible;
                    XAcreTextBox.Visibility = YAcreTextBox.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
