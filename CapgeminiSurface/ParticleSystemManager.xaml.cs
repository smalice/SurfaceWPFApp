﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface.Presentation.Controls;

namespace CapgeminiSurface
{
    public partial class ParticleSystemManager : SurfaceUserControl
    {
        
        private double minParticleSize = 1.0;
        private double maxParticleLife = 10.0;
        private double rendersPerSecond = 24.0;

        private double particleX;
        private double particleY;
        private double particleZ;

        public double particleSpeed { get; set; }
        public bool generateParticles { get; set; }

        public Point3D particlePoint3D { get; set; }

        private int maxParticlesPerColor = 1000;

        private Point3D spawnPoint;
        private double elapsed;
        private double totalElapsed;
        private int lastTick;
        private int currentTick;

        private readonly DispatcherTimer frameTimer;

        private readonly ParticleSystemManagerEntity particleSystemManagerEntity;

        private readonly Random random;

        public ParticleSystemManager()
        {
            InitializeComponent();

            generateParticles = true;

            particleSpeed = 30.0;

            particlePoint3D = new Point3D(particleX, particleY, particleZ);

            frameTimer = new DispatcherTimer();
            frameTimer.Tick += OnFrame;
            frameTimer.Interval = TimeSpan.FromSeconds(1.0/rendersPerSecond);
            frameTimer.Start();

            spawnPoint = new Point3D(0, 0, 0.0);
            lastTick = Environment.TickCount;

            particleSystemManagerEntity = new ParticleSystemManagerEntity();

            WorldModels.Children.Add(particleSystemManagerEntity.CreateParticleSystem(maxParticlesPerColor,
                                                                                      Colors.Silver));
            WorldModels.Children.Add(particleSystemManagerEntity.CreateParticleSystem(maxParticlesPerColor,
                                                                                      Colors.LightBlue));

            random = new Random(GetHashCode());
        }

        private void OnFrame(object sender, EventArgs e)
        {
            currentTick = Environment.TickCount;
            elapsed = (currentTick - lastTick)/1000.0;
            totalElapsed += elapsed;
            lastTick = currentTick;

            particleSystemManagerEntity.Update((float) elapsed);

            // #FELO: ON<->OFF Switch
            if (generateParticles)
            {
                particleSystemManagerEntity.SpawnParticle(spawnPoint, particleSpeed, Colors.Silver,
                                                          minParticleSize + random.NextDouble(),
                                                          maxParticleLife*random.NextDouble());
                particleSystemManagerEntity.SpawnParticle(spawnPoint, particleSpeed, Colors.LightBlue,
                                                          minParticleSize + random.NextDouble(),
                                                          maxParticleLife*random.NextDouble());
                particleSystemManagerEntity.SpawnParticle(spawnPoint, particleSpeed, Colors.Silver,
                                                          minParticleSize + random.NextDouble(),
                                                          maxParticleLife*random.NextDouble());
                particleSystemManagerEntity.SpawnParticle(spawnPoint, particleSpeed, Colors.LightBlue,
                                                          minParticleSize + random.NextDouble(),
                                                          maxParticleLife*random.NextDouble());
                // #FELO: Respawn point.
                spawnPoint = particlePoint3D;
            }
        }
    }

    #region GenericParticleSystemClasses
    
    public class Particle
    {
        public Point3D Position;
        public Vector3D Velocity;
        public double StartDuration;
        public double Duration;
        public double Decay;
        public double StartSize;
        public double Size;
    }

    public class ParticleSystemManagerEntity
    {
        private readonly Dictionary<Color, ParticleSystem> particleSystems;

        public ParticleSystemManagerEntity()
        {
            particleSystems = new Dictionary<Color, ParticleSystem>();
        }

        public void Update(float elapsed)
        {
            foreach (ParticleSystem particleSystem in particleSystems.Values)
            {
                particleSystem.Update(elapsed);
            }
        }

        public void SpawnParticle(Point3D position, double speed, Color color, double size, double duration)
        {
            try
            {
                ParticleSystem particleSystem = particleSystems[color];
                particleSystem.SpawnParticle(position, speed, size, duration, new Particle());
            }
            catch
            {
            }
        }

        public Model3D CreateParticleSystem(int maxCount, Color color)
        {
            var particleSystem = new ParticleSystem(maxCount, color);
            particleSystems.Add(color, particleSystem);
            return particleSystem.ParticleModel;
        }

        public int ActiveParticleCount
        {
            get
            {
                int count = 0;
                foreach (ParticleSystem particleSystem in particleSystems.Values)
                    count += particleSystem.Count;
                return count;
            }
        }
    }

    public class ParticleSystem
    {
        private readonly List<Particle> particleList;
        private readonly GeometryModel3D particleModel;
        private int maxParticleCount;
        private Random random;

        private const double InitialSpeedConst = 0.25f;
        private const double DecayAmount = 1.0f;
        private const double ParticleSizeConst = 32.0;
        private const double StandardColorOffset = 0.25;
        private const double StandardColorOffsetBorder = 1.0;
        private float directionX = 1.0f;
        private float directionY = 1.0f;
        private float directionDistortionX = 2.0f;
        private float directionDistortionY = 2.0f;


