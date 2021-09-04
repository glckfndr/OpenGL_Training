using GlmNet;
using System;
using System.Collections.Generic;

namespace ComputeShader2DVortex
{
    internal class VortexInitializer
    {
        public static List<Vortex> GetVortexesInCircle(vec3 nParticles)
        {
            List<Vortex> list = new List<Vortex>();
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
                        var rankine = 0.001f;
                        var vr = new Vortex(new vec2(x, y), gamma, rankine);
                        list.Add(vr);
                    }
                }
            }

            return list;
        }


        public static List<Vortex> GetVortexesInLayer(vec3 nParticles)
        {
            List<Vortex> list = new List<Vortex>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            float L = 1.0f;
            float thickness = 0.02f;
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var x = (float)(-L + 2 * L * rnd.NextDouble());

                        var y = (float)(-thickness + 2 * thickness * rnd.NextDouble());
                        float gamma = 0.04f;
                        var rankine = 0.01f;
                        var vr = new Vortex(new vec2(x, y), gamma, rankine);
                        list.Add(vr);
                    }
                }
            }

            return list;
        }

        public static List<Vortex> GetVortexesInLayerOrdered(vec3 nParticles, float gamma)
        {
            List<Vortex> list = new List<Vortex>();
            
            //var deltaR = outRadius - innerRadius; 
            float L = 1.0f;
            float thickness = 0.02f;
            float dx = 2 * L / (nParticles.x);
            float dy = 2 * thickness  / (nParticles.y);
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    var x = -L + i * dx + 0.5f * dx;
                    var y = -thickness + j * dy + 0.5f * dy;
                    var rankine = 0.003f;
                    var vr = new Vortex(new vec2(x, y), gamma, rankine);
                    list.Add(vr);
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
