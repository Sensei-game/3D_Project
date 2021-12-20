using OpenTK;
using System;
using OpenTK.Graphics;
using Labs.Utility;
using OpenTK.Graphics.OpenGL;

namespace Labs.Lab2
{
    public class Lab2_2Window : GameWindow
    {//L22T1 Rendered a small green square to start the camera lab

        public Lab2_2Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 2_2 Understanding the Camera",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }
     
        private int[] mVBO_IDs = new int[2];
        private int mVAO_ID;
        private ShaderUtility mShader;
        private ModelUtility mModel;
        private Matrix4 mView;

        
        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.DodgerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);




            // mView = Matrix4.Identity;
             
            //L22T6 Created a perspective camera
           mShader = new ShaderUtility(@"Lab2/Shaders/vLab22.vert", @"Lab2/Shaders/fSimple.frag");

            mView = Matrix4.CreateTranslation(0, 0, -2);
            int uViewLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            Vector3 eye = new Vector3(1.2f, 2.0f, -5.0f);
            Vector3 lookAt = new Vector3(0, 0, 0);
            Vector3 up = new Vector3(0, 1, 0);
            mView = Matrix4.LookAt(eye, lookAt, up);
            GL.UniformMatrix4(uViewLocation, true, ref mView);

            int windowHeight = this.ClientRectangle.Height;
            int windowWidth = this.ClientRectangle.Width;
            float ratio = (float)windowWidth / (float)windowHeight;


mModel =ModelUtility.LoadModel(@"Utility/Models/lab22model.sjg");    
            


            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            int vColourLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vColour");


           int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
            // Matrix4 projection = Matrix4.CreateOrthographic(10, 10, -5, 5);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 50, 50.5f);
            GL.UniformMatrix4(uProjectionLocation, true, ref projection);

            //L22T3 Added a projection matrix to the shader to increase the size of the viewing volume

            

            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            mVAO_ID = GL.GenVertexArray();
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            GL.BindVertexArray(mVAO_ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mModel.Vertices.Length * sizeof(float)), mModel.Vertices, BufferUsageHint.StaticDraw);     
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mModel.Indices.Length * sizeof(float)), mModel.Indices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mModel.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mModel.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vColourLocation);
            GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            //GL.EnableVertexAttribArray(vPositionLocation);
            //GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(vColourLocation);
            //GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(0);

            base.OnLoad(e);
            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.BindVertexArray(mVAO_ID);
            GL.DrawElements(BeginMode.Triangles, mModel.Indices.Length, DrawElementsType.UnsignedInt, 0);

            int uModelLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
          Matrix4 m1 = Matrix4.CreateRotationZ(0.8f);
            Matrix4 r1 = Matrix4.CreateTranslation(3f, 0, 0);
            m1 = Matrix4.Mult(m1, r1);
            
            GL.UniformMatrix4(uModelLocation, true, ref m1);
            


           GL.DrawElements(BeginMode.Triangles, mModel.Indices.Length, DrawElementsType.UnsignedInt, 0); 
          Matrix4 r2 = Matrix4.CreateRotationZ(0.8f);
            Matrix4 m2 =  Matrix4.CreateTranslation(-4f, 0.5f , 0);
              
            m2 = Matrix4.Mult( r2,m2);
           
            GL.UniformMatrix4(uModelLocation, true, ref m2);

            //L22T4 Two squares translated and rotated

            //L22T3 Reused a model to render two squares by changing the model matrix


            GL.BindVertexArray(0);
            this.SwapBuffers();

          //  L22T2 Translated the square one unit to the right
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == 'd')
            {
                mView = mView * Matrix4.CreateTranslation(0.09f, 0, 0);
                MoveCamera();
            }
            if (e.KeyChar == 'a')
            {
                mView = mView * Matrix4.CreateTranslation(-0.09f, 0, 0);
                MoveCamera();
            }
            if (e.KeyChar == 'w')
            {
                mView = mView * Matrix4.CreateRotationY(0.01f);
                MoveCamera();
            }
            if (e.KeyChar == 's')
            {
                mView = mView * Matrix4.CreateRotationY( -0.01f);
                MoveCamera();
            }
            if (e.KeyChar == 'q')
            {
                mView = mView * Matrix4.CreateTranslation(0, 0, -0.09f);
                MoveCamera();
            }
            if (e.KeyChar == 'e')
            {
                mView = mView * Matrix4.CreateTranslation(0, 0, 0.09f);
                MoveCamera();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            //search for 2 bugs in this if shade thingy
            if (mShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                int windowHeight = this.ClientRectangle.Height;
                int windowWidth = this.ClientRectangle.Width;

                if (windowHeight > windowWidth)
                {
                    if (windowWidth < 1) { windowWidth = 1; }
                    float ratio = windowHeight / windowWidth;
                    // Matrix4 projection = Matrix4.CreateOrthographic(ratio * 10, 10, -1, 1);
                    Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 15);
                    GL.UniformMatrix4(uProjectionLocation, true, ref projection);
                }
                else
                {
                    if (windowHeight < 1) { windowHeight = 1; }
                    float ratio = windowWidth / windowHeight;
                    // Matrix4 projection = Matrix4.CreateOrthographic(10, ratio * 10, -1, 1);
                    Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 15);
                    GL.UniformMatrix4(uProjectionLocation, true, ref projection);
                }
            }
        }
        //L22T4 Can change the viewport on resize, but the aspect ratio still isn’t right
        private void MoveCamera()
        {
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);
        }


        //L22T5 Set up vertex shader to use view matrix and set view matrix in new OnKeyPress function
        //L22T6 Refactors camera code and eliminated magic numbers

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArray(mVAO_ID);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
