using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using Avalonia.VisualTree;

// ReSharper disable once CheckNamespace
namespace ShadUI;

internal sealed class SmoothScrollController
{
    private readonly ScrollViewer _instance;
    private IRenderRoot? _visualRoot;
    private TopLevel? _topLevel;

    private double _targetX, _currentX;
    private double _targetY, _currentY;

    private bool _isLoopRunning;
    private TimeSpan _lastFrameTime = TimeSpan.Zero;

    public SmoothScrollController(
        ScrollViewer instance,
        double baseStepSize,
        double smoothingFactor)
    {
        this._instance = instance;
        
        BaseStepSize = baseStepSize;
        SmoothingFactor = smoothingFactor;
        
        instance.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
    }

    public void Stop()
    {
        _instance.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
        
        _isLoopRunning = false;
        _lastFrameTime = TimeSpan.Zero;
		
        _topLevel = null;

        _targetY = 0;
        _currentY = 0;
		
        _targetX = 0;
        _currentX = 0;
    }

    
    public double BaseStepSize { get; set; }
	
    public double SmoothingFactor { get; set; }


    private void OnPointerWheelChanged(
        object? sender,
        PointerWheelEventArgs e)
    {
        if (e.Handled)
            return;
        
        _topLevel ??= TopLevel.GetTopLevel(_instance);
        if (_topLevel is null)
            return;
        
        var dx = e.Delta.X;
        var dy = e.Delta.Y;
        
        if (Math.Abs(dy) > Math.Abs(dx))
            dx = 0;
        else if (Math.Abs(dx) > Math.Abs(dy))
            dy = 0;

        var source = e.Source as Visual;
        
        var sourceRoot = source?.GetVisualRoot();
        _visualRoot ??= _instance.GetVisualRoot();

        if (sourceRoot != _visualRoot)
            return; // this event is from a popup/flyout. TRAP IT!!! >:)

        var isShiftPressed = (e.KeyModifiers & KeyModifiers.Shift) != 0;
        while (source is not null && source != _instance)
        {
            if (source is ScrollViewer inner && inner.IsVisible)
            {
                var innerHasHorizontal = inner.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                var innerHasVertical = inner.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;

                var tryingToMoveX = dx + (isShiftPressed ? dy : 0);
                var tryingToMoveY = isShiftPressed ? 0 : dy;
                
                if (innerHasHorizontal && !innerHasVertical && !isShiftPressed)
                {
                    tryingToMoveX += tryingToMoveY;
                    tryingToMoveY = 0;
                }
                
                var canMoveX = (tryingToMoveX > 0 && inner.Offset.X > 0) || (tryingToMoveX < 0 && inner.Offset.X < (inner.Extent.Width - inner.Viewport.Width));
                var canMoveY = (tryingToMoveY > 0 && inner.Offset.Y > 0) || (tryingToMoveY < 0 && inner.Offset.Y < (inner.Extent.Height - inner.Viewport.Height));
                
                if (canMoveX || canMoveY)
                    return; // Trap it! The inner child can handle this movement itself
            }

            source = source.GetVisualParent();
        }
        
        if (!_isLoopRunning)
        {
            _currentX = _instance.Offset.X;
            _currentY = _instance.Offset.Y;
            _targetX = _currentX;
            _targetY = _currentY;
        }

        var hasHorizontal = _instance.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
        var hasVertical = _instance.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
        
        if (Math.Abs(dx) > 0) 
        {
            _targetX -= dx * BaseStepSize;
            _targetY -= dy * BaseStepSize;
        }
        else if (isShiftPressed || (hasHorizontal && !hasVertical))
        {
            _targetX -= dy * BaseStepSize;
        }
        else
        {
            _targetY -= dy * BaseStepSize;
        }
        
        StartAnimationLoop();
        e.Handled = true;
    }


    private void StartAnimationLoop()
    {
        if (_isLoopRunning || _topLevel is null)
            return;
        
        _isLoopRunning = true;
        _topLevel.RequestAnimationFrame(time =>
        {
            _lastFrameTime = time;
            OnFrameTick(time);
        });
    }

    private void OnFrameTick(
        TimeSpan time)
    {
        if (!_isLoopRunning || _topLevel is null)
            return;

        var dt = (time - _lastFrameTime).TotalSeconds;
        _lastFrameTime = time;

        _targetX = Math.Clamp(_targetX,
            min: 0,
            max: Math.Max(_instance.Extent.Width - _instance.Viewport.Width, 0));
        _targetY = Math.Clamp(_targetY,
            min: 0,
            max: Math.Max(_instance.Extent.Height - _instance.Viewport.Height, 0));
        
        var dx = _targetX - _currentX;
        var dy = _targetY - _currentY;
        
        if (Math.Abs(dx) < 0.1 && Math.Abs(dy) < 0.1) // stop if too small
        {
            _instance.Offset = new(_targetX, _targetY);
            _isLoopRunning = false;

            return;
        }

        var factor = 1.0 - Math.Exp(-SmoothingFactor * dt);
        _currentX += dx * factor;
        _currentY += dy * factor;
        
        _instance.Offset = new Vector(_currentX, _currentY);
        _topLevel.RequestAnimationFrame(OnFrameTick);
    }
}