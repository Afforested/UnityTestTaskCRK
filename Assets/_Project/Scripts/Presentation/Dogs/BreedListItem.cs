namespace App.Presentation.Dogs
{
    public readonly struct BreedListItem
    {
        public BreedListItem(string breedId, string title)
        {
            BreedId = breedId;
            Title = title;
        }

        public string BreedId { get; }

        public string Title { get; }
    }
}
