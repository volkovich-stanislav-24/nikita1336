using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Application1.Models
{
    // Пусть уникальная модель описывает структуру данных, её создание, удаление, их изменение.
    public abstract class Model<ThisT> : INotifyPropertyChanged where ThisT : Model<ThisT>
    {
        // Errors: classes, extending Exception

        public abstract class NameError : Exception
        {
            protected NameError(string message) : base(message)
            {
            }
        }
        public sealed class NoNameError : NameError
        {
            public NoNameError() : base("Имя должно присутствовать.")
            {
            }
        }
        public sealed class NonUniqueNameError : NameError
        {
            public NonUniqueNameError() : base("Имя должно быть уникальным.")
            {
            }
        }

        // Data: fields, properties, immutable methods

        public event PropertyChangedEventHandler? PropertyChanged;
        // Пусть модель сообщает об изменении свойств (как требует INotifyPropertyChanged).
        protected void _OnPropertyChanged(string property_name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
        }

        string __name;
        // Пусть у модели будет уникальный идентификатор - имя.
        public string Name
        {
            get => __name;
            set
            {
                // Пусть имя не должно содержать пробелы в начале и в конце.
                value = value.Trim();
                if (value.Equals(__name)) // Если новое и старое имена совпадают, то ничего не делаем.
                    return;
                if (value.Length == 0)
                    throw new NoNameError();
                if (One(value) != null) // Если по новому имени получается другая модель, то ошибка.
                    throw new NonUniqueNameError();
                __name = value;
                _OnPropertyChanged(nameof(Name)); // Сообщаем, что свойство изменилось.
            }
        }
        public override string ToString()
            => $"{GetType().Name}[{Name}]";

        // Construction: constructor

        public delegate void CreatedDeletedHandler(ThisT model);
        // Пуст модель сообщает о своём создании.
        public static event CreatedDeletedHandler? OnCreated;
        protected Model(string name) // Требуем имя при создании модели.
        {
            Name = name; // Присваиваем имя при создании модели с проверкой.
            __all.Add((ThisT)this); // Добавляем модель в список.
            if (OnCreated != null) // Если есть кому, то сообщаем о создании.
                OnCreated((ThisT)this);
        }

        // Destruction: method Delete

        // Пуст модель сообщает о своём удалении.
        public static event CreatedDeletedHandler? OnDeleted;
        protected virtual void _OnDelete()
        {
        }
        public void Delete()
        {
            _OnDelete();
            __all.Remove((ThisT)this);
            if (OnDeleted != null)
                OnDeleted((ThisT)this);
        }

        // Static data

        // Пусть модели хранятся в списке.
        static readonly List<ThisT> __all = new();
        // Пусть модели можно получить.
        public static IReadOnlyList<ThisT> All => __all;
        // Пусть модель при наличии можно получить по имени.
        public static ThisT? One(ReadOnlySpan<char> name) // Используем ReadOnlySpan для оптимизации.
        {
            foreach (var one in __all)
                if (name.Equals(one.__name, StringComparison.Ordinal)) // Сравниваем тексты без особых условий (StringComparison).
                    return one;
            return null;
        }
    }
}
