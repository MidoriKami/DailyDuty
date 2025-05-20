using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.ComponentNodes.InputText;

namespace DailyDuty.Classes;

public unsafe class Testing : IDisposable{
	private InputText? inputText;
	
	public Testing() {
		System.NameplateAddonController.OnAttach += AttachNodes;
		System.NameplateAddonController.OnDetach += DetachNodes;
	}

	public void Dispose() {
		System.NameplateAddonController.OnAttach -= AttachNodes;
		System.NameplateAddonController.OnDetach -= DetachNodes;
	}
	
	private void AttachNodes(AddonNamePlate* addon) {
		inputText = new InputText {
			Position = new Vector2(700.0f, 500.0f),
			Size = new Vector2(152.0f, 28.0f),
			IsVisible = true,
		};
		
		System.NativeController.AttachToAddon(inputText, addon, addon->RootNode, NodePosition.AsLastChild);
	}
	
	private void DetachNodes(AddonNamePlate* addon) {
		System.NativeController.DetachFromAddon(inputText, addon, () => {
			inputText?.Dispose();
			inputText = null;
		});
	}
}