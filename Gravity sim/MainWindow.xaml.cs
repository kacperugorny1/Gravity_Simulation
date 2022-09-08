using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using System.Diagnostics;
using System.Threading;

namespace Gravity_sim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Gravity Solar_System = new();
        private List<Ellipse> ellipses = new();
        private double pressed_time;
        private Vector2 pressed_pos = new();
        private Vector2 pressed_vel = new();
        private List<SolidColorBrush> colors = new();
        private Ellipse? temp_eli = null;
        private bool next_pinned = false;
        private Line? temp_line = null;
        public MainWindow()
        {
            
            
            InitializeComponent();
            Fill_colors();
            Setup_canvas();
            ellipses[0].Fill = Brushes.Yellow;

        }
        private void Fill_colors()
        {
            colors.Add(Brushes.AliceBlue);
            colors.Add(Brushes.Red);
            colors.Add(Brushes.Green);
            colors.Add(Brushes.Magenta);
            colors.Add(Brushes.MediumTurquoise);
            colors.Add(Brushes.Gold);
            colors.Add(Brushes.Linen);
        }

        private void Setup_canvas()
        {
            foreach(Planet x in Solar_System.planetList)
            {
                Add_Planet(x);
            }
        }


        // add planet to canvas
        private void Add_Planet(Planet x)
        {
            Random z = new();
            Ellipse ellipse = new();
            ellipse.Width = x.radius;
            ellipse.Height = x.radius;
            ellipse.Fill = colors[z.Next(0,colors.Count)];
            ellipse.Stroke = Brushes.White;
            double left = x.pos.X - (ellipse.Width / 2);
            double top = x.pos.Y - (ellipse.Height / 2);
            Sim_Border.Children.Add(ellipse);
            ellipses.Add(ellipse);
            ellipse.Margin = new Thickness(left, top, 0, 0);
        }

        
        // loop game etc
        private void Start_Simulation(object sender, RoutedEventArgs e)
        {
            logic();
        }
        private async void logic()
        {
            while (true)
            {
                foreach (var index in Solar_System.ApplyForce()) // Apply force
                {
                    Solar_System.planetList.RemoveAt(index); // eliminate collided units
                    Sim_Border.Children.Remove(ellipses[index]);
                    ellipses.RemoveAt(index);
                }
                Solar_System.Move(); //move
                Print_velocity_and_pos(); //print vel mass and pos
                if(temp_eli != null) // if temp_eli is not null make it bigger
                {
                    double x = (4 * (float)(pressed_time + Stopwatch.GetTimestamp()) / 10000) / (Planet.density * Math.PI * 3);
                    x = Math.Pow(x, 0.3333);
                    temp_eli.Height = (int)x;
                    temp_eli.Width = (int)x;
                    double left = pressed_pos.X - (temp_eli.Width / 2);
                    double top = pressed_pos.Y - (temp_eli.Height / 2);
                    temp_eli.Margin = new Thickness(left, top, 0, 0);
                }
                Update_frame(); // update scene

                await Task.Delay(1000/144); //delay frame
            }
        }

        private void Update_frame()
        {
            foreach (var x in Solar_System.planetList.Zip(ellipses, Tuple.Create))
            {
                double left = x.Item1.pos.X - (x.Item2.Width / 2);
                double top = x.Item1.pos.Y - (x.Item2.Height / 2);
                x.Item2.Height = x.Item1.radius;
                x.Item2.Width = x.Item1.radius;
                x.Item2.Margin = new Thickness(left, top, 0, 0);
            }
        }



        //count changes
        private void Print_velocity_and_pos()
        {
            int index = 0;
            string mass_text = "Mass: \n";
            string vel_text = "Vel: \n";
            string pos_text = "Pos: \n";
            foreach(Planet planet in Solar_System.planetList)
            {
                var x = planet.velocity.ToString().Split(',', ' ');
                vel_text +=   (x.Length == 4? x[0] + ' ' + x[2] + '>' : planet.velocity.ToString()) + "\n";

                x = planet.pos.ToString().Split(',', ' ');
                pos_text += index.ToString() + ". " + (x.Length == 4 ? x[0] + ' ' + x[2] + '>' : planet.pos.ToString()) + "\n";

                mass_text += planet.mass.ToString() + "\n";
                index++;
            }
            
            Vel_text.Text = vel_text;
            Pos_text.Text = pos_text;
            Mass_text.Text = mass_text;
        }















        // add new planet event with leftclick
        private void Make_planet(object sender, MouseButtonEventArgs e)
        {
            pressed_time = - Stopwatch.GetTimestamp();
            pressed_pos.X = (float)e.GetPosition(Sim_Border).X;
            pressed_pos.Y = (float)e.GetPosition(Sim_Border).Y;
            temp_eli = new Ellipse
            {
                Fill = Brushes.Gray,
                Width = 30,
                Height = 30
            };
            double left = pressed_pos.X - (temp_eli.Width / 2);
            double top = pressed_pos.Y - (temp_eli.Height / 2);


            temp_line = new Line()
            {
                X1 = pressed_pos.X,
                Y1 = pressed_pos.Y,
                X2 = pressed_pos.X,
                Y2 = pressed_pos.Y,
                StrokeThickness = 3,
                Stroke = Brushes.White
            };

            Sim_Border.Children.Add(temp_eli);
            Sim_Border.Children.Add(temp_line);


            temp_eli.Margin = new Thickness(left, top, 0, 0);

        }

        private void Put_Planet(object sender, MouseButtonEventArgs e)
        {
            pressed_time += Stopwatch.GetTimestamp();
            pressed_vel.X = pressed_pos.X - (float)e.GetPosition(Sim_Border).X;
            pressed_vel.Y = pressed_pos.Y - (float)e.GetPosition(Sim_Border).Y;
            pressed_vel *= -10;
            if (next_pinned == true)
                Solar_System.planetList.Add(new Planet(pressed_vel / 100, pressed_pos, (float)pressed_time / 10000) { move = false });
            else
                Solar_System.planetList.Add(new Planet(pressed_vel / 100, pressed_pos, (float)pressed_time / 10000));
            next_pinned = false;
            Sim_Border.Children.Remove(temp_eli);
            Sim_Border.Children.Remove(temp_line);
            temp_line = null;
            Add_Planet(Solar_System.planetList.Last());
            
        }

        private void DrawLine(object sender, MouseEventArgs e)
        {
            if (temp_line is null)
                return;
            temp_line.X2 = e.GetPosition(Sim_Border).X;
            temp_line.Y2 = e.GetPosition(Sim_Border).Y;
 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            next_pinned = true;
        }
    }
}
