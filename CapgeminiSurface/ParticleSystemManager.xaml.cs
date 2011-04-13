using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using CapgeminiSurface.Util;
using Microsoft.Surface.Presentation;

namespace CapgeminiSurface
{
    #region PartialClass

    public partial class ParticleSystemManager
    {
        private double _minParticleSize = 1.0;
        private double _maxParticleLife = 10.0;
        private const double RendersPerSecond = 24.0;

        private bool _whiteParticles;
        private bool _blueParticles;
        private bool _greenParticles;
        private bool _redParticles;
        private bool _yellowParticles;
        private bool _pinkParticles;

        public double ParticleSpeed { get; set; }
        public bool GenerateParticles { get; set; }

        public Point3D ParticlePoint3D { get; set; }

        private int _maxParticlesPerColor = 1000;

        private Point3D _spawnPoint;
        private double _elapsed;
        private double _totalElapsed;
        private int _lastTick;
        private int _currentTick;

        private readonly DispatcherTimer _frameTimer;

        private readonly ParticleSystemManagerEntity _particleSystemManagerEntity;

        private readonly Random _random;

        public ParticleSystemManager()
        {
            InitializeComponent();

            sAmount.Value = _maxParticlesPerColor;
            
            GenerateParticles = true;

            SetParticleDuration(10.0);
            SetSpeedSlider(15.0);

            _frameTimer = new DispatcherTimer();
            _frameTimer.Tick += OnFrame;
            _frameTimer.Interval = TimeSpan.FromSeconds(1.0/RendersPerSecond);
            _frameTimer.Start();

            _spawnPoint = new Point3D(0, 0, 0.0);
            _lastTick = Environment.TickCount;

            _particleSystemManagerEntity = new ParticleSystemManagerEntity();

            WorldModels.Children.Add(_particleSystemManagerEntity.CreateParticleSystem(_maxParticlesPerColor,
                                                                                      Colors.Silver));
            WorldModels.Children.Add(_particleSystemManagerEntity.CreateParticleSystem(_maxParticlesPerColor,
                                                                                      Colors.LightBlue));
            WorldModels.Children.Add(_particleSystemManagerEntity.CreateParticleSystem(_maxParticlesPerColor,
                                                                                      Colors.DarkRed));
            WorldModels.Children.Add(_particleSystemManagerEntity.CreateParticleSystem(_maxParticlesPerColor,
                                                                                      Colors.LightSeaGreen));
            WorldModels.Children.Add(_particleSystemManagerEntity.CreateParticleSystem(_maxParticlesPerColor,
                                                                                      Colors.Yellow));
            WorldModels.Children.Add(_particleSystemManagerEntity.CreateParticleSystem(_maxParticlesPerColor,
                                                                                      Colors.Pink));
            _random = new Random(GetHashCode());
        }

        public void SetSpeedSlider(double number)
        {
            ParticleSpeed = number;
            sSpeed.Value = (int)ParticleSpeed;
        }

        public void SetAmountParticles(int number)
        {
            _maxParticlesPerColor = number;
        }

        public void SetParticleDuration(double number)
        {
            _maxParticleLife = number;
            sAmount.Value = _maxParticleLife;
        }

