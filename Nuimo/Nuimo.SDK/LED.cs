using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Nuimo.SDK
{
	public class LED
	{
		private byte[] _matrix = new byte[11];

		public LED() { }

		public LED(string characterMatrix, byte brightness = 0x7F, byte timeout = 0x64)
		{
			SetDisplay(characterMatrix, brightness, timeout);
		}

		public short Rows { get { return 9; } }
		public short Columns { get { return 9; } }

		public byte[] Matrix
		{
			get { return _matrix; }
			set
			{
				if (value.Length > 11)
					throw new ArgumentException("The matrix can only be 11 bytes long.");

				_matrix = value;
			}
		}
		public byte Brightness { get; set; }
		public byte Timeout { get; set; }

		public static LED FromByteArrary(byte[] buffer)
		{
			if (buffer.Length != 13)
				throw new ArgumentException("The buffer is not a valid length");

			var led = new LED();

			byte[] matrix = new byte[11];
			for (int i = 0; i < matrix.Length; i++)
				matrix[i] = buffer[i];

			led.Matrix = matrix;
			led.Brightness = buffer[11];
			led.Timeout = buffer[12];

			return led;
		}

		public byte[] ToByteArrary()
		{
			byte[] buffer = new byte[13];

			Matrix.CopyTo(buffer, 0);
			buffer[11] = Brightness;
			buffer[12] = Timeout;

			return buffer;
		}

		public void SetDisplay(string characterMatrix, byte brightness = 0xFF, byte timeout = 0x64)
		{
			characterMatrix = Regex.Replace(characterMatrix, "[^* ]", string.Empty);

			var maxBits = Rows * Columns;
			BitArray bits = new BitArray(maxBits);

			for (int characterPosition = 0; characterPosition < bits.Length; characterPosition++)
			{
				if (characterPosition < characterMatrix.Length)
					bits.Set(characterPosition, characterMatrix[characterPosition].Equals('*'));
				else
					bits.Set(characterPosition, false);
			}

			var numberOfBytes = (int)Math.Ceiling(bits.Length / 8d);
			var byteIndex = 0;
			var bitIndex = 0;
			var matrix = new byte[numberOfBytes];

			for (int bit = 0; bit < bits.Length; bit++)
			{
				if (bits[bit])
					matrix[byteIndex] |= (byte)(1 << (7 - bitIndex));

				bitIndex++;

				if (bitIndex == 8)
				{
					byteIndex++;
					bitIndex = 0;
				}
			}

			this.Matrix = matrix;
			this.Brightness = brightness;
			this.Timeout = timeout;
		}

		public override string ToString()
		{
			StringBuilder led = new StringBuilder();

			int ledPosition = 0;
			//	Had to do this because BitArray(Matrix) loses the leading 0
			for (int byteIndex = 0; byteIndex < Matrix.Length; byteIndex++)
			{
				for (int bitIndex = 0; bitIndex < 8; bitIndex++)
				{
					if (((byte)(Matrix[byteIndex] >> (7 - bitIndex)) & 0x01) == 0x01)
						led.Append("*");
					else
						led.Append(" ");

					ledPosition++;

					if (ledPosition == 9)
					{
						if (byteIndex == Matrix.Length - 1)
							break;

						led.AppendFormat("\r\n");
						ledPosition = 0;
					}
				}
			}

			return led.ToString();
		}
	}
}
