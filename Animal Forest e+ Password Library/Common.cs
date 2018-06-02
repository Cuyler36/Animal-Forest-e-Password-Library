using System;

namespace PasswordLibrary
{
    public static class Common
    {
        // Character List
        public static readonly string[] AFe_CharList = new string[256]
        {
            "あ", "い", "う", "え", "お", "か", "き", "く", "け", "こ", "さ", "し", "す", "せ", "そ", "た",
            "ち", "つ", "て", "と", "な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ", "ま", "み",
            " ", "!", "\"", "む", "め", "%", "&", "'", "(", ")", "~", "♥", ", ", "-", ".", "♪",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", "🌢", "<", "+", ">", "?",
            "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "も", "💢", "や", "ゆ", "_",
            "よ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "ら", "り", "る", "れ", "�",
            "□", "。", "｢", "｣", "、", "･", "ヲ", "ァ", "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ", "ッ",
            "ー", "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス", "セ", "ソ",
            "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "マ",
            "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "ン", "ヴ", "☺",
            "ろ", "わ", "を", "ん", "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "ゃ", "ゅ", "ょ", "っ", "\n", "ガ", "ギ",
            "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ",
            "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ", "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ",
            "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ"
        };

        public static readonly int[] mMpswd_prime_number = new int[256]
        {
            0x0011, 0x0013, 0x0017, 0x001D, 0x001F, 0x0025, 0x0029, 0x002B, 0x002F, 0x0035, 0x003B, 0x003D, 0x0043, 0x0047, 0x0049, 0x004F,
            0x0053, 0x0059, 0x0061, 0x0065, 0x0067, 0x006B, 0x006D, 0x0071, 0x007F, 0x0083, 0x0089, 0x008B, 0x0095, 0x0097, 0x009D, 0x00A3,
            0x00A7, 0x00AD, 0x00B3, 0x00B5, 0x00BF, 0x00C1, 0x00C5, 0x00C7, 0x00D3, 0x00DF, 0x00E3, 0x00E5, 0x00E9, 0x00EF, 0x00F1, 0x00FB,
            0x0101, 0x0107, 0x010D, 0x010F, 0x0115, 0x0119, 0x011B, 0x0125, 0x0133, 0x0137, 0x0139, 0x013D, 0x014B, 0x0151, 0x015B, 0x015D,
            0x0161, 0x0167, 0x016F, 0x0175, 0x017B, 0x017F, 0x0185, 0x018D, 0x0191, 0x0199, 0x01A3, 0x01A5, 0x01AF, 0x01B1, 0x01B7, 0x01BB,
            0x01C1, 0x01C9, 0x01CD, 0x01CF, 0x01D3, 0x01DF, 0x01E7, 0x01EB, 0x01F3, 0x01F7, 0x01FD, 0x0209, 0x020B, 0x021D, 0x0223, 0x022D,
            0x0233, 0x0239, 0x023B, 0x0241, 0x024B, 0x0251, 0x0257, 0x0259, 0x025F, 0x0265, 0x0269, 0x026B, 0x0277, 0x0281, 0x0283, 0x0287,
            0x028D, 0x0293, 0x0295, 0x02A1, 0x02A5, 0x02AB, 0x02B3, 0x02BD, 0x02C5, 0x02CF, 0x02D7, 0x02DD, 0x02E3, 0x02E7, 0x02EF, 0x02F5,
            0x02F9, 0x0301, 0x0305, 0x0313, 0x031D, 0x0329, 0x032B, 0x0335, 0x0337, 0x033B, 0x033D, 0x0347, 0x0355, 0x0359, 0x035B, 0x035F,
            0x036D, 0x0371, 0x0373, 0x0377, 0x038B, 0x038F, 0x0397, 0x03A1, 0x03A9, 0x03AD, 0x03B3, 0x03B9, 0x03C7, 0x03CB, 0x03D1, 0x03D7,
            0x03DF, 0x03E5, 0x03F1, 0x03F5, 0x03FB, 0x03FD, 0x0407, 0x0409, 0x040F, 0x0419, 0x041B, 0x0425, 0x0427, 0x042D, 0x043F, 0x0443,
            0x0445, 0x0449, 0x044F, 0x0455, 0x045D, 0x0463, 0x0469, 0x047F, 0x0481, 0x048B, 0x0493, 0x049D, 0x04A3, 0x04A9, 0x04B1, 0x04BD,
            0x04C1, 0x04C7, 0x04CD, 0x04CF, 0x04D5, 0x04E1, 0x04EB, 0x04FD, 0x04FF, 0x0503, 0x0509, 0x050B, 0x0511, 0x0515, 0x0517, 0x051B,
            0x0527, 0x0529, 0x052F, 0x0551, 0x0557, 0x055D, 0x0565, 0x0577, 0x0581, 0x058F, 0x0593, 0x0595, 0x0599, 0x059F, 0x05A7, 0x05AB,
            0x05AD, 0x05B3, 0x05BF, 0x05C9, 0x05CB, 0x05CF, 0x05D1, 0x05D5, 0x05DB, 0x05E7, 0x05F3, 0x05FB, 0x0607, 0x060D, 0x0611, 0x0617,
            0x061F, 0x0623, 0x062B, 0x062F, 0x063D, 0x0641, 0x0647, 0x0649, 0x064D, 0x0653, 0x0655, 0x065B, 0x0665, 0x0679, 0x067F, 0x0683,
        };

