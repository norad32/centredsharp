using ClassicUO.Assets;
using Microsoft.Xna.Framework;

namespace CentrED
{
    public sealed class AnimatedStaticsManager
    {
        private const uint StaticIndexOffset = 0x4000;
        private const uint AnimationDelayMs = 50 * 2;
        private List<AnimationData> _animations = new List<AnimationData>();

        private static readonly Lazy<AnimatedStaticsManager> _instance =
            new Lazy<AnimatedStaticsManager>(() => new AnimatedStaticsManager());

        public static AnimatedStaticsManager Instance => _instance.Value;
        private AnimatedStaticsManager() { }

        public void Initialize()
        {
            var animDataFile = AnimDataLoader.Instance.AnimDataFile;
            var statics = TileDataLoader.Instance.StaticData;
            if (animDataFile == null || statics == null)
            {
                return;
            }

            _animations.Clear();

            unsafe
            {
                long baseAddress = animDataFile.StartAddress.ToInt64();
                long lastValidAddress = baseAddress + animDataFile.Length - sizeof(AnimDataFrame);

                for (int i = 0; i < statics.Length; i++)
                {
                    if (!statics[i].IsAnimated)
                    {
                        continue;
                    }

                    var framePtr = GetAnimDataFramePointer(baseAddress, lastValidAddress, i);
                    if (framePtr == null || framePtr->FrameCount <= 1)
                    {
                        continue;
                    }

                    uint intervalRaw = framePtr->FrameInterval;
                    if (intervalRaw == 0)
                    {
                        intervalRaw = 1;
                    }

                    var frames = new sbyte[framePtr->FrameCount];
                    var span = new ReadOnlySpan<sbyte>(framePtr->FrameData, framePtr->FrameCount);
                    span.CopyTo(frames);

                    uint intervalMs = intervalRaw * AnimationDelayMs;

                    _animations.Add(new AnimationData
                    {
                        StaticIndex = (uint)i + StaticIndexOffset,
                        IntervalMs = intervalMs,
                        FrameCount = framePtr->FrameCount,
                        NextProcessTimeMs = 0,
                        FrameIndex = 0,
                        FrameOffsets = frames,
                    });
                }
            }
        }

        public void Process(GameTime gameTime)
        {
            uint currentTimeMs = (uint)gameTime.TotalGameTime.TotalMilliseconds;
            var artAssets = ArtLoader.Instance.Entries;

            foreach (var animation in _animations)
            {
                if (currentTimeMs < animation.NextProcessTimeMs)
                {
                    continue;
                }

                artAssets[animation.StaticIndex].AnimOffset = animation.FrameOffsets[animation.FrameIndex];
                animation.FrameIndex = (ushort)((animation.FrameIndex + 1) % animation.FrameCount);

                animation.NextProcessTimeMs = currentTimeMs + animation.IntervalMs;
            }
        }

        private unsafe AnimDataFrame* GetAnimDataFramePointer(long baseAddress, long lastValidAddress, int index)
        {
            const int HeaderBlockSize = 4;
            int recordSize = sizeof(AnimDataFrame);
            long blockOffset = HeaderBlockSize * ((index / 8) + 1);
            long offsetBytes = index * recordSize + blockOffset;
            long absoluteAddress = baseAddress + offsetBytes;

            if (absoluteAddress < baseAddress || absoluteAddress + recordSize > lastValidAddress)
            {
                return null;
            }

            return (AnimDataFrame*)absoluteAddress;
        }

        private class AnimationData
        {
            public uint StaticIndex;
            public uint IntervalMs;
            public uint NextProcessTimeMs;
            public ushort FrameCount;
            public ushort FrameIndex;
            public required sbyte[] FrameOffsets;
        }
    }
}
