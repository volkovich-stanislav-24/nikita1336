using Application1.Models;
using System.Collections.Generic;

namespace Application1.ViewModels
{
    public class DeviceViewModel : ViewModel
    {
        readonly Device model;
        public Device Model => model;
        public string Image => $"/Content/Images/{model.GetType().Name}.png";

        public override string ToString()
            => $"View{model.ToString()}";
        public DeviceViewModel(Device model)
        {
            this.model = model;
            __all.Add(this);
        }

        static readonly IList<DeviceViewModel> __all = new List<DeviceViewModel>();
        public static IReadOnlyList<DeviceViewModel> All => __all.AsReadOnly();
        public static DeviceViewModel One(Device model)
        {
            foreach (var one in All)
                if (one.model == model)
                    return one;
            return new DeviceViewModel(model);
        }
    }
}
