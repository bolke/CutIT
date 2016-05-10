using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutIT.Utility
{
    public class Coordinate
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Coordinate()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
        }

        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Relocate(Coordinate other)
        {
            if (other != null)
            {
                X = other.X;
                Y = other.Y;
                Z = other.Z;
                return true;
            }
            return false;
        }

        public bool Relocate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            return true;
        }

        public bool Relocate(string strX, string strY, string strZ)
        {
            strX = strX.Replace(',', '.');
            strY = strY.Replace(',', '.');
            strZ = strZ.Replace(',', '.');
            double newX = double.Parse(strX, CultureInfo.InvariantCulture);
            double newY = double.Parse(strY, CultureInfo.InvariantCulture);
            double newZ = double.Parse(strZ, CultureInfo.InvariantCulture);
            return Relocate(newX, newY, newZ);
        }

        public override string ToString()
        {
            return "(" + X.ToString().Replace(',', '.') + "," + Y.ToString().Replace(',', '.') + "," + Z.ToString().Replace(',', '.') + ")";
        }
    }
}
