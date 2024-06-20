using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HamsterKombatAssistant
{
    public class HkItem : INotifyPropertyChanged, IComparable<HkItem>, IEquatable<HkItem>
    {
        const string Delimiter = ";";
        private int _level;
        private int _value; // current income
        private int _inc;
        private int _incCost;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; private set; }
        public string Name { get; private set; } = "";
        public int GroupId { get; private set; }
        public string GroupName { get; private set; } = "";
        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }
        public int Value { 
            get => _value; 
            set => SetProperty(ref _value, value);
        }
        public int Inc { 
            get => _inc; 
            set => SetProperty(ref _inc, value); 
        }
        public int IncCost { 
            get => _incCost; 
            set => SetProperty(ref _incCost, value);
        }

        private HkItem() { }

        public HkItem(int id, string name, int groupId, string groupName, int level,
            int value, int inc, int incCost)
        {
            Id = id;
            Name = name;
            GroupId = groupId;
            GroupName = groupName;
            Level = level;
            Value = value;
            Inc = inc;
            IncCost = incCost;

        }

        public static HkItem FromCsv(string line)
        {
            var elems = line.Split(',', ';');
            var item = new HkItem
            {
                Id = int.Parse(elems[0]),
                Name = elems[1],
                GroupId = int.Parse(elems[2]),
                GroupName = elems[3],
                Level = int.Parse(elems[4]),
                Value = int.Parse(elems[5].Replace(" ", "")),
                Inc = int.Parse(elems[6].Replace(" ", "")),
                IncCost = int.Parse(elems[7].Replace(" ", ""))
            };

            return item;
        }

        public string AsCsv() =>
            string.Join(Delimiter, new string[]
            {
                Id.ToString(), Name, GroupId.ToString(), GroupName,
                Level.ToString(), Value.ToString(), Inc.ToString(), IncCost.ToString()
            });

        public static List<string> ColumnNames => new List<string>
        {
            "Id", "Name", "GroupId", "GroupName", "Level",
            "Value", "Inc", "IncCost"
        };

        public static string ColumnNamesLine => string.Join(Delimiter, ColumnNames);


        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string? propertyName) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public bool Equals(HkItem? x) => x?.Id == Id;

        public override bool Equals(object? obj) =>
            obj is HkItem item ? Equals(item) : false;
        

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            return $"{Id}: {Name}, {Inc}";
        }

        public int CompareTo(HkItem? other) =>
            other != null ? Id.CompareTo(other.Id) : -1;
        

    }
}
