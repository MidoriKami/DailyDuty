using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Utilities
{
    internal static unsafe class Node
    {
        public static T* GetNodeByID<T>(AtkComponentBase componentBase, uint nodeID, NodeType? type = null) where T : unmanaged
        {
            return GetNodeByID<T>(componentBase.UldManager, nodeID, type);
        }

        public static T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId, NodeType? type = null) where T : unmanaged 
        {
            for (var i = 0; i < uldManager.NodeListCount; i++) 
            {
                var n = uldManager.NodeList[i];
                if (n->NodeID != nodeId || type != null && n->Type != type.Value) continue;
                return (T*)n;
            }
            return null;
        }

        public static T* GetNodeByID<T>(AtkComponentNode* baseNode, uint nodeId, NodeType? type = null) where T : unmanaged
        {
            if (baseNode == null) return null;

            var component = baseNode->Component;
            if (component == null) return null;

            var uldManager = component->UldManager;

            return GetNodeByID<T>(uldManager, nodeId, type);
        }

        public static AtkComponentNode* GetComponentNode(AtkUnitBase* baseNode, uint id)
        {
            if (baseNode != null)
            {
                return (AtkComponentNode*) baseNode->GetNodeById(id);
            }

            return null;
        }

    }
}
