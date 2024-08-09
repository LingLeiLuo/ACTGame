using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : SingleMono<PostProcessingManager>
{
    public Volume Volume;
    private PlayerController controller;
    private Bloom Bloom;
    private ChromaticAberration ChromaticAberration;
    [SerializeField] private float speed;

    private void Start()
    {
        controller = FindFirstObjectByType<PlayerController>();
        Volume.profile.TryGet(out Bloom);
        Volume.profile.TryGet(out ChromaticAberration);
    }

    private void Update()
    {
        Volume.transform.position = controller.playerModel.transform.position;
    }

    public void ChromaticAberrationEF(float ChromaticAberrationValue)
    {
        // ∑¿÷π∂‡¥Œ¥•∑¢
        StopAllCoroutines();
        StartCoroutine(StartChromaticAberrationEF(ChromaticAberrationValue));
    }

    IEnumerator StartChromaticAberrationEF(float ChromaticAberrationValue)
    {
        while(ChromaticAberration.intensity.value < ChromaticAberrationValue)
        {
            yield return null;
            ChromaticAberration.intensity.value += Time.deltaTime * speed;
        }

        while (ChromaticAberration.intensity.value > 0)
        {
            yield return null;
            ChromaticAberration.intensity.value -= Time.deltaTime * speed;
        }
    }
}
