using System;
#nullable enable
using System.Collections.Generic;
using System.Linq;
using Acruxx.Server;
using Dotplay;
using Dotplay.Game;
using Dotplay.Game.Server;
using Godot;

namespace Acruxx;

/// <summary>
/// The bootloader multi view.
/// </summary>
public partial class BootloaderMultiView : Bootloader
{
	[Export]
	public NodePath? RootContainerPath;

	public int ViewSlot = 0;

	[Export]
	public int ViewsPerRow = 2;

	private readonly List<HSplitContainer> _containers = new();
	private readonly List<GameLogic> _logics = new();

	private bool _createNewSlot;
	private GameLogic? _currentLogic;
	private VSplitContainer? _rootContainer;
	private bool _switchToNext;

	/// <inheritdoc />
	public override void _EnterTree()
	{
		base._EnterTree();

		this._rootContainer = this.GetNode<VSplitContainer>(this.RootContainerPath);
		if (this._rootContainer is null)
		{
			throw new Exception("RootContainerPath is null");
		}

		this.ProcessMode = ProcessModeEnum.Always;

		this.CreateSlot(this.ServerLogicScenePath!);
		this.CreateSlot(this.ClientLogicScenePath!);
	}

	/// <inheritdoc/>
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionReleased("view_add"))
		{
			this._createNewSlot = true;
		}

		if (@event.IsActionReleased("switch_view"))
		{
			this._switchToNext = true;
		}

		@event.Dispose();
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (this._createNewSlot)
		{
			this._createNewSlot = false;
			this.CreateSlot(this.ClientLogicScenePath!);
		}

		if (this._switchToNext)
		{
			this._switchToNext = false;
			var currentLogicID = this._logics.IndexOf(this._currentLogic!);
			if (currentLogicID == -1)
			{
				currentLogicID = 0;
			}

			if (this._logics.Count >= (currentLogicID + 2))
			{
				currentLogicID++;
			}
			else
			{
				currentLogicID = 0;
			}

			if (this._logics.Count >= currentLogicID + 1)
			{
				var found = this._logics[currentLogicID];
				if (found is DefaultServerLogic)
				{
					if (this._logics.Count >= currentLogicID + 2)
					{
						this.ActivateGui(this._logics[currentLogicID + 1]);
					}
				}
				else
				{
					this.ActivateGui(found);
				}
			}
		}
	}

	/// <summary>
	/// Activates the gui.
	/// </summary>
	/// <param name="logic">The logic.</param>
	private void ActivateGui(GameLogic logic)
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;

		foreach (var item in this._logics)
		{
			item.GuiDisableInput = item != logic;
			item.AudioListenerEnable2D = (item == logic);
			item.AudioListenerEnable3D = (item == logic);
		}

		this._currentLogic = logic;
	}

	/// <summary>
	/// Creates the container.
	/// </summary>
	/// <returns>A HSplitContainer.</returns>
	private HSplitContainer CreateContainer()
	{
		Logger.LogDebug(this, "Add new split container with id " + this._containers.Count);
		var container = new HSplitContainer
		{
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			SizeFlagsVertical = Control.SizeFlags.ExpandFill
		};

		this._rootContainer!.AddChild(container);
		this._containers.Add(container);

		return container;
	}

	/// <summary>
	/// Creates the slot.
	/// </summary>
	/// <param name="resourcePath">The resource path.</param>
	/// <returns>A GameLogic.</returns>
	private GameLogic CreateSlot(string resourcePath)
	{
		var container = this._containers.LastOrDefault();
		if (container == null)
		{
			container = this.CreateContainer();
		}
		else
		{
			int amount = 0;
			foreach (var child in container.GetChildren())
			{
				if (child is SubViewportContainer subViewportContainer && subViewportContainer?.Visible is true)
				{
					amount++;
				}
			}

			if (amount >= this.ViewsPerRow)
			{
				container = this.CreateContainer();
			}
		}

		if (container == null)
		{
			throw new Exception("Cant create new view");
		}

		Logger.LogDebug(this, "Add new viewport to split container");

		var slot = new SubViewportContainer
		{
			Stretch = true,
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			ProcessMode = ProcessModeEnum.Always,
			MouseFilter = Control.MouseFilterEnum.Pass
		};

		container.AddChild(slot);

		var packed = GD.Load<PackedScene>(resourcePath);
		var node = packed.Instantiate();
		if (node is not GameLogic viewport)
		{
			GD.PushError($"Scene at '{resourcePath}' must have root with script deriving GameLogic. Root type: {node.GetType().Name}");
			throw new InvalidCastException("Scene root is not GameLogic");
		}
		viewport.GuiDisableInput = true;
		viewport.ProcessMode = ProcessModeEnum.Always;
		slot.AddChild(viewport);

		this._logics.Add(viewport);
		this.ActivateGui(viewport);

		if (viewport is NetworkServerLogic)
		{
			slot.Visible = (viewport as NetworkServerLogic)?.CanBeVisible ?? false;
		}

		return viewport;
	}
}
