using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace PasswordLibrary.Decoder
{
    public class Decoder
    {
        public static void mMpswd_decode_bit_shuffle(ref byte[] Data, bool keyIdx)
        {
            int count = keyIdx ? 0x17 : 0x16; // Count
            int bitIdx = keyIdx ? 0x09 : 0x0D; // Bit index

            byte TableIndex = Data[bitIdx];
            byte[] ShuffledData = new byte[23]; // Exclude the r31 byte

            for (int i = 0, idx = 0; i < 23; i++)
            {
                if (i == bitIdx)
                {
                    idx++; // Skip r31 byte
                }
                ShuffledData[i] = Data[idx++];
            }

            byte[] ZeroedData = new byte[23];
            int[] ShuffleTable = Common.mMpswd_select_idx_table[Data[bitIdx] & 3];

            int OffsetIdx = 0;
            int ZeroedDataIdx = 0;
            while (OffsetIdx < count)
            {
                int outputIdx = 0;
                int bit = 0;

                for (int x = 0; x < 8; x++)
                {
                    int OutputOffset = ShuffleTable[outputIdx++] + OffsetIdx;
                    if (OutputOffset >= count)
                    {
                        OutputOffset -= count;
                    }

                    ZeroedData[ZeroedDataIdx] |= (byte)(((ShuffledData[OutputOffset] >> bit) & 1) << bit);
                    bit++;
                }

                OffsetIdx++;
                ZeroedDataIdx++;
            }

            ZeroedData.Take(bitIdx).ToArray().CopyTo(Data, 0);
            Data[bitIdx] = TableIndex;
            ZeroedData.Skip(bitIdx).Take(ZeroedDataIdx - bitIdx).ToArray().CopyTo(Data, bitIdx + 1);
        }

        public static void mMpswd_decode_bit_code(ref byte[] Data)
        {
            int Method = Data[1] & 0x0F;

            if (Method > 12)
            {
                Common.mMpswd_bit_shift(ref Data, -Method * 3);
                Common.mMpswd_bit_reverse(ref Data);
                Common.mMpswd_bit_arrange_reverse(ref Data);
            }
            else if (Method > 8)
            {
                Common.mMpswd_bit_shift(ref Data, Method * 5);
                Common.mMpswd_bit_arrange_reverse(ref Data);
            }
            else if (Method > 4)
            {
                Common.mMpswd_bit_reverse(ref Data);
                Common.mMpswd_bit_shift(ref Data, Method * 5);
            }
            else
            {
                Common.mMpswd_bit_arrange_reverse(ref Data);
                Common.mMpswd_bit_shift(ref Data, -Method * 3);
            }
        }

        public static void mMpswd_decode_RSA_cipher(ref byte[] Data)
        {
            int ModCount = 0;
            byte[] OutputBuffer = Data.Clone() as byte[];

            Tuple<int, int, int, int[]> PrimeData = Common.mMpswd_get_RSA_key_code(Data);

            int Prime1 = PrimeData.Item1;
            int Prime2 = PrimeData.Item2;
            int Prime3 = PrimeData.Item3;
            int[] IndexTable = PrimeData.Item4;

            int PrimeProduct = Prime1 * Prime2;
            int LessProduct = (Prime1 - 1) * (Prime2 - 1);
            int LoopEndValue = 0;
            int ModValue = 0;

            do
            {
                ModCount++;
                LoopEndValue = (ModCount * LessProduct + 1) % Prime3;
                ModValue = (ModCount * LessProduct + 1) / Prime3;
            } while (LoopEndValue != 0);

            int CurrentValue = 0;

            for (int i = 0; i < 8; i++)
            {
                int Value = Data[IndexTable[i]];
                Value |= ((Data[23] >> i) << 8) & 0x100;
                CurrentValue = Value;

                for (int x = 0; x < ModValue - 1; x++)
                {
                    Value = (Value * CurrentValue) % PrimeProduct;
                }

                OutputBuffer[IndexTable[i]] = (byte)Value;
            }

            for (int i = 0; i < 24; i++)
            {
                Data[i] = OutputBuffer[i];
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
                StoredValue |= (((Password[PasswordIndex] >> ShiftRightValue) & 1) << ShiftLeftValue) & 0xFF;
                ShiftRightValue++;
                ShiftLeftValue++;

                if (ShiftLeftValue > 7)
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
                if (ShiftRightValue > 5)
                {
                    ShiftRightValue = 0;
                    PasswordIndex++;
                }
            }
        }

        public static void mMpswd_decode_substitution_cipher(ref byte[] Data)
        {
            for (int i = 0; i < 24; i++)
            {
                for (int x = 0; x < 256; x++)
                {
                    if (Data[i] == Common.mMpswd_chg_code_table[x])
                    {
                        Data[i] = (byte)x;
                        break;
                    }
                }
            }
        }

        // Main Code \\
        public static byte[] Decode(byte[] Input, bool englishPasswords)
        {
            if (Input.Length != 32)
            {
                throw new Exception("Input must be 32 bytes long");
            }

            byte[] PasswordData = new byte[24];

            Common.mMpswd_chg_password_font_code(ref Input, englishPasswords ? Common.usable_to_fontnum_new_translation : Common.usable_to_fontnum_new);
            mMpswd_chg_8bits_code(ref PasswordData, Input);
            Common.mMpswd_transposition_cipher(ref PasswordData, true, 1);
            mMpswd_decode_bit_shuffle(ref PasswordData, true);
            mMpswd_decode_bit_code(ref PasswordData);
            mMpswd_decode_RSA_cipher(ref PasswordData);
            mMpswd_decode_bit_shuffle(ref PasswordData, false);
            Common.mMpswd_transposition_cipher(ref PasswordData, false, 0);
            mMpswd_decode_substitution_cipher(ref PasswordData);

            return PasswordData;
        }

        public static byte[] Decode(string Password, bool englishPasswords = false)
        {
            if (Password.Length == 32)
            {
                byte[] Data = new byte[32];
                for (int i = 0; i < 32; i++)
                {
                    int Idx = Array.IndexOf(Common.AFe_CharList, Password.Substring(i, 1));
                    if (Idx < 0)
                    {
                        throw new Exception("The password contains an invalid character!");
                    }
                    Data[i] = (byte)Idx;
                }
                return Decode(Data, englishPasswords);
            }
            return null;
        }
    }
}
