namespace Arena.UI
{
    public struct CastInfo
    {
        public bool isCasting;
        public string abilityId;
        public string displayName;
        public float castDuration;
        public float remaining;
        public float normalized; // 0 → 1
        public float startedAt;
        public float endsAt;
    }
}
