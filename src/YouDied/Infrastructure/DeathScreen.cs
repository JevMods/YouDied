using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace YouDied.Infrastructure
{
    public sealed class DeathScreen : MonoBehaviour
    {
        private const string FontFile = "EBGaramond.ttf";
        private const string SoundFile = "youdied.wav";
        private const int SortingOrder = 1000;
        private const float ReferenceWidth = 1920f;
        private const float ReferenceHeight = 1080f;
        private const float BandHeight = 300f;
        private const float BandOpacity = 0.85f;
        private const float BandFeather = 0.4f;
        private const int BandTextureHeight = 128;
        private const float FontSize = 130f;
        private const float CharacterSpacing = 12f;
        private const int FontSamplingSize = 90;
        private const int FontAtlasPadding = 9;
        private const int FontAtlasSize = 1024;

        private static readonly Color TitleColor = new Color(0.6f, 0.07f, 0.07f);

        private string _assetsDirectory;
        private AudioClip _sound;
        private AudioSource _source;
        private GameObject _root;
        private RawImage _band;
        private RectTransform _titleRect;
        private TextMeshProUGUI _title;
        private float _elapsed;

        public void Initialize(string assetsDirectory)
        {
            _assetsDirectory = assetsDirectory;
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            enabled = false;
            StartCoroutine(LoadSound());
        }

        public void Play(string title, float volume)
        {
            if (_root == null)
            {
                Build();
            }

            _title.text = title;
            _elapsed = 0f;
            _source.outputAudioMixerGroup = AudioMan.instance ? AudioMan.instance.m_guiMixer : null;
            _source.clip = _sound;
            _source.volume = volume;
            _source.PlayDelayed(DeathTimeline.TitleStartSeconds);
            Apply();
            _root.SetActive(true);
            enabled = true;
        }

        private void Update()
        {
            _elapsed += Time.unscaledDeltaTime;
            if (_elapsed >= DeathTimeline.RespawnDelaySeconds)
            {
                _root.SetActive(false);
                enabled = false;
                return;
            }

            Apply();
        }

        private void OnDestroy()
        {
            if (_root != null)
            {
                Destroy(_root);
            }
        }

        private IEnumerator LoadSound()
        {
            var uri = new Uri(Path.Combine(_assetsDirectory, SoundFile)).AbsoluteUri;
            using (var request = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
            {
                yield return request.SendWebRequest();
                _sound = DownloadHandlerAudioClip.GetContent(request);
            }
        }

        private void Apply()
        {
            var visibility = DeathTimeline.Visibility(_elapsed);
            _band.color = new Color(1f, 1f, 1f, visibility);
            _title.color = new Color(TitleColor.r, TitleColor.g, TitleColor.b, visibility);
            _titleRect.localScale = Vector3.one * DeathTimeline.TitleScale(_elapsed);
        }

        private void Build()
        {
            _root = new GameObject("YouDiedScreen");
            DontDestroyOnLoad(_root);

            var canvas = _root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SortingOrder;

            var scaler = _root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.matchWidthOrHeight = 1f;

            var bandObject = CreateBand("Band");
            _band = bandObject.AddComponent<RawImage>();
            _band.texture = CreateBandTexture();
            _band.raycastTarget = false;

            var titleObject = CreateBand("Title");
            _titleRect = titleObject.GetComponent<RectTransform>();
            _title = titleObject.AddComponent<TextMeshProUGUI>();
            _title.fontSize = FontSize;
            _title.characterSpacing = CharacterSpacing;
            _title.alignment = TextAlignmentOptions.Center;
            _title.raycastTarget = false;
            var font = CreateFont();
            if (font != null)
            {
                _title.font = font;
            }
        }

        private GameObject CreateBand(string name)
        {
            var child = new GameObject(name, typeof(RectTransform));
            var rect = child.GetComponent<RectTransform>();
            rect.SetParent(_root.transform, false);
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, BandHeight);
            rect.anchoredPosition = Vector2.zero;
            return child;
        }

        private TMP_FontAsset CreateFont()
        {
            var font = TMP_FontAsset.CreateFontAsset(
                Path.Combine(_assetsDirectory, FontFile),
                0,
                FontSamplingSize,
                FontAtlasPadding,
                GlyphRenderMode.SDFAA,
                FontAtlasSize,
                FontAtlasSize);
            var gameFont = MessageHud.instance.m_messageCenterText.font;
            if (font != null && gameFont != null)
            {
                font.fallbackFontAssetTable = new List<TMP_FontAsset> { gameFont };
            }

            return font;
        }

        private static Texture2D CreateBandTexture()
        {
            var texture = new Texture2D(1, BandTextureHeight, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            for (var y = 0; y < BandTextureHeight; y++)
            {
                var position = y / (BandTextureHeight - 1f);
                var distanceToEdge = Mathf.Min(position, 1f - position);
                var alpha = Mathf.SmoothStep(0f, 1f, distanceToEdge / BandFeather) * BandOpacity;
                texture.SetPixel(0, y, new Color(0f, 0f, 0f, alpha));
            }

            texture.Apply();
            return texture;
        }
    }
}