        private void OnFrame(object sender, EventArgs e)
        {
            _currentTick = Environment.TickCount;
            _elapsed = (_currentTick - _lastTick)/1000.0;
            _totalElapsed += _elapsed;
            _lastTick = _currentTick;

            _particleSystemManagerEntity.Update((float) _elapsed);

            // #FELO: ON<->OFF Switch
            if (GenerateParticles)
            {
                if (_whiteParticles)
                {
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.Silver,
                                          _minParticleSize + _random.NextDouble(),
                                          _maxParticleLife * _random.NextDouble());
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.Silver,
                                          _minParticleSize + _random.NextDouble(),
                                          _maxParticleLife * _random.NextDouble());
                }
                if (_redParticles)
                {
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.DarkRed,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife * _random.NextDouble());
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.DarkRed,
                                                      _minParticleSize + _random.NextDouble(),
                                                      _maxParticleLife * _random.NextDouble());
                }
                if (_blueParticles)
                {
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.LightBlue,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife * _random.NextDouble());
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.LightBlue,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife * _random.NextDouble());

                }
                if (_greenParticles)
                {
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.LightSeaGreen,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife*_random.NextDouble());
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.LightSeaGreen,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife * _random.NextDouble());
                }
                if (_pinkParticles)
                {
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.Pink,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife * _random.NextDouble());
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.Pink,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife * _random.NextDouble()); 
                }
                if (_yellowParticles)
                {
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.Yellow,
                                                              _minParticleSize + _random.NextDouble(),
                                                              _maxParticleLife * _random.NextDouble());
                    _particleSystemManagerEntity.SpawnParticle(_spawnPoint, ParticleSpeed, Colors.Yellow,
                                                             _minParticleSize + _random.NextDouble(),
                                                             _maxParticleLife * _random.NextDouble());
                }
                // #FELO: Respawn point.
                _spawnPoint = ParticlePoint3D;
            }
        }

        private void SurfaceSliderContactLeave(object sender, ContactEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _minParticleSize = sSlider.Value;
        }

        private void SSpeedContactLeave(object sender, ContactEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            SetSpeedSlider(sSpeed.Value);
        }

        private void SAmountContactLeave(object sender, ContactEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            SetParticleDuration(sAmount.Value);
        }

        public void ToggleControlPanel()
        {
            var controlsAniStory = (Storyboard)FindResource("ControlsAni");
            controlsAniStory.Remove();
            controlsAniStory.Begin();
        }

        private void SurfaceToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            GenerateParticles = false;
        }

        private void SurfaceToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            GenerateParticles = true;
        }

        private void SurfaceButtonContactEnter(object sender, ContactEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            var controlsAniStory = (Storyboard)FindResource("ControlsAniRev");
            controlsAniStory.Remove();
            controlsAniStory.Begin();   
        }

        private void WhiteButtonUnchecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _whiteParticles = false;
        }

        private void WhiteButtonChecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _whiteParticles = true;
        }

        private void BlueButtonUnchecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _blueParticles = false;
        }

        private void BlueButtonChecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _blueParticles = true;
        }

        private void RedButtonUnchecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _redParticles = false;
        }

        private void RedButtonChecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _redParticles = true;
        }

        private void GreenButtonUnchecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _greenParticles = false;

        }

        private void GreenButtonChecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _greenParticles = true;
        }

        private void YellowButtonUnchecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _yellowParticles = false;
        }

        private void YellowButtonChecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _yellowParticles = true;
        }

        private void PinkButtonUnchecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _pinkParticles = false;
        }

        private void PinkButtonChecked(object sender, RoutedEventArgs e)
        {
            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
            _pinkParticles = true;
        }
    }

    #endregion

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
        private readonly Dictionary<Color, ParticleSystem> _particleSystems;

        private int _countParticleErrors;

        public ParticleSystemManagerEntity()
        {
            _particleSystems = new Dictionary<Color, ParticleSystem>();
        }

        public void Update(float elapsed)
        {
            foreach (ParticleSystem particleSystem in _particleSystems.Values)
            {
                particleSystem.Update(elapsed);
            }
        }

        public void SpawnParticle(Point3D position, double speed, Color color, double size, double duration)
        {
            try
            {
                ParticleSystem particleSystem = _particleSystems[color];
                particleSystem.SpawnParticle(position, speed, size, duration, new Particle());
            }
            catch (Exception)
            {
                _countParticleErrors += 1;
            }
        }

        public Model3D CreateParticleSystem(int maxCount, Color color)
        {
            var particleSystem = new ParticleSystem(maxCount, color);
            _particleSystems.Add(color, particleSystem);
            return particleSystem.ParticleModel;
        }

        public int ActiveParticleCount
        {
            get
            {
                return _particleSystems.Values.Sum(particleSystem => particleSystem.Count);
            }
        }
    }

    public class ParticleSystem
    {
        private readonly List<Particle> _particleList;
        private readonly GeometryModel3D _particleModel;
        private int _maxParticleCount;
        private Random _random;

        private const double InitialSpeedConst = 0.25f;
        private const double DecayAmount = 1.0f;
        private const double ParticleSizeConst = 32.0;
        private const double StandardColorOffset = 0.25;
        private const double StandardColorOffsetBorder = 1.0;
        private const float DirectionX = 1.0f;
        private const float DirectionY = 1.0f;
        private const float DirectionDistortionX = 2.0f;
        private const float DirectionDistortionY = 2.0f;


        public ParticleSystem(int maxCount, Color color)
        {
            _maxParticleCount = maxCount;

            _particleModel = new GeometryModel3D {Geometry = new MeshGeometry3D()};

            _particleList = new List<Particle>();

            //FELO: Drawing particle: Ellipse.
            var ellipse = new Ellipse {Width = ParticleSizeConst, Height = ParticleSizeConst};
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
            #if USE_VISUALBRUSH  //--------------------------------------------------------------

            brush = new VisualBrush(e);

            #else

            var renderTarget = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(ellipse);
            renderTarget.Freeze();
            Brush brush = new ImageBrush(renderTarget);

            #endif  //---------------------------------------------------------------------------

            var material = new DiffuseMaterial(brush);

            // #FELO: EmissiveMaterial is more radiating. Could actually be a good alternative.
            //var material = new EmissiveMaterial(brush);

            _particleModel.Material = material;

            _random = new Random(brush.GetHashCode());
        }

        public int MaxParticleCount
        {
            get { return _maxParticleCount; }
            set { _maxParticleCount = value; }
        }

        public int Count
        {
            get { return _particleList.Count; }
        }

        public Model3D ParticleModel
        {
            get { return _particleModel; }
        }

        public void Update(double elapsed)
        {
            var recycleList = new List<Particle>();

            foreach (Particle p in _particleList)
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
                _particleList.Remove(p);
            }
          
            Update3DGeometry();
        }

        private void Update3DGeometry()
        {
            var positions = new Point3DCollection();
            var indices = new Int32Collection();
            var texcoords = new PointCollection();

            for (int i = 0; i < _particleList.Count; ++i)
            {
                int positionIndex = i*4;
                //int indexIndex = i*6;
                Particle particle = _particleList[i];

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

            ((MeshGeometry3D) _particleModel.Geometry).Positions = positions;
            ((MeshGeometry3D) _particleModel.Geometry).TriangleIndices = indices;
            ((MeshGeometry3D) _particleModel.Geometry).TextureCoordinates = texcoords;
        }

        public void SpawnParticle(Point3D position, double speed, double size, double duration, Particle particle)
        {
            if (_particleList.Count > _maxParticleCount)
            {
                return;
            }
            particle.Position = position;
            particle.StartDuration = duration;
            particle.Duration = duration;
            particle.StartSize = size;
            particle.Size = size;

            // #FELO: -1 to 1 for x and -1 to 1 for y coordinate creates 360 degree spread.
            float x = DirectionX - (float)_random.NextDouble() * DirectionDistortionX;
            float z = DirectionY - (float)_random.NextDouble() * DirectionDistortionY;

            var vector3D = new Vector3D( x, z, 0.0 );
            vector3D.Normalize();
            vector3D *= ( InitialSpeedConst + (float)_random.NextDouble() ) * (float)speed;

            particle.Velocity = new Vector3D( vector3D.X, vector3D.Y, vector3D.Z);

            particle.Decay = DecayAmount;

            _particleList.Add(particle);
        }

        public Particle Particle
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }

    #endregion
}
