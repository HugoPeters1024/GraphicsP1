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
        float a;
        Surface map;
        float[,] h;
        float[] vertexData;
        int VBO;


        // member variables
        public Surface screen;
        public Game(OpenTKApp app)
        {
            this.app = app; //Inherited the app pointer to get the window size
        }
	    // initialize
	    public void Init()
        {
            a = 0f;
            map = new Surface("../../assets/heightmap.png");
            h = new float[map.width, map.height];
            for (int y = 0; y < map.height; ++y)
                for (int x = 0; x < map.width; ++x)
                    h[x, y] = ((float)(map.pixels[x + map.width * y] & 255)) / 256f;

            vertexData = new float[(map.width-1) * (map.height-1) * 2 * 3 * 3];
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData<float>(
                BufferTarget.ArrayBuffer,
                (IntPtr)(vertexData.Length * 4),
                vertexData,
                BufferUsageHint.StaticDraw
            );
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 12, 0);
        }

	    // tick: renders one frame
        public void Tick()
	    {
            a += 0.01f;
	    }

        public void RenderGL()
        {
            int i = 0;

            float s = 2f / map.width;
            for (int y = 0; y < map.height - 1; ++y)
                for (int x = 0; x < map.width - 1; ++x)
                {
                    float xc = -1f + x * s;
                    float yc = -1f + y * s;

                    int index = (x + (map.width - 1) * y);

                    vertexData[i++] = xc;
                    vertexData[i++] = yc;
                    vertexData[i++] = h[x, y];

                    vertexData[i++] = xc;
                    vertexData[i++] = yc - s;
                    vertexData[i++] = h[x, y];

                    vertexData[i++] = xc - s;
                    vertexData[i++] = yc;
                    vertexData[i++] = h[x, y];

                    vertexData[i++] = xc - s;
                    vertexData[i++] = yc - s;
                    vertexData[i++] = h[x, y];

                    vertexData[i++] = xc;
                    vertexData[i++] = yc - s;
                    vertexData[i++] = h[x, y];

                    vertexData[i++] = xc - s;
                    vertexData[i++] = yc;
                    vertexData[i++] = h[x, y];

                    /*
                    GL.Color3(0.5f, 0.25f, 0.75f);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(xc, yc, h[x, y]);
                    GL.Vertex3(xc, yc - s, h[x, y]);
                    GL.Vertex3(xc - s, yc - s, h[x, y]);
                    GL.Vertex3(xc - s, yc, h[x, y]);
                    GL.End();

                    GL.Color3(1f, 0, 0);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(xc, yc, h[x,y]);
                    GL.Vertex3(xc, yc -s, h[x,y]);
                    GL.Vertex3(xc, yc - s, h[x+1,y]);
                    GL.Vertex3(xc, yc, h[x+1,y]);
                    GL.End();


                    GL.Color3(0.5f, 0, 0);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(xc, yc, h[x, y]);
                    GL.Vertex3(xc - s, yc, h[x, y]);
                    GL.Vertex3(xc - s, yc, h[x, y + 1]);
                    GL.Vertex3(xc, yc, h[x, y + 1]);
                    GL.End(); */
                }
            Console.WriteLine(i);

            GL.PushMatrix();
            GL.Rotate(150, 1f, 0f, 0f);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, (map.width-1) * (map.height-1) * 2 * 3);
            GL.PopMatrix();
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
