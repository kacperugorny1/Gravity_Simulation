using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Gravity_sim
{
    internal class Gravity
    {
        public List<Planet> planetList = new();

        public Gravity()
        {
            //planetList.Add(new Planet(0, 0, 950, 500, 100000));
            //planetList.First().move = false;

            planetList.Add(new Planet(0, -13, 1300, 400, 100));

            planetList.Add(new Planet(0, -10, 500, 400, 100));


        }

        public void Move()
        {
            foreach (Planet planet in planetList)
            {
                if(planet.move)
                    planet.Move();
            }
        }

        public HashSet<int> ApplyForce() // return set of indexes to remove
        {
            HashSet<int> ans = new();
            List<Planet> skiplist = new();
            foreach(Planet planet in planetList)
            {
                if (planet.move == false) //check if object is movable
                    continue;
                if (skiplist.IndexOf(planet) != -1)
                    continue;
                foreach(Planet planet2 in planetList)
                {
                    if (planet == planet2) // skip
                        continue;
                    if (skiplist.IndexOf(planet) != -1)
                        continue;
                    Vector2 p1p2 = planet2.pos - planet.pos; // move vector
                    if (p1p2.Length() <= planet.radius/2 + planet2.radius/2) { //collision
                        ans.Add(planetList.IndexOf(planet.mass >= planet2.mass ? planet2 : planet));
                        if (planet.mass >= planet2.mass) // check for more mass and calculate velocity by momentum
                        {
                            planet.velocity = (planet2.velocity * planet2.mass + planet.velocity * planet.mass) / (planet.mass + planet2.mass);
                            planet.mass += planet2.mass;
                            planet.radius = planet.CalcRad();
                            skiplist.Add(planet2);
                            continue;
                        }
                        else
                        {
                            planet2.velocity = (planet2.velocity * planet2.mass + planet.velocity * planet.mass) / (planet.mass + planet2.mass);
                            planet2.mass += planet.mass;
                            planet2.radius = planet2.CalcRad();
                            skiplist.Add(planet);
                            continue;
                        }
                    }
                    float speed = (float)(planet2.mass/Math.Pow((p1p2).Length(), 2)); // force
                    Vector2 vel = (p1p2) / (p1p2).Length(); // direction vector
                    vel *= speed; 
                    planet.velocity += vel; // addition
                } 
            }
            return ans;
        }
        

    }
}
