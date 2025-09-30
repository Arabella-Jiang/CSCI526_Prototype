using System.Collections;
using UnityEngine;

public class PlayerFormSwitcher : MonoBehaviour
{
    public enum Form { Light, Shadow }

    [Header("Key")]
    public KeyCode toggleKey = KeyCode.Space;
    public float toggleCooldown = 0.15f;

    [Header("Layer Names (match your project)")]
    public string lightLayerName  = "Player-light";
    public string shadowLayerName = "Player-shadow";

    [Header("Visuals")]
    public SpriteRenderer[] visuals;
    public Color lightColor  = Color.white;
    public Color shadowColor = new Color(0f, 0f, 0f, 0.9f);
    public bool  useTint = true;

    [Header("Tiny FX")]
    public bool doSquash = true;
    public float squashScaleX = 1.08f, squashScaleY = 0.92f, squashTime = 0.08f;

    public Form CurrentForm { get; private set; } = Form.Light;
    float _nextToggle;

    void Awake()
    {
        if (visuals == null || visuals.Length == 0)
            visuals = GetComponentsInChildren<SpriteRenderer>(true);
    }

    void Start() => ApplyForm(CurrentForm, false);

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && Time.time >= _nextToggle)
        {
            ToggleForm();
            _nextToggle = Time.time + toggleCooldown;
        }
    }

    public void ToggleForm() => SetForm(CurrentForm == Form.Light ? Form.Shadow : Form.Light);

    public void SetForm(Form f) { CurrentForm = f; ApplyForm(f, true); }

    void ApplyForm(Form f, bool doFx)
    {
        string layerName = (f == Form.Light) ? lightLayerName : shadowLayerName;
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1) { Debug.LogError($"[PlayerFormSwitcher] Layer not exist: {layerName}"); return; }
        SetLayerRecursively(gameObject, layer);

        if (useTint)
        {
            var c = (f == Form.Light) ? lightColor : shadowColor;
            foreach (var r in visuals) if (r) r.color = c;
        }

        if (doFx && doSquash) StartCoroutine(SquashFX());
    }

    IEnumerator SquashFX()
    {
        var o = transform.localScale;
        var s = new Vector3(o.x * squashScaleX, o.y * squashScaleY, o.z);
        float t = 0f;
        while (t < squashTime) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(o, s, t/squashTime); yield return null; }
        t = 0f;
        while (t < squashTime) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(s, o, t/squashTime); yield return null; }
        transform.localScale = o;
    }

    void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform c in go.transform) SetLayerRecursively(c.gameObject, layer);
    }
}
