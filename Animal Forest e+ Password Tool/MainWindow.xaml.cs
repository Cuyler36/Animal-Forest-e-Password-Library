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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum CodeType : int
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

        public enum MonumentType : int
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

        public MainWindow()
        {
            InitializeComponent();
            //string Result = Encoder.Encode(7, 0, "Town", "Abc", "12600", 0, 0x11); //Encoder.Encode(4, 1, "Hyrule", "Wes", "Abc", 0x30BC, 0);
            string Result = MakeMonumentCode((int)MonumentType.Mushroom, 2, 4, "Town", "Abc", "0");
            //Console.WriteLine(Result);
            EncoderResultTextBox.Text = Result;
        }

        private string MakeMonumentCode(int MonumentType, int AcreY, int AcreX, string TownName, string Recipient, string Price)
        {
            return Encoder.Encode(7, 0, TownName, Recipient, Price, (ushort)(MonumentType % 15), ((AcreY & 7) << 3) | (AcreX & 7));
        }

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
            byte A = Data[0];
            int X = (A >> 5) & 7;
            int Y = (A << 2) & 0x0C;
            int U = Y | ((Data[1] >> 6) & 3);
            int r28 = Data[2];
            ushort Unknown = (ushort)((Data[15] << 8) | Data[16]);
            ushort PresentItemId = (ushort)((Data[21] << 8) | Data[22]);

            // TODO: Figure out Acre X & Y coordinates for monument

            string TownName = ByteArrayToString(Data, 3, 6);
            string PlayerName = ByteArrayToString(Data, 9, 6);
            string SenderString = ByteArrayToString(Data, 15, 6);

            Console.WriteLine("Code Type: " + X);
            CodeTypeLabel.Content = "Code Type: " + CodeTypes[X];

            if (X == 7 && uint.TryParse(SenderString, out uint Price)) // 7 = Monument (Town Decoration)
            {
                int AcreX = Data[1] & 7;
                int AcreY = (Data[1] >> 3) & 7;
                Console.WriteLine(string.Format("Town Name: {0}\r\nPlayer Name: {1}\r\nDecoration Price: {2}\r\nTown Decoration: {6} [0x{3}]\r\nPlacement Acre [Y-X]: {4}-{5}",
                    TownName, PlayerName, Price.ToString("#,##0"),
                    PresentItemId < 0x5853 ? (PresentItemId + 0x5853).ToString("X4") : PresentItemId.ToString("X4"), AcreY, AcreX, MonumentNames[PresentItemId % 15]));

                String1Label.Content = "Recipient's Town Name: " + TownName;
                String2Label.Content = "Recipient's Name: " + PlayerName;
                String3Label.Content = "Town Decoration: " + MonumentNames[PresentItemId % 15];
                String4Label.Content = "Price: " + Price.ToString("#,##0") + " Bells";
                String5Label.Content = "Placement Acre: " + AcreY + "-" + AcreX;
            }
            else
            {
                Console.WriteLine(string.Format("Town Name: {0}\r\nPlayer Name: {1}\r\nSender Name: {2}\r\nSent Item ID: 0x{3}",
                    TownName, PlayerName, SenderString, PresentItemId.ToString("X4")));
            }

            int CodeType = 0;

            if (X == 7)
            {
                CodeType = (A >> 2) & 7;
                // There's more here under mMpswd_new_password
                int AcreX = Data[1] & 7;
                int AcreY = (Data[1] >> 3) & 7;
            }
            else if (X >= 2 && X < 7)
            {
                CodeType = (A >> 2) & 3;
            }
            else // 3 is included in this since it's the same as the default route
            {
                CodeType = (A >> 2) & 7;
            }
        }
    }
}
