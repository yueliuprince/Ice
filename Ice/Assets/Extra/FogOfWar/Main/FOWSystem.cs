using UnityEngine;
using System.Threading;
using System.Collections.Generic;

namespace FogOfWar
{
    [DefaultExecutionOrder(-100)]
    public class FOWSystem : MonoBehaviour
    {
        public enum State
        {
            Blending,
            NeedUpdate,
            UpdateTexture,
        }

        private Vector3 basePoint = Vector3.zero;

        public static FOWSystem PUBLIC;

        static List<FOWAbstractRevealer> mRevealers = new List<FOWAbstractRevealer>();
        static List<FOWAbstractRevealer> mAdded = new List<FOWAbstractRevealer>();
        static List<FOWAbstractRevealer> mRemoved = new List<FOWAbstractRevealer>();

        // Color buffers -- prepared on the worker thread.
        private Color32[] mBuffer0;
        private Color32[] mBuffer1;
        private Color32[] mBuffer2;

        // Whether some color buffer is ready to be uploaded to VRAM
        [System.NonSerialized] public float blendFactor = 0f;
        private float mNextUpdate = 0f;
        private State mState = State.Blending;

        Thread mThread;
        volatile bool mThreadWork;
        private int sqrTextureSize;
        public float worldSize = 128f;
        public int textureSize = 256;
        [Range(0.02f, 0.64f)] public float updateFrequency = 0.2f;
        [Range(0, 1)] public float textureBlendTime = 0.5f;
        [Range(0, 4)] public int blurIterations = 2;
        private float worldToTex;

        public bool enableFog = true;
        public Texture2D FogTexture { get; private set; }
        public void RenderImmediately()
        {
            UpdateBuffer();
            mState = State.UpdateTexture;
            UpdateTexture();
            blendFactor = 1.0f;
        }
        static public void AddRevealer(FOWAbstractRevealer rev) { if (rev != null) lock (mAdded) mAdded.Add(rev); }
        static public void DelRevealer(FOWAbstractRevealer rev) { if (rev != null) lock (mRemoved) mRemoved.Add(rev); }

        void Awake()
        {
            PUBLIC = this;
            worldToTex = textureSize / worldSize;

            basePoint = transform.position;
            basePoint.x -= worldSize * 0.5f;
            basePoint.z -= worldSize * 0.5f;

            sqrTextureSize = textureSize * textureSize;
            mBuffer0 = new Color32[sqrTextureSize];
            mBuffer1 = new Color32[sqrTextureSize];
            mBuffer2 = new Color32[sqrTextureSize];
            mRevealers.Clear();
            mRemoved.Clear();
            mAdded.Clear();

            // Add a thread update function -- all visibility checks will be done on a separate thread
            mThread = new Thread(ThreadUpdate);
            mThreadWork = true;
            mThread.Start();

            GetComponentInChildren<Projector>(true).gameObject.SetActive(true);
        }

        public void OnDestroy()
        {
            if (mThread != null)
            {
                mThreadWork = false;
                mThread.Join();
                mThread = null;
            }
            mBuffer0 = null;
            mBuffer1 = null;
            mBuffer2 = null;
            if (FogTexture != null)
            {
                Destroy(FogTexture);
                FogTexture = null;
            }
        }

        void Update()
        {
            if (textureBlendTime > 0f) blendFactor = Mathf.Clamp01(blendFactor + Time.deltaTime / textureBlendTime);
            else blendFactor = 1f;

            if (mState == State.Blending)
            {
                float time = Time.time;

                if (mNextUpdate < time)
                {
                    mNextUpdate = time + updateFrequency;
                    mState = State.NeedUpdate;
                }
            }
            else if (mState != State.NeedUpdate) UpdateTexture();
        }

        float mElapsed = 0f;

