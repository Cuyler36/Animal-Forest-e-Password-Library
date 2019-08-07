using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace PasswordLibrary.Encoder
{
    public static class Encoder
    {
        public static byte[] mMpswd_make_passcode(CodeType CodeType, int HitRateIndex, string RecipientTown, string Recipient, string Sender, ushort ItemId, int ExtraData)
        {
            byte[] Output = new byte[24];

            int realHitRateIndex;
            int r31 = 0;

            switch (CodeType)
            {
                case CodeType.Famicom:
                case CodeType.User:
                case CodeType.Card_E_Mini:
                    realHitRateIndex = 4;
                    ExtraData = 0;
                    r31 = 0xFF;
                    break;
                case CodeType.NPC:
                case CodeType.New_NPC:
                    ExtraData &= 3;
                    realHitRateIndex = 4;
                    break;
                case CodeType.Magazine:
                    // Valid indices are 0 - 4. Hit rates are: { 80.0f, 60.0f, 30.0f, 0.0f, 100.0f }. The hit is RNG based and the player "wins" if hit < hitRate.
                    realHitRateIndex = HitRateIndex & 7;
                    ExtraData = 0;
                    r31 = 0xFF;
                    break;
                case CodeType.Monument:
                    ExtraData &= 0xFF;
                    realHitRateIndex = 4;
                    r31 = 0xFF;
                    break;
                default:
                    realHitRateIndex = 4;
                    CodeType = CodeType.User;
                    break;
            }

            int Byte0 = ((int) CodeType << 5) & 0xE0;
            Byte0 |= (realHitRateIndex << 2);
            Output[0] = (byte)Byte0;
            Output[1] = (byte)ExtraData;
            Output[2] = (byte)r31;

            // Copy Recipient Name
            for(int i = 0; i < 6; i++)
            {
                if (i >= RecipientTown.Length)
                {
                    Output[3 + i] = 0x20; // Space Character value
                }
                else
                {
                    int CharacterIndex = Array.IndexOf(Common.AFe_CharList, RecipientTown.Substring(i, 1));
                    if (CharacterIndex < 0)
                    {
                        CharacterIndex = 0x20; // Set to space? TODO: Maybe we should return "invalid code" if this happens
                        Console.WriteLine("Encountered an invalid character in the Recipient's Name at string offset: " + i);
                    }
                    Output[3 + i] = (byte)CharacterIndex;
                }
            }

            // Copy Recipient Town Name
            for (int i = 0; i < 6; i++)
            {
                if (i >= Recipient.Length)
                {
                    Output[9 + i] = 0x20; // Space Character value
                }
                else
                {
                    int CharacterIndex = Array.IndexOf(Common.AFe_CharList, Recipient.Substring(i, 1));
                    if (CharacterIndex < 0)
                    {
                        CharacterIndex = 0x20; // Set to space? TODO: Maybe we should return "invalid code" if this happens
                        Console.WriteLine("Encountered an invalid character in the Recipient's Town Name at string offset: " + i);
                    }
                    Output[9 + i] = (byte)CharacterIndex;
                }
            }

            // Copy Sender Name
            for (int i = 0; i < 6; i++)
            {
                if (i >= Sender.Length)
                {
                    Output[15 + i] = 0x20; // Space Character value
                }
                else
                {
                    int CharacterIndex = Array.IndexOf(Common.AFe_CharList, Sender.Substring(i, 1));
                    if (CharacterIndex < 0)
                    {
                        CharacterIndex = 0x20; // Set to space? TODO: Maybe we should return "invalid code" if this happens
                        Console.WriteLine("Encountered an invalid character in the Sender's Name at string offset: " + i);
                    }
                    Output[15 + i] = (byte)CharacterIndex;
                }
            }

            // Copy Item ID
            Output[0x15] = (byte)(ItemId >> 8);
            Output[0x16] = (byte)ItemId;

            // Add up byte totals of all characters in each string
            int Checksum = 0;
            for (int i = 0; i < 6; i++)
            {
                Checksum += Output[3 + i];
            }

            for (int i = 0; i < 6; i++)
            {
                Checksum += Output[9 + i];
            }

            for (int i = 0; i < 6; i++)
            {
                Checksum += Output[15 + i];
            }

            Checksum += ItemId;
            Checksum += r31;
            Output[0] |= (byte)((Checksum >> 2) & 3);
            Output[1] |= (byte)((Checksum & 3) << 6);

            #if DEBUG
            for (int i = 0; i < Output.Length; i++)
            {
                Console.WriteLine(string.Format("Output[{0}]", i) + ": " + Output[i].ToString("X2"));
            }
            #endif

            return Output;
        }

        public static void mMpswd_substitution_cipher(ref byte[] Data)
        {
            for (int i = 0; i < 24; i++)
            {
                Data[i] = Common.mMpswd_chg_code_table[Data[i]];
            }
        }

        public static void mMpswd_bit_shuffle(ref byte[] Data, int Key)
        {
            int CharOffset = Key == 0 ? 0xD : 9;
            int CharCount = Key == 0 ? 0x16 : 0x17;

            byte[] Buffer = Data.Take(CharOffset).Concat(Data.Skip(CharOffset + 1).Take(23 - CharOffset)).ToArray();
            byte[] Output = new byte[CharCount];

            int[] IndexTable = Common.mMpswd_select_idx_table[Data[CharOffset] & 3];

            for (int i = 0; i < CharCount; i++)
            {
                var selectedByte = Buffer[i];
                for (var x = 0; x < 8; x++)
                {
                    var OutputOffset = IndexTable[x] + i;
                    if (OutputOffset >= CharCount)
                    {
                        OutputOffset -= CharCount;
                    }

                    Output[OutputOffset] |= (byte)(((selectedByte >> x) & 1) << x);
                }
            }

            for (int i = 0; i < CharOffset; i++)
            {
                Data[i] = Output[i];
            }

            for (int i = CharOffset; i < CharCount; i++)
            {
                Data[i + 1] = Output[i]; // Data[i + 1] to skip the "Char" byte
            }
        }

        public static void mMpswd_chg_RSA_cipher(ref byte[] Data)
        {
            byte[] Buffer = Data.Clone() as byte[];
            Tuple<int, int, int, int[]> Parameters = Common.mMpswd_get_RSA_key_code(Data);
            int Prime1 = Parameters.Item1;
            int Prime2 = Parameters.Item2;
            int Prime3 = Parameters.Item3;
            int[] IndexTable = Parameters.Item4;

            byte CipherValue = 0;
            int PrimeProduct = Prime1 * Prime2;

            for (int i = 0; i < 8; i++)
            {
                int Value = Data[IndexTable[i]];
                int CurrentValue = Value;

                for (int x = 0; x < Prime3 - 1; x++)
                {
                    Value = (Value * CurrentValue) % PrimeProduct;
                }

                Buffer[IndexTable[i]] = (byte)Value;
                Value = (Value >> 8) & 1;
                CipherValue |= (byte)(Value << i);
            }
            Buffer[23] = CipherValue;

            for (int i = 0; i < 24; i++)
            {
                Data[i] = Buffer[i];
            }
        }

        public static void mMpswd_bit_mix_code(ref byte[] Data)
        {
            int SwitchType = Data[1] & 0x0F;
            if (SwitchType > 0x0C)
            {
                Common.mMpswd_bit_arrange_reverse(ref Data);
                Common.mMpswd_bit_reverse(ref Data);
                Common.mMpswd_bit_shift(ref Data, SwitchType * 3);
            }
            else if (SwitchType > 0x08)
            {
                Common.mMpswd_bit_arrange_reverse(ref Data);
                Common.mMpswd_bit_shift(ref Data, SwitchType * -5);
            }
            else if (SwitchType > 0x04)
            {
                Common.mMpswd_bit_shift(ref Data, SwitchType * -5);
                Common.mMpswd_bit_reverse(ref Data);
            }
            else
            {
                Common.mMpswd_bit_shift(ref Data, SwitchType * 3);
                Common.mMpswd_bit_arrange_reverse(ref Data);
            }
        }

        public static byte[] mMpswd_chg_6bits_code(byte[] Data)
        {
            byte[] Password = new byte[32];

            int bit6Idx = 0;
            int bit8Idx = 0;
            int byte6Idx = 0;
            int byte8Idx = 0;

            int Value = 0;
            int Total = 0;

            while (true)
            {
                Value |= ((Data[byte8Idx] >> bit8Idx) & 1) << bit6Idx;
                bit8Idx++;
                bit6Idx++;

                if (bit6Idx == 6)
                {
                    Password[byte6Idx] = (byte)Value;
                    Value = 0;
                    bit6Idx = 0;
                    byte6Idx++;
                    Total++;
                    if (Total == 32)
                    {
                        return Password;
                    }
                }

                if (bit8Idx == 8)
                {
                    bit8Idx = 0;
                    byte8Idx++;
                }
            }
        }

        public static void mMpswd_chg_common_font_code(ref byte[] Password)
        {
            for (int i = 0; i < 32; i++)
            {
                Password[i] = Common.usable_to_fontnum_new[Password[i]];
            }
        }

#if DEBUG
        public static string Encode(CodeType CodeType, int HitRateIndex, string RecipientTown, string Recipient, string Sender, ushort ItemId, int ExtraData)
        {
            byte[] PasswordData = mMpswd_make_passcode(CodeType, HitRateIndex, RecipientTown, Recipient, Sender, ItemId, ExtraData);
            PrintByteBuffer("mMpswd_make_passcode", PasswordData);
            mMpswd_substitution_cipher(ref PasswordData);
            PrintByteBuffer("mMpswd_substitution_cipher", PasswordData);
            Common.mMpswd_transposition_cipher(ref PasswordData, true, 0);
            PrintByteBuffer("mMpswd_transposition_cipher", PasswordData);
            mMpswd_bit_shuffle(ref PasswordData, 0); // this doesn't change the last byte. Is that necessary? Doesn't seem to be.
            PrintByteBuffer("mMpswd_bit_shuffle", PasswordData);
            mMpswd_chg_RSA_cipher(ref PasswordData);
            PrintByteBuffer("mMpswd_chg_RSA_cipher", PasswordData);
            mMpswd_bit_mix_code(ref PasswordData); // the problem appears to be in the bit mix function.
            PrintByteBuffer("mMpswd_bit_mix_code", PasswordData);
            mMpswd_bit_shuffle(ref PasswordData, 1);
            PrintByteBuffer("mMpswd_bit_shuffle", PasswordData);
            Common.mMpswd_transposition_cipher(ref PasswordData, false, 1);
            PrintByteBuffer("mMpswd_transposition_cipher", PasswordData);
            byte[] Password = mMpswd_chg_6bits_code(PasswordData);
            PrintByteBuffer("mMpswd_chg_6bits_code", Password);
            mMpswd_chg_common_font_code(ref Password);
            PrintByteBuffer("mMpswd_chg_common_font_code", Password);

            // Construct password string
            string PasswordString = "";
            for (int i = 0; i < 32; i++)
            {
                if (i == 16)
                {
                    PasswordString += "\r\n";
                }
                PasswordString += Common.AFe_CharList[Password[i]];
            }

            return PasswordString;
        }

        private static void PrintByteBuffer(string stage, byte[] buffer)
        {
            Console.Write((stage + ":").PadRight(32));
            for (var i = 0; i < buffer.Length; i++)
            {
                if (i > 0 && i % 8 == 0)
                {
                    Console.Write(("\n").PadRight(32) + " ");
                }
                Console.Write(buffer[i].ToString("X2"));
            }
            Console.Write("\n\n");
        }
#else
        public static string Encode(CodeType CodeType, int HitRateIndex, string RecipientTown, string Recipient, string Sender, ushort ItemId, int ExtraData)
        {
            byte[] PasswordData =
 mMpswd_make_passcode(CodeType, HitRateIndex, RecipientTown, Recipient, Sender, ItemId, ExtraData);
            mMpswd_substitution_cipher(ref PasswordData);
            Common.mMpswd_transposition_cipher(ref PasswordData, true, 0);
            mMpswd_bit_shuffle(ref PasswordData, 0);
            mMpswd_chg_RSA_cipher(ref PasswordData);
            mMpswd_bit_mix_code(ref PasswordData);
            mMpswd_bit_shuffle(ref PasswordData, 1);
            Common.mMpswd_transposition_cipher(ref PasswordData, false, 1);
            byte[] Password = mMpswd_chg_6bits_code(PasswordData);
            mMpswd_chg_common_font_code(ref Password);

            // Construct password string
            string PasswordString = "";
            for (int i = 0; i < 32; i++)
            {
                if (i == 16)
                {
                    PasswordString += "\r\n";
                }
                PasswordString += Common.AFe_CharList[Password[i]];
            }

            return PasswordString;
        }
#endif
    }
}
