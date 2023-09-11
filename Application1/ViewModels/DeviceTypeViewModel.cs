using Application1.Models;
using System;

namespace Application1.ViewModels
{
    public class DeviceTypeViewModel : ViewModel<DeviceTypeViewModel, Type>
    {
        public string Name => Model.Name;
        public string Image => $"/Content/Images/{Model.Name}.png";

        protected DeviceTypeViewModel(Type model) : base(model)
        {
            if (!model.IsAssignableTo(typeof(Device)))
                throw new ArgumentException("Тип должен быть производным от Device.");
        }
    }
}
