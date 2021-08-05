using System;
using System.Collections.Generic;
using System.Xml;

namespace EdgeDeviceLibrary.Communicator
{
	public class VINFormat
	{
		public class VINDigit : IComparable<VINDigit>
		{
			private int _position;

			private char[] _possibleValues;

			public int Position => _position;

			public char[] PossibleValues => _possibleValues;

			public VINDigit(int position, char[] possibleValues)
			{
				_position = position;
				_possibleValues = possibleValues;
			}

			public int CompareTo(VINDigit other)
			{
				return _position.CompareTo(other._position);
			}

			public bool ValidateDigit(char value)
			{
				char[] possibleValues = _possibleValues;
				foreach (char value2 in possibleValues)
				{
					if (value.CompareTo(value2) == 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		private List<VINDigit> _digits = new List<VINDigit>();

		public VINDigit[] Digits => _digits.ToArray();

		public VINFormat(XmlDocument xmlDoc)
		{
			XmlNode xmlNode = xmlDoc.DocumentElement.SelectSingleNode("blankvin");
			if (xmlNode == null)
			{
				return;
			}
			foreach (XmlNode item2 in xmlNode.SelectNodes("digit"))
			{
				XmlAttribute xmlAttribute = item2.Attributes["position"];
				XmlAttribute xmlAttribute2 = item2.Attributes["values"];
				string s = xmlAttribute?.Value;
				string text = xmlAttribute2?.Value;
				if (int.TryParse(s, out var result) && text != null)
				{
					VINDigit item = new VINDigit(result, text.ToCharArray());
					_digits.Add(item);
				}
			}
			_digits.Sort();
		}
	}
}
