using Application1.Models;

namespace Application1.ViewModels
{
    public class DeviceViewModel : DeletableViewModel<DeviceViewModel, Device>
    {
        // Data

        public string Image => $"/Content/Images/{Model.GetType().Name}.png";

        // Construction

        // Пусть модель представления нельзя создать, только получить (One).
        protected DeviceViewModel(Device device) : base(device)
        {
        }
    }
}
