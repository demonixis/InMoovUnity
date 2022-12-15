using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.ComputerVision.Filters
{
    public class MLVisualizer : MonoBehaviour
    {
        [SerializeField] protected WebCamTexture _source;
        [SerializeField] protected RawImage _preview;
        
        public void Setup(WebCamTexture texture, RawImage image)
        {
            _source = texture;
            _preview = image;
        }
    }
}