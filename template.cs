using System;
using System.IO;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Template
{
	public class OpenTKApp : GameWindow
	{
		static int screenID;
		static Game game;
		static bool terminated = false;
		protected override void OnLoad( EventArgs e )
		{
			// called upon app init
			ClientSize = new Size( 640, 400 );
			game = new Game(this);
			game.screen = new Surface( Width, Height );
			Sprite.target = game.screen;
			screenID = game.screen.GenTexture();
			game.Init();
		}
		protected override void OnUnload( EventArgs e )
		{
			// called upon app close
			GL.DeleteTextures( 1, ref screenID );
			Environment.Exit( 0 ); // bypass wait for key on CTRL-F5
		}
		protected override void OnResize( EventArgs e )
		{
			// called upon window resize
			GL.Viewport(0, 0, Width, Height);
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			GL.Ortho( -1.0, 1.0, -1.0, 1.0, 0.0, 4.0 );
		}
		protected override void OnUpdateFrame( FrameEventArgs e )
		{
			// called once per frame; app logic
			var keyboard = OpenTK.Input.Keyboard.GetState();
			if (keyboard[OpenTK.Input.Key.Escape]) this.Exit();

            //Rotation controls
            if (keyboard[OpenTK.Input.Key.R]) game.autoRotate = true;
            if (keyboard[OpenTK.Input.Key.F])
            {
                game.rotation = game.a;
                game.autoRotate = false;
            }
            if (game.autoRotate)
            {
                if (keyboard[OpenTK.Input.Key.D]) game.rotateSpeed += 0.05f;
                if (keyboard[OpenTK.Input.Key.A]) game.rotateSpeed -= 0.05f;
            }
            else
            {
                if (keyboard[OpenTK.Input.Key.D]) game.rotation += 0.05f;
                if (keyboard[OpenTK.Input.Key.A]) game.rotation -= 0.05f;
            }

            //Camera controls
            if (keyboard[OpenTK.Input.Key.W]) game.zoom += 0.01f;
            if (keyboard[OpenTK.Input.Key.S]) game.zoom -= 0.01f;
            if (keyboard[OpenTK.Input.Key.Right]) game.translation.X -= 0.01f;
            if (keyboard[OpenTK.Input.Key.Left]) game.translation.X += 0.01f;
            if (keyboard[OpenTK.Input.Key.Up]) game.translation.Z += 0.01f;
            if (keyboard[OpenTK.Input.Key.Down]) game.translation.Z -= 0.01f;

            //Light controls
            if (keyboard[OpenTK.Input.Key.O]) game.intensity -= 0.01f;
            if (keyboard[OpenTK.Input.Key.P]) game.intensity += 0.01f;
            if (keyboard[OpenTK.Input.Key.J]) game.Lpos.X += 0.05f;
            if (keyboard[OpenTK.Input.Key.L]) game.Lpos.X -= 0.05f;
            if (keyboard[OpenTK.Input.Key.K]) game.Lpos.Y += 0.05f;
            if (keyboard[OpenTK.Input.Key.I]) game.Lpos.Y -= 0.05f;
            if (keyboard[OpenTK.Input.Key.U]) game.Lpos.Z += 0.05f;
            if (keyboard[OpenTK.Input.Key.H]) game.Lpos.Z -= 0.05f;

            //Effect controls
            if (keyboard[OpenTK.Input.Key.Z]) game.GoLoco = 2;
            if (keyboard[OpenTK.Input.Key.X]) game.GoLoco = 1;
            if (keyboard[OpenTK.Input.Key.C]) game.GoLoco = 0;
        }
		protected override void OnRenderFrame( FrameEventArgs e )
		{
            // prepare for generic OpenGL rendering
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();
            GL.Color3(1.0f, 1.0f, 1.0f);
            // called once per frame; render
            game.Tick();
			if (terminated) 
			{
				Exit();
				return;
			}
			// convert Game.screen to OpenGL texture
			//GL.BindTexture( TextureTarget.Texture2D, screenID );
			//GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
						   //game.screen.width, game.screen.height, 0, 
						   //OpenTK.Graphics.OpenGL.PixelFormat.Bgra, 
						  // PixelType.UnsignedByte, game.screen.pixels 
						// );
            // clear window contents
            GL.Clear( ClearBufferMask.ColorBufferBit);
            // setup camera
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            // draw screen filling quad

            /*
            GL.Color3(0f, 0f, 0f);
			GL.Begin( PrimitiveType.Quads );
			GL.Vertex2( -1.0f, -1.0f );
			GL.Vertex2(  1.0f, -1.0f );
			GL.Vertex2(  1.0f,  1.0f );
			GL.Vertex2( -1.0f,  1.0f );
			GL.End();
            */

            //Run the game GL
            game.RenderGL();
			// tell OpenTK we're done rendering
			SwapBuffers();
		}
		public static void Main( string[] args ) 
		{ 
			// entry point
			using (OpenTKApp app = new OpenTKApp()) { app.Run( 30.0, 0.0 ); }
		}
	}
}