        public ParticleSystem(int maxCount, Color color)
        {
            maxParticleCount = maxCount;

            particleModel = new GeometryModel3D();
            particleModel.Geometry = new MeshGeometry3D();

            particleList = new List<Particle>();

            //FELO: Drawing particle: Ellipse.
            var ellipse = new Ellipse();
            ellipse.Width = ParticleSizeConst;
            ellipse.Height = ParticleSizeConst;
            var radialGradientBrush = new RadialGradientBrush();
            radialGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, color.R, color.G, color.B), StandardColorOffset));
            radialGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, color.R, color.G, color.B), StandardColorOffsetBorder));
            ellipse.Fill = radialGradientBrush;
            ellipse.Measure(new Size(ParticleSizeConst, ParticleSizeConst));
            ellipse.Arrange(new Rect(0, 0, ParticleSizeConst, ParticleSizeConst));

            ApplyMaterialToBrush(ellipse);
        }

        public void ApplyMaterialToBrush(Ellipse ellipse)
        {
            Brush brush = null;

            #if USE_VISUALBRUSH  //--------------------------------------------------------------

            brush = new VisualBrush(e);

            #else

            var renderTarget = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(ellipse);
            renderTarget.Freeze();
            brush = new ImageBrush(renderTarget);

            #endif  //---------------------------------------------------------------------------

            var material = new DiffuseMaterial(brush);

            // #FELO: EmissiveMaterial is more radiating. Could actually be a good alternative.
            //var material = new EmissiveMaterial(brush);

            particleModel.Material = material;

            random = new Random(brush.GetHashCode());
        }

        public int MaxParticleCount
        {
            get { return maxParticleCount; }
            set { maxParticleCount = value; }
        }

        public int Count
        {
            get { return particleList.Count; }
        }

        public Model3D ParticleModel
        {
            get { return particleModel; }
        }

        public void Update(double elapsed)
        {
            var recycleList = new List<Particle>();

            foreach (Particle p in particleList)
            {
                p.Position += p.Velocity * elapsed;
                p.Duration -= p.Decay * elapsed;
                p.Size = p.StartSize * ( p.Duration / p.StartDuration );
                if (p.Duration <= 0.0)
                {
                    recycleList.Add(p);
                }
            }

            foreach (Particle p in recycleList)
            {
                particleList.Remove(p);
            }
          
            Update3DGeometry();
        }

        private void Update3DGeometry()
        {
            var positions = new Point3DCollection();
            var indices = new Int32Collection();
            var texcoords = new PointCollection();

            for (int i = 0; i < particleList.Count; ++i)
            {
                int positionIndex = i*4;
                //int indexIndex = i*6;
                Particle particle = particleList[i];

                var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);
                var p2 = new Point3D(particle.Position.X, particle.Position.Y + particle.Size, particle.Position.Z);
                var p3 = new Point3D(particle.Position.X + particle.Size, particle.Position.Y + particle.Size,
                                     particle.Position.Z);
                var p4 = new Point3D(particle.Position.X + particle.Size, particle.Position.Y, particle.Position.Z);

                positions.Add(p1);
                positions.Add(p2);
                positions.Add(p3);
                positions.Add(p4);

                var t1 = new Point(0.0, 0.0);
                var t2 = new Point(0.0, 1.0);
                var t3 = new Point(1.0, 1.0);
                var t4 = new Point(1.0, 0.0);

                texcoords.Add(t1);
                texcoords.Add(t2);
                texcoords.Add(t3);
                texcoords.Add(t4);

                indices.Add(positionIndex);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 2);
            }

            ((MeshGeometry3D) particleModel.Geometry).Positions = positions;
            ((MeshGeometry3D) particleModel.Geometry).TriangleIndices = indices;
            ((MeshGeometry3D) particleModel.Geometry).TextureCoordinates = texcoords;
        }

        public void SpawnParticle(Point3D position, double speed, double size, double duration, Particle particle)
        {
            if (particleList.Count > maxParticleCount)
            {
                return;
            }
            particle.Position = position;
            particle.StartDuration = duration;
            particle.Duration = duration;
            particle.StartSize = size;
            particle.Size = size;

            // #FELO: -1 to 1 for x and -1 to 1 for y coordinate creates 360 degree spread.
            float x = directionX - (float)random.NextDouble() * directionDistortionX;
            float z = directionY - (float)random.NextDouble() * directionDistortionY;

            var vector3D = new Vector3D( x, z, 0.0 );
            vector3D.Normalize();
            vector3D *= ( InitialSpeedConst + (float)random.NextDouble() ) * (float)speed;

            particle.Velocity = new Vector3D( vector3D.X, vector3D.Y, vector3D.Z);

            particle.Decay = DecayAmount;

            particleList.Add(particle);
        }
    }

    #endregion
}