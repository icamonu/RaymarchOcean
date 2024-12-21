using System;
using UnityEngine;
using UnityEngine.UI;

namespace OceanSystem
{
    public class Ocean : MonoBehaviour
    {
        public ComputeShader oceanShader;
        public Vector3 sunDirection;
        public Transform lightSource;
        [Range(0f,5f)]public float waveSpeed;
        [Range(0.01f, 1.8f)]public float choppiness;
        RenderTexture renderTexture;
        Camera mainCamera;
        
        private void Awake()
        {
            renderTexture = CreateRenderTexture(Screen.width, Screen.height);
            oceanShader.SetTexture(0, "renderTexture", renderTexture);
            oceanShader.SetFloat("screenWidth", Screen.width);
            oceanShader.SetFloat("screenHeight", Screen.height);
            oceanShader.SetFloat("iterationCountForNormalCalculations", 32);
            oceanShader.SetFloat("iterationCountForRaymarching", 16);
            mainCamera = Camera.main;
        }

        private void Update()
        {
            lightSource.position = sunDirection;
            lightSource.LookAt(mainCamera.transform.position);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, renderTexture);
            oceanShader.SetFloat("time", Time.time);
            oceanShader.SetVector("sunDirection", Vector3.Normalize(sunDirection));
            oceanShader.SetFloat("waveSpeed", waveSpeed);
            oceanShader.SetFloat("choppiness", choppiness);
            oceanShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
            Graphics.Blit(renderTexture, destination);
        }

        private void OnDestroy()
        {
            renderTexture.Release();
        }

        RenderTexture CreateRenderTexture(int width, int height)
        {
            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            renderTexture.enableRandomWrite = true;

            return renderTexture;
        }
        
        public void SetWaveSpeed(Slider slider){waveSpeed = slider.value;}
        public void SetChoppiness(Slider slider){choppiness = slider.value;}
        public void SetSunDirectionX(Slider slider){sunDirection = new Vector3(slider.value, sunDirection.y, 2);}
        public void SetSunDirectionY(Slider slider){sunDirection = new Vector3(sunDirection.x, slider.value, 2);}
    }
}