        public static readonly byte[] usable_to_fontnum_new = new byte[64]
        {
            0x0A, 0x1F, 0x1D, 0xF0, 0xF1, 0xF5, 0x0D, 0x05, 0xF2, 0x1E, 0xE7, 0x60, 0xEB, 0x11, 0x17, 0x04,
            0xED, 0x15, 0x23, 0xE9, 0xE8, 0xEF, 0x16, 0x10, 0x09, 0xF4, 0xC2, 0x12, 0xF8, 0xC0, 0x0F, 0xC3,
            0xF7, 0x5B, 0x7B, 0x5E, 0x08, 0x00, 0x19, 0x02, 0xF9, 0x24, 0x1A, 0x0C, 0xEC, 0x7C, 0x0E, 0xEA,
            0x01, 0x13, 0x07, 0x7E, 0x18, 0xF3, 0x14, 0x1C, 0x5D, 0x03, 0xEE, 0x1B, 0x0B, 0x7D, 0xC1, 0x06
        };

        public static readonly int[] key_idx = new int[2] { 0x16, 0x6 };

        static readonly string[] mMpswd_transposition_cipher_char0_table = new string[16]
        {
            "NiiMasaru",
            "KomatsuKunihiro",
            "TakakiGentarou",
            "MiyakeHiromichi",
            "HayakawaKenzo",
            "KasamatsuShigehiro",
            "SumiyoshiNobuhiro",
            "NomaTakafumi",
            "EguchiKatsuya",
            "NogamiHisashi",
            "IidaToki",
            "IkegawaNoriko",
            "KawaseTomohiro",
            "BandoTaro",
            "TotakaKazuo",
            "WatanabeKunio"
        };

        static readonly string[] mMpswd_transposition_cipher_char1_table = new string[16]
        {
            "RichAmtower",
            "KyleHudson",
            "MichaelKelbaugh",
            "RaycholeLAneff",
            "LeslieSwan",
            "YoshinobuMantani",
            "KirkBuchanan",
            "TimOLeary",
            "BillTrinen",
            "nAkAyOsInoNyuuSankin",
            "zendamaKINAKUDAMAkin",
            "OishikutetUYOKUNARU",
            "AsetoAminofen",
            "fcSFCn64GCgbCGBagbVB",
            "YossyIsland",
            "KedamonoNoMori"
        };

        public static readonly string[][] mMpswd_transposition_cipher_char_table =
            new string[2][] { mMpswd_transposition_cipher_char0_table, mMpswd_transposition_cipher_char1_table };

