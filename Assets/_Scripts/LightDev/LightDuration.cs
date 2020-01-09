using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightDuration : MonoBehaviour
{

    SphereCollider actionZone;
    Light ChildLight;
    Material material;
    public bool canBeTouch = true;
    public Color colorToGo;
    public int howManyOccurance;

    public AudioClip Oof;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        actionZone = this.GetComponent<SphereCollider>();
        ChildLight = this.transform.Find("Point Light").GetComponent<Light>();
        material = this.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }


    public IEnumerator ProgressifLight()
    {
        audioSource.pitch = audioSource.pitch + howManyOccurance * 0.25f;
        audioSource.PlayOneShot(Oof, 1f);
        for (int i =0; i< GameManager.Instance.datas.progressifLighting; i++)
        {
            float flottant = GameManager.Instance.datas.brightnessMax * ((i + 1) / GameManager.Instance.datas.progressifLighting);
            float range = GameManager.Instance.datas.rangeOfAction * ((i + 1) / GameManager.Instance.datas.progressifLighting);

            ChildLight.range = flottant;
            actionZone.radius = range;
            material.color = new Color(colorToGo.r *((i + 1) / GameManager.Instance.datas.progressifLighting),
                            colorToGo.g *((i + 1) / GameManager.Instance.datas.progressifLighting),
                            colorToGo.b *((i + 1) / GameManager.Instance.datas.progressifLighting), 1);
            material.SetColor ("_EmissionColor", material.color);
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(waitBeforeDecrease());
    }

    IEnumerator waitBeforeDecrease()
    {
        yield return new WaitForSeconds(GameManager.Instance.datas.durationOfLight);
        StartCoroutine(decreaseLighting());
    }

    IEnumerator decreaseLighting()
    {
        for (int i = 0; i < GameManager.Instance.datas.progressifLighting; i++)
        {
            float flottant = GameManager.Instance.datas.brightnessMax - GameManager.Instance.datas.brightnessMax * ((i + 1) / GameManager.Instance.datas.progressifLighting);
            float range = GameManager.Instance.datas.rangeOfAction - GameManager.Instance.datas.rangeOfAction  * ((i + 1) / GameManager.Instance.datas.progressifLighting);

            ChildLight.range = flottant;
            actionZone.radius = range;
            material.color = new Color(colorToGo.r - colorToGo.r * ((i + 1) / GameManager.Instance.datas.progressifLighting),
                            colorToGo.g - colorToGo.g * ((i + 1) / GameManager.Instance.datas.progressifLighting),
                            colorToGo.b - colorToGo.b * ((i + 1) / GameManager.Instance.datas.progressifLighting), 1);
            material.SetColor("_EmissionColor", material.color);
            yield return new WaitForSeconds(0.1f);
        }
        canBeTouch = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Light")
        {
            if (other.GetComponent<LightDuration>().canBeTouch == true)
            {
                other.GetComponent<LightDuration>().howManyOccurance = howManyOccurance + 1;
                StartCoroutine(other.GetComponent<LightDuration>().ProgressifLight());
                other.GetComponent<LightDuration>().canBeTouch = false;
            }
        }
    }
}
