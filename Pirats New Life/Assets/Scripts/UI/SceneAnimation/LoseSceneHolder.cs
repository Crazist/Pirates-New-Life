using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LoseSceneHolder : MonoBehaviour
{
    [SerializeField] private GameObject _reload;
    [SerializeField] private GameObject _text;
    [SerializeField] private Animator _anim;
    [SerializeField] private PostProcessVolume _volume;
    [SerializeField] private PostProcessLayer _layer;
     private Vignette _vignette;

    public void TurnOnPostProcessing()
    {
        _layer.enabled = true;
    }
    public GameObject GetReloadButton()
    {
        return _reload;
    }
    public GameObject GetTmp()
    {
        return _text;
    }
    public Animator GetAnimator()
    {
        return _anim;
    }
    public Vignette GetVignette()
    {
        _volume.profile.TryGetSettings(out _vignette);
        _vignette.intensity.Override(0);
        return _vignette;
    }
    public PostProcessVolume GEtVolume()
    {
        return _volume;
    }
    public MonoBehaviour GetMono()
    {
        return this;
    }
}
