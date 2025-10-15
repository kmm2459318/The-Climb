namespace TheClimb.Logging
{
    public readonly struct GravitationTargetID    //  ログプレフィクス定義クラス
    {
        // 事前定義タグ
        public static readonly GravitationTargetID barrel = new("Barrel");

        public string Value { get; }

        private GravitationTargetID(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        // 等価比較（==, !=）できるように
        public override bool Equals(object obj)
        {
            return obj is LogPrefix other && Value == other.Value;
        }

        //  ハッシュコードゲット
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator == (GravitationTargetID a, GravitationTargetID b) => a.Equals(b);
        public static bool operator != (GravitationTargetID a, GravitationTargetID b) => !a.Equals(b);

        // 動的に作成も可能
        public static GravitationTargetID Custom(string value) => new(value);
    }
}