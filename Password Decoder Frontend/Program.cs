using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PasswordLibrary.Decoder;
using PasswordLibrary;

namespace Password_Decoder_Frontend
{
    class Program
    {
        /*
         * Doubutsu no Mori e+ "Upgraded" Secret Code Structure
         *  Length: 24 bytes (0x18 bytes)
         *  Data Map:
         *      [0] = ?
         *      [1] = ?
         *      [2] = ?
         *      [3 - 8] = Town Name
         *      [9 - 14] = Player Name
         *      [15] = ?
         *      [16] = ?
         *      [17] = ?
         *      [18] = ?
         *      
         */

        private static readonly string[] MonumentNames = new string[15]
        {
            "Park Clock", "Gas Lamp", "Windpump", "Flower Clock", "Heliport",
            "Wind Turbine", "Pipe Stack", "Stonehenge", "Egg", "Footprints",
            "Geoglyph", "Mushroom", "Signpost", "Well", "Fountain"
        };

        private static void PrintByteArray(byte[] Input)
        {
            for (int i = 0; i < Input.Length; i++)
            {
                Console.Write(Input[i].ToString("X2") + " ");
            }
            Console.Write("\r\n\r\n");
        }

        private static string ByteArrayToString(byte[] Data, int StartIdx = 0, int Length = -1)
        {
            Length = Length == -1 ? Data.Length : Length;

            string Output = "";
            for (int i = StartIdx; i < StartIdx + Length; i++)
            {
                Output += Common.AFe_CharList[Data[i]];
            }

            return Output;
        }

        private static void ParseDecodedPassword(byte[] Data)
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

            if (X == 7 && uint.TryParse(SenderString, out uint Price)) // 7 = Monument (Town Decoration)
            {
                int AcreX = Data[1] & 7;
                int AcreY = (Data[1] >> 3) & 7;
                Console.WriteLine(string.Format("Town Name: {0}\r\nPlayer Name: {1}\r\nDecoration Price: {2}\r\nTown Decoration: {6} [0x{3}]\r\nPlacement Acre [Y-X]: {4}-{5}",
                    TownName, PlayerName, Price.ToString("#,##0"),
                    PresentItemId < 0x5853 ? (PresentItemId + 0x5853).ToString("X4") : PresentItemId.ToString("X4"), AcreY, AcreX, MonumentNames[PresentItemId % 15]));
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
                int r4 = Data[1] & 7;
                int r0 = (Data[1] >> 3) & 7;
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

        static void Main(string[] args)
        {
            byte[] TestPassword = new byte[32]
            {
                0x1D, 0x11, 0x7D, 0x09, 0x7B, 0xEE, 0x12, 0xEF, 0x24, 0xC1, 0x1C, 0x7D, 0x06, 0x7C, 0x1D, 0x5D,
                0x0B, 0x06, 0x03, 0x5D, 0x5E, 0x02, 0x24, 0xC3, 0x00, 0xE7, 0x24, 0xE8, 0x06, 0x08, 0xC2, 0xC0,
            };

            byte[] TestPassword2 = new byte[32] // Shovel
            {
                0xE9, 0xEB, 0x1C, 0x1F, 0xE8, 0x17, 0x1B, 0x05, 0x11, 0xE8, 0xF2, 0x18, 0xED, 0xF9, 0xF3, 0x15,
                0xF8, 0xE7, 0x07, 0x14, 0x09, 0x0D, 0xF9, 0x1E, 0x06, 0x16, 0xC3, 0xC1, 0x05, 0x1D, 0x14, 0x24,
            };

            /*for (int i = 0; i < TestPassword.Length; i++)
            {
                TestPassword[i] = 0x0C;
            }*/

            byte[] PasswordData = PasswordLibrary.Decoder.Decoder.Decode("きうぐれちいどぜわよつとでりぐおたでろべぎれそほしけむけけけおど"); //PasswordLibrary.Decoder.Decoder.Decode("まにびあじうれぶすぬねごやせくんざるれほねえつでゆみずなへさにぜ");
            string TownName = Encoding.ASCII.GetString(PasswordData, 3, 6);
            string PlayerName = Encoding.ASCII.GetString(PasswordData, 9, 6);

            Console.WriteLine("Valid Password: " + (Common.VerifyChecksum(PasswordData) ? "Yes" : "No"));
            

            PrintByteArray(PasswordData);
            ParseDecodedPassword(PasswordData);
            
            Console.WriteLine("\r\nDone");
            Console.ReadKey();
        }
    }
}
