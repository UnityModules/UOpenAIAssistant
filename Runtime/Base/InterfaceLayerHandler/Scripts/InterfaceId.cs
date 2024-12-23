using UnityEngine;

namespace OpenAI.InterfaceLayer
{
    public class InterfaceId : MonoBehaviour
    {
        public string Id;

        private void OnEnable() =>
            InterfaceLayerHandler.AddLayer(this);
        
        private void OnDisable() =>
            InterfaceLayerHandler.RemoveLayer(this);
    }
}