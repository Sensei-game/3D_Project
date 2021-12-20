using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace Labs.Lab3
{
    public class Lab3Window : GameWindow
    {
        public Lab3Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 3 Lighting and Material Properties",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        private int[] mVBO_IDs = new int[7];
        private int[] mVAO_IDs = new int[4];

        float delta = 0.0f;
        int trigger = 0;
        private ShaderUtility mShader;
        private int mTexture_ID;
        private float mThreshold;
        private int mRateOfDissolve = 5;
        private float timestep = 10.5f;

        private ModelUtility mSphereModelUtility, mBunnyModelUtility, mCylinderModelUtility;
        private Matrix4 mView, mSphereModel, mGroundModel, mBunnyModel, mCylinderModel;

        protected override void OnLoad(EventArgs e)
        {
            
            GL.ClearColor(Color4.Black);
            

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            
            mShader = new ShaderUtility(@"Lab3/Shaders/vPassThrough.vert", @"Lab3/Shaders/fLighting.frag");
            GL.UseProgram(mShader.ShaderProgramID);

            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            
            #region Texture effect - Pretty nice
            //    int vTexCoordsLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vTexCoords");

            //    int uThresholdLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uThreshold");
            //    GL.Uniform1(uThresholdLocation, 0.5f);
            //    { 




            //    string filepath = @"Lab3/Texture.png";
            //    if (System.IO.File.Exists(filepath))
            //    {

            //        Bitmap TextureBitmap = new Bitmap(filepath);
            //        BitmapData TextureData = TextureBitmap.LockBits(
            //        new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
            //        TextureBitmap.Height), ImageLockMode.ReadOnly,
            //        System.Drawing.Imaging.PixelFormat.Format32bppRgb);



            //        GL.ActiveTexture(TextureUnit.Texture0);
            //        GL.GenTextures(0, out mTexture_ID);
            //        GL.BindTexture(TextureTarget.Texture2D, mTexture_ID);

            //        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, TextureData.Scan0);

            //        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            //        (int)TextureMinFilter.Linear);

            //        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            //        (int)TextureMagFilter.Linear);

            //        TextureBitmap.UnlockBits(TextureData);
            //        //L5T4 Loaded texture from memory onto the graphics card


            //        TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //        //L5T8 Flipped the image before loading onto the graphics card 

            //    }
            //    else
            //    {
            //        throw new Exception("Could not find file " + filepath);
            //    }



            //    int uTextureSamplerLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureSampler2");
            //    //L5T9 Loaded a second texture

            //    GL.Uniform1(uTextureSamplerLocation, 0);
            //}
            #endregion

            int vNormalPosition = GL.GetAttribLocation(mShader.ShaderProgramID, "vNormal");

            mView = Matrix4.CreateTranslation(0, -2.5f, 4);

            int EyePosition = GL.GetAttribLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.UniformMatrix4(EyePosition, true, ref mView);

            
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

           
            mGroundModel = Matrix4.CreateTranslation(0, 0, -10f);
            mBunnyModel = Matrix4.CreateTranslation(0, 3, -1.3f);
            mCylinderModel = Matrix4.CreateTranslation(0, 1, -1.3f);

           
            #region Ligth Positions
            int uLightPositionLocation0 = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[0].Position");

            Vector4 lightPosition0 = Vector4.Transform(new Vector4(2, 4, -8.2f, 0.9f), mView);
            GL.Uniform4(uLightPositionLocation0, lightPosition0);

            int uLightPositionLocation1 = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].Position");

            Vector4 lightPosition1 = Vector4.Transform(new Vector4(3.5f, 4, -8.2f, 0.9f), mView);
            GL.Uniform4(uLightPositionLocation1, lightPosition1);
            int uLightPositionLocation2 = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].Position");

            Vector4 lightPosition2 = Vector4.Transform(new Vector4(0.5f, 4, -8.2f, 0.9f), mView);
            GL.Uniform4(uLightPositionLocation2, lightPosition2);

            #endregion 
            //defult values 1.0f, 1.0f, 1.0f

            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            
            float[] vertices = new float[] {-10, 0, -10,0,1,0,
                                             -10, 0, 10,0,1,0,
                                             10, 0, 10,0,1,0,
                                             10, 0, -10,0,1,0,};
            //Ground
            #region Ground VAO AND VBO
            GL.BindVertexArray(mVAO_IDs[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);


            //Ground

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

          
           



            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalPosition);
            GL.VertexAttribPointer(vNormalPosition, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 0);

            //GL.EnableVertexAttribArray(vTexCoordsLocation);
            //GL.VertexAttribPointer(vTexCoordsLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);


            #endregion




            mSphereModelUtility = ModelUtility.LoadModel(@"Utility/Models/sphere.bin");

           //model
           #region Model VAO AND VBO
            mBunnyModelUtility = ModelUtility.LoadModel(@"Utility/Models/model.bin");

          
            GL.BindVertexArray(mVAO_IDs[1]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[1]);
            // GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mSphereModelUtility.Vertices.Length * sizeof(float)), mSphereModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mBunnyModelUtility.Vertices.Length * sizeof(float)), mBunnyModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[2]);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mSphereModelUtility.Indices.Length * sizeof(float)), mSphereModelUtility.Indices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mBunnyModelUtility.Indices.Length * sizeof(float)), mBunnyModelUtility.Indices, BufferUsageHint.StaticDraw);

            





            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            //L3T3 Added per vertex attribute for the normal and linked VBO data to it



            if (mBunnyModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mBunnyModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

       
            

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);


            GL.EnableVertexAttribArray(vNormalPosition);
            GL.VertexAttribPointer(vNormalPosition, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 0);

            //GL.EnableVertexAttribArray(vTexCoordsLocation);
            //GL.VertexAttribPointer(vTexCoordsLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            #endregion



            //cylinder
            #region Cylinder VAO AND VBO
            mCylinderModelUtility = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");

           
            GL.BindVertexArray(mVAO_IDs[2]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[3]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mCylinderModelUtility.Vertices.Length * sizeof(float)), mCylinderModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[4]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mCylinderModelUtility.Indices.Length * sizeof(float)), mCylinderModelUtility.Indices, BufferUsageHint.StaticDraw);

           

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinderModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");

            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinderModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

           
            

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);


            GL.EnableVertexAttribArray(vNormalPosition);
            GL.VertexAttribPointer(vNormalPosition, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 0);

            //GL.EnableVertexAttribArray(vTexCoordsLocation);
            //GL.VertexAttribPointer(vTexCoordsLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            #endregion

            GL.BindVertexArray(0);

            base.OnLoad(e);

        }

        #region Material Methods
        private void Shiness(float x)
        {
            int uShiness = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.Shininess");
            float value = x;
            GL.Uniform1(uShiness, value);
            
        }
        private void SpecularReflectivity(float x,float y , float z)
        {
            int uSpecularReflecivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
            Vector3 SpecularReflectivityColour = new Vector3(x,y,z);
            GL.Uniform3(uSpecularReflecivity, SpecularReflectivityColour);
        }
        private void DiffuseReflectivity(float x, float y, float z)
        {
            int uDiffuseReflecivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
            Vector3 diffuseReflectivityColour = new Vector3(x, y, z);
            GL.Uniform3(uDiffuseReflecivity, diffuseReflectivityColour);
        }
        private void AmbientReflectivity(float x, float y, float z)
        {
            int uAmbientReflecivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
            Vector3 ambientReflectivityColour = new Vector3(x, y, z);
            GL.Uniform3(uAmbientReflecivity, ambientReflectivityColour);
        }
        #endregion

        #region Different Colour Lights
        private void SpecularLigth0()
        {
            int uSpeculiarLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[0].SpecularLight");
            Vector3 specularcolour = new Vector3(1.9f, 0.0f, 0.0f);
            GL.Uniform3(uSpeculiarLightLocation, specularcolour);
        }
        private void SpecularLigth1()
        {
            int uSpeculiarLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].SpecularLight");
            Vector3 specularcolour = new Vector3(0.0f, 1.9f, 0.0f);
            GL.Uniform3(uSpeculiarLightLocation, specularcolour);
        }
        private void SpecularLigth2()
        {
            int uSpeculiarLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].SpecularLight");
            Vector3 specularcolour = new Vector3(0.0f, 0.0f, 1.9f);
            GL.Uniform3(uSpeculiarLightLocation, specularcolour);
        }
        private void DiffuseLigth0()
        {
            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[0].DiffuseLight");
            Vector3 diffusecolour = new Vector3(1.9f, 0.0f, 0.0f);
            GL.Uniform3(uDiffuseLightLocation, diffusecolour);
        }
        private void DiffuseLigth1()
        {
            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].DiffuseLight");
            Vector3 diffusecolour = new Vector3(0.0f, 1.9f, 0.0f);
            GL.Uniform3(uDiffuseLightLocation, diffusecolour);
        }
        private void DiffuseLigth2()
        {
            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].DiffuseLight");
            Vector3 diffusecolour = new Vector3(0.0f, 0.0f, 1.9f);
            GL.Uniform3(uDiffuseLightLocation, diffusecolour);
        }
        private void AmbientLight0()
        {
            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[0].AmbientLight");
            Vector3 ambientcolour = new Vector3(1.9f, 0.0f, 0.0f);
            GL.Uniform3(uAmbientLightLocation, ambientcolour);
        }

        private void AmbientLight1()
        {
            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].AmbientLight");
            Vector3 ambientcolour = new Vector3(0.0f, 1.9f, 0.0f);
            GL.Uniform3(uAmbientLightLocation, ambientcolour);
        }
        private void AmbientLight2()
        {
            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].AmbientLight");
            Vector3 ambientcolour = new Vector3(0.0f, 0.0f, 1.9f);
            GL.Uniform3(uAmbientLightLocation, ambientcolour);
        }
        #endregion

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if(e.KeyChar == 'u' && trigger==0)
            {
                 trigger = 1;
            }
            else if(e.KeyChar == 'u')
                {
                trigger = 0;
            }
            if (e.KeyChar == 'z')
            {
                Vector3 t = mBunnyModel.ExtractTranslation();
                Matrix4 translation = Matrix4.CreateTranslation(t);
                Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
                mBunnyModel = mBunnyModel * inverseTranslation * Matrix4.CreateRotationY(0.25f) *
                translation;
            }
            if (e.KeyChar == 'c')
            {
                Vector3 t = mBunnyModel.ExtractTranslation();
                Matrix4 translation = Matrix4.CreateTranslation(t);
                Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
                mBunnyModel = mBunnyModel * inverseTranslation * Matrix4.CreateRotationY(-0.25f) *
                translation;
            }

            
            if (e.KeyChar == 'e')
            {
                Vector3 t = mGroundModel.ExtractTranslation();
                Matrix4 translation = Matrix4.CreateTranslation(t);
                Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
                mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(-0.025f) *
                translation;
            }
            if (e.KeyChar == 'q')
            {
                Vector3 t = mGroundModel.ExtractTranslation();
                Matrix4 translation = Matrix4.CreateTranslation(t);
                Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
                mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(0.025f) *
                translation;
            }
            

            if (e.KeyChar == 'w') {
                mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, 0.05f);

                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                int EyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                    GL.UniformMatrix4(EyePosition,true,ref mView);
            }
            if (e.KeyChar == 's')
            {
                mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, -0.05f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                int EyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.UniformMatrix4(EyePosition, true, ref mView);
            }
            if (e.KeyChar == 't')
            {
                mView = mView * Matrix4.CreateTranslation(0.0f, 0.05f, 0f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                int EyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.UniformMatrix4(EyePosition, true, ref mView);
            }
            if (e.KeyChar == 'g')
            {
                mView = mView * Matrix4.CreateTranslation(0.0f, -0.05f, 0f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                int EyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.UniformMatrix4(EyePosition, true, ref mView);
            }
            if (e.KeyChar == 'a')
            {
                mView = mView * Matrix4.CreateRotationY(-0.025f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                int EyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.UniformMatrix4(EyePosition, true, ref mView);
            }
            
            if (e.KeyChar == 'd')
            {
                mView = mView * Matrix4.CreateRotationY(0.025f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                int EyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.UniformMatrix4(EyePosition, true, ref mView);
            }
         
            
        }
              
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Vector3 rotate = new Vector3(0, 0.1f, 0);
            Matrix4 cube = Matrix4.CreateRotationY(delta);

            //ground
            GL.BindVertexArray(mVAO_IDs[0]);
            int uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);

            #region Ligths and material properties
            AmbientLight0();
            DiffuseLigth0();
            SpecularLigth0();
            AmbientLight1();
            DiffuseLigth1();
            SpecularLigth1();
            AmbientLight2();
            DiffuseLigth2();
            SpecularLigth2();
            AmbientReflectivity(0.0f, 0.05f, 0.05f);
            DiffuseReflectivity(0.4f, 0.5f, 0.5f);
            SpecularReflectivity(0.04f, 0.7f, 0.7f);
            Shiness(0.078125f);
            #endregion

            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            //Model
            GL.BindVertexArray(mVAO_IDs[1]);
            Matrix4 m = mBunnyModel * mGroundModel*cube;
           int aModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(aModel, true, ref m);

            #region Ligths and material properties
            AmbientLight0();
            DiffuseLigth0();
            SpecularLigth0();
            AmbientLight1();
            DiffuseLigth1();
            SpecularLigth1();
            AmbientLight2();
            DiffuseLigth2();
            SpecularLigth2();
            AmbientReflectivity(0.1745f,0.01175f,0.01175f);
            DiffuseReflectivity(0.61424f,0.04136f,0.04136f);
            SpecularReflectivity(0.727811f,0.626959f,0.626959f);
            Shiness(0.6f);
            #endregion

            GL.DrawElements(PrimitiveType.Triangles, mBunnyModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            
            //Cylinder
            GL.BindVertexArray(mVAO_IDs[2]);
            Matrix4 n = mCylinderModel * mGroundModel;
           int bModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(bModel, true, ref n);

            #region Ligths and material properties
            AmbientLight0();
            DiffuseLigth0();
            SpecularLigth0();
            AmbientLight1();
            DiffuseLigth1();
            SpecularLigth1();
            AmbientLight2();
            DiffuseLigth2();
            SpecularLigth2();
            AmbientReflectivity(0.05375f, 0.05f, 0.06625f);
            DiffuseReflectivity(0.18275f, 0.17f, 0.22525f);
            SpecularReflectivity(0.332741f, 0.328634f, 0.346435f);
            Shiness(0.3f);
            #endregion

            GL.DrawElements(PrimitiveType.Triangles, mCylinderModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);
           


            
            //L3T14 I made a <ruby> <armadilo> on a <obsidian> cylinder, with a <cyan rubber> ground 
            this.SwapBuffers();
            if (trigger == 1)
            {
                delta -= 0.01f;
                if (delta > 0.5f)
                {
                    delta = 0;
                }
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
