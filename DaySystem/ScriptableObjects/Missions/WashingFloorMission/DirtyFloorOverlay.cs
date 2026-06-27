using System;
using UnityEngine;

public class DirtyFloorOverlay : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private int textureResolution = 256;
    [SerializeField] private Color dirtyColor = new Color(0.35f, 0.25f, 0.1f, 1f);
    [Range(0f, 1f)]
    [SerializeField] private float noiseStrength = 0f;

    [Header("Cleaning")]
    [Range(0f, 1f)]
    [SerializeField] private float completionThreshold = 0.85f;

    public float CleanedPercentage => _totalPixels > 0 ? (float)_cleanedPixels / _totalPixels : 1f;
    public bool IsCompleted { get; private set; }
    public Renderer MeshRenderer => _renderer;

    public event Action<DirtyFloorOverlay> CleaningCompleted;

    private Renderer _renderer;
    private Texture2D _dirtyTexture;
    private Material _runtimeMaterial;
    private Color32[] _pixelBuffer;
    private bool[] _isCleanMap;
    private int _totalPixels;
    private int _cleanedPixels;
    private bool _pendingApply;

    private static readonly Color32 ClearColor = new Color32(0, 0, 0, 0);

    private void Awake() => _renderer = GetComponent<Renderer>();

    public void InitializeOverlay()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_dirtyTexture != null) Destroy(_dirtyTexture);
        if (_runtimeMaterial != null) Destroy(_runtimeMaterial);

        IsCompleted = false;
        _cleanedPixels = 0;
        _pendingApply = false;
        _totalPixels = textureResolution * textureResolution;
        _pixelBuffer = new Color32[_totalPixels];
        _isCleanMap = new bool[_totalPixels];

        Color32 baseColor = (Color32)dirtyColor;
        if (noiseStrength > 0f)
        {
            float noiseScale = 4f;
            float ox = UnityEngine.Random.Range(0f, 100f);
            float oy = UnityEngine.Random.Range(0f, 100f);

            for (int y = 0; y < textureResolution; y++)
            {
                for (int x = 0; x < textureResolution; x++)
                {
                    float n = (Mathf.PerlinNoise(ox + x / (float)textureResolution * noiseScale,
                                                 oy + y / (float)textureResolution * noiseScale) - 0.5f) * noiseStrength;
                    _pixelBuffer[y * textureResolution + x] = new Color32(
                        (byte)Mathf.Clamp(baseColor.r + (int)(n * 20), 0, 255),
                        (byte)Mathf.Clamp(baseColor.g + (int)(n * 15), 0, 255),
                        (byte)Mathf.Clamp(baseColor.b + (int)(n * 8), 0, 255),
                        (byte)Mathf.Clamp(baseColor.a + (int)(n * 50), 80, 255)
                    );
                }
            }
        }
        else
        {
            for (int i = 0; i < _totalPixels; i++) _pixelBuffer[i] = baseColor;
        }

        _dirtyTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        _dirtyTexture.SetPixels32(_pixelBuffer);
        _dirtyTexture.Apply();

        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null)
        {
            _runtimeMaterial = new Material(Shader.Find("Standard"));
            _runtimeMaterial.SetFloat("_Mode", 3f);
            _runtimeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _runtimeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _runtimeMaterial.SetInt("_ZWrite", 0);
            _runtimeMaterial.DisableKeyword("_ALPHATEST_ON");
            _runtimeMaterial.EnableKeyword("_ALPHABLEND_ON");
            _runtimeMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            _runtimeMaterial.renderQueue = 3000;
        }
        else
        {
            _runtimeMaterial = new Material(shader);
        }
        _runtimeMaterial.mainTexture = _dirtyTexture;
        _renderer.material = _runtimeMaterial;
    }

    public void CleanArea(Transform brushTip, Vector2 brushSize)
    {
        if (IsCompleted) return;

        Vector3 localPos = transform.InverseTransformPoint(brushTip.position);
        float u = localPos.x + 0.5f;
        float v = localPos.y + 0.5f;

        if (u < 0f || u > 1f || v < 0f || v > 1f) return;

        int cx = Mathf.Clamp(Mathf.RoundToInt(u * (textureResolution - 1)), 0, textureResolution - 1);
        int cy = Mathf.Clamp(Mathf.RoundToInt(v * (textureResolution - 1)), 0, textureResolution - 1);

        float quadSize = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        float pixelsPerUnit = textureResolution / quadSize;

        int halfW = Mathf.Max(1, Mathf.RoundToInt(brushSize.x * 0.5f * pixelsPerUnit));
        int halfH = Mathf.Max(1, Mathf.RoundToInt(brushSize.y * 0.5f * pixelsPerUnit));

        Vector3 brushRight = transform.InverseTransformDirection(brushTip.right);
        Vector3 brushForward = transform.InverseTransformDirection(brushTip.forward);
        Vector2 axisX = new Vector2(brushRight.x, brushRight.y).normalized;
        Vector2 axisY = new Vector2(brushForward.x, brushForward.y).normalized;

        int bound = halfW + halfH;
        bool changed = false;
        for (int px = Mathf.Max(0, cx - bound); px <= Mathf.Min(textureResolution - 1, cx + bound); px++)
        {
            for (int py = Mathf.Max(0, cy - bound); py <= Mathf.Min(textureResolution - 1, cy + bound); py++)
            {
                Vector2 delta = new Vector2(px - cx, py - cy);
                if (Mathf.Abs(Vector2.Dot(delta, axisX)) > halfW) continue;
                if (Mathf.Abs(Vector2.Dot(delta, axisY)) > halfH) continue;

                int idx = py * textureResolution + px;
                if (_isCleanMap[idx]) continue;

                _isCleanMap[idx] = true;
                _pixelBuffer[idx] = ClearColor;
                _cleanedPixels++;
                changed = true;
            }
        }
        if (changed) _pendingApply = true;
    }

    private void LateUpdate()
    {
        if (!_pendingApply) return;
        _pendingApply = false;

        _dirtyTexture.SetPixels32(_pixelBuffer);
        _dirtyTexture.Apply();

        if (!IsCompleted && CleanedPercentage >= CompletionThreshold)
        {
            IsCompleted = true;
            CleaningCompleted?.Invoke(this);
        }
    }

    private void OnDestroy()
    {
        if (_dirtyTexture != null) Destroy(_dirtyTexture);
        if (_runtimeMaterial != null) Destroy(_runtimeMaterial);
    }

    public float CompletionThreshold { get => completionThreshold; set => completionThreshold = value; }
}
