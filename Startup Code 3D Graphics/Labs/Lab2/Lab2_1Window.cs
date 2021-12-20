using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.Lab2
{//L21T1 Rendered a big black triangle!

    class Lab2_1Window : GameWindow
    {        
        private int[] mTriangleVertexBufferObjectIDArray = new int[3];
        private int[] mSquareVertexBufferObjectIDArray = new int[3];
        private ShaderUtility mShader;

        public Lab2_1Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 2_1 Linking to Shaders and VAOs",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.CadetBlue);
            GL.Enable(EnableCap.DepthTest);

            float[] triangleVertices = new float[]  { -0.8f, 0.8f, 0.4f, 1.0f, 0.3f, 0.2f,
                                                       -0.6f, -0.4f, 0.4f, 0.4f, 0.8f, 0.9f,
                                                         0.2f, 0.2f, 0.4f, 0.3f, 0.4f, 0.7f};

            uint[] indices = new uint[] { 0, 1, 2 };

            float[] squareVertices = new float[]
            { -0.2f, -0.4f, 0.2f, 1.0f, 0.3f, 0.0f,
               0.8f, -0.4f, 0.2f, 1.0f, 0.6f, 0.0f,
               0.8f, 0.6f, 0.2f, 0.5f, 1.0f, 0.1f,
              -0.2f, 0.6f, 0.2f, 0.3f, 1.0f, 0.7f};

            //L21T7 Changed vertex colours to see fragments colour values being blended together

            uint[] indices2 = new uint[] { 0, 1, 2, 3 };


            GL.GenBuffers(2, mTriangleVertexBufferObjectIDArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mTriangleVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(triangleVertices.Length * sizeof(float)), triangleVertices, BufferUsageHint.StaticDraw);

            GL.GenBuffers(2, mSquareVertexBufferObjectIDArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mSquareVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(squareVertices.Length * sizeof(float)), squareVertices, BufferUsageHint.StaticDraw);


            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if ((triangleVertices.Length * sizeof(float) != size) && (squareVertices.Length * sizeof(float) != size))
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mTriangleVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mSquareVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices2.Length * sizeof(int)), indices2, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

            if ((indices.Length * sizeof(int) != size) && (indices2.Length * sizeof(int) != size))
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            

            #region Shader Loading Code

            mShader = new ShaderUtility(@"Lab2/Shaders/vLab21.vert", @"Lab2/Shaders/fSimple.frag");

            #endregion

           int vColourLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vColour");
            GL.EnableVertexAttribArray(vColourLocation);

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            int vColourLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vColour");
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            //L21T5 Added depth by adding an extra vertex parameter, changing the shader variable and linking and enabling depth testing


            //draw square
           GL.Uniform4(vColourLocation, Color4.Blue);

            GL.BindBuffer(BufferTarget.ArrayBuffer, mSquareVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mSquareVertexBufferObjectIDArray[1]);

            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 *
sizeof(float), 0);
          GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 *
 sizeof(float), 3 * sizeof(float));

            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 0);

            //draw triangle
            GL.BindBuffer(BufferTarget.ArrayBuffer, mTriangleVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mTriangleVertexBufferObjectIDArray[1]);

            

            #region Shader Loading Code

            GL.UseProgram(mShader.ShaderProgramID);
            
            GL.EnableVertexAttribArray(vPositionLocation);
            

            #endregion



            GL.Uniform4(vColourLocation, Color4.Red);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 *
sizeof(float), 0);
            GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 *
   sizeof(float), 3 * sizeof(float));
            //L21T2 Set uniform variable in the fragment shader to colour all fragments red

            GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);



            //L21T3 Added a blue square by creating new data and element arrays, linking the appropriate variables to the shader and changing the fragment shaders uniform colour variable
            //L21T4 Moved the red triangle so it is drawn on top of the blue square
            //L21T6 Added per vertex colour to the vertex shader, passed it to the fragment shader and linked the data buffer to the shader program
            


            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(2, mTriangleVertexBufferObjectIDArray);
            GL.DeleteBuffers(2, mSquareVertexBufferObjectIDArray);
            GL.UseProgram(0);
            mShader.Delete();
        }
    }
}