        void ThreadUpdate()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            while (mThreadWork)
            {
                if (mState == State.NeedUpdate)
                {
                    sw.Reset();
                    sw.Start();
                    UpdateBuffer();
                    sw.Stop();
                    mElapsed = 0.001f * sw.ElapsedMilliseconds;
                    mState = State.UpdateTexture;
                }
                Thread.Sleep(1);
            }
#if UNITY_EDITOR
            Debug.Log("FOW thread exit!");
#endif
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(worldSize, 0f, worldSize));
            GetComponentInChildren<Projector>(true).orthographicSize = worldSize * 0.5f;
        }

        void UpdateBuffer()
        {
            // Add all items scheduled to be added
            if (mAdded.Count > 0)
            {
                lock (mAdded)
                {
                    while (mAdded.Count > 0)
                    {
                        int index = mAdded.Count - 1;
                        mRevealers.Add(mAdded[index]);
                        mAdded.RemoveAt(index);
                    }
                }
            }

            // Remove all items scheduled for removal
            if (mRemoved.Count > 0)
            {
                lock (mRemoved)
                {
                    while (mRemoved.Count > 0)
                    {
                        int index = mRemoved.Count - 1;
                        mRevealers.Remove(mRemoved[index]);
                        mRemoved.RemoveAt(index);
                    }
                }
            }

            // Use the texture blend time, thus estimating the time this update will finish
            // Doing so helps avoid visible changes in blending caused by the blended result being X milliseconds behind.
            float factor = (textureBlendTime > 0f) ? Mathf.Clamp01(blendFactor + mElapsed / textureBlendTime) : 1f;

            // Clear the buffer's red channel (channel used for current visibility -- it's updated right after)
            for (int i = 0, imax = mBuffer0.Length; i < imax; i++)
            {
                mBuffer0[i] = Color32.Lerp(mBuffer0[i], mBuffer1[i], factor);
                mBuffer1[i].r = 0;
            }

            // Update the visibility buffer, one revealer at a time
            for (int i = 0; i < mRevealers.Count; i++) if (mRevealers[i].isEnabled) PaintCircle(mRevealers[i]);

            // Blur the final visibility data
            for (int i = 0; i < blurIterations; i++) Blur();

            // Reveal the map based on what's currently visible
            for (int i = 0; i < sqrTextureSize; i++)
            {
                if (mBuffer1[i].g < mBuffer1[i].r) mBuffer1[i].g = mBuffer1[i].r;
                mBuffer0[i].b = mBuffer1[i].r;
                mBuffer0[i].a = mBuffer1[i].g;
            }
        }

        void PaintCircle(FOWAbstractRevealer r)
        {
            // Position relative to the fog of war
            Vector3 pos = (r.worldPos - basePoint) * worldToTex;
            float radius = r.radius * worldToTex;

            // Coordinates we'll be dealing with
            int xmin = Mathf.RoundToInt(pos.x - radius);
            int ymin = Mathf.RoundToInt(pos.z - radius);
            int xmax = Mathf.RoundToInt(pos.x + radius);
            int ymax = Mathf.RoundToInt(pos.z + radius);

            int cx = Mathf.RoundToInt(pos.x);
            int cy = Mathf.RoundToInt(pos.z);

            cx = Mathf.Clamp(cx, 0, textureSize - 1);
            cy = Mathf.Clamp(cy, 0, textureSize - 1);

            int radiusSqr = Mathf.RoundToInt(radius * radius);

            if (ymin < 0) ymin = 0;
            if (xmin < 0) xmin = 0;
            if (xmax > textureSize - 1) xmax = textureSize - 1;
            if (ymax > textureSize - 1) ymax = textureSize - 1;

            for (int y = ymin; y <= ymax; y++)
            {
                int yw = y * textureSize;
                for (int x = xmin; x <= xmax; ++x)
                {
                    int xd = x - cx;
                    int yd = y - cy;
                    int dist = xd * xd + yd * yd;

                    // Reveal this pixel
                    if (dist <= radiusSqr) mBuffer1[x + yw].r = 255;
                }
            }
        }

        void Blur()
        {
            Color32 c;
            for (int y = 1; y < textureSize - 1; ++y)
            {
                int yw = y * textureSize;
                int yw0 = (y - 1) * textureSize;
                int yw1 = (y + 1) * textureSize;

                for (int x = 1; x < textureSize - 1; ++x)
                {
                    int x0 = (x - 1);
                    int x1 = (x + 1);

                    int val = mBuffer1[x + yw].r;

                    val += mBuffer1[x0 + yw].r;
                    val += mBuffer1[x1 + yw].r;
                    val += mBuffer1[x + yw0].r;
                    val += mBuffer1[x + yw1].r;

                    val += mBuffer1[x0 + yw0].r;
                    val += mBuffer1[x1 + yw0].r;
                    val += mBuffer1[x0 + yw1].r;
                    val += mBuffer1[x1 + yw1].r;

                    c = mBuffer2[x + yw];
                    c.r = (byte)(val / 9);
                    mBuffer2[x + yw] = c;
                }
            }

            // Swap the buffer so that the blurred one is used
            Color32[] temp = mBuffer1;
            mBuffer1 = mBuffer2;
            mBuffer2 = temp;
        }

        void UpdateTexture()
        {
            if (FogTexture == null)
            {
                // Native ARGB format is the fastest as it involves no data conversion
                FogTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);

                FogTexture.wrapMode = TextureWrapMode.Clamp;
                FogTexture.SetPixels32(mBuffer0);
                FogTexture.Apply();
                mState = State.Blending;
            }
            else if (mState == State.UpdateTexture)
            {
                FogTexture.SetPixels32(mBuffer0);
                FogTexture.Apply();
                blendFactor = 0f;
                mState = State.Blending;
            }
        }

        public bool IsVisible(Vector3 pos)
        {
            if (mBuffer0 == null || mBuffer1 == null) return false;

            pos -= basePoint;
            int cx = Mathf.RoundToInt(pos.x * worldToTex);
            int cy = Mathf.RoundToInt(pos.z * worldToTex);

            cx = Mathf.Clamp(cx, 0, textureSize - 1);
            cy = Mathf.Clamp(cy, 0, textureSize - 1);
            int index = cx + cy * textureSize;
            return mBuffer0[index].r > 64 || mBuffer1[index].r > 0;
        }

        public bool IsExplored(Vector3 pos)
        {
            if (mBuffer0 == null) return false;
            pos -= basePoint;

            int cx = Mathf.RoundToInt(pos.x * worldToTex);
            int cy = Mathf.RoundToInt(pos.z * worldToTex);

            cx = Mathf.Clamp(cx, 0, textureSize - 1);
            cy = Mathf.Clamp(cy, 0, textureSize - 1);
            return mBuffer0[cx + cy * textureSize].g > 0;
        }
    }
}