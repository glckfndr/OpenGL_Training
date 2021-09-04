using DiscreteVortexLibrary;
using GlmNet;
using System;
using System.Collections.Generic;

namespace DemoFlowVisualization
{
    internal class VortexInitializer
    {
        private static float _totalCirculation = 20.00f;

        public static List<VortexStruct> GetVortexesInCircle(vec3 nParticles)
        {
            List<VortexStruct> list = new List<VortexStruct>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            float R = 1.0f;
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;

                        var r = R * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var y = (float)(r * Math.Sin(phi));
                        var gamma = (float)(-0.1 + 0.2 * rnd.NextDouble());
                        var rankine = 0.01f;
                        var vr = new VortexStruct(new Vector2f(x, y), gamma, rankine);
                        list.Add(vr);
                    }
                }
            }

            return list;
        }


        



        public static List<VortexStruct> GetVortexesInLayer(vec3 nParticles)
        {
            List<VortexStruct> list = new List<VortexStruct>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            float L = 1.0f;
            float thickness = 0.02f;
            var totalParticle = nParticles.x * nParticles.y * nParticles.z;
            float gamma = (float)(_totalCirculation / totalParticle);


            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var x = (float)(-L + 2 * L * rnd.NextDouble());

                        var y = (float)(-thickness + 2 * thickness * rnd.NextDouble());
                        // float gamma = 0.004f;
                        var rankine = 0.01f;
                        var vr = new VortexStruct(new Vector2f(x, y), gamma, rankine);
                        list.Add(vr);
                    }
                }
            }

            return list;
        }

        public static List<vec2> GetVelocity(vec3 nParticles)
        {
            List<vec2> list = new List<vec2>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            //float R = 1.0f;
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {

                        var vel = new vec2(0);
                        list.Add(vel);

                    }
                }
            }

            return list;
        }

    }
}
