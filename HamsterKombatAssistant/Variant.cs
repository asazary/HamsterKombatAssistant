namespace HamsterKombatAssistant
{
    public class Variant : IEquatable<Variant>
    {
        public HashSet<HkItem> Items { get; set; } = [];
        public int IncSum { get; set; }
        public int CostSum { get; set; }
        public int Count { get; set; }

        public bool Equals(Variant? x) {
            if (x != null)
            {
                if (x.IncSum == 1081) {
                    ; 
                }
                var res = Items.SetEquals(x.Items);
                return res;
            }
                
            return false;
        }

        public override bool Equals(object? obj) =>
            obj is Variant v && Equals(v);

        public override int GetHashCode()
        {            
            var hash = 0;
            foreach (var item in Items)
                hash += item.GetHashCode() * 15377 % 4717261;
            return hash;
        }        
    }

}
