using System.Diagnostics.CodeAnalysis;

namespace HamsterKombatAssistant
{
    public class VariantsComparer : IEqualityComparer<Variant>
    {
        public bool Equals(Variant? x, Variant? y) =>
            x != null && y != null ? x.Items.SetEquals(y.Items) : false;

        public int GetHashCode([DisallowNull] Variant obj)
        {
            var hash = 0;
            foreach (var item in obj.Items)
                hash += item.GetHashCode() * 15377 % 4717261;
            return hash;
        }
    }
}
