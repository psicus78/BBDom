using System.Globalization;

namespace KNXLib.DPT
{
    internal sealed class DataPointBit : DataPoint
    {
        public override string[] Ids
        {
            get
            {
                return new[] { "1.001", "1.002", "1.008", "1.009" };
            }
        }

        public override object FromDataPoint(string data)
        {
            var dataConverted = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
                dataConverted[i] = (byte) data[i];

            return FromDataPoint(dataConverted);
        }

        public override object FromDataPoint(byte[] data)
        {
            // DPT bits high byte: MEEEEMMM, low byte: MMMMMMMM
            // first M is signed state from two's complement notation

            return data[0] != 0;
        }

        public override byte[] ToDataPoint(string value)
        {
            return ToDataPoint(float.Parse(value, CultureInfo.InvariantCulture));
        }

        public override byte[] ToDataPoint(object val)
        {
            var dataPoint = new byte[] { 0x00 };
            if (val is bool && (bool)val)
            {
                dataPoint = new byte[] { 0x01 };
            }            
            return dataPoint;
        }
    }
}
