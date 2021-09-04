using GlmNet;
using System;
using System.Collections.Generic;

namespace ComputeShaderTwoVortexRing
{
    internal static class InitialPosition
    {
        private static double outDeltaR = 0.05f;
        private static double innerDeltaR = 0.05f;

        public static List<float> InCircle(vec3 nParticles, double ringRadius, double yCoord)
        {
            List<float> initialPosition = new List<float>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;


                        var r = ringRadius - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = (float)yCoord;
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }


            return initialPosition;
        }

        public static List<float> InCylinderOrdered(vec3 nParticles, double ringRadius, double yCoord)
        {
            double height = 0.2;
            List<float> initialPosition = new List<float>();
            double ringWidth = 0.2;
            var deltaR = (ringWidth) / nParticles.x;
            var deltaPhi = 2.0 * Math.PI / nParticles.y;
            var deltaY = height / nParticles.z;

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
            
                        var r = ringRadius - ringWidth + i * deltaR;
                        var x = (float)(r * Math.Cos(j * deltaPhi));
                        var z = (float)(r * Math.Sin(j * deltaPhi));
                        var y = (float)(yCoord + k*deltaY);
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }


            return initialPosition;
        }

        public static List<float> InTwoCylinderOrdered(vec3 nParticles, double ringRadius1, double ringRadius2, double yCoord1, double yCoord2)
        {
            double height = 0.1;
            List<float> initialPosition = new List<float>();
            double ringWidth = 0.1;
            var deltaR = (ringWidth) / nParticles.x;
            var deltaPhi = 2.0 * Math.PI / nParticles.y;
            var deltaY = height / (nParticles.z/2);

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z/2; k++)
                    {

                        var r = ringRadius1 - ringWidth + i * deltaR;
                        var x = (float)(r * Math.Cos(j * deltaPhi));
                        var z = (float)(r * Math.Sin(j * deltaPhi));
                        var y = (float)(yCoord1 + k * deltaY);
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }

            
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z/2; k++)
                    {

                        var r = ringRadius2 - ringWidth + i * deltaR;
                        var x = (float)(r * Math.Cos(j * deltaPhi));
                        var z = (float)(r * Math.Sin(j * deltaPhi));
                        var y = (float)(yCoord2 + k * deltaY);
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }


            return initialPosition;
        }


        public static List<float> InTwoCircle(vec3 nParticles, double ringRadius, double yCoord1, double yCoord2)
        {
            List<float> initialPosition = new List<float>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 

            for (int i = 0; i < nParticles.x / 2; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;


                        var r = ringRadius - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = (float)yCoord1;
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }

            for (int i = 0; i < nParticles.x / 2; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;


                        var r = ringRadius - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = (float)yCoord2;
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }


            return initialPosition;
        }

        public static List<float> InCylinder(vec3 nParticles, double ringRadius, double yCoord, double height)
        {
            List<float> initialPosition = new List<float>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;


                        var r = ringRadius - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = (float)(yCoord + height * rnd.NextDouble());
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }

            return initialPosition;
        }

        public static List<float> InTwoRing(vec3 nParticles, double ringRadius1, double ringRadius2, double yCoord1, double yCoord2)
        {
            List<float> initialPosition = new List<float>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 

            for (int i = 0; i < nParticles.x / 2; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;


                        var r = ringRadius1 - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = (float)yCoord1;
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }

            for (int i = 0; i < nParticles.x / 2; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;


                        var r = ringRadius2 - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = (float)yCoord2;
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }


            return initialPosition;
        }
    }
}
