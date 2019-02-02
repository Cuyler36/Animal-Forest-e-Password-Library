using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using PasswordLibrary;
using PasswordLibrary.Decoder;
using PasswordLibrary.Encoder;

namespace Animal_Forest_e__Password_Tool
{
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

    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly int[] HitRateIndexAdjust = { 3, 2, 1, 0, 4 };

        public MainWindow()
        {
            InitializeComponent();
            CodeTypeComboBox.SelectionChanged += CodeTypeComboBox_SelectionChanged;
        }

        // Encoder Code \\

        private static string MakeMonumentCode(int monumentType, int acreY, int acreX, string townName,
            string recipient, string price) => Encoder.Encode(CodeType.Monument, 0, townName, recipient, price,
            (ushort) (monumentType % 15), ((acreY & 7) << 3) | (acreX & 7));

        private static string MakeUserCode(string townName, string recipient, string sender, ushort itemId) =>
            Encoder.Encode(CodeType.User, 1, townName, recipient, sender, itemId, 0);

        private static string MakeMagazineCode(string senderTownName, string sender, string unknown, ushort itemId, int hitRateIndex) =>
            Encoder.Encode(CodeType.Magazine, hitRateIndex, senderTownName, sender, unknown, itemId, 0);

        private static string MakeNewNpcCode(string recepiantTownName, string recepiantName, string senderName, ushort itemId) =>
            Encoder.Encode(CodeType.New_NPC, 0, recepiantTownName, recepiantName, senderName, itemId, 0);

        private static string MakeFamicomCode(string recepiantTownName, string recepiantName, string senderName, ushort itemId) =>
            Encoder.Encode(CodeType.Famicom, 0, recepiantTownName, recepiantName, senderName, itemId, 0);

        //private static string MakeCardECode(string )

