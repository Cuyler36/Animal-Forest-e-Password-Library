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

using PasswordLibrary.Encoder;

namespace Animal_Forest_e__Password_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Code Types:
        // 4 = Player-to-Player
        // 7 = Monument (Town Decoration) [From Object Delivery Service]

        public MainWindow()
        {
            InitializeComponent();
            //string Result = Encoder.Encode(7, 0, "Town", "Abc", "12600", 0, 0x11); //Encoder.Encode(4, 1, "Hyrule", "Wes", "Abc", 0x30BC, 0);
            string Result = MakeMonumentCode(8, 3, 2, "Town", "Abc", "1000");
            Console.WriteLine(Result);
            EncoderResultTextBox.Text = Result;
        }

        private string MakeMonumentCode(int MonumentType, int AcreY, int AcreX, string TownName, string Receipiant, string Price)
        {
            return Encoder.Encode(7, 0, TownName, Receipiant, Price, (ushort)(MonumentType % 15), ((AcreY & 7) << 3) | (AcreX & 7));
        }
    }
}
