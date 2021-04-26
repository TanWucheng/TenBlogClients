using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace UwpApp.Controls
{
    /// <summary>
    ///     FlipSide翻转动画
    ///     See: https://github.com/cnbluefire/FlipSide
    /// </summary>
    public sealed class FlipSide : Control
    {
        public static readonly DependencyProperty IsFlippedProperty =
            DependencyProperty.Register("IsFlipped", typeof(bool), typeof(FlipSide), new PropertyMetadata(false,
                (s, a) =>
                {
                    if (a.NewValue == a.OldValue) return;
                    if (s is FlipSide sender)
                        sender.OnIsFlippedChanged();
                }));

        public static readonly DependencyProperty Side1Property =
            DependencyProperty.Register("Side1", typeof(object), typeof(FlipSide), new PropertyMetadata(null));

        public static readonly DependencyProperty Side2Property =
            DependencyProperty.Register("Side2", typeof(object), typeof(FlipSide), new PropertyMetadata(null));

        private Vector2 _axis;
        private Grid _layoutRoot;

        private Visual _s1Visual;
        private Visual _s2Visual;

        private ContentPresenter _side1Content;
        private ContentPresenter _side2Content;
        private SpringScalarNaturalMotionAnimation _springAnimation1;
        private SpringScalarNaturalMotionAnimation _springAnimation2;

        public FlipSide()
        {
            _axis = new Vector2(0, 1);
            DefaultStyleKey = typeof(FlipSide);
        }

        public Vector2 Axis
        {
            get => _axis;
            set
            {
                _axis = value;
                UpdateAxis(_side1Content);
                UpdateAxis(_side2Content);
            }
        }

        public bool IsFlipped
        {
            get => (bool)GetValue(IsFlippedProperty);
            set => SetValue(IsFlippedProperty, value);
        }


        public object Side1
        {
            get => GetValue(Side1Property);
            set => SetValue(Side1Property, value);
        }

        public object Side2
        {
            get => GetValue(Side2Property);
            set => SetValue(Side2Property, value);
        }

        private void OnIsFlippedChanged()
        {
            float f1, f2;
            if (IsFlipped)
            {
                f1 = 180f;
                f2 = 360f;
                VisualStateManager.GoToState(this, "Slide2", false);
            }
            else
            {
                f1 = 0f;
                f2 = 180f;
                VisualStateManager.GoToState(this, "Slide1", false);
            }

            if (_springAnimation1 != null && _springAnimation2 != null)
            {
                _springAnimation1.FinalValue = f1;
                _springAnimation2.FinalValue = f2;
                _s1Visual.StartAnimation("RotationAngleInDegrees", _springAnimation1);
                _s2Visual.StartAnimation("RotationAngleInDegrees", _springAnimation2);
            }
            else
            {
                _s1Visual.RotationAngleInDegrees = f1;
                _s2Visual.RotationAngleInDegrees = f2;
            }
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _side1Content = GetTemplateChild("Side1Content") as ContentPresenter;
            _side2Content = GetTemplateChild("Side2Content") as ContentPresenter;
            _layoutRoot = GetTemplateChild("LayoutRoot") as Grid;

            InitComposition();
        }

        private void InitComposition()
        {
            if (_side1Content == null || _side2Content == null || _layoutRoot == null) return;

            _s1Visual = ElementCompositionPreview.GetElementVisual(_side1Content);
            _s2Visual = ElementCompositionPreview.GetElementVisual(_side2Content);

            var compositor = Window.Current.Compositor;

            var opacity1Animation =
                compositor.CreateExpressionAnimation("this.Target.RotationAngleInDegrees > 90 ? 0f : 1f");
            var opacity2Animation =
                compositor.CreateExpressionAnimation("(this.Target.RotationAngleInDegrees - 180) > 90 ? 1f : 0f");

            _s1Visual.StartAnimation("Opacity", opacity1Animation);
            _s2Visual.StartAnimation("Opacity", opacity2Animation);

            OnIsFlippedChanged();

            _springAnimation1 = compositor.CreateSpringScalarAnimation();
            _springAnimation1.DampingRatio = 0.4f;
            _springAnimation1.Period = TimeSpan.FromMilliseconds(80);
            _springAnimation1.FinalValue = 180f;

            _springAnimation2 = compositor.CreateSpringScalarAnimation();
            _springAnimation2.DampingRatio = 0.4f;
            _springAnimation2.Period = TimeSpan.FromMilliseconds(80);
            _springAnimation2.FinalValue = 180f;

            UpdateAxis(_side1Content);
            UpdateAxis(_side2Content);
            UpdateTransformMatrix(_layoutRoot);

            _layoutRoot.SizeChanged += LayoutRoot_SizeChanged;
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTransformMatrix(_layoutRoot);
            UpdateAxis(_side1Content);
            UpdateAxis(_side2Content);
        }

        private static void UpdateTransformMatrix(UIElement element)
        {
            var host = ElementCompositionPreview.GetElementVisual(element);
            var size = element.RenderSize.ToVector2();
            if (size.X == 0 || size.Y == 0) return;
            var n = -1f / size.X;

            var perspective = new Matrix4x4(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, n,
                0.0f, 0.0f, 0.0f, 1.0f);

            host.TransformMatrix =
                Matrix4x4.CreateTranslation(-size.X / 2, -size.Y / 2, 0f) *
                perspective *
                Matrix4x4.CreateTranslation(size.X / 2, size.Y / 2, 0f);
        }

        private void UpdateAxis(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            var size = element.RenderSize.ToVector2();

            visual.CenterPoint = new Vector3(size.X / 2, size.Y / 2, 0f);
            visual.RotationAxis = new Vector3(_axis, 0f);
        }
    }
}