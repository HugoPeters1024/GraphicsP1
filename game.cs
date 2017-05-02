using System;
using System.IO;
using System.Drawing;
using System.Collections;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Template {

class Game
{
        OpenTKApp app;
        KeyboardState KeyPress, PrevKeyPress;
        // member variables
        public Surface screen;
        public Game(OpenTKApp app)
        {
            this.app = app; //Inherited the app pointer to get the window size
        }
	    // initialize
	    public void Init()
        {
        }

	    // tick: renders one frame
        public void Tick()
	    {

	    }

        public void RenderGL()
        {

            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(-0.5f, -0.5f, 0);
            GL.Vertex3(0.5f, -0.5f, 0);
            GL.Vertex3(-0.5f, 0.5f, 0);
            GL.End();
        }

        int CalculateColor(int r, int g, int b)
        {
            return (r << 16) + (g << 8) + b;
        }

        /// <summary>
        /// converts values of x from -range/2 ... range/2 to 0 ...640
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        int TX(float x, float range, float centerX=0)
        {
            x *= Math.Min((float)app.Height / screen.height, (float)app.Width / screen.width); //Mutliply by the mininum of the width or height ratio to ensure a fit
            x *= (float)app.Height / app.Width; //Multiply by the aspect ratio. (Where app is the OpenTKApp that represents the screen)
            x += (range/2) + centerX; //CenterX representing the Cartesian coordinate system with screen center as origin (-range, range)
            x *= screen.width / range; //Mulitply the unit line by the screen width over the range to get a pixel coordinate
            return (int)x;
        }

        /// <summary>
        /// converts values of y from -range/2 ... range/2 to 0 ... 400 (and negates y)
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        int TY(float y, float range, float centerY=0)
        {
            y *= Math.Min((float)app.Height / screen.height, (float)app.Width / screen.width); //Mutliply by the mininum of the width or height ratio to ensure a fit
            y = -y; //Reverse y
            y += (range/2) - centerY; //Correct again for the Cartesian coordinate system
            y *= screen.height / range; //Mulitply the unit line by the screen height over the range to get a pixel coordinate
            return (int)y;
        }

        void HandleKeyBoard()
        {
            PrevKeyPress = KeyPress;
            //Get the current state
            KeyPress = Keyboard.GetState();
        }

}

} // namespace Template5

/* Exercise 1&2
Point offset = new Point(100, 100);
for (int y = 0; y < 256; ++y)
    for (int x = 0; x < 256; ++x)
        screen.Plot(x + offset.x, y + offset.y, CalculateColor(x, y, 0));
        */
 /*
//An aesthetically pleasing checker pattern background
screen.Clear(0);
            for (int y = 0; y<screen.height; ++y)
                for (int x = 0; x<screen.width; ++x)
                    screen.Plot(x, y, (x ^ y)%256);

            HandleKeyBoard(); //update keyboard status
screen.Print("Function Plotting", 2, 2, 0xffffff );
            screen.Line(2, 20, 160, 20, 0xff0000);
            screen.Print("f(x) = Sin(x) + Sin(5x)", 2, 24, 0xffffff);

            //This will plot the func PlottedLine by connecting straight lines
            //between arbitrarily small values to create the illusion of a small line;
            float stepSize = rangeX / screen.width;
            for(float x = -rangeX; x<rangeX; x += stepSize)
            {
                float y1 = func.Function(x); //first y value
float y2 = func.Function(x + stepSize); //second y value
int drawX1 = TX(x, rangeX, centerX); //convert x to pixel value
int drawY1 = TY(y1, rangeY, centerY); //conver y to pixel value;
int drawX2 = TX(x + stepSize, rangeX, centerX); //second x value
int drawY2 = TY(y2, rangeY, centerY);  //second y value
screen.Line(drawX1, drawY1, drawX2, drawY2, 0xffff00); //plot a line between to the points
            }

            if (KeyPress.IsKeyDown(Key.Left))
                func.translation += TRANSLATION_SPEED;

            if (KeyPress.IsKeyDown(Key.Right))
                func.translation -= TRANSLATION_SPEED;

            if (KeyPress.IsKeyDown(Key.A))
            {
                rangeX += ZOOM_SPEED;
                rangeY += ZOOM_SPEED;
            }

            if (KeyPress.IsKeyDown(Key.Z))
            {
                rangeX -= ZOOM_SPEED;
                rangeY -= ZOOM_SPEED;
            }
*/



/*
int rx1 = TX((float)(x1 * Math.Cos(a) + y1 * Math.Sin(a)), rangeX);
int ry1 = TY((float)(x1 * Math.Sin(a) - y1 * Math.Cos(a)), rangeY);
int rx2 = TX((float)(x1 * Math.Cos(a) + y2 * Math.Sin(a)), rangeX);
int ry2 = TY((float)(x1 * Math.Sin(a) - y2 * Math.Cos(a)), rangeY);
int rx3 = TX((float)(x2 * Math.Cos(a) + y2 * Math.Sin(a)), rangeX);
int ry3 = TY((float)(x2 * Math.Sin(a) - y2 * Math.Cos(a)), rangeY);
int rx4 = TX((float)(x2 * Math.Cos(a) + y1 * Math.Sin(a)), rangeX);
int ry4 = TY((float)(x2 * Math.Sin(a) - y1 * Math.Cos(a)), rangeY);
int c = 0x00ffff;

screen.Line(rx1, ry1, rx2, ry2, c);
screen.Line(rx2, ry2, rx3, ry3, c);
screen.Line(rx3, ry3, rx4, ry4, c);
screen.Line(rx4, ry4, rx1, ry1, c); */
