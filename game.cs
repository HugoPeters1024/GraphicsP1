using System;
using System.IO;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
namespace Template {

class Game
{
        float x1 = -1;
        float x2 = 1;
        float y1 = 1;
        float y2 = -1;
        float a = 0;
        OpenTKApp app;
        // member variables
        public Surface screen;
        public Game(OpenTKApp app)
        {
            this.app = app;
        }
	    // initialize
	    public void Init()
	    {

        }

	    // tick: renders one frame
        public void Tick()
	    {
            a += 0.02f;
            screen.Clear( 0 );
            screen.Print(((float)app.Height).ToString(), 2, 2, 0xffffff );
            screen.Line(2, 20, 160, 20, 0xff0000);
            float centerX = 0, centerY = 0;
            int rx1 = TX((float)(x1 * Math.Cos(a) + y1 * Math.Sin(a)), centerX);
            int ry1 = TY((float)(x1 * Math.Sin(a) - y1 * Math.Cos(a)), centerY);
            int rx2 = TX((float)(x1 * Math.Cos(a) + y2 * Math.Sin(a)), centerX);
            int ry2 = TY((float)(x1 * Math.Sin(a) - y2 * Math.Cos(a)), centerY);
            int rx3 = TX((float)(x2 * Math.Cos(a) + y2 * Math.Sin(a)), centerX);
            int ry3 = TY((float)(x2 * Math.Sin(a) - y2 * Math.Cos(a)), centerY);
            int rx4 = TX((float)(x2 * Math.Cos(a) + y1 * Math.Sin(a)), centerX);
            int ry4 = TY((float)(x2 * Math.Sin(a) - y1 * Math.Cos(a)), centerY);
            int c = 0x00ffff;
            
            screen.Line(rx1, ry1, rx2, ry2, c);
            screen.Line(rx2, ry2, rx3, ry3, c);
            screen.Line(rx3, ry3, rx4, ry4, c);
            screen.Line(rx4, ry4, rx1, ry1, c);
	    }

        int CalculateColor(int r, int g, int b)
        {
            return (r << 16) + (g << 8) + b;
        }

        /// <summary>
        /// converts values of x from -2 ... 2 to 0 ...640
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        int TX(float x, float centerX)
        {
            x *= (float)app.Width/screen.width; //where app is the OpenTkApp that defines the screen
            x += 2 + centerX; //CenterX representing the Cartesian coordinate system with screen center as origin (-2, 2)
            x *= screen.width >> 2; //Multiply the 4 unit line by 1/4th of the screen width to gain a pixel coordinate
            return (int)x;
        }

        /// <summary>
        /// converts values of y from -2 ... 2 to 0 ... 400 (and negates y)
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        int TY(float y, float centerY)
        {
            y = -y;
            y += 2 - centerY;
            y *= (screen.height >> 2);
            return (int)y;
        }

}

} // namespace Template5

/* Exercise 1&2
Point offset = new Point(100, 100);
for (int y = 0; y < 256; ++y)
    for (int x = 0; x < 256; ++x)
        screen.Plot(x + offset.x, y + offset.y, CalculateColor(x, y, 0));
        */
