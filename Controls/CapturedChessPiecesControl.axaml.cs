using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using ChessAvalonia.Services;
using static System.Diagnostics.Debug;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using ChessAvalonia.ViewModels.Pages.Main;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using Avalonia.Controls.Templates;

namespace ChessAvalonia.Controls
{
    public class CapturedChessPiecesControl : TemplatedControl
    {
        #region Constructors
        public CapturedChessPiecesControl()
        {
            InitializeMessageHandlers();
            KeepCheckingIsCapturedBoxAnimating();

        }
        #endregion

        #region Fields
        public bool IsMouseOverArrowLabel = false;
        private bool isCapturedBoxTriggered = false;
        private Border capturedBox = null;
        #endregion

        #region Styled Properties
        public static readonly StyledProperty<bool> CapturedBoxIsVisibleProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, bool>(nameof(CapturedBoxIsVisible));

        public bool CapturedBoxIsVisible
        {
            get { return GetValue(CapturedBoxIsVisibleProperty); }
            set
            {
                if (value)
                {
                    CapturedBoxWidth = 500;
                    isCapturedBoxTriggered = true;
                }
                else
                {
                    CapturedBoxWidth = 0;
                    Arrow = "<";
                    isCapturedBoxTriggered = false;
                }
                SetValue(CapturedBoxIsVisibleProperty, value);
            }
        }

        public static readonly StyledProperty<object> ContentProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, object>(nameof(Content));

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly StyledProperty<double> SideButtonOpacityProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, double>(nameof(SideButtonOpacity));

        public double SideButtonOpacity
        {
            get { return GetValue(SideButtonOpacityProperty); }
            set { SetValue(SideButtonOpacityProperty, value); }
        }

        public static readonly StyledProperty<double> CapturedBoxTopOffsetProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, double>(nameof(CapturedBoxTopOffset));

        public double CapturedBoxTopOffset
        {
            get { return GetValue(CapturedBoxTopOffsetProperty); }
            set { SetValue(CapturedBoxTopOffsetProperty, value); }
        }

        public static readonly StyledProperty<int> CapturedBoxWidthProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, int>(nameof(CapturedBoxWidth));

        public int CapturedBoxWidth
        {
            get { return GetValue(CapturedBoxWidthProperty); }
            set { SetValue(CapturedBoxWidthProperty, value); }
        }

        public static readonly StyledProperty<int> CapturedBoxHeightProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, int>(nameof(CapturedBoxHeight));

        public int CapturedBoxHeight
        {
            get { return GetValue(CapturedBoxHeightProperty); }
            set { SetValue(CapturedBoxHeightProperty, value); }
        }

        public static readonly StyledProperty<int> CapturedBoxMaxWidthProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, int>(nameof(CapturedBoxMaxWidth));

        public int CapturedBoxMaxWidth
        {
            get { return GetValue(CapturedBoxMaxWidthProperty); }
            set { SetValue(CapturedBoxMaxWidthProperty, value); }
        }

        public static readonly StyledProperty<bool> CapturedBoxAnimatedProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, bool>(nameof(CapturedBoxAnimated));

        public bool CapturedBoxAnimated
        {
            get { return GetValue(CapturedBoxAnimatedProperty); }
            set { SetValue(CapturedBoxAnimatedProperty, value); }
        }

        public static readonly StyledProperty<string> ArrowProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, string>(nameof(Arrow));

        public string Arrow
        {
            get { return GetValue(ArrowProperty); }
            set { SetValue(ArrowProperty, value); }
        }
        #endregion

        #region Methods
        private void KeepCheckingIsCapturedBoxAnimating()
        {
            Task.Run(async () =>
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        DispatchService.Invoke(() =>
                        {
                            if (capturedBox!.IsAnimating(WidthProperty))
                            {
                                CapturedBoxAnimated = true;
                            }
                            else
                            {
                                CapturedBoxAnimated = false;
                                if (! IsMouseOverArrowLabel &&
                                    CapturedBoxWidth == 0)
                                {
                                    SideButtonOpacity = 0.0;
                                }
                            }
                        });
                        Thread.Sleep(100);
                    }
                });
            });
        }

        public void OnArrowLabelPressed()
        {
            
            if (! isCapturedBoxTriggered)
            {
                CapturedBoxWidth = CapturedBoxMaxWidth;
                Arrow = ">";
            }
            else
            {
                CapturedBoxWidth = 0;
                Arrow = "<";
            }

            isCapturedBoxTriggered = !isCapturedBoxTriggered;
        }

        public void OnArrowLabelEnter()
        {
            SideButtonOpacity = 1.0;
            IsMouseOverArrowLabel = true;
        }

        public void OnArrowLabelLeave()
        {
            IsMouseOverArrowLabel = false;

            if (!CapturedBoxAnimated &&
                CapturedBoxWidth != CapturedBoxMaxWidth)
            {
                SideButtonOpacity = 0.0;
            }
        }
        private void InitializeMessageHandlers()
        {
            WeakReferenceMessenger.Default.Register<CapturedChessPiecesControl, CapturedChessPiecesControlRequestMessage>(this, (r, m) =>
            {
                m.Reply(r);
            });
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            capturedBox = e.NameScope.Find<Border>("CapturedBox");
            capturedBox.SetValue(MarginProperty, new Thickness(0, CapturedBoxTopOffset, 0, 0));
        }
        #endregion

        #region Message Handlers:
        internal class CapturedChessPiecesControlRequestMessage : RequestMessage<CapturedChessPiecesControl> { }
        #endregion
    }
}
