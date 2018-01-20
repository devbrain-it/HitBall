namespace Assets.scripts
{
    public interface IPlayableClip
    {
        string Name { get; set; }

        void Play();

        void Stop();
    }
}