using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Gravity_sim
{
    internal class Planet
    {
        public const double density = 0.00257269;
        public Vector2 velocity;
        public Vector2 pos;
        public int radius;
        public float mass;
        public bool move = true;


        public Planet(float vx, float vy, float x, float y, float mass)
        {
            velocity = new Vector2(vx, vy);
            pos = new Vector2(x, y);
            this.mass = mass;
            radius = CalcRad();
        }

        public Planet(Vector2 velocity, Vector2 pos, float mass)
        {
            this.velocity = velocity;
            this.pos = pos;
            this.mass = mass;
            radius = CalcRad();
        }

        public int CalcRad()
        {
            double rad;
            rad = (mass * 4) / (density * Math.PI * 3);
            rad = Math.Pow(rad, 0.333333);
            return (int)rad;
        }

        public void Move()
        {
            if(move == true)
                pos += velocity;
        }
    }
}