        public static readonly byte[] mMpswd_chg_code_table = new byte[256]
        {
            0xF0, 0x83, 0xFD, 0x62, 0x93, 0x49, 0x0D, 0x3E, 0xE1, 0xA4, 0x2B, 0xAF, 0x3A, 0x25, 0xD0, 0x82,
            0x7F, 0x97, 0xD2, 0x03, 0xB2, 0x32, 0xB4, 0xE6, 0x09, 0x42, 0x57, 0x27, 0x60, 0xEA, 0x76, 0xAB,
            0x2D, 0x65, 0xA8, 0x4D, 0x8B, 0x95, 0x01, 0x37, 0x59, 0x79, 0x33, 0xAC, 0x2F, 0xAE, 0x9F, 0xFE,
            0x56, 0xD9, 0x04, 0xC6, 0xB9, 0x28, 0x06, 0x5C, 0x54, 0x8D, 0xE5, 0x00, 0xB3, 0x7B, 0x5E, 0xA7,
            0x3C, 0x78, 0xCB, 0x2E, 0x6D, 0xE4, 0xE8, 0xDC, 0x40, 0xA0, 0xDE, 0x2C, 0xF5, 0x1F, 0xCC, 0x85,
            0x71, 0x3D, 0x26, 0x74, 0x9C, 0x13, 0x7D, 0x7E, 0x66, 0xF2, 0x9E, 0x02, 0xA1, 0x53, 0x15, 0x4F,
            0x51, 0x20, 0xD5, 0x39, 0x1A, 0x67, 0x99, 0x41, 0xC7, 0xC3, 0xA6, 0xC4, 0xBC, 0x38, 0x8C, 0xAA,
            0x81, 0x12, 0xDD, 0x17, 0xB7, 0xEF, 0x2A, 0x80, 0x9D, 0x50, 0xDF, 0xCF, 0x89, 0xC8, 0x91, 0x1B,
            0xBB, 0x73, 0xF8, 0x14, 0x61, 0xC2, 0x45, 0xC5, 0x55, 0xFC, 0x8E, 0xE9, 0x8A, 0x46, 0xDB, 0x4E,
            0x05, 0xC1, 0x64, 0xD1, 0xE0, 0x70, 0x16, 0xF9, 0xB6, 0x36, 0x44, 0x8F, 0x0C, 0x29, 0xD3, 0x0E,
            0x6F, 0x7C, 0xD7, 0x4A, 0xFF, 0x75, 0x6C, 0x11, 0x10, 0x77, 0x3B, 0x98, 0xBA, 0x69, 0x5B, 0xA3,
            0x6A, 0x72, 0x94, 0xD6, 0xD4, 0x22, 0x08, 0x86, 0x31, 0x47, 0xBE, 0x87, 0x63, 0x34, 0x52, 0x3F,
            0x68, 0xF6, 0x0F, 0xBF, 0xEB, 0xC0, 0xCE, 0x24, 0xA5, 0x9A, 0x90, 0xED, 0x19, 0xB8, 0xB5, 0x96,
            0xFA, 0x88, 0x6E, 0xFB, 0x84, 0x23, 0x5D, 0xCD, 0xEE, 0x92, 0x58, 0x4C, 0x0B, 0xF7, 0x0A, 0xB1,
            0xDA, 0x35, 0x5F, 0x9B, 0xC9, 0xA9, 0xE7, 0x07, 0x1D, 0x18, 0xF3, 0xE3, 0xF1, 0xF4, 0xCA, 0xB0,
            0x6B, 0x30, 0xEC, 0x4B, 0x48, 0x1C, 0xAD, 0xE2, 0x21, 0x1E, 0xA2, 0xBD, 0x5A, 0xD8, 0x43, 0x7A,
        };

