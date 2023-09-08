using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Application1.Models
{
    public abstract class Model<This> : INotifyPropertyChanged where This : Model<This>
    {
        public sealed class NameError : Exception
        {
            public readonly string name;

            public NameError(string name) : base("Имя должно быть уникальным.")
            {
                this.name = name;
            }
        }

        string __name;
        public string Name
        {
            get => __name;
            set
            {
                if (GetOne(value) != null)
                    throw new NameError(value);
                __name = value;
                _OnPropertyChanged(nameof(Name));
            }
        }
        protected Model(string name)
        {
            Name = name;
            __all.Add((This)this);
        }
        protected virtual void _OnDelete()
        {
        }
        public void Delete()
        {
            _OnDelete();
            __all.Remove((This)this);
        }
        public override string ToString()
            => $"Model[{Name}]";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void _OnPropertyChanged([CallerMemberName] string property_name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
        }

        static readonly IList<This> __all = new List<This>();
        public static IReadOnlyList<This> All => __all.AsReadOnly();
        public static This? GetOne(ReadOnlySpan<char> name)
        {
            foreach (var one in __all)
                if (name.Equals(one.__name, StringComparison.Ordinal))
                    return one;
            return null;
        }
    }
}
