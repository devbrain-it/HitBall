namespace Assets.scripts
{
    public interface IPlayableClip : IUpdate
    {
        string Name { get; set; }

        bool IsPlaying { get; }

        void Play();

        void Stop();
    }
}