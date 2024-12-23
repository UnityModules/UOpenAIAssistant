using UnityEngine;

namespace OpenAI.InterfaceLayer.Test
{
    public class InterfaceLayerRuntimeTester : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                Debug.Log(InterfaceLayerHandler.Message);
        }
    }
}