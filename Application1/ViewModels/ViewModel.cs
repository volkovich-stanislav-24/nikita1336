using System;
using System.Reflection;
using System.Collections.Generic;

namespace Application1.ViewModels
{
    // Пусть уникальная модель представления представляет модель, описывая необходимые только представлению вычисляемые данные.
    public abstract class ViewModel<ThisT, ModelT> where ThisT : ViewModel<ThisT, ModelT>
    {
        // Data

        readonly ModelT __model;
        public ModelT Model => __model;
        public override string ToString()
            => $"View{__model}";

        // Construction

        protected ViewModel(ModelT model)
        {
            __model = model;
            _all.Add((ThisT)this);
        }

        // Static data

        protected static readonly List<ThisT> _all = new();
        public static IReadOnlyList<ThisT> All => _all;

        // Static behavior

        // Пусть модель представления можно получить по модели.
        public static ThisT One(ModelT model)
        {
            foreach (var one in _all)
                if (one.__model.Equals(model))
                    return one;
            // Если модель представления, представляющая модель, отсутствует, создадим новую.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
            return (ThisT)Activator.CreateInstance(
                typeof(ThisT),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { model },
                null
            );
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
    }
}
