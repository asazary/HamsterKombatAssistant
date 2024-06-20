using System.Collections.ObjectModel;
using System.IO;

namespace HamsterKombatAssistant
{
    public class Logic
    {
        public ObservableCollection<HkItem> Items { get; set; } = [];
        public ObservableCollection<Variant> Variants { get; set; } = [];
        public ObservableCollection<HkItem> ItemsOfVariant { get; set; } = [];
        public event Action<int>? RecalcDoneEvent;
        public event Action<int>? VariantsCountChanged;
        public event Action<int>? VariantsStackCountChanged;
        private VariantsComparer _variantsComparer = new();
        private HkItemsComparer _hkItemsComparer = new();


        public void ImportFromCsv(string filePath)
        {
            var data = File.ReadAllLines(filePath)
            .Skip(1)
            .Select(HkItem.FromCsv)
            .ToList();

            Items.Clear();
            data.ForEach(Items.Add);

            RecalcCurrentInc();
        }

        public List<string> GetItemsAsCsvList()
        {
            var list = Items.Select(s => s.AsCsv()).ToList();
            list.Insert(0, HkItem.ColumnNamesLine);
            return list;
        }

        public void LoadDataFromFile(string filePath) => ImportFromCsv(filePath);

        public void RecalcCurrentInc()
        {
            var sum = Items.Select(s => s.Value).Sum();
            RecalcDoneEvent?.Invoke(sum);
        }


        public HashSet<Variant>? CalculateVariants(int moneyLimit, int minInc, CancellationToken? cancelToken)
        {            
            var variants = new HashSet<Variant>(_variantsComparer);

            var sortedItems = Items.OrderByDescending(x => x.Inc).ToList();
            //var sortedItems = Items.OrderBy(x => x.IncCost).ToList();
            var stack = new Stack<Variant>();

            var initItem = new HkItem(0, "null", 0, "null", 0, 0, 0, 0);
            var initInfo = new Variant
            {
                Items = new HashSet<HkItem>(_hkItemsComparer),
                IncSum = initItem.Inc,
                CostSum = initItem.IncCost,
                Count = 0
            };

            stack.Push(initInfo);

            while (stack.Count > 0)
            {
                if (cancelToken?.IsCancellationRequested == true) { break; }

                var info = stack.Pop();
                VariantsStackCountChanged?.Invoke(stack.Count);

                var goodItems = sortedItems.Where(x => !info.Items.Contains(x) && x.Inc >= minInc);
                foreach (var item in goodItems) {
                    if (info.CostSum + item.IncCost > moneyLimit)
                    {
                        if (variants.Add(info))
                            VariantsCountChanged?.Invoke(variants.Count);
                        continue;
                    }

                    var extItems = new HashSet<HkItem>(info.Items, _hkItemsComparer) { item };

                    var elem = new Variant
                    {
                        Items = extItems,
                        IncSum = info.IncSum + item.Inc,
                        CostSum = info.CostSum + item.IncCost,
                        Count = info.Count + 1
                    };

                    if (variants.Contains(elem)) continue;

                    stack.Push(elem);
                    VariantsStackCountChanged?.Invoke(stack.Count);
                }
            }

            return variants;
        }
    }
}