        static readonly int[] mMpswd_select_idx0 = new int[8] { 0x11, 0x0B, 0x00, 0x14, 0x0E, 0x06, 0x08, 0x04 };
        static readonly int[] mMpswd_select_idx1 = new int[8] { 0x05, 0x08, 0x0B, 0x10, 0x04, 0x06, 0x09, 0x13 };
        static readonly int[] mMpswd_select_idx2 = new int[8] { 0x09, 0x0E, 0x11, 0x15, 0x0B, 0x0A, 0x13, 0x02 };
        static readonly int[] mMpswd_select_idx3 = new int[8] { 0x00, 0x02, 0x01, 0x04, 0x12, 0x0A, 0x0B, 0x08 };
        static readonly int[] mMpswd_select_idx4 = new int[8] { 0x11, 0x13, 0x10, 0x14, 0x0E, 0x08, 0x02, 0x09 };
        static readonly int[] mMpswd_select_idx5 = new int[8] { 0x10, 0x02, 0x01, 0x08, 0x12, 0x04, 0x07, 0x06 };
        static readonly int[] mMpswd_select_idx6 = new int[8] { 0x13, 0x06, 0x0A, 0x11, 0x01, 0x10, 0x08, 0x09 };
        static readonly int[] mMpswd_select_idx7 = new int[8] { 0x11, 0x07, 0x12, 0x10, 0x0F, 0x02, 0x0B, 0x00 };
        static readonly int[] mMpswd_select_idx8 = new int[8] { 0x06, 0x02, 0x0B, 0x01, 0x08, 0x0E, 0x00, 0x10 };
        static readonly int[] mMpswd_select_idx9 = new int[8] { 0x13, 0x10, 0x0B, 0x08, 0x11, 0x02, 0x06, 0x0E };
        static readonly int[] mMpswd_select_idx10 = new int[8] { 0x12, 0x0F, 0x02, 0x07, 0x0A, 0x0B, 0x01, 0x0E };
        static readonly int[] mMpswd_select_idx11 = new int[8] { 0x08, 0x00, 0x0E, 0x02, 0x14, 0x0B, 0x0F, 0x11 };
        static readonly int[] mMpswd_select_idx12 = new int[8] { 0x09, 0x01, 0x02, 0x00, 0x13, 0x08, 0x0E, 0x0A };
        static readonly int[] mMpswd_select_idx13 = new int[8] { 0x0A, 0x0B, 0x0E, 0x10, 0x13, 0x07, 0x11, 0x08 };
        static readonly int[] mMpswd_select_idx14 = new int[8] { 0x13, 0x08, 0x06, 0x01, 0x11, 0x09, 0x0E, 0x0A };
        static readonly int[] mMpswd_select_idx15 = new int[8] { 0x09, 0x07, 0x11, 0x0E, 0x13, 0x0A, 0x01, 0x0B };

        public static readonly int[][] mMpswd_select_idx_table = new int[16][]
        {
            mMpswd_select_idx0, mMpswd_select_idx1, mMpswd_select_idx2, mMpswd_select_idx3,
            mMpswd_select_idx4, mMpswd_select_idx5, mMpswd_select_idx6, mMpswd_select_idx7,
            mMpswd_select_idx8, mMpswd_select_idx9, mMpswd_select_idx10, mMpswd_select_idx11,
            mMpswd_select_idx12, mMpswd_select_idx13, mMpswd_select_idx14, mMpswd_select_idx15
        };

        // Methods

        public static byte mMpswd_chg_password_font_code_sub(byte Character)
        {
            for (byte i = 0; i < 0x40; i++)
            {
                if (usable_to_fontnum_new[i] == Character)
                {
                    return i;
                }
            }

            return 0xFF;
        }

        public static void mMpswd_chg_password_font_code(ref byte[] Password)
        {
            for (int i = 0; i < 32; i++)
            {
                Password[i] = mMpswd_chg_password_font_code_sub(Password[i]);
            }
        }

        public static void mMpswd_chg_8bits_code(ref byte[] StoredLocation, byte[] Password)
        {
            int PasswordIndex = 0;
            int StoredLocationIndex = 0;

            int StoredValue = 0;
            int Count = 0;
            int ShiftRightValue = 0;
            int ShiftLeftValue = 0;

            while (true)
            {
                StoredValue |= (((Password[PasswordIndex] >> ShiftRightValue++) & 1) << ShiftLeftValue++) & 0xFF;
                if (ShiftLeftValue >= 8)
                {
                    Count++;
                    StoredLocation[StoredLocationIndex++] = (byte)StoredValue;
                    ShiftLeftValue = 0;
                    if (Count >= 24)
                    {
                        return;
                    }
                    StoredValue = 0;
                }
                if (ShiftRightValue >= 6)
                {
                    ShiftRightValue = 0;
                    PasswordIndex++;
                }
            }
        }

