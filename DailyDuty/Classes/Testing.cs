using System;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace DailyDuty.Classes;

public unsafe class Testing : IDisposable{
	
	public Testing() {
		System.NameplateAddonController.OnAttach += AttachNodes;
		System.NameplateAddonController.OnDetach += DetachNodes;
	}

	public void Dispose() {
		System.NameplateAddonController.OnAttach -= AttachNodes;
		System.NameplateAddonController.OnDetach -= DetachNodes;
	}
	
	private void AttachNodes(AddonNamePlate* addon) {

	}
	
	private void DetachNodes(AddonNamePlate* addon) {

	}
}