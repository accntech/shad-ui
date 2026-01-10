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
	readonly ScrollViewer instance;
    IRenderRoot? visualRoot;
	TopLevel? topLevel;
    
	double targetX, currentX;
	double targetY, currentY;
    
	bool isLoopRunning;
	TimeSpan lastFrameTime = TimeSpan.Zero;

    public SmoothScrollController(
        ScrollViewer instance,
        double baseStepSize,
        double smoothingFactor)
    {
        this.instance = instance;
        
        BaseStepSize = baseStepSize;
        SmoothingFactor = smoothingFactor;
        
        instance.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
    }

    public void Stop()
    {
        instance.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
        
        isLoopRunning = false;
        lastFrameTime = TimeSpan.Zero;
		
        topLevel = null;

        targetY = 0;
        currentY = 0;
		
        targetX = 0;
        currentX = 0;
    }

    
    public double BaseStepSize { get; set; }
	
    public double SmoothingFactor { get; set; }


    void OnPointerWheelChanged(
        object? sender,
        PointerWheelEventArgs e)
    {
        if (e.Handled)
            return;
        
        topLevel ??= TopLevel.GetTopLevel(instance);
        if (topLevel is null)
            return;
        
        // Prevent scroll direction leaks
        double dx = e.Delta.X;
        double dy = e.Delta.Y;
        
        if (Math.Abs(dy) > Math.Abs(dx))
            dx = 0;
        else if (Math.Abs(dx) > Math.Abs(dy))
            dy = 0;

        // Check if this event is actually for us
        Visual? source = e.Source as Visual;
        
        //  Flyouts
        IRenderRoot? sourceRoot = source?.GetVisualRoot();
        visualRoot ??= instance.GetVisualRoot();

        if (sourceRoot != visualRoot)
            return; // this event is from a popup/flyout. TRAP IT!!! >:)

        //  Chaining
        bool isShiftPressed = (e.KeyModifiers & KeyModifiers.Shift) != 0;
        while (source is not null && source != instance)
        {
            if (source is ScrollViewer inner && inner.IsVisible)
            {
                bool innerHasHorizontal = inner.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                bool innerHasVertical = inner.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;

                double tryingToMoveX = dx + (isShiftPressed ? dy : 0);
                double tryingToMoveY = isShiftPressed ? 0 : dy;
                
                if (innerHasHorizontal && !innerHasVertical && !isShiftPressed)
                {
                    tryingToMoveX += tryingToMoveY;
                    tryingToMoveY = 0;
                }
                
                bool canMoveX = (tryingToMoveX > 0 && inner.Offset.X > 0) || (tryingToMoveX < 0 && inner.Offset.X < (inner.Extent.Width - inner.Viewport.Width));
                bool canMoveY = (tryingToMoveY > 0 && inner.Offset.Y > 0) || (tryingToMoveY < 0 && inner.Offset.Y < (inner.Extent.Height - inner.Viewport.Height));
                
                if (canMoveX || canMoveY)
                    return; // Trap it! The inner child can handle this movement itself
            }

            source = source.GetVisualParent();
        }
        
        // Sync current position if we were idle
        if (!isLoopRunning)
        {
            currentX = instance.Offset.X;
            currentY = instance.Offset.Y;
            targetX = currentX;
            targetY = currentY;
        }

        // Updating target
        bool hasHorizontal = instance.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
        bool hasVertical = instance.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
        
        if (Math.Abs(dx) > 0) 
        {
            targetX -= dx * BaseStepSize;
            targetY -= dy * BaseStepSize;
        }
        else if (isShiftPressed || (hasHorizontal && !hasVertical))
        {
            targetX -= dy * BaseStepSize;
        }
        else
        {
            targetY -= dy * BaseStepSize;
        }
        
        StartAnimationLoop();
        e.Handled = true;
    }

    
    void StartAnimationLoop()
    {
        if (isLoopRunning || topLevel is null)
            return;
        
        isLoopRunning = true;
        topLevel.RequestAnimationFrame(time =>
        {
            lastFrameTime = time;
            OnFrameTick(time);
        });
    }

    void OnFrameTick(
        TimeSpan time)
    {
        if (!isLoopRunning || topLevel is null)
            return;

        double dt = (time - lastFrameTime).TotalSeconds;
        lastFrameTime = time;

        // Clamp target (doing it in frame tick and not before in case content resizes mid-scroll)
        targetX = Math.Clamp(targetX,
            min: 0,
            max: Math.Max(instance.Extent.Width - instance.Viewport.Width, 0));
        targetY = Math.Clamp(targetY,
            min: 0,
            max: Math.Max(instance.Extent.Height - instance.Viewport.Height, 0));
        
        // Calculate positions
        double dx = targetX - currentX;
        double dy = targetY - currentY;
        
        if (Math.Abs(dx) < 0.1 && Math.Abs(dy) < 0.1) // stop if too small
        {
            instance.Offset = new(targetX, targetY);
            isLoopRunning = false;

            return;
        }

        double factor = 1.0 - Math.Exp(-SmoothingFactor * dt);
        currentX += dx * factor;
        currentY += dy * factor;
        
        // Update positions
        instance.Offset = new(currentX, currentY);
        topLevel.RequestAnimationFrame(OnFrameTick);
    }
}