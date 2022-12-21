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

namespace ChessAvalonia.Controls
{
    public class CapturedChessPiecesControl : TemplatedControl
    {
        #region Constructors
        public CapturedChessPiecesControl()
        {
            InitializeMessageHandlers();
            KeepCheckingIsSlideInBoxAnimating();
        }
        #endregion

        #region Fields
        public bool IsMouseOverArrowLabel = false;
        private bool isSlideInBoxTriggered = false;
        private Border slideInBox = null;
        #endregion

        #region Styled Properties
        public static readonly StyledProperty<bool> SlideInBoxIsVisibleProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, bool>(nameof(SlideInBoxIsVisible));

        public bool SlideInBoxIsVisible
        {
            get { return GetValue(SlideInBoxIsVisibleProperty); }
            set
            {
                if (value)
                {
                    SlideInBoxWidth = 500;
                    isSlideInBoxTriggered = true;
                }
                else
                {
                    SlideInBoxWidth = 0;
                    Arrow = "<";
                    isSlideInBoxTriggered = false;
                }
                SetValue(SlideInBoxIsVisibleProperty, value);
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

        public static readonly StyledProperty<int> SlideInBoxWidthProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, int>(nameof(SlideInBoxWidth));

        public int SlideInBoxWidth
        {
            get { return GetValue(SlideInBoxWidthProperty); }
            set { SetValue(SlideInBoxWidthProperty, value); }
        }

        public static readonly StyledProperty<int> SlideInBoxHeightProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, int>(nameof(SlideInBoxHeight));

        public int SlideInBoxHeight
        {
            get { return GetValue(SlideInBoxHeightProperty); }
            set { SetValue(SlideInBoxHeightProperty, value); }
        }

        public static readonly StyledProperty<int> SlideInBoxMaxWidthProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, int>(nameof(SlideInBoxMaxWidth));

        public int SlideInBoxMaxWidth
        {
            get { return GetValue(SlideInBoxMaxWidthProperty); }
            set { SetValue(SlideInBoxMaxWidthProperty, value); }
        }

        public static readonly StyledProperty<bool> IsSlideInBoxAnimatedProperty =
            AvaloniaProperty.Register<CapturedChessPiecesControl, bool>(nameof(IsSlideInBoxAnimated));

        public bool IsSlideInBoxAnimated
        {
            get { return GetValue(IsSlideInBoxAnimatedProperty); }
            set { SetValue(IsSlideInBoxAnimatedProperty, value); }
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
        private void KeepCheckingIsSlideInBoxAnimating()
        {
            Task.Run(async () =>
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        DispatchService.Invoke(() =>
                        {
                            if (slideInBox!.IsAnimating(WidthProperty))
                            {
                                IsSlideInBoxAnimated = true;
                            }
                            else
                            {
                                IsSlideInBoxAnimated = false;
                                if (! IsMouseOverArrowLabel &&
                                    SlideInBoxWidth == 0)
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

        public void OnSlideInBoxInitialized(object o)
        {
            slideInBox = o as Border;
        }

        public void OnArrowLabelPressed()
        {
            
            if (! isSlideInBoxTriggered)
            {
                SlideInBoxWidth = SlideInBoxMaxWidth;
                Arrow = ">";
            }
            else
            {
                SlideInBoxWidth = 0;
                Arrow = "<";
            }

            isSlideInBoxTriggered = !isSlideInBoxTriggered;
        }

        public void OnArrowLabelEnter()
        {
            SideButtonOpacity = 1.0;
            IsMouseOverArrowLabel = true;
        }

        public void OnArrowLabelLeave()
        {
            IsMouseOverArrowLabel = false;

            if (!IsSlideInBoxAnimated &&
                SlideInBoxWidth != SlideInBoxMaxWidth)
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
        #endregion

        #region Message Handlers:
        internal class CapturedChessPiecesControlRequestMessage : RequestMessage<CapturedChessPiecesControl> { }
        #endregion
    }
}
