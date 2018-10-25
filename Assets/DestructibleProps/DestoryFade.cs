using UnityEngine;

public class DestoryFade : MonoBehaviour
{
    [Range(0, 1)] public float fAlphaToDisableCollider = 0.5f;
    Collider _Collider;
    Material _Material;
    Color _OriginColor;
    float _Alpha;
    void OnEnable()
    {
        _Material = GetComponent<Renderer>().material;
        _Collider = GetComponent<Collider>();
        _Collider.enabled = true;
        _OriginColor = _Material.color;
        _Alpha = 1f;
    }
    void Update()
    {
        _Alpha -= Time.deltaTime * 0.3f;
        _Material.color = new Color(_OriginColor.r, _OriginColor.g, _OriginColor.b, _Alpha);
        if (_Alpha <= fAlphaToDisableCollider)
        {
            _Collider.enabled = false;
            if (_Alpha <= 0f)
                gameObject.SetActive(false);
        }
    }
    void OnDisable()
    {
        _Material.color = _OriginColor;
    }
}