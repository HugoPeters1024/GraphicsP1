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
        public float a;
        Surface map;
        float[,] h;
        float[] vertexData, vertexNormalData, colorData;

        public bool autoRotate = true;
        public float rotateSpeed = 1.0f;
        public float rotation = 0.0f;
        public float zoom = 0.75f;
        public Vector3 translation = new Vector3(0, -0.2f, -1.4f);
        float animation;

        int programID;
        int vsID, fsID;
        int attribute_vpos,
            attribute_vnorm,
            attribute_vcol,
            uniform_mview,
            uniform_ldir,
            uniform_lpos,
            uniform_intensity,
            uniform_animation,
            uniform_goloco;
        int vbo_pos,
            vbo_norm,
            vbo_col;

        Matrix4 M;
        public Vector3 Ldir, Lpos;
        public float intensity;
        public float GoLoco = 0;


        // member variables
        public Surface screen;
        public Game(OpenTKApp app)
        {
            this.app = app; //Inherited the app pointer to get the window size
        }
	    // initialize
	    public void Init()
        {
            animation = 1f;
            intensity = 2f;
            Ldir = (new Vector3(0, 0, 0)).Normalized();
            Lpos = new Vector3(0, 0, 2f);
            a = 0f; //Transformation angle
            /* 
             * Good heightmaps:
             * heightmap.png
             * heightmap3.png
             * heightmap7.png
             * heightmap8.png
             * heightmap9.png
             */
            map = new Surface("../../assets/heightmap3.png");
            h = new float[map.width, map.height];
            for (int y = 0; y < map.height; ++y)
                for (int x = 0; x < map.width; ++x)
                    h[x, y] = ((float)(map.pixels[x + map.width * y] & 255)) / 256f;

            vertexData = new float[(map.width - 1) * (map.height - 1) * 2 * 3 * 3]; //2 triangles * 3 vertices * 3 coordinates
            vertexNormalData = new float[(map.width - 1) * (map.height - 1) * 2 * 3 * 3]; //2 triangles * 3 vertices * 3 coordinates
            colorData = new float[(map.width - 1) * (map.height - 1) * 2 * 3 * 3];  //2 traingles * 3 vertices * 3 color values

            //Fill the color array
            int i = 0;
            for (int y = 0; y < map.height - 1; ++y)
                for (int x = 0; x < map.width - 1; ++x)
                    for (int n=0; n<2; ++n)   //Two triangles
                        for (int v = 0; v < 3; ++v) //Three vertices
                        {
                            colorData[i++] = 0.4f;
                            colorData[i++] = 0.2f;
                            colorData[i++] = 0.6f;
                        }

           InitVertex();
           InitShader();
        }

	    // tick: renders one frame
        public void Tick()
	    {
            if (autoRotate)
            {
                a += 0.01f * rotateSpeed;
            }
            else
            {
                a = rotation;
            }
            animation += 0.05f;
            
            M = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), a);
            M *= Matrix4.CreateScale(zoom);
            M *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), -1f);
            M *= Matrix4.CreateTranslation(translation);
            M *= Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.3f, .1f, 10f);

            Ldir = new Vector3(3f * (float)Math.Sin(5 * a), -(float)Math.Sin(a), 1).Normalized();
        }

        public void RenderGL()
        {
            Console.WriteLine(GL.GetError()); //Give OpenTK a channel for error communication

            //Note that the color pointer is still valid and will also be used

            GL.UseProgram(programID); //Use the shaders
            GL.UniformMatrix4(uniform_mview, false, ref M); //Transform with Matrix M
            GL.ProgramUniform3(programID, uniform_ldir, Ldir.X, Ldir.Y, Ldir.Z);
            GL.ProgramUniform3(programID, uniform_lpos, Lpos.X, Lpos.Y, Lpos.Z);
            GL.ProgramUniform1(programID, uniform_intensity, intensity);
            GL.ProgramUniform1(programID, uniform_animation, animation);
            GL.ProgramUniform1(programID, uniform_goloco, GoLoco);
            GL.DrawArrays(PrimitiveType.Triangles, 0, (map.width-1) * (map.height-1) * 2 * 3);  //Draw the vertices

        }

        void LoadShader(String name, ShaderType type, int program, out int ID)
        {
            ID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(name))
                GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            Console.WriteLine(GL.GetShaderInfoLog(ID));
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

        void InitShader()
        {
            vsID = GL.CreateShader(ShaderType.VertexShader);  //Init the Vertex Shader
            fsID = GL.CreateShader(ShaderType.FragmentShader); //Init the Fragment Shader
            programID = GL.CreateProgram();
            LoadShader("../../shaders/vs.glsl",
             ShaderType.VertexShader, programID, out vsID);
            LoadShader("../../shaders/fs.glsl",
             ShaderType.FragmentShader, programID, out fsID);
            GL.LinkProgram(programID);

            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vnorm = GL.GetAttribLocation(programID, "vNormal");
            attribute_vcol = GL.GetAttribLocation(programID, "vColor");
            uniform_mview = GL.GetUniformLocation(programID, "M");
            uniform_ldir = GL.GetUniformLocation(programID, "Ldir");
            uniform_lpos = GL.GetUniformLocation(programID, "Lpos");
            uniform_intensity = GL.GetUniformLocation(programID, "intensity");
            uniform_animation = GL.GetUniformLocation(programID, "animation");
            uniform_goloco = GL.GetUniformLocation(programID, "GoLoco");

            vbo_pos = GL.GenBuffer();  //Generate a vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_pos); //Bind the buffer
                                                              
            GL.BufferData<float>
                (BufferTarget.ArrayBuffer,
                (IntPtr)(vertexData.Length * 4),
                vertexData,
                BufferUsageHint.StaticDraw
             );
            GL.VertexAttribPointer(attribute_vpos, 3,    //Get the vertex pointer from that buffer
             VertexAttribPointerType.Float,
            false, 0, 0
             );

            vbo_norm = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_norm);
            GL.BufferData<float>
                (BufferTarget.ArrayBuffer,
                (IntPtr)(vertexNormalData.Length * 4),
                vertexNormalData,
                BufferUsageHint.StaticDraw
             );
            GL.VertexAttribPointer(attribute_vnorm, 3,    //Get the vertex pointer from that buffer
             VertexAttribPointerType.Float,
            false, 0, 0
             );


            vbo_col = GL.GenBuffer();       //Generate a color buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_col); //Bind the buffer
            GL.BufferData<float>(BufferTarget.ArrayBuffer,    //Color data is static and therefore fill here once
             (IntPtr)(colorData.Length * 4),
            colorData, BufferUsageHint.StaticDraw
             );
            GL.VertexAttribPointer(attribute_vcol, 3,       //Get the color pointer from that buffer
             VertexAttribPointerType.Float,
            false, 0, 0
             );

            GL.EnableVertexAttribArray(attribute_vpos);     //Enable vertex drawing
            GL.EnableVertexAttribArray(attribute_vcol);     //Enable color mapping
            GL.EnableVertexAttribArray(attribute_vnorm);
        }

        void InitVertex()
        {
            int i = 0; //Hold an index counter
            float s = 2f / map.width; //Dynamically adjust step size to ensure a fit
            for (int y = 0; y < map.height - 1; ++y)
                for (int x = 0; x < map.width - 1; ++x)
                {
                    float xc = -1f + x * s; //A corrected x value
                    float yc = -1f + y * s; //A corrected y value

                    //TRIANGLE 1   3(x, y, z)
                    vertexData[i++] = xc;
                    vertexData[i++] = yc;
                    vertexData[i++] = h[x, y];

                    vertexData[i++] = xc;
                    vertexData[i++] = yc + s;
                    vertexData[i++] = h[x, y + 1];

                    vertexData[i++] = xc + s;
                    vertexData[i++] = yc;
                    vertexData[i++] = h[x + 1, y];

                    //TRIANGLE 2  3(x, y, z)
                    vertexData[i++] = xc + s;
                    vertexData[i++] = yc + s;
                    vertexData[i++] = h[x + 1, y + 1];

                    vertexData[i++] = xc + s;
                    vertexData[i++] = yc;
                    vertexData[i++] = h[x + 1, y];

                    vertexData[i++] = xc;
                    vertexData[i++] = yc + s;
                    vertexData[i++] = h[x, y + 1];
                }

            i = 0;
            for (int y = 0; y < map.height - 1; ++y)
                for (int x = 0; x < map.width - 1; ++x)
                {
                    for (int n = 0; n < 2; ++n)
                    {
                        //float f = (float)Math.Sqrt(vertexData[i] * vertexData[i] + vertexData[i + 1] * vertexData[i + 1] + vertexData[i + 2] * vertexData[i + 2]);
                        Vector3[] point = new Vector3[3];
                        for (int z = 0; z < 3; ++z)
                        {
                            point[z] = new Vector3(vertexData[i + z*3], vertexData[i+1 + z*3], vertexData[i+2 + z*3]);
                        }
                        Vector3 v1 = (point[1] - point[0]);
                        v1.Normalize();
                        Vector3 v2 = (point[2] - point[0]);
                        v2.Normalize();


                        Vector3 normal = new Vector3(
                            v1.Y * v2.Z - v2.Y * v1.Z,
                            (v1.X * v2.Z - v2.X * v1.Z) * -1,
                            v1.X * v2.Y - v2.X * v1.Y
                            );
                        normal.Normalize();

                        //Console.WriteLine(normal);

                        for (int z = 0; z < 9; ++z, ++i)
                            vertexNormalData[i] = normal[z % 3];
                    }
                }
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


/*TOP
vertexData[i++] = xc;
vertexData[i++] = yc;
vertexData[i++] = h[x, y];

vertexData[i++] = xc;
vertexData[i++] = yc - s;
vertexData[i++] = h[x, y];

vertexData[i++] = xc - s;
vertexData[i++] = yc - s;
vertexData[i++] = h[x, y];

vertexData[i++] = xc - s;
vertexData[i++] = yc;
vertexData[i++] = h[x, y]; */


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
GL.End(); */

/*SIDE ONE
vertexData[i++] = xc;
vertexData[i++] = yc;
vertexData[i++] = h[x, y];

vertexData[i++] = xc;
vertexData[i++] = yc - s;
vertexData[i++] = h[x, y];

vertexData[i++] = xc;
vertexData[i++] = yc - s;
vertexData[i++] = h[x + 1, y];

vertexData[i++] = xc;
vertexData[i++] = yc;
vertexData[i++] = h[x+1, y]; */

/*
GL.Color3(0.5f, 0, 0);
GL.Begin(PrimitiveType.Quads);
GL.Vertex3(xc, yc, h[x, y]);
GL.Vertex3(xc - s, yc, h[x, y]);
GL.Vertex3(xc - s, yc, h[x, y + 1]);
GL.Vertex3(xc, yc, h[x, y + 1]);
GL.End(); */

/* SIDE TWO
 vertexData[i++] = xc;
 vertexData[i++] = yc;
 vertexData[i++] = h[x, y];

 vertexData[i++] = xc - s;
 vertexData[i++] = yc;
 vertexData[i++] = h[x, y];

 vertexData[i++] = xc - s;
 vertexData[i++] = yc;
 vertexData[i++] = h[x, y + 1];

 vertexData[i++] = xc;
 vertexData[i++] = yc;
 vertexData[i++] = h[x, y + 1]; */


/*
VBO = GL.GenBuffer(); //Generate a vertex buffer
GL.EnableClientState(ArrayCap.VertexArray); //Notify OpenTK that we will use a vertex array
GL.BindBuffer(BufferTarget.ArrayBuffer, VBO); //Bind the buffer (needed for making the pointer)
GL.VertexPointer(3, VertexPointerType.Float, 12, 0); //Make a vertex pointer
//Data will be filled dynamically in the RenderGL() function.

CBO = GL.GenBuffer();  //Generate a color buffer
GL.EnableClientState(ArrayCap.ColorArray); //Notify OpenTK that we will use a color array
GL.BindBuffer(BufferTarget.ArrayBuffer, CBO); //Bind the buffer
GL.BufferData<float>(                       //The color data is constant, so we'll set this once
BufferTarget.ArrayBuffer,           
(IntPtr)(colorData.Length * 4),         //Length of the array in bytes
colorData,
BufferUsageHint.StaticDraw
);
GL.ColorPointer(3, ColorPointerType.Float, 12, 0);   //Finally, make the color pointer
*/