        public static void mMpswd_transposition_cipher(ref byte[] Data, bool Negate, int KeyIndex)
        {
            int Multiplier = Negate ? -1 : 1;
            byte Key = Data[key_idx[KeyIndex]];
            string[] TranspositionTable = mMpswd_transposition_cipher_char_table[KeyIndex];
            string TranspositionCipher = TranspositionTable[Key & 0x0F];

            int CipherIndex = 0;

            for (int i = 0; i < 24; i++)
            {
                if (i != key_idx[KeyIndex])
                {
                    int ValueModifier = (TranspositionCipher[CipherIndex++] * Multiplier) & 0xFF;
                    Data[i] = (byte)(Data[i] + ValueModifier);
                    if (CipherIndex >= TranspositionCipher.Length)
                    {
                        CipherIndex = 0;
                    }
                }
            }
        }

        public static void mMpswd_bit_reverse(ref byte[] Data)
        {
            for (int i = 0; i < 24; i++)
            {
                if (i != 1)
                {
                    Data[i] ^= 0xFF;
                }
            }
        }

        public static void mMpswd_bit_arrange_reverse(ref byte[] Data)
        {
            byte[] Buffer = new byte[23];
            byte[] OutputBuffer = new byte[23];
            for (int i = 0, idx = 0; i < 24; i++)
            {
                if (i != 1)
                {
                    Buffer[idx++] = Data[i];
                }
            }

            for (int i = 0; i < 23; i++) // pretty sure this should be < 23
            {
                byte Value = Buffer[22 - i]; // this should be 22
                byte ChangedValue = (byte)(
                      ((Value & 0x80) >> 7)
                    | ((Value & 0x40) >> 5)
                    | ((Value & 0x20) >> 3)
                    | ((Value & 0x10) >> 1)
                    | ((Value & 0x08) << 1)
                    | ((Value & 0x04) << 3)
                    | ((Value & 0x02) << 5)
                    | ((Value & 0x01) << 7));
                OutputBuffer[i] = ChangedValue;
            }

            for (int i = 0, idx = 0; i < 23; i++)
            {
                if (i == 1)
                {
                    idx++;
                }
                Data[idx++] = OutputBuffer[i];
            }
        }

        public static void mMpswd_bit_shift(ref byte[] Data, int Shift)
        {
            byte[] Buffer = new byte[23];
            for (int i = 0, idx = 0; i < 24; i++)
            {
                if (i != 1)
                {
                    Buffer[idx++] = Data[i];
                }
            }

            byte[] OutputBuffer = new byte[23];

            if (Shift > 0)
            {
                int DestinationPosition = Shift / 8;
                int DestinationOffset = Shift % 8;

                for (int i = 0; i < 23; i++)
                {
                    OutputBuffer[(i + DestinationPosition) % 23] = (byte)((Buffer[i] << DestinationOffset)
                        | (Buffer[(i + 22) % 23] >> (8 - DestinationOffset)));
                }

                // Copy to original buffer
                for (int i = 0, idx = 0; i < 23; i++)
                {
                    if (i == 1) // Skip copying the second byte
                    {
                        idx++;
                    }
                    Data[idx++] = OutputBuffer[i];
                }
            }
            else if (Shift < 0)
            {
                for (int i = 0; i < 23; i++)
                {
                    OutputBuffer[i] = Buffer[22 - i];
                }
                Shift = -Shift;

                int DestinationPosition = Shift / 8;
                int DestinationOffset = Shift % 8;

                for (int i = 0; i < 23; i++)
                {
                    Buffer[(i + DestinationPosition) % 23] = OutputBuffer[i];
                }

                for (int i = 0; i < 23; i++)
                {
                    OutputBuffer[i] = (byte)((Buffer[i] >> DestinationOffset) | ((Buffer[(i + 22) % 23]) << (8 - DestinationOffset)));
                }

                for (int i = 0, idx = 0; i < 23; i++)
                {
                    if (i == 1)
                    {
                        idx++;
                    }
                    Data[idx++] = OutputBuffer[22 - i];
                }
            }
        }

