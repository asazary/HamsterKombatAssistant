using System.Diagnostics.CodeAnalysis;

namespace HamsterKombatAssistant
{
    public class HkItemsComparer : IEqualityComparer<HkItem>
    {
        public bool Equals(HkItem? x, HkItem? y) =>
            x != null && y != null ? x.Id == y.Id : false;

        public int GetHashCode([DisallowNull] HkItem obj) =>
            obj.Id.GetHashCode();
    }
}
