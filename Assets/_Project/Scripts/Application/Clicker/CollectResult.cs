namespace App.Application.Clicker
{
    public readonly struct CollectResult
    {
        public CollectResult(CollectSource source, bool isSuccess, int reward)
        {
            Source = source;
            IsSuccess = isSuccess;
            Reward = reward;
        }
        public CollectSource Source { get; }
        public bool IsSuccess { get; }
        public int Reward { get; }
    }
}