        public static Tuple<int, int, int, int[]> mMpswd_get_RSA_key_code(byte[] Data)
        {
            int Bit10 = 0;
            int Bit32 = 0;
            int ByteTable = 0;

            Bit10 = Data[3] & 3;
            Bit32 = (Data[3] >> 2) & 3;

            if (Bit10 == 3)
            {
                Bit10 = (Bit10 ^ Bit32) & 3;
                if (Bit10 == 3)
                {
                    Bit10 = 0;
                }
            }

            if (Bit32 == 3)
            {
                Bit32 = (Bit10 + 1) & 3;
                if (Bit32 == 3)
                {
                    Bit32 = 1;
                }
            }

            if (Bit10 == Bit32)
            {
                Bit32 = (Bit10 + 1) & 3;
                if (Bit32 == 3)
                {
                    Bit32 = 1;
                }
            }

            ByteTable = ((Data[3] >> 2) & 0x3C) >> 2;

            return new Tuple<int, int, int, int[]>( mMpswd_prime_number[Bit10], mMpswd_prime_number[Bit32],
                mMpswd_prime_number[Data[0xC]], mMpswd_select_idx_table[ByteTable]);
        }

        public static bool mMpswd_new_password_zuru_check(int SavedZuru, int CodeType, string Reciepiant, string TownName, string Sender, ushort ItemId, int NpcCode, int Unknown)
        {
            bool Invalid = true;
            if (CodeType != 2 && CodeType < 8)
            {
                int Zuru = 0;
                Zuru += GetStringByteValue(Reciepiant);
                Zuru += GetStringByteValue(TownName);
                Zuru += GetStringByteValue(Sender);
                Zuru += ItemId;

                if ((Zuru & 0xF) == SavedZuru && mMpswd_check_default_hit_rate(CodeType, NpcCode) && mMpswd_check_default_npc_code(CodeType, NpcCode, Unknown))
                {
                    Invalid = false;
                }
            }

            return Invalid;
        }

        private static bool mMpswd_check_default_hit_rate(int CodeType, int CodeCheck)
        {
            bool HitRate = false;
            if (CodeType == 3 && CodeCheck < 5)
            {
                HitRate = true;
            }
            else if (CodeCheck == 4)
            {
                HitRate = true;
            }

            return HitRate;
        }

        private static bool mMpswd_check_default_npc_code(int CodeType, int NpcCode, int r3_9)
        {
            bool Valid = false;
            if (CodeType >= 5)
            {
                if (CodeType == 7)
                {
                    if (NpcCode == 0xFF)
                    {
                        Valid = true;
                    }
                }
                else
                {
                    Valid = true;
                }
            }
            else
            {
                if (CodeType == 1)
                {
                    Valid = true;
                }
                else if (CodeType >= 0)
                {
                    if (r3_9 == 0 && NpcCode == 0xFF)
                    {
                        Valid = true;
                    }
                }
            }

            return Valid;
        }

        // Custom Functions \\
        public static int GetPasswordChecksum(byte[] PasswordData)
        {
            int Checksum = 0;

            for (int i = 0x03; i < 0x15; i++)
            {
                Checksum += PasswordData[i];
            }

            Checksum += (PasswordData[0x15] << 8) | PasswordData[16];
            Checksum += PasswordData[2];

            return (((Checksum >> 2) & 3) << 2) | (((Checksum << 6) & 0xC0) >> 6);
        }

        public static bool VerifyChecksum(byte[] PasswordData)
        {
            int CalculatedChecksum = GetPasswordChecksum(PasswordData);
            int StoredChecksum = ((PasswordData[0] & 3) << 2) | ((PasswordData[1] & 0xC0) >> 6);

            Console.WriteLine(string.Format("Calculated Checksum: 0x{0}\r\nStored Checksum: 0x{1}", CalculatedChecksum.ToString("X"), StoredChecksum.ToString("X")));

            return CalculatedChecksum == StoredChecksum;
        }

        public static byte[] StringToAFByteArray(string Input)
        {
            byte[] Output = new byte[Input.Length];
            for (int i = 0; i < Input.Length; i++)
            {
                int Idx = Array.IndexOf(AFe_CharList, Input.Substring(i, 1));
                if (Idx < 0)
                {
                    throw new Exception("The string had an invalid character in it!");
                }

                Output[i] = (byte)Idx;
            }

            return Output;
        }

        private static int GetStringByteValue(string Input)
        {
            byte[] Data = StringToAFByteArray(Input);
            int Value = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                Value += Data[i];
            }

            return Value;
        }
    }
}
