using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes;

public unsafe class OverlayController : NameplateAddonController {
	private static SimpleOverlayNode? overlayContainerNode;

	public OverlayController() : base(Service.PluginInterface) {
		OnPreEnable += OnAddonPreEnable;
		OnAttach += AttachNodes;
		OnDetach += DetachNodes;
	}

	private void OnAddonPreEnable(AddonNamePlate* addon) {
		System.TimersController.Load();
		System.TodoListController.Load();
	}
	
	private void AttachNodes(AddonNamePlate* addon) {
		overlayContainerNode = new SimpleOverlayNode {
			Size = addon->AtkUnitBase.Size(), 
			IsVisible = true, 
			NodeId = 100000002,
		};
		System.NativeController.AttachNode(overlayContainerNode, addon->RootNode, NodePosition.AsFirstChild);
		
		System.TimersController.AttachNodes(overlayContainerNode);
		System.TodoListController.AttachNodes(overlayContainerNode);
	}

	private void DetachNodes(AddonNamePlate* addon) {
		System.TimersController.DetachNodes();
		System.TodoListController.DetachNodes();
		
		System.NativeController.DisposeNode(ref overlayContainerNode);
	}
}