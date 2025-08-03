using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes;

public unsafe class OverlayController : NameplateAddonController {
	public static SimpleOverlayNode? OverlayContainerNode { get; set; }

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
		OverlayContainerNode = new SimpleOverlayNode {
			Size = addon->AtkUnitBase.Size(), 
			IsVisible = true, 
			NodeId = 100000002,
		};
		System.NativeController.AttachNode(OverlayContainerNode, addon->RootNode, NodePosition.AsFirstChild);
		
		System.TimersController.AttachNodes(OverlayContainerNode);
		System.TodoListController.AttachNodes(OverlayContainerNode);
	}

	private void DetachNodes(AddonNamePlate* addon) {
		System.TimersController.DetachNodes();
		System.TodoListController.DetachNodes();
		
		System.NativeController.DetachNode(OverlayContainerNode, () => {
			OverlayContainerNode?.Dispose();
			OverlayContainerNode = null;
		});
	}
}