        // Decoder Code \\

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DecoderTextBox.Text)) return;

            var secretCode = DecoderTextBox.Text.Replace("\r", "").Replace("\n", "");
            if (ContainsInvalidCharacters(secretCode)) return;

            // TODO: Add some way of displaying the info
            var decoderResult = Decoder.Decode(secretCode);
            ParseDecodedPassword(decoderResult);
        }

        private static bool ContainsInvalidCharacters(string text) => text.Where((t, i) =>
            Array.IndexOf(PasswordLibrary.Common.AFe_CharList, text.Substring(i, 1)) < 0).Any();

        private static readonly string[] MonumentNames = {
            "Park Clock", "Gas Lamp", "Windpump", "Flower Clock", "Heliport",
            "Wind Turbine", "Pipe Stack", "Stonehenge", "Egg", "Footprints",
            "Geoglyph", "Mushroom", "Signpost", "Well", "Fountain"
        };

        private static void PrintByteArray(IEnumerable<byte> input)
        {
            foreach (var b in input)
            {
                Console.Write(b.ToString("X2") + " ");
            }

            Console.Write("\r\n\r\n");
        }

        private static void mMpswd_new_password(in byte[] passwordData)
        {

        }

        private static string ByteArrayToString(IReadOnlyList<byte> data, int startIdx = 0, int length = -1)
        {
            length = length == -1 ? data.Count : length;

            var output = "";
            for (var i = startIdx; i < startIdx + length; i++)
            {
                output += Common.AFe_CharList[data[i]];
            }

            return output;
        }

        private void ParseDecodedPassword(byte[] data)
        {
            var codeType = (CodeType) ((data[0] >> 5) & 7);
            var checksum = ((data[0] << 2) & 0x0C) | ((data[1] >> 6) & 3);
            var r28 = data[2];
            var unknown = (ushort)((data[15] << 8) | data[16]);
            var presentItemId = (ushort)((data[21] << 8) | data[22]);

            // TODO: Figure out Acre X & Y coordinates for monument

            var townName = ByteArrayToString(data, 3, 6);
            var playerName = ByteArrayToString(data, 9, 6);
            var senderString = ByteArrayToString(data, 15, 6);

            Console.WriteLine("Code Type: " + codeType);
            CodeTypeLabel.Content = "Code Type: " + codeType;

            switch (codeType)
            {
                case CodeType.Monument when uint.TryParse(senderString, out var price):
                    var acreX = data[1] & 7;
                    var acreY = (data[1] >> 3) & 7;
                    Console.WriteLine(
                        "Town Name: {0}\r\nPlayer Name: {1}\r\nDecoration Price: {2:#,##0}\r\nTown Decoration: {6} [0x{3}]\r\nPlacement Acre [Y-X]: {4}-{5}",
                        townName, playerName, price,
                        presentItemId < 0x5853 ? (presentItemId + 0x5853).ToString("X4") : presentItemId.ToString("X4"),
                        acreY, acreX, MonumentNames[presentItemId % 15]);

                    String1Label.Content = "Recipient's Town Name: " + townName;
                    String2Label.Content = "Recipient's Name: " + playerName;
                    String3Label.Content = "Town Decoration: " + MonumentNames[presentItemId % 15];
                    String4Label.Content = "Price: " + price.ToString("#,##0") + " Bells";
                    String5Label.Content = "Placement Acre: " + acreY + "-" + acreX;
                    break;
                case CodeType.User:
                case CodeType.Magazine:
                    Console.WriteLine(
                        $"Town Name: {townName}\r\nPlayer Name: {playerName}\r\nSender Name: {senderString}\r\nSent Item ID: 0x{presentItemId:X4}");

                    String1Label.Content = "Recipient's Town Name: " + townName;
                    String2Label.Content = "Recipient's Name: " + playerName;
                    String3Label.Content = "Sender's Name: " + senderString;
                    String4Label.Content = "Item ID: 0x" + presentItemId.ToString("X4");
                    String5Label.Content = "";
                    break;
            }

            var codeTypeValue = 0;

            switch (codeType)
            {
                case CodeType.Famicom:
                case CodeType.NPC:
                case CodeType.Magazine:
                    codeTypeValue = (data[0] >> 2) & 7;
                    break;
                case CodeType.Card_E:
                case CodeType.Card_E_Mini:
                case CodeType.User:
                case CodeType.New_NPC:
                    codeTypeValue = (data[0] >> 2) & 3;
                    break;
                case CodeType.Monument:
                    codeTypeValue = (data[0] >> 2) & 7;
                    break;
            }
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeTypeComboBox.SelectedIndex <= -1 || CodeTypeComboBox.SelectedIndex >= 8) return;

            var passwordCodeType = (CodeType) CodeTypeComboBox.SelectedIndex;
            switch (passwordCodeType)
            {
                case CodeType.Magazine when ushort.TryParse(ItemIdTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out var itemId) &&
                                            HitRateComboBox.SelectedIndex > -1 && HitRateComboBox.SelectedIndex < 5:
                    EncoderResultTextBox.Text = MakeMagazineCode(PadAFString(TownNameTextBox.Text), PadAFString(RecipientTextBox.Text),
                        PadAFString(SenderTextBox.Text), itemId, HitRateIndexAdjust[HitRateComboBox.SelectedIndex]);
                    break;
                case CodeType.Famicom when ushort.TryParse(ItemIdTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out var itemId) && itemId >= 0x1DE8 && itemId <= 0x1E23:
                case CodeType.User when ushort.TryParse(ItemIdTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out itemId):
                    EncoderResultTextBox.Text = MakeUserCode(PadAFString(TownNameTextBox.Text), PadAFString(RecipientTextBox.Text),
                        PadAFString(SenderTextBox.Text), itemId);
                    break;
                case CodeType.Famicom when ushort.TryParse(ItemIdTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out var itemId) && itemId < 0x1DE8 || itemId > 0x1E23:
                    MessageBox.Show(
                        "The item id you entered is invalid!\nFamicom passwords can only accept NES item ids, which are in the range of 0x1DE8 - 0x1E23!",
                        "Famicom Password Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    Label5.Content = "Item ID:";
                    DecorationComboBox.Visibility = DecorationPriceTextBox.Visibility = Visibility.Hidden;
                    SenderTextBox.Visibility = ItemIdTextBox.Visibility = Visibility.Visible;
                    Label6.Visibility = Label7.Visibility = Visibility.Hidden;
                    XAcreTextBox.Visibility = YAcreTextBox.Visibility = Visibility.Hidden;
                    HitRateLabel.Visibility = HitRateComboBox.Visibility = Visibility.Visible;
                    break;
                case CodeType.Famicom:
                case CodeType.User:
                case CodeType.New_NPC:
                    Label2.Content = "Recipient's Town Name:";
                    Label3.Content = "Recipient's Name:";
                    Label4.Content = "Sender's Name:";
                    Label5.Content = "Item ID:";
                    DecorationComboBox.Visibility = DecorationPriceTextBox.Visibility = Visibility.Hidden;
                    SenderTextBox.Visibility = ItemIdTextBox.Visibility = Visibility.Visible;
                    Label6.Visibility =  Label7.Visibility = Visibility.Hidden;
                    XAcreTextBox.Visibility = YAcreTextBox.Visibility = Visibility.Hidden;
                    HitRateLabel.Visibility = HitRateComboBox.Visibility = Visibility.Hidden;
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
                    HitRateLabel.Visibility = HitRateComboBox.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
