//=============================================================================
// Christian Fenkart
// (c)Copyright (2017) All Rights Reserved
//=============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CF_TimeSpanSlider
{
    class CF_calc
    {
        #region Kurventransformation
        public static float Kurventransformation(float rX, float rX1, float rX2, float rY1, float rY2)
        {
            return (float)doubleKurventransformation(rX, rX1, rX2, rY1, rY2);
        }
        public static float Kurventransformation(float rX, float rX1, float rX2, float rY1, float rY2, float rKorrekturPunktX, float rKorrekturPunktY)
        {
            double d = doubleKurventransformation(rX, rX1, rX2, rY1, rY2, rKorrekturPunktX, rKorrekturPunktY);
            return (float)d;
        }

        public static double doubleKurventransformation(double rX, double rX1, double rX2, double rY1, double rY2)
        {
            return doubleKurventransformation(rX, rX1, rX2, rY1, rY2, 0d, 0d);
        }
        public static double doubleKurventransformation(double rX, double rX1, double rX2, double rY1, double rY2, double rKorrekturPunktX, double rKorrekturPunktY)
        {
            double rY = 0f;

            #region Temp
            double rXt, rYt, rYtXY, rKorrekturPunktXt, rKorrekturPunktYt, rK, rKorrekturWert, rXNorm;
            bool blKorrekturPunktXYaktiv = (rKorrekturPunktX != 0d) && (rKorrekturPunktY != 0d);
            int iError = 0, iWarnung = 0;
            #endregion

            rKorrekturPunktXt = rKorrekturPunktX;
            rKorrekturPunktYt = rKorrekturPunktY;

            #region ErrorHandling
            if (blKorrekturPunktXYaktiv)
            {
                if ((rX1 < rX2) && (rKorrekturPunktXt >= rX2))  // Error rKorrekturPunktX zu gross (begrenzt mit rX2)
                {
                    rKorrekturPunktXt = rX2;
                    rKorrekturPunktYt = rY2;
                    iError = 1;
                }
                if ((rX1 > rX2) && (rKorrekturPunktXt <= rX2))  // Error rKorrekturPunktX zu klein (begrenzt mit rX2)
                {
                    rKorrekturPunktXt = rX2;
                    rKorrekturPunktYt = rY2;
                    iError = 2;
                }
                if ((rX1 < rX2) && (rKorrekturPunktXt <= rX1))  // Error rKorrekturPunktX zu klein (begrenzt mit rX1)
                {
                    rKorrekturPunktXt = rX1;
                    rKorrekturPunktYt = rY1;
                    iError = 3;
                }
                if ((rX1 > rX2) && (rKorrekturPunktXt >= rX1))  // Error rKorrekturPunktX zu gross (begrenzt mit rX1)
                {
                    rKorrekturPunktXt = rX1;
                    rKorrekturPunktYt = rY1;
                    iError = 4;
                }
                if ((rY1 < rY2) && (rKorrekturPunktYt >= rY2))  // Error rKorrekturPunktY zu gross (begrenzt mit rY2)
                {
                    rKorrekturPunktXt = rX2;
                    rKorrekturPunktYt = rY2;
                    iError = 6;
                }
                if ((rY1 < rY2) && (rKorrekturPunktYt <= rY1))  // Error rKorrekturPunktY zu klein (begrenzt mit rY1)
                {
                    rKorrekturPunktXt = rX1;
                    rKorrekturPunktYt = rY1;
                    iError = 7;
                }
                if ((rY1 < rY2) && (rKorrekturPunktYt <= rY1))  // Error rKorrekturPunktY zu gross (begrenzt mit rY1)
                {
                    rKorrekturPunktXt = rX1;
                    rKorrekturPunktYt = rY1;
                    iError = 8;
                }
            }
            else
            {
                rKorrekturPunktXt = rX1;
                rKorrekturPunktYt = rY1;
            }

            if (rX1 == rX2)
            {
                iError = 11; // Error rX1 = rX2 ==> Divide by  0!!
                throw new System.ArgumentException("Curve transformation ->  Divide by  0!!", "rX1 = rX2");
            }
            #endregion

            #region CALC
            if (iError == 0)
            {

                if (((rX1 < rX2) && (rX > rX2)) || ((rX1 > rX2) && (rX < rX2)))
                {
                    rXt = rX2;
                }
                else if (((rX1 > rX2) && (rX > rX1)) || ((rX1 < rX2) && (rX < rX1)))
                {
                    rXt = rX1;
                }
                else
                {
                    rXt = rX;
                }

                rK = (rY2 - rY1) / (rX2 - rX1);

                if (blKorrekturPunktXYaktiv)
                {
                    rKorrekturWert = rKorrekturPunktYt - ((rKorrekturPunktXt - rX1) * rK + rY1);

                    if (((rX1 < rX2) && (rXt < rKorrekturPunktXt)) || ((rX1 > rX2) && (rXt > rKorrekturPunktXt)))
                    {
                        rXNorm = (rXt - rX1) / (rKorrekturPunktXt - rX1);
                    }
                    else
                    {
                        rXNorm = (rXt - rX2) / (rKorrekturPunktXt - rX2);
                    }

                    rYtXY = (Math.Sin(rXNorm * (Math.PI / 2.0)) * rKorrekturWert);
                }
                else
                    rYtXY = 0;

                rYt = ((rXt - rX1) * rK + rY1) + rYtXY;

                #region LIMIT 
                if ((rY1 < rY2) && (rYt > rY2))
                {
                    rYt = rY2;
                    iWarnung = 1; // Warnung rY zu gross (begrenzt mit rY2)
                }
                else if ((rY1 < rY2) && (rYt < rY1))
                {
                    rYt = rY1;
                    iWarnung = 2; // Warnung rY zu klein (begrenzt mit rY1)
                }
                else if ((rY1 > rY2) && (rYt < rY2))
                {
                    rYt = rY2;
                    iWarnung = 3; // Warnung rY zu klein (begrenzt mit rY2)
                }
                else if ((rY1 > rY2) && (rYt > rY1))
                {
                    rYt = rY1;
                    iWarnung = 4; // Warnung rY zu gross (begrenzt mit rY1)
                }

                if (rY1 == rY2)
                {
                    iWarnung = 11; // Warnung rY1 = rY2 ==> Multiplikation mit 0!!
                }
                #endregion

                rY = rYt;
            }

            #endregion

            return rY;
        }
        #endregion


        public static float floatLimit(float input, float min, float max)
        {
            return (float)doubleLimit(input, min, max);
        }
        public static double doubleLimit(double input, double min, double max)
        {
            double retval = input;

            if (retval < min)
                retval = min;

            if (retval > max)
                retval = max;

            return retval;
        }

        public static DateTime DateTimeLimit(DateTime input, DateTime min, DateTime max)
        {
            DateTime retval = input;

            if (retval < min)
                retval = min;

            if (retval > max)
                retval = max;

            return retval;
        }
        public static DateTime DateTimeMin(DateTime in1, DateTime in2)
        {
            DateTime retval = in1;

            if (retval > in2)
                retval = in2;

            return retval;
        }
        public static DateTime DateTimeMax(DateTime in1, DateTime in2)
        {
            DateTime retval = in1;

            if (retval < in2)
                retval = in2;

            return retval;
        }


        public static int intMax(int in1, int in2)
        {
            int retval = in1;

            if (retval < in2)
                retval = in2;

            return retval;
        }
        public static int intMin(int in1, int in2)
        {
            int retval = in1;

            if (retval > in2)
                retval = in2;

            return retval;
        }

        public static float floatMax(float in1, float in2)
        {
            float retval = in1;

            if (retval < in2)
                retval = in2;

            return retval;
        }
        public static float floatMin(float in1, float in2)
        {
            float retval = in1;

            if (retval > in2)
                retval = in2;

            return retval;
        }

        public static double doubleMax(double in1, double in2)
        {
            double retval = in1;

            if (retval < in2)
                retval = in2;

            return retval;
        }
        public static double doubleMin(double in1, double in2)
        {
            double retval = in1;

            if (retval > in2)
                retval = in2;

            return retval;
        }


        public static Rectangle getRect(Point p1, Point p2)
        {
            Rectangle value = new Rectangle();

            int xMin = intMin(p1.X, p2.X);
            int yMin = intMin(p1.Y, p2.Y);
            int xMax = intMax(p1.X, p2.X);
            int yMax = intMax(p1.Y, p2.Y);

            int xDelta = xMax - xMin;
            int yDelta = yMax - yMin;

            value.X = xMin;
            value.Y = yMin;
            value.Width = xDelta;
            value.Height = yDelta;

            return value;
        }

        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
