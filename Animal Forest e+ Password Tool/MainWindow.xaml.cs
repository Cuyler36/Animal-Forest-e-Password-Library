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

        private string MakeMonumentCode(int MonumentType, int AcreY, int AcreX, string TownName, string Receipiant, string Price)
        {
            return Encoder.Encode(7, 0, TownName, Receipiant, Price, (ushort)(MonumentType % 15), ((AcreY & 7) << 3) | (AcreX & 7));
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
    }
}
